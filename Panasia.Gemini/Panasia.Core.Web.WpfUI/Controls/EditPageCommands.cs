using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Panasia.Core.Wpf;

namespace Panasia.Core.Web.Controls
{
    public class EditPageCommands
    {
        #region GetTableCommand
        private RelayCommand _GetTableCommand = null;
        public RelayCommand GetTableCommand
        {
            get
            {
                if (_GetTableCommand == null)
                {
                    _GetTableCommand = new RelayCommand(OnGetTable, CanGetTable);
                }
                return _GetTableCommand;
            }
        }

        private bool CanGetTable(object parameter)
        {
            return parameter is EditField;
        }

        private void OnGetTable(object parameter)
        {
            var editField = parameter as EditField;
            SelectDataTableWindow win = new SelectDataTableWindow((group, table) =>
            {
                editField.ForeignGroup = group;
                editField.ForeignTable = table; 
            });
            win.ShowDialog(); 
        }
        #endregion
    }
}
