﻿using HtmlAgilityPack;
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
        //public string Country { get; private set; }
        //public string ReleaseYear { get; private set; }
        //public string Genre { get; private set; }
        public string Description { get; private set; }
        //public bool Status { get; private set; }

        public async Task Load()
        {
            var page = await Common.GetPage(this.Link);
            var doc = new HtmlDocument();
            doc.LoadHtml(page);
            var mid = Common.GetMid(doc);

            SetNews(doc);
            SetContent(mid.FindFirst("div"));
        }

        private void SetContent(HtmlNode content)
        {
            Title = content.ChildNodes.FindFirst("h1").InnerText;
            Image = "https://www.lostfilm.tv" + content.ChildNodes.FindFirst("img").Attributes["src"].Value;
            Description = Common.ReplaceSomeSymbols(
                content.ChildNodes.LastOrDefault(n => n.Name == "span").InnerText.Trim()
            );
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