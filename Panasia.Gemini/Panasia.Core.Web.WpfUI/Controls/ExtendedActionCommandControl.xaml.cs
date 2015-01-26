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
using Panasia.Core.Wpf;
using Panasia.Core.Contents;

namespace Panasia.Core.Web.Controls
{
    /// <summary>
    /// JsonActionCommandControl.xaml 的交互逻辑
    /// </summary>
    [UIControl("CommandControls", "ExtendedAction")]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public partial class ExtendedActionCommandControl : UserControl
    {
        public ExtendedActionCommandControl()
        {
            InitializeComponent();

            templates.ItemsSource = ContentActions.GetItems();
        }
    }
}
