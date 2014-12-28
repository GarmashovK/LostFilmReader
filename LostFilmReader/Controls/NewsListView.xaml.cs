using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Threading.Tasks;
using LostFilmLibrary.News;

namespace LostFilmReader.Controls
{
    public partial class NewsListView : UserControl
    {
        //public NewsLoader Loader { get; set; }
        public NewsLoader _newsLoader { get; set; }
        private uint counter { get; set; }
        private bool _stop = false;

        public NewsListView()
        {
            InitializeComponent();

            _newsLoader = new NewsLoader();
            counter = 0;
        }

        public NewsListView(NewsLoader loader)
        {
            InitializeComponent();

            _newsLoader = loader;
            counter = 0;
        }

        public async Task LoadNextPage()
        {
            if (!_stop)
            {
                counter += 10;
                await LoadNewsList();
            }
            
        }

        public async Task LoadNewsList()
        {
            SystemTray.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator.IsVisible = true;
            SystemTray.ProgressIndicator.IsIndeterminate = true;
            try
            {
                await _newsLoader.LoadNewsAsync(counter);
            }
            catch (Exception exception)
            {
                if (exception is NewsNotFoundException)
                    _stop = true;
                counter -= 10;
                MessageBox.Show(exception.Message);
            }

            SystemTray.ProgressIndicator.IsVisible = false;
        }

        private void View_Loaded(object sender, RoutedEventArgs e)
        {
            NewsList.ItemsSource = _newsLoader.NewsList;
        }
    }
}
