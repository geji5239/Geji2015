using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Panasia.Core.Wpf;
using Panasia.Core.Commands;
using Panasia.Core.App;
using Microsoft.Win32;

namespace Panasia.Core.Web.ViewModels
{
    [PageModel("SqlReports")]
    public class SqlReportsViewModel : Panasia.Core.Wpf.PageModel
    {
        #region 属性 ReportItems
        private CollectionBase<SqlReportItem> _ReportItems = new CollectionBase<SqlReportItem>();
        public CollectionBase<SqlReportItem> ReportItems
        {
            get
            {
                return _ReportItems;
            }
        }
        #endregion

        #region 属性 DisplayItems
        public IEnumerable<SqlReportItem> DisplayItems
        {
            get
            {
                if (string.IsNullOrEmpty(FilterText))
                {
                    return ReportItems;
                }

                return ReportItems.Where(r => r.InFilter(FilterText));
            }
        }
        #endregion

        #region 属性 FilterText
        private string _FilterText = string.Empty;
        public string FilterText
        {
            get
            {
                return _FilterText;
            }
            set
            {
                _FilterText = value;
                RaisePropertyChanged(() => FilterText);
                RaisePropertyChanged(() => DisplayItems);
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
            return true;
        }

        private void OnReload(object parameter)
        {

        }
        #endregion

        #region ExportCommand
        private RelayCommand _ExportCommand = null;
        public RelayCommand ExportCommand
        {
            get
            {
                if (_ExportCommand == null)
                {
                    _ExportCommand = new RelayCommand(OnExport, CanExport);
                }
                return _ExportCommand;
            }
        }

        private bool CanExport(object parameter)
        {
            return true;
        }

        private void OnExport(object parameter)
        {
            SaveFileDialog file = new SaveFileDialog();
            file.Filter = "excel文件|*.xls";

            if (file.ShowDialog() == true)
            {
                using (var stream = file.OpenFile())
                {
                    var bytes = System.Text.Encoding.UTF8.GetBytes(ReportItems.CreateTable());
                    stream.Write(bytes, 0, bytes.Length);
                }

                if (MessageBox.Show("导出文件完成，是否需要打开?", "打开文件",
                    System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
                {
                    System.Diagnostics.Process.Start(file.FileName);
                }
            }
        }
        #endregion

        #region SaveSqlDataCommand
        private RelayCommand _SaveSqlDataCommand = null;
        public RelayCommand SaveSqlDataCommand
        {
            get
            {
                if (_SaveSqlDataCommand == null)
                {
                    _SaveSqlDataCommand = new RelayCommand(OnSaveSqlData, CanSaveSqlData);
                }
                return _SaveSqlDataCommand;
            }
        }

        private bool CanSaveSqlData(object parameter)
        {
            return true;
        }

        private void OnSaveSqlData(object parameter)
        {
            SqlData.Current.Save();
        }
        #endregion

        private void Reload()
        {
            ReportItems.Clear();
            LoadAllItems().AddToCollection(ReportItems);
        }

        private IEnumerable<SqlReportItem> LoadAllItems()
        {
            foreach (var group in App.AppConfig.Current.PageGroups)
            {
                foreach (var page in group.Pages)
                {
                    foreach (var action in page.Config.Actions)
                    {
                        foreach (var command in action.Commands)
                        {
                            try
                            {
                                if(command.Config ==null)
                                {
                                    MessageBox.Show(page.PageID + "/" + action.Name + "/" + command.Name + ":配置加载失败", "", System.Windows.MessageBoxButton.OK);
                                    continue;
                                }
                            }
                            catch (Exception ex)
                            {
                                this.Log(page.PageID + "/" + action.Name + "/" +command.Name + "\r\n" + ex.ToString(), Category.Exception);
                            }
                            switch (command.CommandType)
                            {
                                #region SqlCommand
                                case "ScalarSqlCommand":
                                case "NoneQuerySqlCommand":
                                case "QuerySqlCommand":
                                case "QueryJsonSqlCommand":
                                    var sqlCommand = command.Config as SqlCommand;
                                    if (sqlCommand != null)
                                    {
                                        var sqlItem = sqlCommand.SqlItem;

                                        yield return new SqlReportItem
                                        {
                                            ModuleName = group.Name,
                                            PageID = page.PageID,
                                            PageName = page.Title,
                                            ActionName = action.Name,
                                            ActionTitle = action.Title,
                                            CommandName = command.Name,
                                            SqlGroupName = sqlCommand.GroupName,
                                            SqlTableName = sqlCommand.TableName,
                                            SqlName = sqlCommand.CommandName,
                                            SqlDescription = sqlItem == null ? "" : sqlItem.Description,
                                            SqlType = sqlItem == null ? "" : sqlItem.SqlType.ToString(),
                                            SqlItem = sqlItem
                                        };
                                    }
                                    break;
                                #endregion

                                #region EditForm
                                case "EditDialogActionCommand":
                                    var dialogCommand = command.Config as EditDialogActionCommand;
                                    if (dialogCommand != null)
                                    {
                                        foreach (var input in dialogCommand.Page.Form.Fields)
                                        {
                                            switch (input.FieldType)
                                            {
                                                case UIFieldType.ComboBox:
                                                    if (input.Input is UIComboBox)
                                                    {
                                                        var comboBox = input.Input as UIComboBox;
                                                        var item = CreateSqlReportItem(comboBox.DataUrl, group, page, action, command);
                                                        if (item != null)
                                                        {
                                                            yield return item;
                                                        }
                                                    }
                                                    break;
                                                case UIFieldType.ComboTree:
                                                    if (input.Input is UIComboTree)
                                                    {
                                                        var comboBox = input.Input as UIComboTree;
                                                        var item = CreateSqlReportItem(comboBox.DataUrl, group, page, action, command);
                                                        if (item != null)
                                                        {
                                                            yield return item;
                                                        }
                                                    }
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }
                                    break;
                                #endregion
                                default:
                                    break;
                            }

                        }
                    }
                }
            }
            yield break;
        }

        private static SqlReportItem CreateSqlReportItem(string dataUrl, PageGroup group, Page page, Panasia.Core.App.Action action,
            CommandConfig command)
        {
            if (!string.IsNullOrEmpty(dataUrl))
            {
                if (dataUrl.StartsWith("/Share/Query", StringComparison.OrdinalIgnoreCase))
                {
                    var name = dataUrl.Substring(13).Split('&').FirstOrDefault(p => p.StartsWith("name=", StringComparison.OrdinalIgnoreCase));
                    if (name != null)
                    {
                        var shareSqlName = name.Substring(5);
                        var sqlItem = SqlData.Current.GetShare(shareSqlName);

                        var item = new SqlReportItem
                        {
                            ModuleName = group.Name,
                            PageID = page.PageID,
                            PageName = page.Title,
                            ActionName = action.Name,
                            ActionTitle = action.Title,
                            CommandName = command.Name
                        };

                        if (sqlItem != null)
                        {
                            item.SqlName = string.Format("{0}[{1}]", sqlItem.Name, shareSqlName);
                            item.SqlDescription = sqlItem.Description;
                            item.SqlType = sqlItem.SqlType.ToString();

                            if (sqlItem.Group != null)
                            {
                                item.SqlTableName = sqlItem.Group.Name;
                                if (sqlItem.Group.Group != null)
                                {
                                    item.SqlGroupName = sqlItem.Group.Group.Name;
                                }
                            }
                        }
                        else
                        {
                            item.SqlName = shareSqlName;
                            item.SqlDescription = "命令不存在";
                        }
                        item.SqlItem = sqlItem;
                        return item;
                    }
                }
            }
            return null;
        }

        public override void Open(Dictionary<string, string> paras, Action<bool> callBack)
        {
            base.Open(paras, callBack);
            try
            {
                Reload();
            }
            catch (Exception ex)
            {
                this.Log(ex.ToString(), Category.Exception);
            }
        }
    }

    static class SqlReportExtension
    {
        public static string CreateTable(this IEnumerable<SqlReportItem> items)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<table>");

            sb.Append("<tr>");
            sb.AppendFormat("<th>{0}</th>", "模块名称");
            sb.AppendFormat("<th>{0}</th>", "页面名称");
            sb.AppendFormat("<th>{0}</th>", "页面编号");
            sb.AppendFormat("<th>{0}</th>", "功能名称");
            sb.AppendFormat("<th>{0}</th>", "功能标题");
            sb.AppendFormat("<th>{0}</th>", "功能命令");
            sb.AppendFormat("<th>{0}</th>", "SQL分组");
            sb.AppendFormat("<th>{0}</th>", "表名");
            sb.AppendFormat("<th>{0}</th>", "命令名");
            sb.AppendFormat("<th>{0}</th>", "命令名");
            sb.AppendFormat("<th>{0}</th>", "命令说明");
            sb.Append("</tr>");

            foreach (var item in items)
            {
                sb.Append("<tr>");
                sb.AppendFormat("<td>{0}</td>", item.ModuleName);
                sb.AppendFormat("<td>{0}</td>", item.PageName);
                sb.AppendFormat("<td>{0}</td>", item.PageID);
                sb.AppendFormat("<td>{0}</td>", item.ActionName);
                sb.AppendFormat("<td>{0}</td>", item.ActionTitle);
                sb.AppendFormat("<td>{0}</td>", item.CommandName);
                sb.AppendFormat("<td>{0}</td>", item.SqlGroupName);
                sb.AppendFormat("<td>{0}</td>", item.SqlTableName);
                sb.AppendFormat("<td>{0}</td>", item.CommandName);
                sb.AppendFormat("<td>{0}</td>", item.SqlName);
                sb.AppendFormat("<td>{0}</td>", item.SqlDescription);
                sb.Append("</tr>");
            }

            sb.Append("</table>");
            return sb.ToString();
        }
    }

    public class SqlReportItem : EntityBase
    {
        #region 属性 ModuleName
        private string _ModuleName = string.Empty;
        public string ModuleName
        {
            get
            {
                return _ModuleName;
            }
            set
            {
                _ModuleName = value;
                RaisePropertyChanged(() => ModuleName);
            }
        }
        #endregion

        #region 属性 PageName
        private string _PageName = string.Empty;
        public string PageName
        {
            get
            {
                return _PageName;
            }
            set
            {
                _PageName = value;
                RaisePropertyChanged(() => PageName);
            }
        }
        #endregion

        #region 属性 PageID
        private string _PageID = string.Empty;
        public string PageID
        {
            get
            {
                return _PageID;
            }
            set
            {
                _PageID = value;
                RaisePropertyChanged(() => PageID);
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

        #region 属性 ActionTitle
        private string _ActionTitle = string.Empty;
        public string ActionTitle
        {
            get
            {
                return _ActionTitle;
            }
            set
            {
                _ActionTitle = value;
                RaisePropertyChanged(() => ActionTitle);
            }
        }
        #endregion

        #region 属性 CommandName
        private string _CommandName = string.Empty;
        public string CommandName
        {
            get
            {
                return _CommandName;
            }
            set
            {
                _CommandName = value;
                RaisePropertyChanged(() => CommandName);
            }
        }
        #endregion

        #region 属性 SqlGroupName
        private string _SqlGroupName = string.Empty;
        public string SqlGroupName
        {
            get
            {
                return _SqlGroupName;
            }
            set
            {
                _SqlGroupName = value;
                RaisePropertyChanged(() => SqlGroupName);
            }
        }
        #endregion

        #region 属性 SqlTableName
        private string _SqlTableName = string.Empty;
        public string SqlTableName
        {
            get
            {
                return _SqlTableName;
            }
            set
            {
                _SqlTableName = value;
                RaisePropertyChanged(() => SqlTableName);
            }
        }
        #endregion

        #region 属性 SqlName
        private string _SqlName = string.Empty;
        public string SqlName
        {
            get
            {
                return _SqlName;
            }
            set
            {
                _SqlName = value;
                RaisePropertyChanged(() => SqlName);
            }
        }
        #endregion

        #region 属性 SqlType
        private string _SqlType = string.Empty;
        public string SqlType
        {
            get
            {
                return _SqlType;
            }
            set
            {
                _SqlType = value;
                RaisePropertyChanged(() => SqlType);
            }
        }
        #endregion

        #region 属性 SqlDescription
        private string _SqlDescription = string.Empty;
        public string SqlDescription
        {
            get
            {
                return _SqlDescription;
            }
            set
            {
                _SqlDescription = value;
                RaisePropertyChanged(() => SqlDescription);

                if (SqlItem != null)
                {
                    SqlItem.Description = value;
                }
            }
        }
        #endregion

        #region 属性 SqlItem
        private SqlItem _SqlItem = null;
        public SqlItem SqlItem
        {
            get
            {
                return _SqlItem;
            }
            set
            {
                _SqlItem = value;
                RaisePropertyChanged(() => SqlItem);
            }
        }
        #endregion

        public bool InFilter(string filterText)
        {
            return ModuleName.Contains(filterText) ||
                PageName.Contains(filterText) ||
                PageID.Contains(filterText) ||
                ActionName.Contains(filterText) ||
                ActionTitle.Contains(filterText) ||
                SqlGroupName.Contains(filterText) ||
                SqlTableName.Contains(filterText) ||
                SqlName.Contains(filterText) ||
                SqlDescription.Contains(filterText);
        }
    }
}
