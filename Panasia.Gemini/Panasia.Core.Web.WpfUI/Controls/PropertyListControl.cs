using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Panasia.Core.Web.Controls
{
    public class PropertyListControl:ListBox
    {
        #region Properties DependencyProperty
        public string Properties
        {
            get { return (string)GetValue(PropertiesProperty); }
            set { SetValue(PropertiesProperty, value); }
        }
        public static readonly DependencyProperty PropertiesProperty =
                DependencyProperty.Register("Properties", typeof(string), typeof(PropertyListControl),
                new PropertyMetadata("", new PropertyChangedCallback(PropertyListControl.OnPropertiesPropertyChanged)));

        private static void OnPropertiesPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is PropertyListControl)
            {
                (obj as PropertyListControl).OnPropertiesValueChanged();
            }
        }

        protected void OnPropertiesValueChanged()
        {

        }
        #endregion

        public PropertyListControl()
        {
            DataContextChanged += PropertyListControl_DataContextChanged;
            ResetItems();
        }

        void PropertyListControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetItems();
        }

        private void ResetItems()
        {
            if(DataContext==null)
            {
                ItemsSource = null;
                return;
            }

        }
    }


}
