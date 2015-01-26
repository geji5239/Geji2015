using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Panasia.Core.Wpf;
using Panasia.Core.Web.Settings;

namespace Panasia.Core.Web.ViewModels
{
    [PageModel("ControlSettings")]
    public class ControlSettingViewModel : Panasia.Core.Wpf.PageModel
    {
        #region 属性 ControlSettings
        private ControlSettings _ControlSettings = ControlSettings.Current;
        public ControlSettings ControlSettings
        {
            get
            {
                return _ControlSettings;
            } 
        }
        #endregion

        #region SaveCommand
        private RelayCommand _SaveCommand = null;
        public RelayCommand SaveCommand
        {
            get
            {
                if (_SaveCommand == null)
                {
                    _SaveCommand = new RelayCommand(OnSave, CanSave);
                }
                return _SaveCommand;
            }
        }

        private bool CanSave(object parameter)
        {
            return true;
        }

        private void OnSave(object parameter)
        {
            ControlSettings.Current.Save();
        }
        #endregion
    }
}
