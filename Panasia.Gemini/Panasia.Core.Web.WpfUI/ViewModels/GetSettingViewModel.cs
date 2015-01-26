using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Panasia.Core;
using Panasia.Core.Wpf;
using Panasia.Core.Web.Settings;

namespace Panasia.Core.Web
{
    public class GetSettingViewModel : EntityBase
    {
        private Action<string> _Callback = null;
        private string _Split = "";

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
                RaisePropertyChanged(() => Title);
            }
        }
        #endregion

        #region 属性 Settings
        private CollectionBase<SettingItemViewModel> _Settings = null;
        public CollectionBase<SettingItemViewModel> Settings
        {
            get
            {
                if(_Settings ==null)
                {
                    _Settings = new CollectionBase<SettingItemViewModel>();
                }
                return _Settings;
            }
        }
        #endregion

        public GetSettingViewModel(IEnumerable<SettingItem> groups, string controlName, Action<string> callback, string split)
        {
            _Callback = callback;
            _Split = split;
            
            groups.Where(s => string.IsNullOrEmpty(s.Targets)
                    || s.Targets.IndexOf(controlName, StringComparison.OrdinalIgnoreCase) >= 0)
                    .Select(s => new SettingItemViewModel { SettingItem = s })
                    .AddToCollection(Settings);
        }

        #region OKCommand
        private RelayCommand _OKCommand = null;
        public RelayCommand OKCommand
        {
            get
            {
                if (_OKCommand == null)
                {
                    _OKCommand = new RelayCommand(OnOK, CanOK);
                }
                return _OKCommand;
            }
        }

        private bool CanOK(object parameter)
        {
            return true;
        }

        private void OnOK(object parameter)
        {
            if (_Callback != null)
            {
                _Callback(Settings.Where(s => s.IsSelected).Select(s => s.SettingItem).GetValues(_Split));
            }
            if (parameter != null && parameter is Window)
            {
                (parameter as Window).Close();
            }
        }
        #endregion
    }

    public class SettingItemViewModel : EntityBase
    {
        #region 属性 SettingItem
        private SettingItem _SettingItem = null;
        public SettingItem SettingItem
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
                _IsSelected = value;
                RaisePropertyChanged(() => IsSelected);
            }
        }
        #endregion
    }
}
