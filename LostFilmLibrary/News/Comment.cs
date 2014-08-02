using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostFilmLibrary.News
{
    public class Comment
    {
        public string UserDetails { get; set; }
        public string UserName { get; set; }
        public string Micro { get; set; }
        public string Image { get; set; }
        public uint ID { get; set; }
        public DateTime Date { get; set; }
        public CommentContent Content { get; set; }

        public string DoQuote()
        {
            return "[quote=" + UserName + "]" + ContentQuote(Content) + "[/quote]";
        }

        public override string ToString()
        {
            return DoQuote();
        }

        private string ContentQuote(CommentContent content)
        {
            var result = "";

            foreach (var item in content)
            {
                if (item is CommentText)
                    result += ((CommentText)item).Text;
                else if (item is Quote){
                    var quote = (Quote)item;
                    result += "[quote=" + quote.Author + "]" + ContentQuote(quote.Content) + "[/quote]";
                }
            }

            return result;
        }
    }
}
