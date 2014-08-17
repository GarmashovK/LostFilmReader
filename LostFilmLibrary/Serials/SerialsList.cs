using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostFilmLibrary.Serials
{
    class SerialsList : ObservableCollection<SerialItem>
    {
        public Task LoadAsync()
        {
            var url = "http://www.lostfilm.tv/serials.php";
            var page = await Common.GetPage(url);
            var doc = new HtmlDocument();
            doc.LoadHtml(page);

            var mid = Common.GetMid(doc);
        }
    }
}
