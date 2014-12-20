using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using LostFilmLibrary.News;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LostFilmReader.Resources;
using LostFilmLibrary;
using LostFilmReader.Controls;

namespace LostFilmReader
{
    public partial class MainPage : PhoneApplicationPage
    {
        

        // Конструктор
        public MainPage()
        {
            InitializeComponent();

            //_newsLoader = new NewsLoader();
            //counter = 0;
            // Пример кода для локализации ApplicationBar
            //BuildLocalizedApplicationBar();

            //((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = false;            
        }

        // Пример кода для сборки локализованной панели ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Установка в качестве ApplicationBar страницы нового экземпляра ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Создание новой кнопки и установка текстового значения равным локализованной строке из AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Создание нового пункта меню с локализованной строкой из AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //очистить последовательность переходов
            NavigationService.RemoveBackEntry();

            //NextButton.IsEnabled = false
            if (NewsList.NewsList.ItemsSource != null || 
                !System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings.Contains("IsAuthorized") ||
                (bool)System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings["IsAuthorized"] == false) return;
            //NewsListView.ItemsSource = _newsLoader.NewsList;
            //await LoadNewsList();
            NewsList.LoadNewsList();
        }


        //private async System.Threading.Tasks.Task LoadNewsList()
        //{
        //    SystemTray.ProgressIndicator = new ProgressIndicator();
        //    SystemTray.ProgressIndicator.IsVisible = true;
        //    SystemTray.ProgressIndicator.IsIndeterminate = true;
        //    try
        //    {
        //        await _newsLoader.LoadNewsAsync(counter);
        //    }
        //    catch (Exception exception)
        //    {
        //        counter -= 10;
        //        MessageBox.Show(exception.Message);
        //    }

        //    SystemTray.ProgressIndicator.IsVisible = false;
        //}

        private void NewsListView_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var item = (NewsItem)NewsList.NewsList.SelectedItem;
            if (item != null)
                NavigationService.Navigate(new Uri("/NewsPage.xaml" + "?Title=" + item.Title + "&Link=" + item.Link, UriKind.Relative));
        }

        //private async void NextButton_Click(object sender, EventArgs e)
        //{
        //    if (counter >= 10)
        //    {
        //        counter -= 10;
        //        if (counter == 0) ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = false;
        //    }
        //    else return;

        //    _newsLoader.NewsList.Clear();
        //    await LoadNewsList();
        //}
        
        private async void PrevButton_Click(object sender, EventArgs e)
        {
            //if (counter == 0)
            //    ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = true;
            //counter += 10;

            //await LoadNewsList();
            NewsList.LoadNextPage();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {            
            if (!System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings.Contains("IsAuthorized") ||
                (bool)System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings["IsAuthorized"] == false)
            {
                GoToLoginPage();
            }
            else
            {
                NewsList.NewsList.Tap += NewsListView_Tap;
            }
        }

        private void LogoutItem_Click(object sender, EventArgs e)
        {
            GoToLoginPage();
        }

        private void GoToLoginPage()
        {
            NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
        }
    }
}