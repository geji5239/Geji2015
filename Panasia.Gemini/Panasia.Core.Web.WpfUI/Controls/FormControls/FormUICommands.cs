using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Panasia.Core;
using FirstFloor.ModernUI.Presentation;
using Panasia.Core.Web.Settings;

namespace Panasia.Core.Web.Controls
{
    public class ControlCommand : RelayCommand
    {
        #region 属性 Title
        private string _Title = string.Empty;
        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                _Title = value;
            }
        }
        #endregion

        public ControlCommand(string title, Action<object> execute, Func<object, bool> canExecute = null)
            : base(execute, canExecute)
        {
            Title = title;
        }
    }

    public class ControlSettingCommand : RelayCommand
    { 
        #region 属性 Title
        private string _Title = string.Empty;
        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                _Title = value;
            }
        }
        #endregion

        public ControlSettingCommand(string title, Action<object> execute, Func<object, bool> canExecute = null)
            : base(execute, canExecute)
        {
            Title = title;
        }

    }

    public class FormUICommands : EntityBase
    {
        public FormUICommands()
        {
            ControlCommands = new ObservableCollection<ControlCommand>
            { 
                CreateAddControlCommand("添加编辑框",()=>new EditBox()), 
                CreateAddControlCommand("添加标签",()=>new Label()), 
                CreateAddControlCommand("添加文本框",()=>new TextBox()), 
                CreateAddControlCommand("添加按钮",()=>new Button()),  
                CreateAddControlCommand("添加多选框",()=>new CheckBox()),
                CreateAddControlCommand("添加下拉框",()=>new ComboBox()), 
                CreateAddControlCommand("添加搜索选择框",()=>new PickBox()), 
                CreateAddControlCommand("添加日期",()=>new DateBox()), 
                CreateAddControlCommand("添加图片",()=>new ImageBox()),
                CreateAddControlCommand("添加网格",()=>new GridView()), 
                CreateAddControlCommand("添加数据窗体",()=>new DataView()),  
                CreateAddControlCommand("添加容器",()=>new StackPanel()), 
                CreateAddControlCommand("添加工具栏",()=>new ToolBar()), 
                CreateAddControlCommand("添加分组框",()=>new GridBox())
            };
        }

        private ControlCommand CreateAddControlCommand(string title, Func<ControlBase> create)
        {
            return new ControlCommand(title,
                (obj) =>
                {
                    var panel = obj as Panel;
                    panel.Controls.Add(create());
                },
                (obj) =>
                {
                    return obj != null && obj is Panel;
                });
        }

        #region 属性 SelectedItem
        private object _SelectedItem = null;
        public object SelectedItem
        {
            get
            {
                return _SelectedItem;
            }
            set
            {
                _SelectedItem = value;
                RaisePropertyChanged(() => SelectedItem);
            }
        }
        #endregion
        
        #region 属性 ControlCommands
        private ObservableCollection<ControlCommand> _ControlCommands = null;
        public ObservableCollection<ControlCommand> ControlCommands
        {
            get
            {
                if (_ControlCommands == null)
                {
                    _ControlCommands = new ObservableCollection<ControlCommand>();
                }
                return _ControlCommands;
            }
            set
            {
                _ControlCommands = value;
                RaisePropertyChanged(() => ControlCommands);
            }
        }
        #endregion

        #region MoveUpCommand
        private RelayCommand _MoveUpCommand = null;
        public RelayCommand MoveUpCommand
        {
            get
            {
                if (_MoveUpCommand == null)
                {
                    _MoveUpCommand = new RelayCommand(OnMoveUp, CanMoveUp);
                }
                return _MoveUpCommand;
            }
        }

        private bool CanMoveUp(object parameter)
        {
            if (SelectedItem != null && SelectedItem is ControlBase && (SelectedItem as ControlBase).Parent != null)
            {
                var parent = (SelectedItem as ControlBase).Parent;
                if (parent is Panel)
                {
                    var panel = parent as Panel;
                    return panel.Controls.IndexOf(SelectedItem as ControlBase) > 0;
                }
            }
            return false;
        }

        private void OnMoveUp(object parameter)
        {
            var parent = (SelectedItem as ControlBase).Parent;
            if (parent is Panel)
            {
                var panel = parent as Panel;
                panel.Controls.MoveUp(SelectedItem as ControlBase);
            }
        }
        #endregion

        #region MoveDownCommand
        private RelayCommand _MoveDownCommand = null;
        public RelayCommand MoveDownCommand
        {
            get
            {
                if (_MoveDownCommand == null)
                {
                    _MoveDownCommand = new RelayCommand(OnMoveDown, CanMoveDown);
                }
                return _MoveDownCommand;
            }
        }

        private bool CanMoveDown(object parameter)
        {
            if (SelectedItem != null && SelectedItem is ControlBase && (SelectedItem as ControlBase).Parent != null)
            {
                var parent = (SelectedItem as ControlBase).Parent;
                if (parent is Panel)
                {
                    var panel = parent as Panel;
                    return panel.Controls.IndexOf(SelectedItem as ControlBase) < panel.Controls.Count - 1;
                }
            }
            return false;
        }

        private void OnMoveDown(object parameter)
        {
            var parent = (SelectedItem as ControlBase).Parent;
            if (parent is Panel)
            {
                var panel = parent as Panel;
                panel.Controls.MoveDown(SelectedItem as ControlBase);
            }
        }
        #endregion

        #region CopyCommand
        private RelayCommand _CopyCommand = null;
        public RelayCommand CopyCommand
        {
            get
            {
                if (_CopyCommand == null)
                {
                    _CopyCommand = new RelayCommand(OnCopy, CanCopy);
                }
                return _CopyCommand;
            }
        }

        private bool CanCopy(object parameter)
        {
            return (SelectedItem != null && SelectedItem is ControlBase && (SelectedItem as ControlBase).Parent != null);
        }

        private void OnCopy(object parameter)
        {
            var parent = (SelectedItem as ControlBase).Parent;
            if (parent is Panel)
            {
                var panel = parent as Panel;
                var newControl = SelectedItem.ToXString().ToXObject(SelectedItem.GetType());
                panel.Controls.Add(newControl as ControlBase);
            }
        }
        #endregion

        #region DeleteCommand
        private RelayCommand _DeleteCommand = null;
        public RelayCommand DeleteCommand
        {
            get
            {
                if (_DeleteCommand == null)
                {
                    _DeleteCommand = new RelayCommand(OnDelete, CanDelete);
                }
                return _DeleteCommand;
            }
        }

        private bool CanDelete(object parameter)
        {
            return (SelectedItem != null && SelectedItem is ControlBase && (SelectedItem as ControlBase).Parent != null);
        }

        private void OnDelete(object parameter)
        {            
            var parent = (SelectedItem as ControlBase).Parent;
            if (parent is Panel)
            {
                var panel = parent as Panel;
                panel.Controls.Remove(SelectedItem as ControlBase);
            }
        }
        #endregion

        #region SetStyleCommand
        private RelayCommand _SetStyleCommand = null;
        public RelayCommand SetStyleCommand
        {
            get
            {
                if (_SetStyleCommand == null)
                {
                    _SetStyleCommand = new ControlSettingCommand("Style", (obj) => { ShowSetting(ControlSettings.Current.StyleItems, ""); }, CanShowSetting);
                }
                return _SetStyleCommand;
            }
        }
        #endregion

        #region SetClassCommand
        private RelayCommand _SetClassCommand = null;
        public RelayCommand SetClassCommand
        {
            get
            {
                if (_SetClassCommand == null)
                {
                    _SetClassCommand = new ControlSettingCommand("Class", (obj) => { ShowSetting(ControlSettings.Current.ClassItems, " "); }, CanShowSetting);
                }
                return _SetClassCommand;
            }
        }
        #endregion

        #region SetOptionCommand
        private RelayCommand _SetOptionCommand = null;
        public RelayCommand SetOptionCommand
        {
            get
            {
                if (_SetOptionCommand == null)
                {
                    _SetOptionCommand = new ControlSettingCommand("Option", (obj) => { ShowSetting(ControlSettings.Current.OptionItems, ""); }, CanShowSetting);
                }
                return _SetOptionCommand;
            }
        }
        #endregion
        
        private bool CanShowSetting(object obj)
        {
            return SelectedItem != null && (SelectedItem is ControlBase || SelectedItem is GridColumn);
        }

        private void ShowSetting(SettingCollection items, string split = "")
        {
            string controlName = "";
            Action<string> callback = null;
            if (SelectedItem is ControlBase)
            {
                var control = SelectedItem as ControlBase;
                controlName = control.GetType().Name;
                callback = (value) => { control.Style = value; };
            }
            else if (SelectedItem is GridColumn)
            {
                var control = SelectedItem as GridColumn;
                controlName = control.GetType().Name;
                callback = (value) => { control.Style = value; };
            }
            else
            {
                return;
            }

            SettingSelectWindow win = new SettingSelectWindow();
            GetSettingViewModel vm = new GetSettingViewModel(
                items, controlName,
                callback, split);
            win.DataContext = vm;
            win.ShowDialog();
        }
    }
}
