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
using Panasia.Core.App;
using Panasia.Core.Wpf;
using Panasia.Core;

namespace Panasia.Core.Web.Controls
{
    /// <summary>
    /// ViewActionCommandControl.xaml 的交互逻辑
    /// </summary>
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    [UIControl("CommandControls", "EditDialogActionCommand")]
    public partial class EditDialogActionCommandControl : UserControl
    {
        EditDialogActionCommand _Command = null;

        public EditDialogActionCommandControl()
        {
            InitializeComponent();

            DataContextChanged += EditDialogActionCommandControl_DataContextChanged;
            ResetCommand();

            CreateFieldMenus();
        }

        void EditDialogActionCommandControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetCommand();
        }

        private void ResetCommand()
        {
            if (DataContext != null && DataContext is CommandConfig && (DataContext as CommandConfig).Config is EditDialogActionCommand)
            {
                _Command = (DataContext as CommandConfig).Config as EditDialogActionCommand;
                _Command.Page.Form.ToolBar.PageID = (DataContext as CommandConfig).ActionConfig.Page.PageID;
                UIField.FieldTypeChanged += UIField_FieldTypeChanged;
            }
            else
            {
                _Command = null;
                UIField.FieldTypeChanged -= UIField_FieldTypeChanged;
            }
        }
        void UIField_FieldTypeChanged(object sender, EventArgs e)
        {
            if (DataContext != null && DataContext is CommandConfig && (DataContext as CommandConfig).Config is EditDialogActionCommand)
            {
                if (sender != null && sender is UIField)
                {
                    var field = sender as UIField;
                    if (field == fields.SelectedItem)
                    {
                        field.ResetInput();
                    }
                }
            }
        }

        private void TabItem_GotFocus(object sender, RoutedEventArgs e)
        {
            PageRender();
        }

        private void PageRender()
        {
            if (DataContext != null && DataContext is CommandConfig && (DataContext as CommandConfig).Config is EditDialogActionCommand)
            {
                var vm = (DataContext as CommandConfig).Config as EditDialogActionCommand;
                contentSource.Text = vm.Page.Render(null, settingType.Text).ToFormatHtml();
                //contentSource.NavigateToString(vm.Page.Render(null, settingType.Text));
            }
        }

        #region 创建按钮的右键菜单
        private void CreateButtonMenus()
        {
            CollectionBase<CommandMenu> menus = new CollectionBase<CommandMenu>
            {
                new CommandMenu("增加",(obj)=>{
                    if(_Command==null)
                    {
                        return;
                    } 
                    var items = _Command.Page.Form.ToolBar.Buttons;
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
                    var items = _Command.Page.Form.ToolBar.Buttons;

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
                    var items = _Command.Page.Form.ToolBar.Buttons;

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
                    var items = _Command.Page.Form.ToolBar.Buttons; 
                    items.Remove(selectedItem as UIButton);
                })
            };
            buttonMenu.ItemsSource = menus;
        }
        #endregion

        #region 创建字段的右键菜单
        private void CreateFieldMenus()
        {
            var createMenu = new CommandMenu { Title = "增加" };
            InputTypes.GetItems().Select(c =>
                new CommandMenu(c.Title,
                    (obj) =>
                    {
                        AddField(() => new UIField { Input = c.Func() });
                    }
                    )).AddToCollection(createMenu.SubMenus);

            var updateMenu = new CommandMenu { Title = "转换" };
            InputTypes.GetItems().Select(c =>
                new CommandMenu(c.Title,
                    (obj) =>
                    {
                        ChangeField(() => c.Func());
                    }
                    )).AddToCollection(updateMenu.SubMenus);

            var menus = new CollectionBase<CommandMenu>
            {
                createMenu,updateMenu,
                new CommandMenu("删除",(obj)=>{RemoveField();}),
                new CommandMenu("上移",(obj)=>{MoveUpField();}),
                new CommandMenu("下移",(obj)=>{MoveDownField();})
            };

            menuFields.ItemsSource = menus;
        }
        #endregion

        private void AddField(Func<UIField> add)
        {
            if (_Command == null)
            {
                return;
            }
            var field = add();
            _Command.Page.Form.Fields.Add(field);
        }

        private void SelectField(System.Action<UIField> action)
        {
            if (_Command == null)
            {
                return;
            }
            var selectedItem = fields.SelectedItem;
            if (selectedItem == null || (!(selectedItem is UIField)))
            {
                return;
            }
            var selectField = selectedItem as UIField;
            action(selectField);
        }

        private void ChangeField(Func<UIInput> add)
        {
            SelectField((selectField) =>
            {
                var input = add();
                selectField.Input.CopyTo(input);
                selectField.Input = input;
            });
        }

        private void RemoveField()
        {
            SelectField((selectField) =>
            {
                _Command.Page.Form.Fields.Remove(selectField);
            });
        }

        private void MoveUpField()
        {
            SelectField((selectField) =>
            {
                _Command.Page.Form.Fields.MoveUp(selectField);
            });
        }

        private void MoveDownField()
        {
            SelectField((selectField) =>
            {
                _Command.Page.Form.Fields.MoveDown(selectField);
            });
        }

        private void inputExtends_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //var data = inputExtends.DataContext;
            //if (!(data is UIField))
            //{
            //    return;
            //}
            //var input = (data as UIField).Input;

            //if (input is UIComboBox)
            //{
            //    inputExtends.ControlName = InputControlSettings.ComboBox;
            //    return;
            //}
            //if (input is UIPickBox)
            //{
            //    inputExtends.ControlName = InputControlSettings.PickBox;
            //    return;
            //}
            //else
            //{
            //    inputExtends.ControlName = InputControlSettings.Input;
            //}
        }
        private void btnBrowser_Click(object sender, RoutedEventArgs e)
        {
            PageRender();
        }

        private void fields_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangeEventArgs e)
        {
            if (fields.SelectedItem != null && fields.SelectedItem is UIField)
            {
                var field = fields.SelectedItem as UIField;
                bool isSameType = false;

                switch (field.FieldType)
                {
                    case UIFieldType.None:
                        isSameType = field.Input is UIInput;
                        break;
                    case UIFieldType.TextBox:
                        isSameType = field.Input is UIInput;
                        break;
                    case UIFieldType.PasswordBox:
                        isSameType = field.Input is UIInput;
                        break;
                    case UIFieldType.TextArea:
                        isSameType = field.Input is UITextArea;
                        break;
                    case UIFieldType.Hidden:
                        isSameType = field.Input is UIInput;
                        break;
                    case UIFieldType.CheckBox:
                        isSameType = field.Input is UICheckBox;
                        break;
                    case UIFieldType.ComboBox:
                        isSameType = field.Input is UIComboBox;
                        break;
                    case UIFieldType.ComboTree:
                        isSameType = field.Input is UIComboTree;
                        break;
                    case UIFieldType.PickBox:
                        isSameType = field.Input is UIPickBox;
                        break;
                    case UIFieldType.DateBox:
                        isSameType = field.Input is UIDateBox;
                        break;
                    case UIFieldType.DateTimeBox:
                        isSameType = field.Input is UIDateTimeBox;
                        break;
                    case UIFieldType.NumberBox:
                        isSameType = field.Input is UINumericBox;
                        break;
                    case UIFieldType.FileBox:
                        isSameType = field.Input is UIFileBox;
                        break;
                    case UIFieldType.ImageBox:
                        isSameType = field.Input is UIImageBox;
                        break;
                    default:
                        Panasia.Core.Wpf.MessageBox.Show(string.Format("没有定义的类型[{0}]", field.FieldType), "无效的类型", MessageBoxButton.OK);
                        break;
                }

                if (!isSameType)
                {
                    Panasia.Core.Wpf.MessageBox.Show(string.Format("控件类型不匹配，请重新选择[{0}]", field.Input.GetType().FullName), "无效的类型", MessageBoxButton.OK);
                }
            }
        }

        private void chkTableLayout_Checked(object sender, RoutedEventArgs e)
        {
            if(chkTableLayout.IsChecked== true)
            {
                expTableLayout.IsExpanded = true;

                if (DataContext != null && DataContext is CommandConfig && (DataContext as CommandConfig).Config is EditDialogActionCommand)
                {
                    var vm = (DataContext as CommandConfig).Config as EditDialogActionCommand;

                    var form = vm.Page.Form;
                    int row = 0;
                    int col = 0;
                    foreach(var field in form.Fields)
                    {
                        field.RowIndex = row;
                        field.ColumnIndex = col;
                        field.ColumnSpan = 2;
                        if(col==0)
                        {
                            col += field.ColumnSpan;
                        }
                        else
                        {
                            col = 0;
                            row++;
                        }
                    }
                }
            }
        }
    }
}
