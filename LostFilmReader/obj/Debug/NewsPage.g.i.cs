﻿#pragma checksum "C:\Users\Константин\Documents\Visual Studio 2013\LostFilmReader\LostFilmReader\NewsPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "F936FD47199EA0191D3F497DD8FF962F"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using LostFilmReader.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
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
    
    
    public partial class NewsPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton RefreshButton;
        
        internal Microsoft.Phone.Shell.ApplicationBarMenuItem ShareBtn;
        
        internal Microsoft.Phone.Shell.ApplicationBarMenuItem GoToCommentButton;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.TextBlock TitleBox;
        
        internal LostFilmReader.Controls.NewsPageView NewsPageContent;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/LostFilmReader;component/NewsPage.xaml", System.UriKind.Relative));
            this.RefreshButton = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("RefreshButton")));
            this.ShareBtn = ((Microsoft.Phone.Shell.ApplicationBarMenuItem)(this.FindName("ShareBtn")));
            this.GoToCommentButton = ((Microsoft.Phone.Shell.ApplicationBarMenuItem)(this.FindName("GoToCommentButton")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.TitleBox = ((System.Windows.Controls.TextBlock)(this.FindName("TitleBox")));
            this.NewsPageContent = ((LostFilmReader.Controls.NewsPageView)(this.FindName("NewsPageContent")));
        }
    }
}

