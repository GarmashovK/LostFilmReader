﻿#pragma checksum "C:\Users\Константин\Documents\Visual Studio 2013\LostFilmReader\LostFilmReader\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "52639CEF06F2604DE14174343861FFA5"
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
    
    
    public partial class MainPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton PrevButton;
        
        internal Microsoft.Phone.Shell.ApplicationBarMenuItem SerialsListBtn;
        
        internal Microsoft.Phone.Shell.ApplicationBarMenuItem LogoutItem;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal Microsoft.Phone.Controls.Pivot MyPivot;
        
        internal LostFilmReader.Controls.NewsListView NewsList;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/LostFilmReader;component/MainPage.xaml", System.UriKind.Relative));
            this.PrevButton = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("PrevButton")));
            this.SerialsListBtn = ((Microsoft.Phone.Shell.ApplicationBarMenuItem)(this.FindName("SerialsListBtn")));
            this.LogoutItem = ((Microsoft.Phone.Shell.ApplicationBarMenuItem)(this.FindName("LogoutItem")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.MyPivot = ((Microsoft.Phone.Controls.Pivot)(this.FindName("MyPivot")));
            this.NewsList = ((LostFilmReader.Controls.NewsListView)(this.FindName("NewsList")));
        }
    }
}

