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
using System.Windows.Shapes;

namespace Panasia.Core.AppTest
{
    /// <summary>
    /// SelectDataTableWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SelectTestTypeWindow : Window
    {
        private Action<string> _SetAction = null;

        public SelectTestTypeWindow(Action<string> actionOK)
        {
            InitializeComponent();

            testTypes.ItemsSource = Panasia.Core.App.TestJobConfigs.GetItems();

            _SetAction = actionOK;
            this.Owner = Application.Current.MainWindow;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if(testTypes.SelectedValue==null)
            {
                MessageBox.Show("请选择类型");
                return;
            } 

            if(_SetAction!=null)
            {
                _SetAction(testTypes.SelectedValue.ToString());
            }
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
