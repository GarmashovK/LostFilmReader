using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LostFilmLibrary.News;

namespace LostFilmReader.Controls
{
    public partial class NewsPageView : UserControl
    {
        private NewsPageContent _content;
        public NewsPageContent NewsContent { get { return _content; } set { _content = value; ContentChanged(this, new EventArgs()); } }
        public EventHandler ContentChanged { get; set; }

        public NewsPageView()
        {
            InitializeComponent();
            ContentChanged += OnContentChanged;
        }

        private void OnContentChanged(object sender, EventArgs e)
        {
            LayoutRoot.Content = GetContentPanel();
        }

        private UIElement GetContentPanel()
        {
            var panel = new StackPanel();
            panel.Orientation = Orientation.Vertical;
            
            foreach (var item in NewsContent)
            {
                if (item is TextItem)
                {
                    var textBlock = new TextBlock();
                    textBlock.TextWrapping = TextWrapping.Wrap;
                    textBlock.Text = ((TextItem)item).Text;
                    panel.Children.Add(textBlock);
                }
                else if (item is ImageItem)
                {
                    var image = new Image();
                    image.Source = new BitmapImage(((ImageItem)item).Url);
                    image.Stretch = System.Windows.Media.Stretch.UniformToFill;

                    panel.Children.Add(image);
                }
                else if (item is VideoItem)
                {                    
                    var videoItem = (VideoItem)item;

                    var grid = new Grid();
                    grid.DataContext = item;
                    
                    var image = new Image();
                    image.Source = new BitmapImage(videoItem.Screen);
                    image.Stretch = System.Windows.Media.Stretch.UniformToFill;
                    
                    grid.Children.Add(image);

                    var playText = new TextBlock();
                    playText.FontWeight = FontWeights.Bold;
                    playText.Text = "Воспроизвести";
                    playText.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    playText.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    //var play_image = new Image();
                    //play_image.Source = new BitmapImage(new Uri("Assets/play.png", UriKind.Relative));
                    //play_image.Stretch = System.Windows.Media.Stretch.None;
                    //play_image.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    //play_image.VerticalAlignment = System.Windows.VerticalAlignment.Center;

                    grid.Children.Add(playText);

                    var btn = new Button();
                    btn.DataContext = videoItem;
                    btn.Content = grid;
                    btn.Click += playButtonClick;

                    panel.Children.Add(btn);
                }
            }

            return panel;
        }

        private void playButtonClick(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var item = (VideoItem)btn.DataContext;

            var mediaPlayerLauncher = new MediaPlayerLauncher();

            mediaPlayerLauncher.Media = item.Url;
            mediaPlayerLauncher.Location = MediaLocationType.Data;
            mediaPlayerLauncher.Controls = MediaPlaybackControls.All;
            mediaPlayerLauncher.Orientation = MediaPlayerOrientation.Landscape;

            mediaPlayerLauncher.Show();
        }
    }
}
