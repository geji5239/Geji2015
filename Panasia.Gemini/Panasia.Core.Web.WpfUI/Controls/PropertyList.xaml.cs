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

namespace Panasia.Core.Web.Controls
{
    /// <summary>
    /// PropertyList.xaml 的交互逻辑
    /// </summary>
    public partial class PropertyList : UserControl
    {
        #region Properties DependencyProperty
        public string Properties
        {
            get { return (string)GetValue(PropertiesProperty); }
            set { SetValue(PropertiesProperty, value); }
        }
        public static readonly DependencyProperty PropertiesProperty =
                DependencyProperty.Register("Properties", typeof(string), typeof(PropertyList),
                new PropertyMetadata("", new PropertyChangedCallback(PropertyList.OnPropertiesPropertyChanged)));

        private static void OnPropertiesPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is PropertyList)
            {
                (obj as PropertyList).OnPropertiesValueChanged();
            }
        }

        protected void OnPropertiesValueChanged()
        {

        }
        #endregion
			
        public PropertyList()
        {
            InitializeComponent();

            DataContextChanged += PropertyList_DataContextChanged;
        }

        void PropertyList_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetItems();
        }

        private void ResetItems()
        {
            if(DataContext == null )
            {

            }
        }
    }
}
