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
    /// JsonActionCommandControl.xaml 的交互逻辑
    /// </summary>
    [UIControl("CommandControls", "JsonActionCommand")]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public partial class JsonActionCommandControl : UserControl
    {
        public JsonActionCommandControl()
        {
            InitializeComponent();
        }
    }
}
