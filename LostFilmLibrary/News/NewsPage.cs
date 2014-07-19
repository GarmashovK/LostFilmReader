using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft;
using System.Net;


namespace LostFilmLibrary.News
{
    public static class Extensions
    {
        public static string TrimStartWith(this string str, string substr)
        {
            var start = str.IndexOf(substr);
            if (start != -1)
                return str.Remove(start, substr.Length);
            else return str;
        }

        public static string TrimEndWith(this string str, string substr)
        {            
            var end = str.LastIndexOf(substr);

            if (end != -1)
                return str.Remove(end, substr.Length);
            else return str;
        }

        public static string Trim(this string str, string substr)
        {
            return str.TrimStartWith(substr).TrimEndWith(substr);
        }
    }

    public interface INewsContentItem { }
    public class TextItem : INewsContentItem
    {
        public string Text { get; set; }
    }
    public class ImageItem : INewsContentItem
    {
        public Uri Url { get; set; }
    }
    public class VideoItem : INewsContentItem
    {
        public Uri Url { get; set; }
        public Uri Screen { get; set; }
    }

    public class NewsPageContent : List<INewsContentItem> { }

    public class NewsPage
    {
        public string Title { get; set; }
        public ObservableCollection<Comment> Comments;
        public NewsPageContent PageContent { get; set; }
        public DateTime PublicationTime { get; set; }

        private uint NumOfComments { get; set; }
        private const string CommonUrl = "http://www.lostfilm.tv/news.php?act=comments&id=";
        
        internal static string SecurityKey { get; set; }

        //public async Task UpdateSecurityKey(uint id)
        //{
        //    var url = "http://www.lostfilm.tv/news.php?act=comments&id=" + id;

        //    await UpdateSecurityKey(url);
        //}

        //public async Task UpdateSecurityKey(string url)
        //{
        //    var page = await Common.GetPage(url);

        //    var doc = new HtmlDocument();
        //    doc.LoadHtml(page);

        //    var mid = GetMid(doc);
        //    SecurityKey = GetSecure(mid);
        //}

        public NewsPage()
        {
            Comments = new ObservableCollection<Comment>();
        }
               

        public async Task LoadNewsPageAsync(uint id)
        {
            await LoadNewsPageAsync(CommonUrl + id);
        }

        public async Task LoadNewsPageAsync(string url)
        {
            var page = await Common.GetPage(url);

            var doc = new HtmlDocument();
            doc.LoadHtml(page);

            var mid = GetMid(doc);

            //var comments = GetComments(mid);
            //foreach (var item in comments)
            //    Comments.Add(item);

            Title = GetTitle(mid);
            var contentBody = GetContentBody(mid);
            var content = GetContent(contentBody);

            PageContent = GetPageContent(content);
            PublicationTime = GetTime(contentBody);
            NumOfComments = GetNumOfComments(contentBody);
        }

        private NewsPageContent GetPageContent(IEnumerable<HtmlNode> content)
        {
            var pageContent = new NewsPageContent();
            var textTemp = "";

            foreach (var p in content)
            {
                foreach (var item in p.ChildNodes)
                {
                    switch (item.Name)
                    {
                        case "#text":
                        case "em":
                        case "i":
                        case "strong":
                            textTemp += item.InnerText;
                            break;
                        case "br":
                            textTemp = textTemp.Trim() + "\n";
                            break;
                        case "img":
                        case "iframe":
                            textTemp = textTemp.Trim();
                            if (textTemp != "")
                                pageContent.Add(new TextItem() { Text = "\t" + Common.ReplaceSomeSymbols(textTemp) });
                            if (item.Name == "img")
                                try
                                {
                                    pageContent.Add(new ImageItem() { Url = new Uri("http://www.lostfilm.tv" + item.Attributes["src"].Value) });
                                }
                                catch (Exception)
                                {
                                    pageContent.Add(new ImageItem() { Url = new Uri("http://www.lostfilm.tv" + item.Attributes["src"].Value) });
                                }
                            else
                                pageContent.Add(GetVideoItem(item.Attributes["src"].Value));
                            textTemp = "";
                            break;
                    }
                }
            }
            textTemp = textTemp.Trim();
            if (textTemp != "")
                pageContent.Add(new TextItem() { Text = "\t" + Common.ReplaceSomeSymbols(textTemp) });

            return pageContent;
        }

        private INewsContentItem GetVideoItem(string p)
        {
            var item = new VideoItem();
            var fullUrl = Common.ReplaceSomeSymbols(p);

            var linkPosStart = fullUrl.IndexOf("link=") + 5;
            var linkLen = fullUrl.Substring(linkPosStart).IndexOf("&");

            var screenPosStart = fullUrl.IndexOf("screen=") + 7;
            var screenLen = fullUrl.Substring(screenPosStart).IndexOf("&");

            item.Url = new Uri(fullUrl.Substring(linkPosStart, linkLen));
            item.Screen = new Uri(fullUrl.Substring(screenPosStart, screenLen));

            return item;
        }
                
        public async Task<uint> LoadCommentsAsync(uint id, uint start)
        {
            var url = CommonUrl + id + "&o=" + start + "&nocache=" + new System.Random().NextDouble();
                
            var page = await Common.GetPage(url);

            var doc = new HtmlDocument();
            doc.LoadHtml(page);

            var mid = GetMid(doc);
            NumOfComments = GetNumOfComments(GetContentBody(mid));
            if (NumOfComments == 0) return NumOfComments;
            if (start > NumOfComments) throw new Exception("Out of rage");

            var comments = GetComments(mid);
            foreach (var item in comments)
                Comments.Add(item);

            SecurityKey = GetSecure(mid);

            return NumOfComments;
        }
        // вытащить SecurityKey
        private string GetSecure(HtmlNodeCollection mid)
        {
            var divAddComment = mid.First(n => n.Name == "div" && n.Attributes.Contains("style") 
                && n.Attributes["style"].Value == "padding:5px 10px 0 0");
            var table = divAddComment.ChildNodes.FindFirst("table");
            var tr = table.ChildNodes.Where(n => n.Name == "tr").ToArray()[1];
            var td = tr.ChildNodes.First(n => n.Name == "td" && n.Attributes.Contains("align") && n.Attributes["align"].Value == "right");
            var tmp = td.ChildNodes.FindFirst("a").Attributes["onclick"].Value;

            var startPos = tmp.IndexOf(",'");
            var secKey = tmp.Substring(startPos + 2, tmp.IndexOf("')") - startPos - 2);

            return secKey;
        }
        
        public async Task<uint> LoadLastCommentsAsync(uint id)
        {
            var url = CommonUrl + id;

            var page = await Common.GetPage(url);

            var doc = new HtmlDocument();
            doc.LoadHtml(page);

            var mid = GetMid(doc);
            NumOfComments = GetNumOfComments(GetContentBody(mid));
            if (NumOfComments <= 20)
                return await LoadCommentsAsync(id, 0);
            else
                return await LoadCommentsAsync(id, NumOfComments - 20);
        }

        public async Task<string> PostComment(uint news_id,string comment)
        {
            var url = "http://www.lostfilm.tv/news.php";

            var handler = new HttpClientHandler();
            handler.CookieContainer = LFOptions.Cookies;
            handler.UseCookies = true;
            handler.UseDefaultCredentials = false;
            handler.AllowAutoRedirect = false;

            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript, */*; q=0.01");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip,deflate,sdch");
            client.DefaultRequestHeaders.Add("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.6,en;q=0.4");
            client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            client.DefaultRequestHeaders.Add("Origin", @"http://www.lostfilm.tv");
            client.DefaultRequestHeaders.Add("Referer", @"http://www.lostfilm.tv/news.php?act=full&type=1&id=" + news_id.ToString());

            var postData = GetPostСommentData(news_id, comment);
            HttpResponseMessage response_msg = await client.PostAsync(url, postData);
            response_msg.EnsureSuccessStatusCode();
            
            var response_result = await response_msg.Content.ReadAsStringAsync();
            
            return DeserializeResponse(response_result);
        }

        private string DeserializeResponse(string response_result)
        {
            var deserialized = JsonConvert
                .DeserializeObject<Dictionary<string,string>>(response_result);
            
            if (!deserialized.ContainsKey("error"))
            {
                return "ok";
            }
            else
            {
                return deserialized["error"];
            }
        }
        
        private static HttpContent GetPostСommentData(uint news_id, string comment)
        {
            var postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("module", "1"));
            postData.Add(new KeyValuePair<string, string>("code", "clean"));
            postData.Add(new KeyValuePair<string, string>("act", "add_comment"));            
            postData.Add(new KeyValuePair<string, string>("secure", SecurityKey));
            postData.Add(new KeyValuePair<string, string>("id", news_id.ToString()));
            postData.Add(new KeyValuePair<string, string>("edit", "undefined"));
            postData.Add(new KeyValuePair<string, string>("comment", comment));

            return new FormUrlEncodedContent(postData);
        }

        private IEnumerable<Comment> GetComments(IEnumerable<HtmlNode> mid)
        {
            return mid.Where(n => n.Name == "div" && n.Attributes.Contains("id"))
                .Select(n =>
                {
                    var comment = new Comment();
                    var tds = new List<HtmlNode>(n.ChildNodes.FindFirst("table")
                        .ChildNodes.FindFirst("tr").ChildNodes.Where(node => node.Name == "td"));

                    var fstTd = tds[0];
                    var sndTd = tds[1];

                    comment.Image = fstTd.ChildNodes.FindFirst("img").Attributes[
                        "src"].Value.Trim("[img]");
                    if (!comment.Image.StartsWith("http"))
                        comment.Image = "http://www.lostfilm.tv" + comment.Image;

                    var userData =
                        sndTd.ChildNodes.First(node => node.Name == "div" && node.Attributes.Contains("style") &&
                                                       node.Attributes["style"].Value ==
                                                       "background:#E8E8E8;height:35px;vertical-align:middle;float:left;width:202px;margin-right:2px;text-align:left;padding:5px 0 0 10px")
                            .ChildNodes;                                       
                    comment.UserName = userData.FindFirst("a").InnerText;
                    comment.UserDetails = userData.FindFirst("a").Attributes["href"].Value;
                    comment.Micro = userData.FindFirst("span").InnerText;

                    var temp = 
                            sndTd.ChildNodes.First(node => node.Name == "div" && node.Attributes.Contains("style") &&
                                                           node.Attributes["style"].Value ==
                                                           "background:#E8E8E8;height:35px;vertical-align:middle;float:left;width:150px;margin-right:2px;text-align:center;padding:5px 0 0")
                                .ChildNodes.FindFirst("span").InnerText;
                    comment.Date =
                        DateTime.ParseExact(temp, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
                    comment.ID = GetId(n.Attributes["id"].Value);
                    comment.Content =
                        GetCommentContent(
                            sndTd.ChildNodes.First(node => node.Name == "div" && node.Attributes.Contains("style") &&
                                                           node.Attributes["style"].Value ==
                                                           "background:#FFFFFF;display:block;padding:10px;float:left;overflow-x:hidden;width:350px;"));

                    //Comments.Add(comment);
                    
                    return comment;
                });
        }

        private uint GetId(string value)
        {
            var startPos = value.LastIndexOf('_') + 1;

            return uint.Parse(value.Substring(startPos));
        }

        private CommentContent GetCommentContent(HtmlNode content)
        {
            var commentContent = new CommentContent();

            HtmlNode temp = null;

            foreach (var item in content.ChildNodes)
            {
                if (item.Name == "#text" || item.Name == "i")
                {
                    commentContent.Add(new CommentText() { Text = Common.ReplaceSomeSymbols(item.InnerText.Trim()) });
                }
                else if (item.Name == "div")
                {
                    if (item.Attributes["class"].Value == "bb_quote_author")
                    {
                        temp = item;
                    }
                    else if (item.Attributes["class"].Value == "bb_quote")
                    {
                        commentContent.Add(GetQuote(temp, item));
                    }
                }                
            }
            return commentContent;
        }
                
        private Quote GetQuote(HtmlNode div_Author,HtmlNode div_content)
        {
            var Author = "";
            if (div_Author != null)
                Author = div_Author.InnerText.Trim();

            return new Quote()
            {
                Author = Author,
                Content = GetCommentContent(div_content)
            };
        }

        private uint GetNumOfComments(IEnumerable<HtmlNode> contentBody)
        {
            var a = contentBody.First(
                n =>
                    n.Name == "p" && n.Attributes.Contains("style") &&
                    n.Attributes["style"].Value == "display:block;text-align:right").ChildNodes
                .FindFirst("a").InnerText;

            return uint.Parse(a);
        }

        private DateTime GetTime(IEnumerable<HtmlNode> contentBody)
        {
            var p =
                contentBody.First(
                    n =>
                        n.Name == "p" && n.Attributes.Contains("style") &&
                        n.Attributes["style"].Value == "display:block;text-align:right").InnerText;

            var time = p.Substring(p.IndexOf("Дата: ") + 6);
            time = time.Substring(0, time.IndexOf("\n")).Trim().Replace("(", "").Replace(")", "");
            
            var pubTime = DateTime.ParseExact(time, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture);
            return pubTime;
        }

        private IEnumerable<HtmlNode> GetContent(IEnumerable<HtmlNode> contentBody)
        {
            return contentBody.Where(n => n.Name == "p" && !n.HasAttributes || n.Name == "div" && !n.HasAttributes);
        }

        private string GetTitle(IEnumerable<HtmlNode> mid)
        {
            return mid.First(
                    n =>
                        n.Name == "div" && n.Attributes.Contains("class") &&
                        n.Attributes["class"].Value == "content").ChildNodes
                        .First(n => n.Name == "div" && n.Attributes.Contains("class") && n.Attributes["class"].Value == "content_head")
                        .InnerText;
        }

        private IEnumerable<HtmlNode> GetContentBody(IEnumerable<HtmlNode> mid)
        {
            return mid.First(
                n =>
                    n.Name == "div" && n.Attributes.Contains("class") &&
                    n.Attributes["class"].Value == "content_body").ChildNodes;
        }


        private HtmlNodeCollection GetMid(HtmlDocument doc)
        {
            return doc.DocumentNode.Descendants()
                .First(n => n.Name == "body").ChildNodes
                .First(n => n.Name == "div" && n.Attributes.Contains("id") && n.Attributes["id"].Value == "MainDiv")
                .ChildNodes
                .First(n => n.Name == "div" && n.Attributes.Contains("id") && n.Attributes["id"].Value == "Onwrapper")
                .ChildNodes
                .First(
                    n =>
                        n.Name == "div" && n.Attributes.Contains("style") &&
                        n.Attributes["style"].Value == "width:498px;padding:0 6px 0 6px;float:left;margin:0px;")
                .ChildNodes
                .First(n => n.Name == "div" && n.Attributes.Contains("class") && n.Attributes["class"].Value == "mid")
                .ChildNodes;
        }
    }
}
