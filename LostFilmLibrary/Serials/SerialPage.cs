using HtmlAgilityPack;
using System;
using System.Threading.Tasks;

namespace LostFilmLibrary.Serials
{
    public class SerialPage
    {
        public string Country { get; set; }
        public string ReleaseYear { get; set; }
        public string Genre { get; set; }
        public bool Status { get; set; }

        public async Task Load(string pageUrl)
        {
            var page = await Common.GetPage(pageUrl);
            var doc = new HtmlDocument();
            doc.LoadHtml(page);


        }
    }
}
