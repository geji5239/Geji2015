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
    public partial class SelectSqlItemWindow : Window
    {
        private Action<string, string,string> _SetAction = null;

        public SelectSqlItemWindow(Action<string, string,string> actionOK)
        {
            InitializeComponent();

            _SetAction = actionOK;
            this.Owner = Application.Current.MainWindow;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if(modules.SelectedValue==null)
            {
                MessageBox.Show("请选择模块");
                return;
            }
            if(tables.SelectedValue==null)
            {
                MessageBox.Show("请选择表");
                return;
            }
            if(sqls.SelectedValue==null)
            {
                MessageBox.Show("请选择命令");
                return;
            }

            if(_SetAction!=null)
            {
                _SetAction(modules.SelectedValue.ToString(), tables.SelectedValue.ToString(), sqls.SelectedValue.ToString());
            }
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
