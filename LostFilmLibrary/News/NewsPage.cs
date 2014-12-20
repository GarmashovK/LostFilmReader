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
    internal static class Extensions
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
        public NewsPageContent PageContent { get; set; }
        public DateTime PublicationTime { get; set; }

        private const string CommonUrl = "http://www.lostfilm.tv/news.php?act=comments&id=";
        
        //internal static string SecurityKey { get; set; }

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
            //Comments = new ObservableCollection<Comment>();
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

            var mid = Common.GetMid(doc);

            //var comments = GetComments(mid);
            //foreach (var item in comments)
            //    Comments.Add(item);

            Title = GetTitle(mid);
            var contentBody = Common.GetContentBody(mid);
            var content = GetContent(contentBody);

            PageContent = GetPageContent(content);
            PublicationTime = GetTime(contentBody);
        }

        public void SetNews(HtmlNodeCollection mid)
        {
            //var comments = GetComments(mid);
            //foreach (var item in comments)
            //    Comments.Add(item);

            Title = GetTitle(mid);
            var contentBody = Common.GetContentBody(mid);
            var content = GetContent(contentBody);

            PageContent = GetPageContent(content);
            PublicationTime = GetTime(contentBody);
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
    }
}
