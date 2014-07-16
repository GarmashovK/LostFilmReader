using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using LostFilmLibrary.News;

namespace LostFilmReader.Controls
{
    public partial class CommentsView : UserControl
    {
        private ObservableCollection<Comment> _items;
        public ObservableCollection<Comment> ItemsSource 
        { 
            get { return _items; }
            set
            {
                _items = value;
                OnItemsSourceChanged();
                _items.CollectionChanged += CollectionChanged;
            }
        }

        public event EventHandler CommentsLoaded;

        private void CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //switch (e.Action)
            //{
            //    case NotifyCollectionChangedAction.Add:
            //        {
            //            var startIndex = e.NewStartingIndex;
            //            var count = e.NewItems.Count;
            //            break;
            //        }
            //    case NotifyCollectionChangedAction.Remove:
            //        {                        
            //            var startIndex = e.OldStartingIndex;
            //            var count = e.OldItems.Count;

            //            for (int i = 0; i < count; i++)
            //                ListView.Children.RemoveAt(startIndex);

            //            break;
            //        }
            //}
        }

        
        private void OnItemsSourceChanged()
        {
            ListView.Children.Clear();
            if (ItemsSource.Count == 0)
            {
                var noComments = new TextBlock();
                noComments.Text = "Нет комментариев";
                noComments.FontSize = 32;
                ListView.Children.Add(noComments);
                return;
            }
                
            foreach (var item in ItemsSource)
            {
                var commentItem = new CommentItem();
                commentItem.DataContext = item;
                //try
                //{
                    commentItem.ImageBox.Source = new BitmapImage(new Uri(item.Image));
                //}
                //catch (UriFormatException e)
                //{
                //    commentItem.ImageBox.Source = new BitmapImage(new Uri("http://www.lostfilm.tv/Tmpl/LostFilm/img/avatar.gif"));
                //}
                commentItem.NickNameTextBox.Text = item.UserName;
                commentItem.DateBox.Text = item.Date.ToString();
                commentItem.Margin = new Thickness(0, 5, 0, 5);
                commentItem.LayoutRoot.Children.Add(GetContent(item.Content));

                ListView.Children.Add(commentItem);
            }

            CommentsLoaded(this, new EventArgs());
        }

        public CommentsView()
        {
            InitializeComponent();
        }


        private static UIElement GetContent(CommentContent content)
        {
            var panel = new StackPanel();
            panel.Margin = new Thickness(2, 0, 2, 0);
            panel.Orientation = System.Windows.Controls.Orientation.Vertical;

            foreach (var item in content)
            {
                if (item is Quote)
                {
                    var border = new Border();
                    border.BorderThickness = new Thickness(1);
                    border.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
                    border.Child = GetQuote((Quote)item);

                    panel.Children.Add(border);
                }
                else
                    if (item is CommentText)
                    {
                        var textBox = new TextBlock();
                        textBox.TextWrapping = TextWrapping.Wrap;

                        textBox.Text = ((CommentText)item).Text;
                        panel.Children.Add(textBox);
                    }
            }

            return panel;
        }

        private static UIElement GetQuote(Quote quote)
        {
            var panel = new StackPanel();

            var userNameBox = new TextBlock();
            userNameBox.FontWeight = FontWeights.Bold;
            userNameBox.Text = quote.Author;

            panel.Children.Add(userNameBox);
            panel.Children.Add(GetContent(quote.Content));

            return panel;
        }
    }
}
