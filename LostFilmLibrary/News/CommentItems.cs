using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostFilmLibrary.News
{
    public interface ICommentItem { }

    public class CommentText : ICommentItem
    {
        public string Text { get; set; }
    }

    public class CommentImage : ICommentItem
    {
        public string Url { get; set; }
    }

    public class Quote : ICommentItem
    {
        public string Author { get; set; }
        public CommentContent Content { get; set; }
    }

    public class CommentContent : List<ICommentItem> { }
}
