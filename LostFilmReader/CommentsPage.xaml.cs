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
        private uint CommentsPosition { get; set; }
        private LostFilmLibrary.News.NewsPage NewsPageModel { get; set; }

        public CommentsPage()
        {
            InitializeComponent();

            NewsPageModel = new LostFilmLibrary.News.NewsPage();
            CommentsPosition = 0;
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
            CommentsViewer.CommentsLoaded += CommentsViewer_CommentsLoaded;
                        
            LoadComments(CommentsPosition);           
        }

        private void CommentsViewer_CommentsLoaded(object sender, EventArgs e)
        {
            var tmpChildren = ((StackPanel)CommentsViewer.ListView).Children;
            
            foreach (Controls.CommentItem item in tmpChildren)
            {
                item.DoQuote += QuoteMenuItem_Click;
            }
        }

        private async void LoadComments(uint start)
        {
            if (CommentsPosition == null)
                CommentsPosition = 0;

            //----------- Загрузка комментариев
            SystemTray.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator.IsVisible = true;
            SystemTray.ProgressIndicator.IsIndeterminate = true;

            try
            {
                NewsPageModel.Comments.Clear();
                NumOfComments = await NewsPageModel.LoadCommentsAsync(Id, start);
                CommentsViewer.ItemsSource = NewsPageModel.Comments;

                if (CommentsPosition + 20 > NumOfComments)
                    ((ApplicationBarIconButton)ApplicationBar.Buttons[3]).IsEnabled = false;
                if (CommentsPosition < 20)
                    ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = false;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            SystemTray.ProgressIndicator.IsVisible = false;

            CommentsPositionBlock.Text = CommentsPosition.ToString() + '/' + NumOfComments.ToString();
            
            //----------- Окончание загрузки
        }

        private void QuoteMenuItem_Click(object sender, EventArgs e)
        {
            // adding quote to the CommentBox
            var data = (LostFilmLibrary.News.Comment)sender;

            CommentBox.Text += data;
        }

        private async void PostCommentButton_Click(object sender, EventArgs e)
        {
            // comment posting
            if (CommentBox.Text != null && CommentBox.Text != string.Empty)
            {
                var result = await NewsPageModel.PostComment(Id, CommentBox.Text);

                switch (result)
                {
                        //successful result
                    case "ok":
                        LoadComments(CommentsPosition);
                        MessageBox.Show("Комментарий успешно добавлен.");
                        break;
                        // bad results
                    case "forbidden":
                        MessageBox.Show("Запрещено.");
                        break;
                    case "deleted":
                        MessageBox.Show("Комментарий уже удален.");
                        break;
                    case "required_error":
                        MessageBox.Show("Ошибка. Не все обязательные поля присутствуют.");
                        break;
                    case "bad_words":
                        MessageBox.Show("Ошибка. Ваш комментарий не может быть отправлен. Он нарушает ПРАВИЛА сайта.");
                        break;
                    case "lol":
                        MessageBox.Show("Ошибка. Обсуждать пока нечего.");
                        break;
                    case "newbee":
                        MessageBox.Show("Возможность комментирования активируется на 2-е сутки после регистрации.");
                        break;
                    case "closed":
                        MessageBox.Show("Возможность комментирования данной новости отключена.");
                        break;
                    case "system_error":
                        MessageBox.Show("Ошибка. Отсутствует, либо неверен ключ.");
                        break;
                }
                CommentBox.Text = "";
            }
            else
                MessageBox.Show("Пустой комментарий!");
        }

        private void PrevButton_Click(object sender, EventArgs e)
        {
            if (CommentsPosition >= 20 && CommentsPosition <= NumOfComments)
            {
                if (CommentsPosition == 20)
                {
                    ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = false;
                    ((ApplicationBarIconButton)ApplicationBar.Buttons[3]).IsEnabled = true;
                }
                CommentsPosition -= 20;
                LoadComments(CommentsPosition);
            }
            else
                ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = false;
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            if (CommentsPosition + 20 < NumOfComments)
            {
                if (CommentsPosition == 0)
                    ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = true;
                CommentsPosition += 20;
                LoadComments(CommentsPosition);
            }
            else
                ((ApplicationBarIconButton)ApplicationBar.Buttons[3]).IsEnabled = false;
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            LoadComments(CommentsPosition);
        }
    }
}