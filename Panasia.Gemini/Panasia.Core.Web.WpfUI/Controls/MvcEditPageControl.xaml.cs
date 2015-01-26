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
using Panasia.Core;
using Panasia.Core.Wpf;
using Page = Panasia.Core.App.Page;

namespace Panasia.Core.Web.Controls
{
    /// <summary>
    /// MvcPageControl.xaml 的交互逻辑
    /// </summary>
    [UIControl("PageConfigControls", "MvcEditPage")]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public partial class MvcEditPageControl : UserControl
    {
        MvcEditPage _Page = null;
        public MvcEditPageControl()
        {
            InitializeComponent();

            CreateEditFieldsMenus();
            DataContextChanged += MvcEditPageControl_DataContextChanged;
        }

        void MvcEditPageControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext != null && DataContext is Page && (DataContext as Page).Config is MvcEditPage)
            {
                _Page = (DataContext as Page).Config as MvcEditPage; 
            } 
        }

        private void btnUpdateDataSource_Click(object sender, RoutedEventArgs e)
        {
            if (_Page != null)
            {
                _Page.ImportTable();
            }
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext != null && DataContext is Page && (DataContext as Page).Config is MvcEditPage)
            {
                SelectActionTypeWindow win = new SelectActionTypeWindow((actions) =>
                {
                    ((DataContext as Page).Config as MvcEditPage).CreateSettings(actions);
                });
                win.ShowDialog();
            }
        }

        private void btnClearDataSource_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext != null && DataContext is Page && (DataContext as Page).Config is MvcEditPage)
            {
                ((DataContext as Page).Config as MvcEditPage).EditSource.Fields.Clear();
            }
        }

        private void btnGetTable_Click(object sender, RoutedEventArgs e)
        {
            SelectDataTableWindow win = new SelectDataTableWindow((group, table) =>
            {
                if (DataContext != null && DataContext is Page && (DataContext as Page).Config is MvcEditPage)
                {
                    var page = (DataContext as Page).Config as MvcEditPage;
                    var source = page.EditSource;
                    source.GroupName = group;
                    source.TableName = table;
                    if (System.Windows.MessageBox.Show("数据表已更改，重新刷新列吗？", "刷新列", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        page.EditSource.Fields.Clear();
                        page.ImportTable();
                    }
                }
            });
            win.ShowDialog();
        }


        private void CreateEditFieldsMenus()
        {
            CollectionBase<CommandMenu> menus = new CollectionBase<CommandMenu>
            {
                new CommandMenu("上移" ,(obj) =>
                { 
                    if(_Page==null)
                    {
                        return;
                    } 
                    var selectedItem =editFields.SelectedItem;
                    if(selectedItem ==null)
                    {
                        return;
                    }
                    var items = _Page.EditSource.Fields;

                    var index = items.IndexOf(selectedItem as EditField);
                    var count = items.Count;
                    if (index > 0)
                    {
                        items.MoveUp(selectedItem as EditField);
                    }
                }),

                new CommandMenu("下移", (obj) =>
                {
                    if(_Page==null)
                    {
                        return;
                    } 
                    var selectedItem =editFields.SelectedItem;
                    if(selectedItem ==null)
                    {
                        return;
                    }
                    var items = _Page.EditSource.Fields; 

                    var index = items.IndexOf(selectedItem as EditField);
                    var count = items.Count;
                    if (index < count - 1)
                    {
                        items.MoveDown(selectedItem as EditField);
                    }
                }),

                new CommandMenu("删除",(obj) =>
                {
                    if(_Page==null)
                    {
                        return;
                    } 
                    var selectedItem =editFields.SelectedItem;
                    if(selectedItem ==null)
                    {
                        return;
                    }
                    var items = _Page.EditSource.Fields; 

                    items.Remove(selectedItem as EditField);
                })
            };
            editFieldMenu.ItemsSource = menus;
        }
    }
}
