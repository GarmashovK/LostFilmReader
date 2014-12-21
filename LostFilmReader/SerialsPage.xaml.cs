using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LostFilmLibrary.Serials;

namespace LostFilmReader
{
    public partial class SerialsPage : PhoneApplicationPage
    {
        private SerialsList Serials { get; set; }

        public SerialsPage()
        {
            InitializeComponent();

            Serials = new SerialsList();
        }

        private async void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            await Serials.LoadAsync();
        }
    }
}