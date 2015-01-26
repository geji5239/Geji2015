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
using Panasia.Core.Contents;
using Panasia.Core.Web.Controls.Razors;

namespace Panasia.Core.Web.Views
{
    /// <summary>
    /// WebViewsPage.xaml 的交互逻辑
    /// </summary>
    [Page("ContentLib")]
    public partial class ContentLibPage : UserControl,IPage
    {
        public ContentLibPage()
        {
            InitializeComponent();
        }

        private void root_CurrentItemChanged(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if(root.CurrentItem ==null)
            {
                selectView.Content = null;
                return;
            }
            if(root.CurrentItem is ContentFolder)
            {
                selectView.Content = new ContentFolderControl();
                return;
            }
            if(root.CurrentItem is RazorTemplate)
            {
                selectView.Content = new ContentFileControl();
                return;
            }
        }
    }
}
