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
    [Page("LangTexts")]
    public partial class LangTextsPage : UserControl, IPage
    {
        public LangTextsPage()
        {
            InitializeComponent();

            this.DataContext = LangTexts.Current;
            listItems.ItemsSource = LangTexts.Current.Items;
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            LangTexts.Reload();
            this.DataContext = LangTexts.Current;
            listItems.ItemsSource = LangTexts.Current.Items;     
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            LangTexts.Current.Save();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { 
        }

        private void textFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(textFilter.Text))
            {
                listItems.ItemsSource = LangTexts.Current.Items;
            }
            else
            {
                string filter = textFilter.Text.Trim();
                listItems.ItemsSource = LangTexts.Current.Items.Where(s =>
                    s.ID.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0
                    || s.Name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0
                    || s.Description.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var item = new LangTextItem
            {
                ID = "0000"
            };
            LangTexts.Current.Items.Add(item);
            listItems.SelectedItem = item;
        }
    }
}
