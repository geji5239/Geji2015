using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Telerik.Windows.Controls;

namespace Panasia.Core.Web.Controls
{

    /// <summary>
    /// UISelectSettingControl.xaml 的交互逻辑
    /// </summary>
    public partial class UISelectSettingControl : UserControl
    {
        private CollectionBase<UISelectSettingItem> ListItems = new CollectionBase<UISelectSettingItem>();
        private bool _IsMutiSelect = false;
        private bool _CanEdit = false;
        private bool _IsSelfChanged = false;

        public UISelectSettingControl()
        {
            InitializeComponent();
            
            comboBox.ItemsSource = ListItems;
            this.DataContextChanged += UISelectSettingControl_DataContextChanged;
        }

        void UISelectSettingControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_IsSelfChanged)
            {
                return;
            }
            _IsSelfChanged = true;
            ResetItemSelected();
            _IsSelfChanged = false;            
        }

        #region SettingType DependencyProperty
        public string SettingType
        {
            get { return (string)GetValue(SettingTypeProperty); }
            set { SetValue(SettingTypeProperty, value); }
        }
        public static readonly DependencyProperty SettingTypeProperty =
                DependencyProperty.Register("SettingType", typeof(string), typeof(UISelectSettingControl),
                new PropertyMetadata(null, new PropertyChangedCallback(UISelectSettingControl.OnSettingTypePropertyChanged)));

        private static void OnSettingTypePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is UISelectSettingControl)
            {
                (obj as UISelectSettingControl).OnSettingTypeValueChanged();
            }
        }

        protected void OnSettingTypeValueChanged()
        {
            ResetItems();
        }
        #endregion

        #region SplitSeperate DependencyProperty
        public string SplitSeperate
        {
            get { return (string)GetValue(SplitSeperateProperty); }
            set { SetValue(SplitSeperateProperty, value); }
        }
        public static readonly DependencyProperty SplitSeperateProperty =
                DependencyProperty.Register("SplitSeperate", typeof(string), typeof(UISelectSettingControl),
                new PropertyMetadata(",", new PropertyChangedCallback(UISelectSettingControl.OnSplitSeperatePropertyChanged)));

        private static void OnSplitSeperatePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is UISelectSettingControl)
            {
                (obj as UISelectSettingControl).OnSplitSeperateValueChanged();
            }
        }

        protected void OnSplitSeperateValueChanged()
        {

        }
        #endregion

        private void ResetItems()
        {
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }
            if (ListItems.Count > 0)
            {
                foreach (var item in ListItems)
                {
                    item.IsSelectedChanged -= item_IsSelectedChanged;
                }
                ListItems.Clear();
            }
            if (string.IsNullOrEmpty(SettingType))
            {
                return;
            }
            var setting = UISettings.Current.Items[SettingType];
            if (setting != null && (setting is UISelectSetting))
            {
                var selectSetting = setting as UISelectSetting;

                _CanEdit = selectSetting.CanUserInput;
                _IsMutiSelect = selectSetting.CanMutiSelect;
                comboBox.IsEditable = _CanEdit;

                var selects = GetTextItems();
                foreach (var setitem in selectSetting.Items)
                {
                    var item = new UISelectSettingItem
                    {
                        SettingItem = setitem,
                        IsSelected = selects.FirstOrDefault(s => s.Equals(setitem.Name, StringComparison.OrdinalIgnoreCase)) != null
                    };
                    item.IsSelectedChanged += item_IsSelectedChanged;
                    ListItems.Add(item);
                } 
            }
            else
            {
                var selectSetting = new UISelectSetting
                {
                    TargetType = SettingType,
                    CanMutiSelect = true,
                    CanUserInput = true,
                    Description = "自动创建"
                };
                _CanEdit = selectSetting.CanUserInput;
                _IsMutiSelect = selectSetting.CanMutiSelect;
                comboBox.IsEditable = _CanEdit;
                UISettings.Current.AddSetting(selectSetting); 
            }
        }

        private string[] GetTextItems()
        {
            if (string.IsNullOrEmpty(comboBox.Text))
            {
                return new string[0] { };
            }
            if(string.IsNullOrEmpty(SplitSeperate))
            {
                return new string[] { comboBox.Text };
            }
            var selects = comboBox.Text.Split(new string[] { SplitSeperate }, StringSplitOptions.RemoveEmptyEntries);
            return selects;
        } 

        void item_IsSelectedChanged(object sender, EventArgs e)
        {
            if (_IsSelfChanged)
            {
                return;
            }

            _IsSelfChanged = true;

            var item = sender as UISelectSettingItem;

            if (item.IsSelected && _IsMutiSelect==false)
            {
                foreach (var s in ListItems.Where(w => w.IsSelected && (w != item)))
                {
                    s.IsSelected = false;
                }
            }
            var texts = GetTextItems().ToList();
            if (item.IsSelected)
            {
                if (texts.FirstOrDefault(s => s.Equals(item.SettingItem.Name, StringComparison.OrdinalIgnoreCase)) == null)
                {
                    texts.Add(item.SettingItem.Name);
                }
            }
            else
            {
                var sitem = texts.FirstOrDefault(s => s.Equals(item.SettingItem.Name, StringComparison.OrdinalIgnoreCase));
                if (sitem != null)
                {
                    texts.Remove(sitem);
                }
            }
            StringBuilder sb = new StringBuilder();
            texts.ForEachWithFirst((s) =>
            {
                sb.Append(s);
            },
            (s) =>
            {
                sb.AppendFormat("{0}{1}", SplitSeperate, s);
            });
            comboBox.Text = sb.ToString();
            _IsSelfChanged = false;
        }
        
        private void ResetItemSelected()
        {
            var selects = GetTextItems();
            foreach (var item in ListItems)
            {
                item.IsSelected = selects.FirstOrDefault(s => s.Equals(item.SettingItem.Name, StringComparison.OrdinalIgnoreCase)) != null;
            }
        }

        private void comboBox_TextInput(object sender, TextCompositionEventArgs e)
        {

        }
    }

    public class UISelectSettingItem : EntityBase
    {
        public event EventHandler IsSelectedChanged;

        #region 属性 IsSelected
        private bool _IsSelected = false;
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                if (_IsSelected == value)
                {
                    return;
                }
                _IsSelected = value;
                RaisePropertyChanged(() => IsSelected);
                if (IsSelectedChanged != null)
                {
                    IsSelectedChanged(this, new EventArgs());
                }
            }
        }
        #endregion

        #region 属性 SettingItem
        private UISettingOption _SettingItem = null;
        public UISettingOption SettingItem
        {
            get
            {
                return _SettingItem;
            }
            set
            {
                _SettingItem = value;
                RaisePropertyChanged(() => SettingItem);
            }
        }
        #endregion
    }

    public class UISelectSettingComboBox : System.Windows.Controls.ComboBox
    {
        #region SettingType DependencyProperty
        public string SettingType
        {
            get { return (string)GetValue(SettingTypeProperty); }
            set { SetValue(SettingTypeProperty, value); }
        }
        public static readonly DependencyProperty SettingTypeProperty =
                DependencyProperty.Register("SettingType", typeof(string), typeof(UISelectSettingComboBox),
                new PropertyMetadata(null, new PropertyChangedCallback(UISelectSettingComboBox.OnSettingTypePropertyChanged)));

        private static void OnSettingTypePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is UISelectSettingComboBox)
            {
                (obj as UISelectSettingComboBox).OnSettingTypeValueChanged();
            }
        }

        protected void OnSettingTypeValueChanged()
        {
            ResetItems();
        }

        private void ResetItems()
        {
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }
            var setting = UISettings.Current.Items[SettingType];
            if (setting != null && (setting is UISelectSetting))
            {
                var selectSetting = setting as UISelectSetting;
                this.ItemsSource = selectSetting.Items;
                this.DisplayMemberPath = "Description";
                this.SelectedValuePath = "Name";
            }
        }
        #endregion
			
    }

}
