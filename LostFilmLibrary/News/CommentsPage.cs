using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LostFilmLibrary.News
{
    public class CommentsPage
    {
        public ObservableCollection<Comment> Comments;
        private uint NumOfComments { get; set; }
        private const string CommonUrl = "http://www.lostfilm.tv/news.php?act=comments&id=";
        internal static string SecurityKey { get; set; }

        public CommentsPage()
        {
            Comments = new ObservableCollection<Comment>();
        }

        public async Task<uint> LoadCommentsAsync(uint id, uint start)
        {
            var url = CommonUrl + id + "&o=" + start + "&nocache=" + new System.Random().NextDouble();

            var page = await Common.GetPage(url);

            var doc = new HtmlDocument();
            doc.LoadHtml(page);

            var mid = Common.GetMid(doc);
            NumOfComments = GetNumOfComments(Common.GetContentBody(mid));
            if (NumOfComments == 0) return NumOfComments;
            if (start > NumOfComments) throw new Exception("Out of rage");

            var comments = GetComments(mid);
            foreach (var item in comments)
                Comments.Add(item);

            SecurityKey = Common.GetSecure(mid);

            return NumOfComments;
        }

        public async Task<uint> LoadLastCommentsAsync(uint id)
        {
            var url = CommonUrl + id;

            var page = await Common.GetPage(url);

            var doc = new HtmlDocument();
            doc.LoadHtml(page);

            var mid = Common.GetMid(doc);
            NumOfComments = GetNumOfComments(Common.GetContentBody(mid));
            if (NumOfComments <= 20)
                return await LoadCommentsAsync(id, 0);
            else
                return await LoadCommentsAsync(id, NumOfComments - 20);
        }

        public async Task<string> PostComment(uint id, string comment)
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
            client.DefaultRequestHeaders.Add("Referer", @"http://www.lostfilm.tv/news.php?act=full&type=1&id=" + id.ToString());

            var postData = GetPostСommentData(id, comment);
            HttpResponseMessage response_msg = await client.PostAsync(url, postData);
            response_msg.EnsureSuccessStatusCode();

            var response_result = await response_msg.Content.ReadAsStringAsync();

            return DeserializeResponse(response_result);
        }

        private string DeserializeResponse(string response_result)
        {
            var deserialized = JsonConvert
                .DeserializeObject<Dictionary<string, string>>(response_result);

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
                        DateTime.ParseExact(temp, "dd.MM.yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
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

        private Quote GetQuote(HtmlNode div_Author, HtmlNode div_content)
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
    }
}