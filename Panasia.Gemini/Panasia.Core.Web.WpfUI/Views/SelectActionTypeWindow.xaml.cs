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
using System.Windows.Shapes;
using Panasia.Core;

namespace Panasia.Core.Web
{
    /// <summary>
    /// SelectDataTableWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SelectActionTypeWindow : Window
    {
        private Action<ActionType> _SetAction = null;
        ActionSelectModel _VM = new ActionSelectModel();

        public SelectActionTypeWindow(Action<ActionType> actionOK)
        {
            InitializeComponent();

            _SetAction = actionOK;
            this.Owner = Application.Current.MainWindow;
            DataContext = _VM;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        { 
            ActionType actions = ActionType.None;
            foreach(var item in _VM.Items.Where(a => a.IsSelected))
            {
                actions = actions | item.ActionValue;
            }
            
            if(actions == ActionType.None) 
            {
                MessageBox.Show("请选择");
                return;
            }

            if (_SetAction != null)
            {
                _SetAction(actions);
            }
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
    
    public class ActionSelectModel:EntityBase
    {
        #region 属性 SelectedAll
        private bool _SelectedAll = false;
        public bool SelectedAll
        {
            get
            {
                return _SelectedAll;
            }
            set
            {
                _SelectedAll = value;
                RaisePropertyChanged(() => SelectedAll);
                foreach(var item in Items)
                {
                    item.IsSelected = _SelectedAll;
                }
            }
        }
        #endregion

        #region 属性 Items
        private CollectionBase<ActionSelectItem> _Items = null;
        public CollectionBase<ActionSelectItem> Items
        {
            get
            {
                if(_Items==null)
                {
                    _Items = new CollectionBase<ActionSelectItem>
                    {
                        new ActionSelectItem{ IsSelected = true, ActionName = "Index-列表" , ActionValue = ActionType.Index},
                        new ActionSelectItem{ IsSelected = true, ActionName = "Search-检索" , ActionValue = ActionType.Search},
                        new ActionSelectItem{ IsSelected = true, ActionName = "Query-检索" , ActionValue = ActionType.Query},
                        new ActionSelectItem{ IsSelected = true, ActionName = "Detail-查看" , ActionValue = ActionType.Detail},
                        new ActionSelectItem{ IsSelected = true, ActionName = "Create-新增" , ActionValue = ActionType.Create},
                        new ActionSelectItem{ IsSelected = true, ActionName = "Add-新增" , ActionValue = ActionType.Add},
                        new ActionSelectItem{ IsSelected = true, ActionName = "Edit-修改" , ActionValue = ActionType.Edit},
                        new ActionSelectItem{ IsSelected = true, ActionName = "Update-修改" , ActionValue = ActionType.Update},
                        new ActionSelectItem{ IsSelected = true, ActionName = "Delete-删除" , ActionValue = ActionType.Delete} 
                    };
                }
                return _Items;
            }
            set
            {
                _Items = value;
                RaisePropertyChanged(() => Items);
            }
        }
        #endregion

    }

    public class ActionSelectItem:EntityBase
    {
        #region 属性 IsSelected
        private bool _IsSelected = true;
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                _IsSelected = value;
                RaisePropertyChanged(() => IsSelected);
            }
        }
        #endregion

        #region 属性 ActionName
        private string _ActionName = string.Empty;
        public string ActionName
        {
            get
            {
                return _ActionName;
            }
            set
            {
                _ActionName = value;
                RaisePropertyChanged(() => ActionName);
            }
        }
        #endregion

        #region 属性 ActionValue
        private ActionType _ActionValue = ActionType.Index;
        public ActionType ActionValue
        {
            get
            {
                return _ActionValue;
            }
            set
            {
                _ActionValue = value;
                RaisePropertyChanged(() => ActionValue);
            }
        }
        #endregion
    }
}
