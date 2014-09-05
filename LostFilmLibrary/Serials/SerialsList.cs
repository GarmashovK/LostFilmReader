using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostFilmLibrary.Serials
{
    public class SerialsList : ObservableCollection<SerialItem>
    {
        public async Task LoadAsync()
        {
            var url = "http://www.lostfilm.tv/serials.php";
            var page = await Common.GetPage(url);
            var doc = new HtmlDocument();
            doc.LoadHtml(page);

            var mid = Common.GetMid(doc);
            var bb = GetBB(mid);
            LoadItems(bb);
        }

        private void LoadItems(HtmlNode bb)
        {
            var nodes = bb.ChildNodes.Where(n => n.Name =="a").ToList();

            foreach (HtmlNode item in nodes)
            {
                var serialItem = new SerialItem();
                var tmpNames = item.InnerText.Replace("\n","");

                serialItem.Url = "http://www.lostfilm.tv" + item.Attributes["href"].Value;
                serialItem.Name = GetName(tmpNames);
                serialItem.OriginalName = GetOriginalName(tmpNames);

                this.Add(serialItem);
            }

        }

        private string GetOriginalName(string tmpNames)
        {
            var posStart = tmpNames.IndexOf("(");
            var len = tmpNames.IndexOf(")") - posStart - 1;

            return tmpNames.Substring(posStart + 1, len);
        }

        private string GetName(string tmpNames)
        {
            var len = tmpNames.IndexOf("(");

            return tmpNames.Substring(0, len);
        }

        private HtmlNode GetBB(HtmlNodeCollection mid)
        {
            return mid.First(n => n.Attributes.Contains("class") && n.Attributes["class"].Value == "bb");
        }
    }
}
