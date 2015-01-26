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

namespace Panasia.Core.Web.Controls
{
    /// <summary>
    /// ViewActionCommandControl.xaml 的交互逻辑
    /// </summary>
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    [UIControl("CommandControls", "ViewActionCommand")]
    public partial class ViewActionCommandControl : UserControl
    {
        public ViewActionCommandControl()
        {
            InitializeComponent();
        }
    }
}
