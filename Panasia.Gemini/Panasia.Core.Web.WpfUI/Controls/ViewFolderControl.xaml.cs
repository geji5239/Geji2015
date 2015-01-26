﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Panasia.Core;
using Panasia.Core.Wpf;

namespace Panasia.Core.Web.Controls
{
    /// <summary>
    /// ViewFolderControl.xaml 的交互逻辑
    /// </summary>
    [UIControl("ViewFileContents", "Folder")]
    public partial class ViewFolderControl : UserControl
    {
        public ViewFolderControl()
        {
            InitializeComponent();
        }

        private void listItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(listItems.SelectedItem!=null)
            {
                if(DataContext is WebViewsViewModel)
                {
                    var vm = DataContext as WebViewsViewModel;
                    vm.SelectedItem = listItems.SelectedItem as ViewFileBase;
                }
            }
        }
    }
}