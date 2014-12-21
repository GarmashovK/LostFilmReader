using HtmlAgilityPack;
using LostFilmLibrary.News;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LostFilmLibrary.Serials
{
    public class SerialPage : NewsLoader
    {
        public string Title { get; private set; }
        public string Image { get; private set; }
        public string Country { get; private set; }
        public string ReleaseYear { get; private set; }
        public string Genre { get; private set; }
        public string Description { get; private set; }
        public bool Status { get; private set; }

        public async Task Load(string pageUrl)
        {
            var page = await Common.GetPage(pageUrl);
            var doc = new HtmlDocument();
            doc.LoadHtml(page);
            var mid = Common.GetMid(doc);

            SetNews(doc);
            SetDescription(mid.FindFirst("div"));
        }

        private void SetDescription(HtmlNode content)
        {
            var desc_nodes = content.ChildNodes.Where(n => n.Name != "div" || n.Name != "br");
            Description = Common.ReplaceSomeSymbols(
                content.ChildNodes.LastOrDefault(n => n.Name == "span").InnerText.Trim()
            );

            var enumerator = desc_nodes.GetEnumerator();

            do
            {
                if (enumerator.Current.Name == "h1")
                    Title = GetTitle(enumerator.Current);
                else if (enumerator.Current.Name == "img")
                    Image = GetImage(enumerator.Current);
                else
                {

                }
            } while (enumerator.MoveNext());
        }

        private string GetTitle(HtmlNode node)
        {
            if (node.Name != "h1") return null;
            return node.InnerText;
        }

        private string GetImage(HtmlNode node)
        {
            if (node.Name != "img") return null;
            return "http://www.lostfilm.tv" + node.Attributes["href"].Value;
        }
    }
}