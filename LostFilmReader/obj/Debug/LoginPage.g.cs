﻿#pragma checksum "C:\Users\Константин\Documents\Visual Studio 2013\LostFilmReader\LostFilmReader\LoginPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "9368FD6AE7740F5D6BCB042CB6E433B8"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace LostFilmReader {
    
    
    public partial class LoginPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.StackPanel LayoutRoot;
        
        internal System.Windows.Controls.TextBox EmailBox;
        
        internal System.Windows.Controls.PasswordBox PassBox;
        
        internal System.Windows.Controls.Button LoginButton;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/LostFilmReader;component/LoginPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.StackPanel)(this.FindName("LayoutRoot")));
            this.EmailBox = ((System.Windows.Controls.TextBox)(this.FindName("EmailBox")));
            this.PassBox = ((System.Windows.Controls.PasswordBox)(this.FindName("PassBox")));
            this.LoginButton = ((System.Windows.Controls.Button)(this.FindName("LoginButton")));
        }
    }
}

