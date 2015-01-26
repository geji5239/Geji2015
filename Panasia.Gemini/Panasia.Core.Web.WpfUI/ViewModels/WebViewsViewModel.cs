using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Panasia.Core.Wpf;

namespace Panasia.Core.Web
{
    [PageModel("WebViews")]
    public class WebViewsViewModel:Panasia.Core.Wpf.PageModel
    {
        #region 属性 Views
        private ViewFileCollection _Views = null;
        public ViewFileCollection Views
        {
            get
            {
                if(_Views==null)
                {
                    var root = new ViewFileFolder { Root = "~\\", FileName="Views" };
                    root.Reload();
                    _Views = new ViewFileCollection
                    {
                        root
                    };
                }
                return _Views;
            }
            set
            {
                _Views = value;
                RaisePropertyChanged(() => Views);
            }
        }
        #endregion

        #region 属性 SelectedItem
        private ViewFileBase _SelectedItem = null;
        public ViewFileBase SelectedItem
        {
            get
            {
                return _SelectedItem;
            }
            set
            {
                _SelectedItem = value;
                RaisePropertyChanged(() => SelectedItem);

                ControlName = (SelectedItem ==null)? "" :( (SelectedItem is ViewFileFolder) ? "Folder" : "File");
            }
        }
        #endregion

        #region 属性 ControlName
        private string _ControlName = string.Empty;
        public string ControlName
        {
            get
            {
                return _ControlName;
            }
            set
            {
                _ControlName = value;
                RaisePropertyChanged(() => ControlName);
            }
        }
        #endregion
        
        #region ReloadCommand
        private RelayCommand _ReloadCommand = null;
        public RelayCommand ReloadCommand
        {
            get
            {
                if (_ReloadCommand == null)
                {
                    _ReloadCommand = new RelayCommand(OnReload, CanReload);
                }
                return _ReloadCommand;
            }
        }

        private bool CanReload(object parameter)
        {
            return SelectedItem !=null;
        }

        private void OnReload(object parameter)
        {
            if(SelectedItem!=null)
            {
                SelectedItem.Reload();
            }
        }
        #endregion

        #region SaveFileCommand
        private RelayCommand _SaveFileCommand = null;
        public RelayCommand SaveFileCommand
        {
            get
            {
                if (_SaveFileCommand == null)
                {
                    _SaveFileCommand = new RelayCommand(OnSaveFile, CanSaveFile);
                }
                return _SaveFileCommand;
            }
        }

        private bool CanSaveFile(object parameter)
        {
            return SelectedItem is ViewFile;
        }

        private void OnSaveFile(object parameter)
        {
            if(SelectedItem is ViewFile)
            {
                (SelectedItem as ViewFile).Save();
            }
        }
        #endregion
    }   

}
