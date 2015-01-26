using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Panasia.Core;
using Panasia.Core.Wpf;

namespace Panasia.Core.Web.Controls
{
    public class CommandMenu:EntityBase
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
                RaisePropertyChanged(() => Title);
            }
        }
        #endregion

        #region MenuCommand
        private RelayCommand _MenuCommand = null;
        public RelayCommand MenuCommand
        {
            get
            { 
                return _MenuCommand;
            }
            set
            {
                _MenuCommand = value;
                RaisePropertyChanged(() => MenuCommand);
            }
        }
        #endregion

        #region 属性 SubMenus
        private CollectionBase<CommandMenu> _SubMenus = null;
        public CollectionBase<CommandMenu> SubMenus
        {
            get
            {
                if (_SubMenus == null)
                {
                    _SubMenus = new CollectionBase<CommandMenu>();
                }
                return _SubMenus;
            }
            set
            {
                _SubMenus = value;
                RaisePropertyChanged(() => SubMenus);
            }
        }
        #endregion

        public CommandMenu()
        {

        }

        public CommandMenu(string title,Action<object> execute)
        {
            Title = title;
            MenuCommand = new RelayCommand(execute, (obj)=>true);
        }
        public CommandMenu(string title,Action<object> execute, Func<object, bool> canExecute)
        {
            Title = title;
            MenuCommand = new RelayCommand(execute, canExecute);
        }
    }
}
