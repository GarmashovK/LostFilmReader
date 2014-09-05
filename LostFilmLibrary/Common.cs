using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LostFilmLibrary
{
    public static class Common
    {
        public static Encoding LFEncoding { get; set; }

        public static async Task<string> GetPage(string url)
        {
            var handler = new HttpClientHandler();
            if (LFOptions.Cookies != null)
                handler.CookieContainer = LFOptions.Cookies;

            var client = new HttpClient(
                handler
                );

            var responseBytes = await client.GetByteArrayAsync(url);

            if (Common.LFEncoding == null)
                Common.LFEncoding = Encoding.GetEncoding("windows-1251");

            return Common.LFEncoding.GetString(responseBytes, 0, responseBytes.Length - 1);
        }
        
        internal static IEnumerable<HtmlNode> GetContentBody(IEnumerable<HtmlNode> mid)
        {
            return mid.First(
                n =>
                    n.Name == "div" && n.Attributes.Contains("class") &&
                    n.Attributes["class"].Value == "content_body").ChildNodes;
        }

        // вытащить SecurityKey
        internal static string GetSecure(HtmlNodeCollection mid)
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

        internal static HtmlNodeCollection GetMid(HtmlDocument doc)
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

        internal static string ReplaceSomeSymbols(string str)
        {
            string[] oldSym =
            {
                "&#1042;","&mdash;","&#39;","&rdquo;","&ldquo;","&laquo;","&raquo;","&nbsp;","&quot;","amp;","&#039;","&ndash;"
            };
            string[] newSym = { "В", "-", "'", "”", "“", "«", "»", " ", "\"", "", "'", "–" };

            for (int i = 0; i < oldSym.Length; i++)
            {
                str = str.Replace(oldSym[i], newSym[i]);
            }

            return str;
        }
    }
}
