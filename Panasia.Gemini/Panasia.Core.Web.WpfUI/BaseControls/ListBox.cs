using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Telerik.Windows.Controls;
using ListBox = Panasia.Core.Web.Controls.ListBox;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections; 

namespace Panasia.Core.Web.Controls
{
    public class ListBox : System.Windows.Controls.ListBox
    {
        public ListBox()
            : base()
        { 
            this.MouseDoubleClick += ListBox_MouseDoubleClick;
            this.MouseLeftButtonUp += ListBox_MouseLeftButtonUp; 
        }

        void ListBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ExecuteItemCommand(e.ClickCount);
        }

        void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ExecuteItemCommand(2);
        }

        #region ItemSelectedCommand DependencyProperty
        public ICommand ItemSelectedCommand
        {
            get { return (ICommand)GetValue(ItemSelectedCommandProperty); }
            set { SetValue(ItemSelectedCommandProperty, value); }
        }
        public static readonly DependencyProperty ItemSelectedCommandProperty =
                DependencyProperty.Register("ItemSelectedCommand", typeof(ICommand), typeof(ListBox),
                new PropertyMetadata(null, new PropertyChangedCallback(ListBox.OnItemSelectedCommandPropertyChanged)));

        private static void OnItemSelectedCommandPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is ListBox)
            {
                (obj as ListBox).OnItemSelectedCommandValueChanged();
            }
        }

        protected void OnItemSelectedCommandValueChanged()
        {

        }
        #endregion

        #region ItemSelectedCommandParameter DependencyProperty
        public object ItemSelectedCommandParameter
        {
            get { return (object)GetValue(ItemSelectedCommandParameterProperty); }
            set { SetValue(ItemSelectedCommandParameterProperty, value); }
        }
        public static readonly DependencyProperty ItemSelectedCommandParameterProperty =
                DependencyProperty.Register("ItemSelectedCommandParameter", typeof(object), typeof(ListBox),
                new PropertyMetadata(null, new PropertyChangedCallback(ListBox.OnItemSelectedCommandParameterPropertyChanged)));

        private static void OnItemSelectedCommandParameterPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is ListBox)
            {
                (obj as ListBox).OnItemSelectedCommandParameterValueChanged();
            }
        }

        protected void OnItemSelectedCommandParameterValueChanged()
        {

        }
        #endregion

        #region ItemClickCommand DependencyProperty
        public ICommand ItemClickCommand
        {
            get { return (ICommand)GetValue(ItemClickCommandProperty); }
            set { SetValue(ItemClickCommandProperty, value); }
        }
        public static readonly DependencyProperty ItemClickCommandProperty =
                DependencyProperty.Register("ItemClickCommand", typeof(ICommand), typeof(ListBox),
                new PropertyMetadata(null, new PropertyChangedCallback(ListBox.OnItemClickCommandPropertyChanged)));

        private static void OnItemClickCommandPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is ListBox)
            {
                (obj as ListBox).OnItemClickCommandValueChanged();
            }
        }

        protected void OnItemClickCommandValueChanged()
        {

        }
        #endregion

        #region ItemClickCommandParameter DependencyProperty
        public object ItemClickCommandParameter
        {
            get { return (object)GetValue(ItemClickCommandParameterProperty); }
            set { SetValue(ItemClickCommandParameterProperty, value); }
        }
        public static readonly DependencyProperty ItemClickCommandParameterProperty =
                DependencyProperty.Register("ItemClickCommandParameter", typeof(object), typeof(ListBox),
                new PropertyMetadata(null, new PropertyChangedCallback(ListBox.OnItemClickCommandParameterPropertyChanged)));

        private static void OnItemClickCommandParameterPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is ListBox)
            {
                (obj as ListBox).OnItemClickCommandParameterValueChanged();
            }
        }

        protected void OnItemClickCommandParameterValueChanged()
        {

        }
        #endregion

        #region ItemDoubleClickCommand DependencyProperty
        public ICommand ItemDoubleClickCommand
        {
            get { return (ICommand)GetValue(ItemDoubleClickCommandProperty); }
            set { SetValue(ItemDoubleClickCommandProperty, value); }
        }
        public static readonly DependencyProperty ItemDoubleClickCommandProperty =
                DependencyProperty.Register("ItemDoubleClickCommand", typeof(ICommand), typeof(ListBox),
                new PropertyMetadata(null, new PropertyChangedCallback(ListBox.OnItemDoubleClickCommandPropertyChanged)));

        private static void OnItemDoubleClickCommandPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is ListBox)
            {
                (obj as ListBox).OnItemDoubleClickCommandValueChanged();
            }
        }

        protected void OnItemDoubleClickCommandValueChanged()
        {

        }
        #endregion

        #region ItemDoubleClickCommandParameter DependencyProperty
        public object ItemDoubleClickCommandParameter
        {
            get { return (object)GetValue(ItemDoubleClickCommandParameterProperty); }
            set { SetValue(ItemDoubleClickCommandParameterProperty, value); }
        }
        public static readonly DependencyProperty ItemDoubleClickCommandParameterProperty =
                DependencyProperty.Register("ItemDoubleClickCommandParameter", typeof(object), typeof(ListBox),
                new PropertyMetadata(null, new PropertyChangedCallback(ListBox.OnItemDoubleClickCommandParameterPropertyChanged)));

        private static void OnItemDoubleClickCommandParameterPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is ListBox)
            {
                (obj as ListBox).OnItemDoubleClickCommandParameterValueChanged();
            }
        }

        protected void OnItemDoubleClickCommandParameterValueChanged()
        {

        }
        #endregion

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (SelectedItem != null)
            {
                SelectedItem = null;
            }
            base.OnGotFocus(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            SetItemSelected(e);
            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            SetItemSelected(e);
            base.OnMouseRightButtonUp(e);
        }

        private void SetItemSelected(MouseButtonEventArgs e)
        {
            var item = FindElementsInHostCoordinates(e.GetPosition(null)).OfType<RadListBoxItem>().FirstOrDefault();
            if (item != null)
            {
                item.IsSelected = true;
            }
            else if (this.SelectedItem != null)
            {
                var listItem = this.ItemContainerGenerator.ContainerFromItem(this.SelectedItem);
                if (listItem is RadListBoxItem)
                {
                    (listItem as RadListBoxItem).IsSelected = false;
                }
            }
        }

        IEnumerable FindElementsInHostCoordinates(Point p)
        {
            var result = new List<Object>();
    
            VisualTreeHelper.HitTest(
                this,
                null,
                new HitTestResultCallback(
                    (HitTestResult hit) =>
                    {
                        result.Add(hit.VisualHit);
                        return HitTestResultBehavior.Continue;
                    }),
                new PointHitTestParameters(p));
            return result;
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            if ((SelectedItem != null) && (ItemSelectedCommand != null)
                && ItemSelectedCommand.CanExecute(ItemSelectedCommandParameter))
            {
                try
                {
                    ItemSelectedCommand.Execute(ItemSelectedCommandParameter);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            base.OnSelectionChanged(e);
        }


        private void ExecuteItemCommand(int clickCount)
        {
            if (clickCount > 1)
            { 
                if (ItemDoubleClickCommand != null)
                {
                    if (ItemDoubleClickCommand.CanExecute(ItemDoubleClickCommandParameter))
                    {
                        ItemDoubleClickCommand.Execute(ItemDoubleClickCommandParameter);
                    }
                }
            }
            else
            {
                if (ItemClickCommand != null)
                {
                    if (ItemClickCommand.CanExecute(ItemClickCommandParameter))
                    {
                        ItemClickCommand.Execute(ItemClickCommandParameter);
                    }
                }
            }
        }
    }
}
