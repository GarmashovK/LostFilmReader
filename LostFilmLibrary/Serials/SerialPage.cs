using HtmlAgilityPack;
using LostFilmLibrary.News;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LostFilmLibrary.Serials
{
    public class SerialPage : NewsPage
    {
        public string Country { get; private set; }
        public string ReleaseYear { get; private set; }
        public string Genre { get; private set; }
        public string Description { get; private set; }
        public bool Status { get; set; }

        public async Task Load(string pageUrl)
        {
            var page = await Common.GetPage(pageUrl);
            var doc = new HtmlDocument();
            doc.LoadHtml(page);
            var mid = Common.GetMid(doc);

            SetNews(mid);
            SetDescription(mid.FindFirst("div"));
        }

        private void SetDescription(HtmlNode content)
        {
            var content_nodes = content.ChildNodes.Where(n => n.Name != "div" && n.Name != "br");
            
        }
    }
}
