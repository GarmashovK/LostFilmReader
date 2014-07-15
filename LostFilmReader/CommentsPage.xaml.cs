using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace LostFilmReader
{
    public partial class CommentsPage : PhoneApplicationPage
    {
        private uint Id { get; set; }
        private string Link { get; set; }
        private uint NumOfComments { get; set; }
        private LostFilmLibrary.News.NewsPage NewsPageModel { get; set; }

        public CommentsPage()
        {
            InitializeComponent();

            NewsPageModel = new LostFilmLibrary.News.NewsPage();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
                                   
            // Getting data after navigation
            Id = uint.Parse(this.NavigationContext.QueryString["Id"]);
            Link = this.NavigationContext.QueryString["Link"];
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadComments(0);

            var tmpChildren = ((StackPanel)CommentsViewer.ListView).Children;
            foreach (Controls.CommentItem item in tmpChildren)
            {
                item.QuoteMenuItem.Click += QuoteMenuItem_Click;
            }
        }

        private async void LoadComments(uint start)
        {
            //----------- Загрузка комментариев
            SystemTray.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator.IsVisible = true;
            SystemTray.ProgressIndicator.IsIndeterminate = true;

            try
            {
                NumOfComments = await NewsPageModel.LoadCommentsAsync(Id, start);
                CommentsViewer.ItemsSource = NewsPageModel.Comments;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            SystemTray.ProgressIndicator.IsVisible = false;
            //----------- Окончание загрузки
        }

        private void QuoteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var tmp = (Controls.CommentItem)sender;
            var data = (LostFilmLibrary.News.Comment)tmp.DataContext;

            CommentBox.Text += data;
        }

        private void PostCommentButton_Click(object sender, EventArgs e)
        {
            NewsPageModel.PostComment(Id, CommentBox.Text);
        }

        private void PrevButton_Click(object sender, EventArgs e)
        {

        }

        private void NextButton_Click(object sender, EventArgs e)
        {

        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {

        }

        private void CommentBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CommentBox.Text == "" && PostCommentButton.IsEnabled)
                PostCommentButton.IsEnabled = false;
            if (CommentBox.Text != null && !PostCommentButton.IsEnabled)
                PostCommentButton.IsEnabled = true;
        }
    }
}