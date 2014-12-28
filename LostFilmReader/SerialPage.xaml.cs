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

namespace LostFilmReader
{
    public partial class SerialPage : PhoneApplicationPage
    {
        private LostFilmLibrary.Serials.SerialPage _page;

        public SerialPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (_page == null)
            {
                _page = new LostFilmLibrary.Serials.SerialPage();
                _page.Link = NavigationContext.QueryString["link"];
                NewsListBox.SetLoader(_page);
            }
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            SystemTray.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator.IsVisible = true;
            SystemTray.ProgressIndicator.IsIndeterminate = true;

            try
            {
                await _page.Load();

                ContentPivot.Title = _page.Title;
                SerialImg.Source = new BitmapImage(new Uri(_page.Image));
                SerialDesc.Text = _page.Description;
                _page.Link = "http://www.lostfilm.tv/news.php?cat="
                    + _page.Link.Substring(_page.Link.IndexOf("cat="));
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }

            SystemTray.ProgressIndicator.IsVisible = false;
        }
    }
}