using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
using Panasia.Core.App;
using Panasia.Core.Wpf;

namespace Panasia.Core.Web.Controls
{
    /// <summary>
    /// ViewActionCommandControl.xaml 的交互逻辑
    /// </summary>
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    [UIControl("CommandControls", "FormViewActionCommand")]
    public partial class FormViewActionCommandControl : UserControl
    {
        public FormViewActionCommandControl()
        {
            InitializeComponent();

            this.Loaded += FormViewActionCommandControl_Loaded;
        }

        void FormViewActionCommandControl_Loaded(object sender, RoutedEventArgs e)
        {
            //if(Panasia.Core.Wpf.MessageBox.Show("本设计器已过时，是否执行转换?","设计器过时", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            //{
            //    if(DataContext!=null && DataContext is CommandConfig)
            //    {
            //        var command = DataContext as CommandConfig;
            //        if(command.Config is FormViewActionCommand)
            //        {
            //            (command.Config as FormViewActionCommand).CopyToUIPage();
            //        }
            //    }
            //}
        }
        
    }
}
