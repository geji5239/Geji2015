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
using Panasia.Core.Sys;

namespace Panasia.Core.Web.Controls
{
    /// <summary>
    /// JsonActionCommandControl.xaml 的交互逻辑
    /// </summary>
    [UIControl("CommandControls", "MailCommand")]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public partial class MailActionCommandControl : UserControl
    {
        public MailActionCommandControl()
        {
            InitializeComponent();
            try
            {
                templates.SetBinding(System.Windows.Controls.ComboBox.ItemsSourceProperty, new Binding("RazorFiles")
                {
                    Source = ContentLib.Current
                });

                servers.ItemsSource = MailService.GetIndex();

                password.PasswordChanged += password_PasswordChanged;
            }
            catch(Exception ex)
            {
                this.Log(ex.ToString(), Category.Exception);
            }
        }

        void password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext == null)
            {
                return;
            }
            dynamic data = DataContext;
            data.Config.UserPassword = password.Password;
        }
    }
}
