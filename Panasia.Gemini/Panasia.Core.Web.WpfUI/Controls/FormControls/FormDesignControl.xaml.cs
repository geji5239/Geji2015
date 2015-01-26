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
using Panasia.Core;
using Panasia.Core.App;

namespace Panasia.Core.Web.Controls
{
    /// <summary>
    /// FormDesignControlxaml.xaml 的交互逻辑
    /// </summary>
    public partial class FormDesignControl : UserControl
    {
        public FormDesignControl()
        {
            InitializeComponent();

            uiTree.DataContextChanged += uiTree_DataContextChanged;
        }

        void uiTree_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (uiTree.DataContext is Form)
            {
                uiTree.ItemsSource = new List<object> { uiTree.DataContext };
            }
        }

        private void btnForm2Xml_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DataContext is CommandConfig)
                {
                    var config = DataContext as CommandConfig;
                    if (config.Config is FormViewActionCommand)
                    {
                        var formAction = config.Config as FormViewActionCommand;
                        var xml = formAction.Form.ToXString();
                        xmlText.Text = xml;            
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnXml2Form_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DataContext is CommandConfig)
                {
                    var config = DataContext as CommandConfig;
                    if (config.Config is FormViewActionCommand)
                    {
                        var formAction = config.Config as FormViewActionCommand;
                        var form = xmlText.Text.ToXObject<Form>();
                        if (form != null)
                        {
                            formAction.Form = form;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void PropertyGrid_AutoGeneratingPropertyDefinition(object sender, Telerik.Windows.Controls.Data.PropertyGrid.AutoGeneratingPropertyDefinitionEventArgs e)
        {

        }

        private void uiTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
 
        }
    }
}
