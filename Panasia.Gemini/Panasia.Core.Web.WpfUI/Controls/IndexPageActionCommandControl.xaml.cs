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
using Panasia.Core.App;
using Panasia.Core.Wpf;

namespace Panasia.Core.Web.Controls
{
    /// <summary>
    /// ViewActionCommandControl.xaml 的交互逻辑
    /// </summary>
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    [UIControl("CommandControls", "IndexPageActionCommand")]
    public partial class IndexPageActionCommandControl : UserControl
    {
        IndexPageActionCommand _Command = null;
        public IndexPageActionCommandControl()
        {
            InitializeComponent();

            CreateButtonMenus();
            CreateColumnMenus();
            DataContextChanged += IndexPageActionCommandControl_DataContextChanged; 
        }

        void IndexPageActionCommandControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext != null && DataContext is CommandConfig && (DataContext as CommandConfig).Config is IndexPageActionCommand)
            {
                _Command = (DataContext as CommandConfig).Config as IndexPageActionCommand;
                _Command.Page.ToolBar.PageID = (DataContext as CommandConfig).ActionConfig.Page.PageID; 
            }
            else
            {
                _Command = null;
            }
        }


        private void TabItem_GotFocus(object sender, RoutedEventArgs e)
        {
            PageRender();
        }

        private void PageRender()
        {
            if (DataContext != null && DataContext is CommandConfig && (DataContext as CommandConfig).Config is IndexPageActionCommand)
            {
                var vm = (DataContext as CommandConfig).Config as IndexPageActionCommand;
                contentSource.Text = vm.Page.Render(null, settingType.Text).ToFormatHtml();
                //contentSource.NavigateToString(vm.Page.Render(null, settingType.Text));
            }
        }

        private void CreateButtonMenus()
        {
            CollectionBase<CommandMenu> menus = new CollectionBase<CommandMenu>
            {
                new CommandMenu("增加",(obj)=>{
                    if(_Command==null)
                    {
                        return;
                    } 
                    var items = _Command.Page.ToolBar.Buttons;
                    items.Add(new UIButton());
                }), 
                new CommandMenu("上移" ,(obj) =>
                { 
                    if(_Command==null)
                    {
                        return;
                    } 
                    var selectedItem = buttons.SelectedItem;
                    if(selectedItem ==null)
                    {
                        return;
                    }
                    var items = _Command.Page.ToolBar.Buttons;

                    var index = items.IndexOf(selectedItem as UIButton);
                    var count = items.Count;
                    if (index > 0)
                    {
                        items.MoveUp(selectedItem as UIButton);
                    }
                }),

                new CommandMenu("下移", (obj) =>
                {
                    if(_Command==null)
                    {
                        return;
                    } 
                    var selectedItem = buttons.SelectedItem;
                    if(selectedItem ==null)
                    {
                        return;
                    }
                    var items = _Command.Page.ToolBar.Buttons;

                    var index = items.IndexOf(selectedItem as UIButton);
                    var count = items.Count;
                    if (index < count - 1)
                    {
                        items.MoveDown(selectedItem as UIButton);
                    }
                }),

                new CommandMenu("删除",(obj) =>
                {
                    if(_Command==null)
                    {
                        return;
                    } 
                    var selectedItem = buttons.SelectedItem;
                    if(selectedItem ==null)
                    {
                        return;
                    }
                    var items = _Command.Page.ToolBar.Buttons; 
                    items.Remove(selectedItem as UIButton);
                })
            };
            buttonMenu.ItemsSource = menus;
        }


        private void CreateColumnMenus()
        {
            CollectionBase<CommandMenu> menus = new CollectionBase<CommandMenu>
            {
                new CommandMenu("增加",(obj)=>{
                    if(_Command==null)
                    {
                        return;
                    }
                    var items = _Command.Page.DataGrid.Columns;
                    items.Add(new UIDataColumn());
                }), 
                new CommandMenu("上移" ,(obj) =>
                { 
                    if(_Command==null)
                    {
                        return;
                    } 
                    var selectedItem =columns.SelectedItem;
                    if(selectedItem ==null)
                    {
                        return;
                    }
                    var items = _Command.Page.DataGrid.Columns;

                    var index = items.IndexOf(selectedItem as UIDataColumn);
                    var count = items.Count;
                    if (index > 0)
                    {
                        items.MoveUp(selectedItem as UIDataColumn);
                    }
                }),

                new CommandMenu("下移", (obj) =>
                {
                    if(_Command==null)
                    {
                        return;
                    }
                    var selectedItem = columns.SelectedItem;
                    if(selectedItem ==null)
                    {
                        return;
                    }
                    var items = _Command.Page.DataGrid.Columns;

                    var index = items.IndexOf(selectedItem as UIDataColumn);
                    var count = items.Count;
                    if (index < count - 1)
                    {
                        items.MoveDown(selectedItem as UIDataColumn);
                    }
                }),

                new CommandMenu("删除",(obj) =>
                {
                    if(_Command==null)
                    {
                        return;
                    }
                    var selectedItem = columns.SelectedItem;
                    if(selectedItem ==null)
                    {
                        return;
                    }
                    var items = _Command.Page.DataGrid.Columns; 
                    items.Remove(selectedItem as UIDataColumn);
                })
            };
            columnMenu.ItemsSource = menus;
        }

        private void btnBrowser_Click(object sender, RoutedEventArgs e)
        {
            PageRender();
        }
    }
}
