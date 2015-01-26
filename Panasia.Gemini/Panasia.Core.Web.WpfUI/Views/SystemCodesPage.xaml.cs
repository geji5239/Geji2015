using System;
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
using Panasia.Core.Wpf;
using Panasia.Core.Sys;

namespace Panasia.Core.Web.Views
{
    /// <summary>
    /// UISettingsPage.xaml 的交互逻辑
    /// </summary>
    [Page("SystemCodes")]
    public partial class SystemCodesPage : UserControl, IPage
    {
        public SystemCodesPage()
        {
            InitializeComponent();

            this.DataContext = SystemCodes.Current;
            listItems.ItemsSource = SystemCodes.Current.Items;
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            SystemCodes.Reload();
            this.DataContext = SystemCodes.Current;       
            listItems.ItemsSource = SystemCodes.Current.Items;     
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SystemCodes.Current.Save();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { 
        }
        
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var item = new SystemCodeItem
            {
                Code="0000",                
                Name = "0000"
            };
            SystemCodes.Current.Items.Add(item);
            listItems.SelectedItem = item;
        }
    }
}
