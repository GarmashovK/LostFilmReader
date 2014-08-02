using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace LostFilmReader
{
    public partial class CommentsPage : PhoneApplicationPage
    {
        private uint Id { get; set; }
        private string Link { get; set; }
        private uint NumOfPage { get; set; }
        private uint CommentsPosition { get; set; }
        private LostFilmLibrary.News.CommentsPage CommentsPageModel { get; set; }

        private ApplicationBarIconButton NextButton;
        private ApplicationBarIconButton PrevButton;

        public CommentsPage()
        {
            InitializeComponent();

            CommentsPageModel = new LostFilmLibrary.News.CommentsPage();
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
            PrevButton = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
            NextButton = (ApplicationBarIconButton)ApplicationBar.Buttons[3];

            CommentsPosition = 0;
            await LoadComments();
        }

        private void CommentsViewer_CommentsLoaded(object sender, EventArgs e)
        {
            var tmpChildren = ((StackPanel)CommentsViewer.ListView).Children;
            
            foreach (Controls.CommentItem item in tmpChildren)
            {
                item.DoQuote += QuoteMenuItem_Click;
            }
        }

        private async Task LoadComments()
        {
            //----------- Загрузка комментариев
            SystemTray.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator.IsVisible = true;
            SystemTray.ProgressIndicator.IsIndeterminate = true;

            uint tmpNumOfComments;

            try
            {
                CommentsPageModel.Comments.Clear();
                tmpNumOfComments = await CommentsPageModel.LoadCommentsAsync(Id, CommentsPosition * 20);
                CommentsViewer.ItemsSource = CommentsPageModel.Comments;
                NumOfPage = tmpNumOfComments / 20;
                if (tmpNumOfComments % 20 > 0)
                    NumOfPage++;

                if (CommentsPosition == NumOfPage)
                    NextButton.IsEnabled = false;
                if (CommentsPosition == 0)
                    PrevButton.IsEnabled = false;

                CommentsPositionBlock.Text = (CommentsPosition + 1).ToString() + '/' + NumOfPage.ToString()
                    + '(' + tmpNumOfComments.ToString() + ')';
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            SystemTray.ProgressIndicator.IsVisible = false;
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
                var result = await CommentsPageModel.PostComment(Id, CommentBox.Text);

                switch (result)
                {
                        //successful result
                    case "ok":
                        CommentsPosition = NumOfPage - 1;
                        await LoadComments();
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

        private async void PrevButton_Click(object sender, EventArgs e)
        {
            if (CommentsPosition >= 0 && CommentsPosition <= NumOfPage)
            {                
                CommentsPosition--;
                await LoadComments();

                if (CommentsPosition == 0)
                    PrevButton.IsEnabled = false;
                NextButton.IsEnabled = true;
            }
        }

        private async void NextButton_Click(object sender, EventArgs e)
        {
            if (CommentsPosition + 1 < NumOfPage)
            {               
                CommentsPosition++;
                await LoadComments();

                if (CommentsPosition + 1 == NumOfPage)
                    NextButton.IsEnabled = false;
                PrevButton.IsEnabled = true;
            }
        }

        private async void RefreshButton_Click(object sender, EventArgs e)
        {
            await LoadComments();
        }
    }
}