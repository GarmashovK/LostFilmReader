﻿#pragma checksum "C:\Users\Константин\Documents\Visual Studio 2013\LostFilmReader\LostFilmReader\SerialPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "66C8F328FE2E50FE2F365257B10FE45A"
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
    
    
    public partial class SerialPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal Microsoft.Phone.Controls.Pivot ContentPivot;
        
        internal System.Windows.Controls.Image SerialImg;
        
        internal System.Windows.Controls.TextBlock SerialDesc;
        
        internal LostFilmReader.Controls.NewsListView NewsListBox;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/LostFilmReader;component/SerialPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ContentPivot = ((Microsoft.Phone.Controls.Pivot)(this.FindName("ContentPivot")));
            this.SerialImg = ((System.Windows.Controls.Image)(this.FindName("SerialImg")));
            this.SerialDesc = ((System.Windows.Controls.TextBlock)(this.FindName("SerialDesc")));
            this.NewsListBox = ((LostFilmReader.Controls.NewsListView)(this.FindName("NewsListBox")));
        }
    }
}

