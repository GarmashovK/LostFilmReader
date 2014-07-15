using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace LostFilmLibrary.News
{

    public class NewsLoader
    {
        public ObservableCollection<NewsItem> NewsList { get; set; }

        public NewsLoader()
        {
            NewsList = new ObservableCollection<NewsItem>();
        }

        public async Task LoadNewsAsync(uint start)
        {
            await LoadNewsAsync("http://www.lostfilm.tv/news.php?o=" + start);
        }
        
        public async Task LoadNewsAsync(string url)
        {
            var page = await Common.GetPage(url);

            var doc = new HtmlDocument();
            doc.LoadHtml(page);

            var contentBody = GetContentBody(doc);

            var titles = GetTitles(contentBody);
            var images = GetImages(contentBody);
            var othersOptions = GetSomeOptions(contentBody);


            const string lostfilmLink = "http://www.lostfilm.tv";
            
            for (var i = 0; i < titles.Length; i++)
            {
                NewsList.Add(new NewsItem
                {
                    Title = titles[i],
                    Image = lostfilmLink + images[i],
                    Link = lostfilmLink + (string)othersOptions[i]["link"],
                    PulbicationTime = (DateTime)othersOptions[i]["time"]
                });
            }
        }

        private Dictionary<string, object>[] GetSomeOptions(IEnumerable<HtmlNode> contentBody)
        {
            return contentBody
                .Where(n => n.Name == "p" && n.InnerText != ""
                    && n.ChildNodes.FindFirst("table") != null)
                        .Select(n =>
                        {
                            var dictionary = new Dictionary<string, object>();
                            //var text = n.InnerText.Trim();
                            //bool spoilers = false;
                            
                            //uint numOfComments;
                            
                            var neededTr = n.ChildNodes.FindFirst("table").ChildNodes.Last(node => node.Name == "tr");
                            var span = neededTr.ChildNodes.FindFirst("td").ChildNodes.FindFirst("span");
                            var a = neededTr.ChildNodes.FindFirst("td").ChildNodes.FindFirst("span").ChildNodes.FindFirst("a");
                            var innerText = span.InnerText;
                                                       
                            dictionary.Add("link", a.Attributes["href"].Value);
                            dictionary.Add("time", GetTime(innerText));

                            return dictionary;
                        })
                        .ToArray();
        }
        
        private DateTime GetTime(string innerText)
        {
            DateTime result;
            var fst = innerText.IndexOf("Дата: ") + 6;
            var snd = innerText.IndexOf('\n', fst + 1);

            var time = innerText.Substring(fst, snd - fst).Trim().Trim('.');

            DateTime.TryParse(time, out result);
            
            return result;
        }
        
        private string[] GetImages(IEnumerable<HtmlNode> contentBody)
        {
            return contentBody
                .Where(n => n.Name == "img" || n.Name == "p" && n.Attributes.Contains("style")
                    && n.Attributes["style"].Value == "min-height:1px;margin-top:10px;" 
                    && n.ChildNodes.FindFirst("img") != null
                )
                .Select(n =>
                {
                    switch (n.Name)
                    {
                        case "img":
                            return n.Attributes["src"].Value;
                        case "p":
                            return n.ChildNodes.FindFirst("img").Attributes["src"].Value;
                        default:
                            return "";
                    }                        
                }).ToArray();
        }

        private string[] GetTitles(IEnumerable<HtmlNode> contentBody)
        {
            return contentBody.Where(n => n.Name == "h1").Select(n => Common.ReplaceSomeSymbols(n.InnerText)).ToArray();
        }

        private HtmlNodeCollection GetContentBody(HtmlDocument doc)
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
                .ChildNodes
                .First(
                    n =>
                        n.Name == "div" && n.Attributes.Contains("class") &&
                        n.Attributes["class"].Value == "content_body").ChildNodes;
        }
    }
}
