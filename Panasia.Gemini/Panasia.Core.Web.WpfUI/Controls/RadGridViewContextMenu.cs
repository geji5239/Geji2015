using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Panasia.Core;
using Telerik.Windows.Controls;

namespace Panasia.Core.Web.Controls
{ 
    public static class CreateMenus
    {
        public static void CreateCollectionMenus(this ContextMenu menu,
            Func<ObservableCollection<object>> getItems, Func<object> getSelected, Func<object> create)
        {
            CollectionBase<CommandMenu> menus = new CollectionBase<CommandMenu>
            {
                new CommandMenu("增加",(obj)=>{
                    var itemsSource = getItems();
                    System.Windows.MessageBox.Show("增加" + itemsSource.GetType().ToString());
                    if (itemsSource == null || (!(itemsSource is ObservableCollection<object>)))
                    {
                        return;
                    }
                    System.Windows.MessageBox.Show("增加" + itemsSource.GetType().ToString());
                    var items = itemsSource as ObservableCollection<object>;
                    items.Add(create());
                }), 
                new CommandMenu("上移" ,(obj) =>
                {
                    var itemsSource = getItems();
                    var selectedItem = getSelected();
                    if (selectedItem == null || itemsSource == null || (!(itemsSource is ObservableCollection<object>)))
                    {
                        return;
                    }
                    var items = itemsSource as ObservableCollection<object>;
                    var index = items.IndexOf(selectedItem);
                    var count = items.Count;
                    if (index > 0)
                    {
                        items.MoveUp(selectedItem);
                    }
                }),

                new CommandMenu("下移", (obj) =>
                {
                    var itemsSource = getItems();
                    var selectedItem = getSelected();
                    if (selectedItem == null || itemsSource == null || (!(itemsSource is ObservableCollection<object>)))
                    {
                        return;
                    }
                    var items = itemsSource as ObservableCollection<object>;
                    var index = items.IndexOf(selectedItem);
                    var count = items.Count;
                    if (index < count - 1)
                    {
                        items.MoveDown(selectedItem);
                    }
                }),

                new CommandMenu("删除",(obj) =>
                {
                    var itemsSource = getItems();
                    var selectedItem = getSelected();
                    if (selectedItem == null || itemsSource == null || (!(itemsSource is ObservableCollection<object>)))
                    {
                        return;
                    }
                    var items = itemsSource as ObservableCollection<object>;
                    items.Remove(selectedItem);
                })
            };
            menu.ItemsSource = menus;
        }
    }
}
