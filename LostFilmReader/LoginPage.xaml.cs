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
                LFOptions.Cookies = cookies;
                SaveCookies(cookies);
                settings["IsAuthorized"] = true;
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
            catch (Exception)
            {
                settings["IsAuthorized"] = false;
                MessageBox.Show("Проблемы с соединением!");
            }

            settings.Save();
            SystemTray.ProgressIndicator.IsVisible = false;
        }

        private static void SaveCookies(CookieContainer cookies)
        {
            string strCookies = "";

            foreach (Cookie item in cookies
                .GetCookies(new Uri("http://www.lostfilm.tv/")))
            {
                strCookies += item.Name + '=' + item.Value + ';';
            }
            IsolatedStorageSettings.ApplicationSettings["cookies"] = strCookies;
        }

        public static void LoadCookies()
        {
            string strCookies = IsolatedStorageSettings.ApplicationSettings["cookies"] as string;
            var cookies = LFOptions.Cookies = new CookieContainer();
            var cookieUri = new Uri("http://www.lostfilm.tv/");

            do
            {
                var tmpStr = strCookies.Substring(0, strCookies.IndexOf(';'));

                int sign = tmpStr.IndexOf('=');

                var cookieName = tmpStr.Substring(0, sign);
                var cookieValue = tmpStr.Substring(sign + 1);

                cookies.Add(cookieUri,
                    new Cookie(cookieName, cookieValue));
                strCookies = strCookies.Substring(strCookies.IndexOf(';') + 1);

            } while (strCookies.Length != 0);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {
                while (1)
                    NavigationService.RemoveBackEntry();
            }
        }
    }
}