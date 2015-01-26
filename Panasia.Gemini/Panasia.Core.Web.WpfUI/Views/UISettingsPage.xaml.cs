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

namespace Panasia.Core.Web.Views
{
    /// <summary>
    /// UISettingsPage.xaml 的交互逻辑
    /// </summary>
    [Page("UISettings")]
    public partial class UISettingsPage : UserControl, IPage
    {
        public UISettingsPage()
        {
            InitializeComponent();

            this.DataContext = UISettings.Current;
            listItems.ItemsSource = UISettings.Current.Items;
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            UISettings.Reload();
            this.DataContext = UISettings.Current;            
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            UISettings.Current.Save();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            if(listBox.SelectedItem==null)
            {
                settingContent.Content = null;
                return;
            }
            if(listBox.SelectedItem is UISetting)
            {
                var content = new Controls.PageContentSetting();
                content.DataContext = listBox.SelectedItem;
                settingContent.Content = content;
                return;
            }
            if (listBox.SelectedItem is UISelectSetting)
            {
                var content = new Controls.SelectOptionSetting();
                content.DataContext = listBox.SelectedItem;
                settingContent.Content = content;
                return;
            }
        }

        private void textFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(textFilter.Text))
            {
                listItems.ItemsSource = UISettings.Current.Items;
            }
            else
            {
                string filter = textFilter.Text.Trim();
                listItems.ItemsSource = UISettings.Current.Items.Where(s =>
                    s.TargetType.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0
                    || s.Description.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }
    }
}
