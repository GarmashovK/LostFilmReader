using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using LostFilmLibrary.News;
using System.Threading.Tasks;

namespace LostFilmReader
{
    public partial class NewsPage : PhoneApplicationPage
    {
        private string NewsLink { get; set; }
        private uint Id { get; set; }

        public NewsPage()
        {            
            InitializeComponent();
            
            //var id = NavigationContext.QueryString;
            
            //NavigationContext.QueryString.TryGetValue("id",out tempId);
            //NavigationContext.QueryString.TryGetValue("Link",out tempLink);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //var dic = this.NavigationContext.QueryString;
            Id = uint.Parse(this.NavigationContext.QueryString["id"]);
            NewsLink = this.NavigationContext.QueryString["Link"];
            TitleBox.Text = this.NavigationContext.QueryString["Title"];
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadPage();
        }

        private async Task LoadPage()
        {
            SystemTray.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator.IsVisible = true;
            SystemTray.ProgressIndicator.IsIndeterminate = true;

            var NewsPageModel = new LostFilmLibrary.News.NewsPage();

            try
            {
                await NewsPageModel.LoadNewsPageAsync(Id);
                NewsPageContent.NewsContent = NewsPageModel.PageContent;
            }
            catch (Exception)
            {
                MessageBox.Show("Проблемы с соединением!");
            }
            SystemTray.ProgressIndicator.IsVisible = false;
        }

        //private void MainPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (MainPivot.SelectedIndex == 1)
        //    {
        //        //if comments is display it
        //        if (CommentsViewer.ItemsSource.Count != 0)
        //            ApplicationBar.IsVisible = true;
        //    }
        //    else
        //        ApplicationBar.IsVisible = false;
        //}

        //private void PrevButton_Click(object sender, EventArgs e)
        //{

        //}

        private async void RefreshButton_Click(object sender, EventArgs e)
        {
            await LoadPage();
        }

        private void GoToCommentButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/CommentsPage.xaml" + "?Link=" + NewsLink + "&Id=" + Id, UriKind.Relative));
        }

        private void ShareBtn_Click(object sender, EventArgs e)
        {
            var shareTask = new ShareLinkTask();

            shareTask.Title = shareTask.Message = TitleBox.Text;
            shareTask.LinkUri = new Uri(NewsLink);
            
            shareTask.Show();
        }

        //private void NextButton_Click(object sender, EventArgs e)
        //{

        //}
    }
}