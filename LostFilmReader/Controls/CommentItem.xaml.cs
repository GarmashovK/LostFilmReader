﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LostFilmLibrary.News;
using System.Windows.Data;

namespace LostFilmReader.Controls
{
    public partial class CommentItem : UserControl
    {
        public event EventHandler DoQuote;

        public CommentItem()
        {
            InitializeComponent();
        }

        private void QuoteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            DoQuote(DataContext, new EventArgs());
        }                
    }
}
