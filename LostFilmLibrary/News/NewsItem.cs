using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostFilmLibrary.News
{
    public class NewsItem
    {
        public string Title { get; set; }
        //public string Description { get; set; }
        public string Link { get; set; }
        public string Image { get; set; }
        //public bool Spoilers { get; set; }
        //public string NComments { get; set; }
        public DateTime PulbicationTime { get; set; }
    }
}
