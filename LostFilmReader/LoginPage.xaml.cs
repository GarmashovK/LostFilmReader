using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.IO.IsolatedStorage;
using LostFilmLibrary;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace LostFilmReader
{
    public partial class LoginPage : PhoneApplicationPage
    {
        public LoginPage()
        {
            InitializeComponent();      
        }        

        private async void LoginButton_OnClick(object sender, RoutedEventArgs e)
        {
            var pass = PassBox.Password;
            var email = EmailBox.Text;

            if (email == string.Empty || pass == string.Empty)
            {
                MessageBox.Show("Заполните поля");
                return;
            }

            SystemTray.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator.IsVisible = true;
            SystemTray.ProgressIndicator.IsIndeterminate = true;

            var settings = IsolatedStorageSettings.ApplicationSettings;

            try
            {
                var cookies = await Authorization.TryLogin(email, pass);
                settings["cookies"] = LFOptions.Cookies = cookies;
                settings["IsAuthorized"] = true;
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
            catch (Exception exc)
            {
                settings["IsAuthorized"] = false;
                MessageBox.Show(exc.Message);
            }

            settings.Save();
            SystemTray.ProgressIndicator.IsVisible = false;
        }
    }
}