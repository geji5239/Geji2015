using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Panasia.Core;
using Panasia.Core.Commands;
using Panasia.Core.App;
using Action = Panasia.Core.App.Action;

namespace Panasia.Core.Web
{
    [PageConfig("MvcEditPage", "一般编辑页面", typeof(MvcEditPage))]
    public class MvcEditPage : MvcPage
    {
        internal const string CommandType_FormView = "FormViewActionCommand";

        internal const string Default_DetailControl = "TextBox";

        internal const string SP_GetPage = "sp_GetPage";

        internal const string SP_GetCount = "sp_GetCount";

        internal const string Query_Page = "QueryPage";

        internal const string Query_Count = "QueryCount";

        private bool _SqlNotSave = false;

        #region 属性 Layout
        private Layout _Layout = null;
        [XmlElement("Layout")]
        public Layout Layout
        {
            get
            {
                if (_Layout == null)
                {
                    _Layout = new Layout();
                }
                return _Layout;
            }
            set
            {
                _Layout = value;
                RaisePropertyChanged(() => Layout);
            }
        }
        #endregion

        #region 属性 Buttons
        private EditButtonCollection _Buttons = null;
        [XmlArray("Buttons"), XmlArrayItem("Button", typeof(EditButton))]
        public EditButtonCollection Buttons
        {
            get
            {
                if (_Buttons == null)
                {
                    _Buttons = new EditButtonCollection();
                }
                return _Buttons;
            }
            set
            {
                _Buttons = value;
                RaisePropertyChanged(() => Buttons);
            }
        }
        #endregion

        #region 属性 EditSource
        private EditSource _EditSource = null;
        [XmlElement("EditSource")]
        public EditSource EditSource
        {
            get
            {
                if (_EditSource == null)
                {
                    _EditSource = new EditSource();
                }
                return _EditSource;
            }
            set
            {
                _EditSource = value;
                RaisePropertyChanged(() => EditSource);
            }
        }
        #endregion

        #region 创建基本按钮
        public void CreateButtons()
        {
            if (Page == null)
            {
                return;
            }
            var editButtonClass = Layout.EditButtonClass;

            Buttons = new EditButtonCollection
            {
                new EditButton("btn-search",2, "检索",
                    "showActDlg('检索','/Page/"+Page.PageID+"/Search',400,300)",true,
                    "easyui-linkbutton normal-button","","iconCls:'icon-search',plain:true"),

                new EditButton("btn-query",  2,"确定",
                    "submitAct_DGReplace('dataform','/Page/"+Page.PageID+"/Query','datagrid')",false,
                    editButtonClass,"","iconCls:'icon-ok'"),

                new EditButton(  "btn-detail", 4, "查看",  
                    "showActDlg_DGItemActDlg('datagrid','查看','/Page/"+Page.PageID+"/Detail',400,300)",  true, 
                    "easyui-linkbutton toolradio normal-button",  "",  "iconCls:'icon-detail',plain:true"),

                new EditButton(  "btn-create", 8,"新增", 
                    "showActDlg('新增','/Page/"+Page.PageID+"/Create',400,300)",  true, 
                    "easyui-linkbutton normal-button",  "",  "iconCls:'icon-add',plain:true"),

                new EditButton("btn-add",  8,"确定",
                    "submitAct_DGAppend('dataform','/Page/"+Page.PageID+"/Add','datagrid')",false,
                    editButtonClass,"","iconCls:'icon-ok'"),

                new EditButton("btn-edit",16,"修改",
                    "showActDlg_DGItemActDlg('datagrid','修改','/Page/"+Page.PageID+"/Edit',400,300)",true,
                    "easyui-linkbutton toolradio normal-button","","iconCls:'icon-edit',plain:true"),

                new EditButton("btn-update", 16,"确定",
                    "submitAct_DGUpdate('dataform','/Page/"+Page.PageID+"/Update','datagrid')",false,
                    editButtonClass,"","iconCls:'icon-ok',plain:true"),
                 
                new EditButton("btn-delete",32,"删除",
                    "showActDlg_DGItemsConfirm('datagrid','删除','确认删除选择项？','/Page/"+Page.PageID+"/Delete',400,300)",true,
                    "easyui-linkbutton toolmultiple normal-button","","iconCls:'icon-cancel',plain:true"),

                new EditButton("btn-import",64,"导入",
                    "showActDlg('导入','/Page/"+Page.PageID+"/Import',400,300)",true,
                    "easyui-linkbutton normal-button","","iconCls:'icon-import',plain:true"),

                new EditButton("btn-export",128,"导出",
                    "showActDlg('导出','/Page/"+Page.PageID+"/Export',400,300)",true,
                    "easyui-linkbutton normal-button","","iconCls:'icon-export',plain:true"),

                new EditButton("btn-cancel", 0,"取消",
                    "cancel()",false,
                    editButtonClass,"","iconCls:'icon-cancel'"),

                new EditButton("btn-close", 0,"确定",
                    "cancel()",false,
                    editButtonClass,"","")
            };
            this.Log(string.Format("create button Page:{0}-{1}", Page.PageID, Page.Title));
        }
        #endregion

        #region 导入表
        public bool ImportTable()
        {
            if (EditSource == null || string.IsNullOrEmpty(EditSource.GroupName) || string.IsNullOrEmpty(EditSource.TableName))
            {
                return false;
            }

            var group = SqlData.Current.Groups[EditSource.GroupName];
            if (group == null)
            {
                return false;
            }

            var table = group.Items[EditSource.TableName];
            if (table == null)
            {
                return false;
            }

            var conn = SqlData.Current.Connections[table.ConnectionName];
            var tableSchema = conn.Tables[table.TableName];
            if (tableSchema == null)
            {
                return false;
            }

            EditSource.Key = tableSchema.Key;
            var deleteFlag = tableSchema.Columns.FirstOrDefault(c => c.Name.Equals("IsActive", StringComparison.OrdinalIgnoreCase));

            EditSource.LogicDeleteFlag = (deleteFlag == null) ? "" : string.Format("{0}=0", deleteFlag.Name);

            var systemColumns = new string[] { "CreatedUser", "CreatedTime", "ModifiedUser", "ModifiedTime", "AutoKey" };
            Func<Column, bool> isSystemColumn = (column) =>
            {
                return (column.IsAutoIncrement ||
                    systemColumns.FirstOrDefault(c => c.Equals(column.Name, StringComparison.OrdinalIgnoreCase)) != null);
            };

            foreach (var column in tableSchema.Columns)
            {
                var field = EditSource.Fields.FirstOrDefault(f => f.Name.Equals(column.Name, StringComparison.OrdinalIgnoreCase));
                if (field == null)
                {
                    field = new EditField
                    {
                        Name = column.Name,
                        Title = column.Name,
                        ShowInCreate = !isSystemColumn(column),
                        ShowInDetail = !isSystemColumn(column),
                        ShowInEdit = !isSystemColumn(column),
                        ShowInGrid = !isSystemColumn(column),
                        ShowInSearch = !isSystemColumn(column),
                        IsRequired = !isSystemColumn(column),
                        EnableEdit = !isSystemColumn(column)
                    };

                    if (column.Name.Equals(EditSource.Key))
                    {
                        field.EditBox = UIFieldType.Hidden;
                    }
                    else if (column.Name.IndexOf("Password") >= 0)
                    {
                        field.EditBox = UIFieldType.PasswordBox;
                    }
                    else
                    {
                        field.EditBox = UIFieldType.TextBox;
                    }

                    EditSource.Fields.Add(field);
                }
                field.DataType = column.ColumnType.ToString();
                field.Length = (int)column.Length;

                if (field.DataType.Equals("bit", StringComparison.OrdinalIgnoreCase))
                {
                    field.EditBox = UIFieldType.ComboBox;
                    field.ItemsSource = "1:是|0:否";
                }
            }

            if (Buttons.Count == 0)
            {
                CreateButtons();
            }

            return true;
        }
        #endregion

        #region 创建方法
        public void CreateSettings(ActionType updateActions = ActionType.All)
        {
            _SqlNotSave = false;

            //Create Index
            if ((updateActions & ActionType.Index) == ActionType.Index)
            {
                CreateActionIndex();
            }

            if ((updateActions & ActionType.Search) == ActionType.Search)
            {
                CreateActionSearch();
            }

            if ((updateActions & ActionType.Query) == ActionType.Query)
            {
                CreateActionQueryIndex(); //CreateActionQuery();
            }

            if ((updateActions & ActionType.Detail) == ActionType.Detail)
            {
                CreateActionDetail();
            }

            if ((updateActions & ActionType.Create) == ActionType.Create)
            {
                CreateActionCreate();
            }

            if ((updateActions & ActionType.Add) == ActionType.Add)
            {
                CreateActionAdd();
            }

            if ((updateActions & ActionType.Edit) == ActionType.Edit)
            {
                CreateActionEdit();
            }

            if ((updateActions & ActionType.Update) == ActionType.Update)
            {
                CreateActionUpdate();
            }

            if ((updateActions & ActionType.Delete) == ActionType.Delete)
            {
                CreateActionDelete();
            }

            if (_SqlNotSave)
            {
                SqlData.Current.Save();
            }
        }
        #endregion

        #region 创建Action_Index列表
        private void CreateActionIndex()
        {
            var action = GetOrCreateAction("Index", 1, "列表");

            var command = CreateCommand(action, ActionCommands.IndexPageActionCommand, "列表页面", "这个命令会直接返回到客户端，无需后继命令");

            var config = command.Config;

            if (config != null && config is IndexPageActionCommand)
            {
                var commandConfig = config as IndexPageActionCommand;

                var page = commandConfig.Page;
                var toolBar = commandConfig.Page.ToolBar;

                toolBar.PageID = PageID;
                toolBar.Buttons.Clear();
                Buttons.Where(b => b.ShowInToolBar)
                    .Select(item => new UIButton
                    {
                        ID = item.ID,
                        Title = item.Title,
                        ActionValue = item.ActionValue,
                        Action = item.Action,
                        Class = item.Class,
                        Options = item.Option
                    }).AddToCollection(toolBar.Buttons);

                var grid = page.DataGrid;
                grid.Options = string.Format("url:'/Page/{0}/Query',", PageID) + grid.Options;
                grid.Key = EditSource.Key;
                EditSource.Fields.Where(f => f.ShowInGrid || f.Name.Equals(EditSource.Key)).ForEach(f =>
                {
                    var col = new UIDataColumn
                    {
                        IsCheckBox = f.Name.Equals(EditSource.Key),
                        Title = f.Title,
                        FieldName = f.Name,
                        HAlign = f.ValueType == QueryValueType.Char ? HAlignment.Left :
                            f.ValueType == QueryValueType.Number ? HAlignment.Right : HAlignment.Center
                    };
                    grid.Columns.Add(col);
                });
            }
        }

        #endregion

        #region 创建Action_Search搜索页面
        private void CreateActionSearch()
        {
            var action = GetOrCreateAction("Search", 2, "检索");

            var commands = Create_ItemsSource(action, EditSource.Fields.Where(f => f.ShowInSearch && f.IsForeignKey), "检索页面").ToList();

            var commandConfig = CreateCommand_EditDialogActionCommand(action, "检索页面", "这个命令会直接返回到客户端，无需后继命令");
            commandConfig.Page.Form = Create_SearchForm();

            commands.ForEach(c =>
            {
                commandConfig.Parameters.Add(new CommandParameter
                {
                    Name = c.Name,
                    Type = CommandParameterType.Parameter,
                    Path = string.Format("Results[{0}].Items", c.Name)
                });
            });
        }
        #endregion

        #region 创建Action_执行搜索查询
        private void CreateActionQueryIndex()
        {
            var action = GetOrCreateAction("Query", 2, "检索");

            #region 合计数的命令
            var countCommand = CreateCommand_SqlCommand(action, ActionCommands.ScalarSqlCommand, "查询总数", "本查询的结果值为JSON，参数路径为Results[检索分页].Json",
                "分页查询",
                Query_Count,
                () => new SqlItem
                    {
                        Name =Query_Count,
                        Description = "检索查询总数", 
                        CommandText = CreateSql_QueryCount()
                    });
            if (countCommand != null)
            {
                UpdateQueryCommandParameters(countCommand);
            }
            CreateConverter_SearchField(countCommand);
            #endregion

            #region 分页查询命令
            //添加查询命令
            var sqlCommand = CreateCommand_SqlCommand(action, ActionCommands.QuerySqlCommand, "分页查询", "本查询的结果值为JSON，参数路径为Results[检索分页].Json",
                "搜索结果", Query_Page,
                () => new SqlItem
                    {
                        Name =Query_Page,
                        Description = "检索分页查询",
                        CommandText = CreateSql_QueryPage()
                    });
            if (sqlCommand != null)
            {
                UpdateQueryCommandParameters(sqlCommand);
            }

            CreateConverter_SearchField(sqlCommand);
            #endregion
            
            //返回搜索结果
            var command = CreateCommand(action, ActionCommands.JsonActionCommand, "搜索结果", "返回搜索结果");
            if (command == null)
            {
                return;
            }
            var config = command.Config;

            if (config != null && config is JsonActionCommand)
            {
                var commandConfig = config as JsonActionCommand;
                commandConfig.Parameters.Add(new CommandParameter
                {
                    Name = "total",
                    Path = string.Format("Results[{0}].Value", "查询总数")
                });
                commandConfig.Parameters.Add(new CommandParameter
                {
                    Name = "rows",
                    Path = string.Format("Results[{0}].Items", "分页查询")
                });
                commandConfig.Parameters.Add(new CommandParameter
                {
                    Name = "HasError",
                    Path = string.Format("LastResult.IsError")
                });
                commandConfig.Parameters.Add(new CommandParameter
                {
                    Name = "ErrorMessage",
                    Path = string.Format("LastResult.ErrorMessage")
                });
            }
             
        }

        private void CreateConverter_SearchField(SqlCommand sqlCommand)
        {
            //修改查询参数
            var hasForeignColumns = EditSource.Fields.FirstOrDefault(f => f.ShowInGrid && f.IsForeignColumn) != null;
            sqlCommand.Parameters.ForEach(p =>
            {
                var column = EditSource.Fields.FirstOrDefault(f => f.Name.Equals(p.Name));
                if (column == null)
                {
                    return;
                }

                p.Converter = "CreateSearchField";
                var shortName = hasForeignColumns ?
                    (column.IsForeignColumn ? EditSource.GetFreignTableShortName(column) :
                        "M") :
                    "";
                if (hasForeignColumns)
                {
                    p.ConverterParameter = string.Format("{0},{1},{2}", p.Name, (column != null && column.ValueType == QueryValueType.Number) ? "1" : "0", shortName);

                }
                else
                {
                    p.ConverterParameter = string.Format("{0},{1}", p.Name, (column != null && column.ValueType == QueryValueType.Number) ? "1" : "0");
                }
            });
        }
        private void CreateActionQuery()
        {
            var action = GetOrCreateAction("Query", 2, "检索");

            //添加查询命令
            var sqlCommand = CreateCommand_QueryJson(action, "执行查询", "本查询的结果值为JSON，参数路径为Results[执行查询].Json", "搜索结果", "Query",
                () => new SqlItem
                    {
                        Name = "Query",
                        Description = "检索查询",
                        CommandText = CreateSql_SearchText()
                    });
            //修改查询参数
            var hasForeignColumns = EditSource.Fields.FirstOrDefault(f => f.ShowInGrid && f.IsForeignColumn) != null;
            sqlCommand.Parameters.ForEach(p =>
            {
                p.Converter = "CreateSearchField";
                var column = EditSource.Fields.FirstOrDefault(f => f.Name.Equals(p.Name));

                var shortName = hasForeignColumns ?
                    (column.IsForeignColumn ? EditSource.GetFreignTableShortName(column) :
                        "M") :
                    "";
                if (hasForeignColumns)
                {
                    p.ConverterParameter = string.Format("{0},{1},{2}", p.Name, (column != null && column.ValueType == QueryValueType.Number) ? "1" : "0", shortName);

                }
                else
                {
                p.ConverterParameter = string.Format("{0},{1}", p.Name, (column != null && column.ValueType == QueryValueType.Number) ? "1" : "0");
                }
            });

            //返回搜索结果
            CreateCommand_JsonResult(action, "搜索结果", "返回搜索结果", "执行查询");
        }

        #endregion

        #region 创建Action_Detail明细页面
        private void CreateActionDetail()
        {
            var action = GetOrCreateAction("Detail", 4, "查看");

            var detailQueryCommand = CreateCommand_SqlCommand(action, ActionCommands.QuerySqlCommand, "查询明细", "查询明细", "查看页面",
                "GetDetail", () => new SqlItem
                {
                    Name = "GetDetail",
                    Description = "查询明细",
                    CommandText = CreateSql_GetDetailText()
                });

            if (detailQueryCommand != null && detailQueryCommand.SqlItem != null)
            {
                detailQueryCommand.Parameters.Add(new CommandParameter
                {
                    Name = EditSource.Key,
                    Type = CommandParameterType.Parameter,
                    Path = string.Format("Request[ID]")
                });
            }

            var commandConfig = CreateCommand_EditDialogActionCommand(action, "查看页面", "这个命令会直接返回到客户端，无需后继命令");
            var form = Create_DetailForm();

            form.DataBinding = "Parameters[Data]";
            commandConfig.Page.Form = form;

            if (commandConfig == null)
            {
                return;
            }
            //引入查询结果，作为窗体的数据源
            commandConfig.Parameters.Add(new CommandParameter
            {
                Name = "Data",
                Type = CommandParameterType.Parameter,
                Path = "LastResult.Items"
            });
        }
        #endregion

        #region 创建Action_Create新增页面
        private void CreateActionCreate()
        {
            var action = GetOrCreateAction("Create", 8, "新增");

            var commands = Create_ItemsSource(action, EditSource.Fields.Where(f => f.ShowInCreate && f.IsForeignKey), "新增页面").ToList();

            var commandConfig = CreateCommand_EditDialogActionCommand(action, "新增页面", "这个命令会直接返回到客户端，无需后继命令");
            var form = Create_CreateForm();
            commandConfig.Page.Form = form;

            commands.ForEach(c =>
            {
                commandConfig.Parameters.Add(new CommandParameter
                {
                    Name = c.Name,
                    Type = CommandParameterType.Parameter,
                    Path = string.Format("Results[{0}].Items", c.Name)
                });
            });
        }
        #endregion

        #region 创建Action_Add新增
        private void CreateActionAdd()
        {
            var action = GetOrCreateAction("Add", 8, "新增");
            //添加新增记录数据库
            CreateCommand_QueryJson(action, "执行新增", "本查询的结果值为JSON，参数路径为Results[执行新增].Json", "新增结果", "Add",
                () => new SqlItem
                {
                    Name = "Add",
                    Description = "新增",
                    CommandText = CreateSql_AddText()
                });
            //返回搜索结果
            CreateCommand_JsonResult(action, "新增结果", "返回新增结果", "执行新增");
        }
        #endregion

        #region 创建Action_Edit编辑页面
        private void CreateActionEdit()
        {
            var action = GetOrCreateAction("Edit", 16, "编辑");
            var commands = Create_ItemsSource(action, EditSource.Fields.Where(f => f.ShowInCreate && f.IsForeignKey), "查询编辑").ToList();

            var queryCommand = CreateCommand_SqlCommand(action,
                ActionCommands.QuerySqlCommand,
                "查询编辑", "查询编辑", "编辑页面",
                "GetEdit", () => new SqlItem
                {
                    Name = "GetEdit",
                    Description = "查询编辑",
                    CommandText = CreateSql_GetEditText()
                });

            if (queryCommand != null && queryCommand.SqlItem != null)
            {
                queryCommand.Parameters.Add(new CommandParameter
                {
                    Name = EditSource.Key,
                    Type = CommandParameterType.Parameter,
                    Path = string.Format("Request[ID]")
                });
            }

            var commandConfig = CreateCommand_EditDialogActionCommand(action, "编辑页面", "这个命令会直接返回到客户端，无需后继命令");
            var form = Create_EditForm();
            form.DataBinding = "Parameters[Data]";
            commandConfig.Page.Form = form;

            if (commandConfig == null)
            {
                return;
            }
            //引入查询结果，作为窗体的数据源
            commandConfig.Parameters.Add(new CommandParameter
            {
                Name = "Data",
                Type = CommandParameterType.Parameter,
                Path = "LastResult.Items"
            });

            commands.ForEach(c =>
            {
                commandConfig.Parameters.Add(new CommandParameter
                {
                    Name = c.Name,
                    Type = CommandParameterType.Parameter,
                    Path = string.Format("Results[{0}].Items", c.Name)
                });
            });
        }
        #endregion

        #region 创建Action_Update编辑保存
        private void CreateActionUpdate()
        {
            var action = GetOrCreateAction("Update", 16, "编辑");
            //添加编辑记录数据库
            CreateCommand_QueryJson(action, "执行更新", "本查询的结果值为JSON，参数路径为Results[执行更新].Json", "编辑结果", "Update",
                () => new SqlItem
                {
                    Name = "Update",
                    Description = "更新",
                    CommandText = CreateSql_UpdateText()
                });
            //返回搜索结果
            CreateCommand_JsonResult(action, "编辑结果", "返回编辑结果", "执行更新");
        }
        #endregion

        #region 创建Action_Delete删除
        private void CreateActionDelete()
        {
            var action = GetOrCreateAction("Delete", 32, "删除");
            //添加编辑记录数据库
            CreateCommand_QueryJson(action, "执行删除", "本查询的结果值为JSON，参数路径为Results[执行删除].Json", "删除结果", "Delete",
                () => new SqlItem
                {
                    Name = "Delete",
                    Description = "删除",
                    CommandText = CreateSql_DeleteText()
                });
            //返回搜索结果
            CreateCommand_JsonResult(action, "删除结果", "返回删除结果", "执行删除");
        }
        #endregion


        #region 创建命令_EditDialogActionCommand

        private EditDialogActionCommand CreateCommand_EditDialogActionCommand(Action action, string name, string description)
        {
            var command = CreateCommand(action, ActionCommands.EditDialogActionCommand, name, description);

            if (command == null)
            {
                return null;
            }
            var config = command.Config;

            if (config != null && config is EditDialogActionCommand)
            {
                var commandConfig = config as EditDialogActionCommand;
                return commandConfig;
            }
            return null;
        }

        #endregion

        #region 创建命令_FormActionCommand 以后不用
        private FormViewActionCommand CreateCommand_FormActionCommand(Action action, string name, string description,
            Func<DataView> createView,
            string viewPath = "~/Views/Page/Edit.cshtml")
        {
            var command = CreateCommand(action, CommandType_FormView, name, description);

            if (command == null)
            {
                return null;
            }
            var config = command.Config;

            if (config != null && config is FormViewActionCommand)
            {
                var commandConfig = config as FormViewActionCommand;
                commandConfig.ViewPath = viewPath;
                commandConfig.ContentType = "text/html";

                commandConfig.Form.Controls.Clear();
                var editForm = createView();

                commandConfig.Form.Controls.Add(editForm);
                return commandConfig;
            }
            return null;
        }

        #endregion

        #region 创建命令_SqlCommand
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="commandType"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="nextCommand"></param>
        /// <param name="sqlName"></param>
        /// <param name="createSql"></param>
        /// <returns></returns>
        private SqlCommand CreateCommand_SqlCommand(Action action, string commandType, string name, string description, string nextCommand,
            string sqlName, Func<SqlItem> createSql = null)
        {
            var command = CreateCommand(action, commandType, name, description);
            if (command == null)
            {
                return null;
            }

            var config = command.Config;

            if (config != null && config is SqlCommand)
            {
                var commandConfig = config as SqlCommand;
                commandConfig.GroupName = EditSource.GroupName;
                commandConfig.TableName = EditSource.TableName;
                commandConfig.NextCommand = nextCommand;

                //TODO:判断命令存在与否，不存在则创建 
                var sqlItem = GetOrCreateSqlItem(commandConfig.GroupName, commandConfig.TableName, sqlName, createSql);
                commandConfig.CommandName = sqlName;
                return commandConfig;
            }

            return null;
        }
        #endregion

        #region 创建命令_QueryJson
        private SqlCommand CreateCommand_QueryJson(Action action, string name, string description, string nextCommand,
            string sqlName, Func<SqlItem> createSql = null)
        {
            var commandConfig = CreateCommand_SqlCommand(action, ActionCommands.QueryJsonSqlCommand, name, description, nextCommand,
                sqlName, createSql);
            if (commandConfig != null)
            {
                UpdateQueryCommandParameters(commandConfig);
            }
            return commandConfig;
        }
        #endregion

        #region 创建命令_JsonResult
        private void CreateCommand_JsonResult(Action action, string name, string description, string lastCommandName)
        {
            var command = CreateCommand(action, ActionCommands.JsonActionCommand, name, description);
            if (command == null)
            {
                return;
            }
            var config = command.Config;

            if (config != null && config is JsonActionCommand)
            {
                var commandConfig = config as JsonActionCommand;
                commandConfig.Parameters.Add(new CommandParameter
                {
                    Name = "Data",
                    Path = string.Format("Results[{0}].Json", lastCommandName)
                });
                commandConfig.Parameters.Add(new CommandParameter
                {
                    Name = "HasError",
                    Path = string.Format("Results[{0}].IsError", lastCommandName)
                });
                commandConfig.Parameters.Add(new CommandParameter
                {
                    Name = "ErrorMessage",
                    Path = string.Format("Results[{0}].ErrorMessage", lastCommandName)
                });
            }
        }
        #endregion

        private IEnumerable<CommandConfig> Create_ItemsSource(Action action, IEnumerable<EditField> fields, string nextCommand)
        {
            var commands = Create_ItemsSource(action, fields).ToList();
            if (commands.Count > 0)
            {
                for (int i = 0; i < commands.Count - 1; i++)
                {
                    commands[i].Config.NextCommand = commands[i + 1].Name;
                }
                var lastCommand = commands.Last();
                lastCommand.Config.NextCommand = nextCommand;
            }
            return commands;
        }
        private IEnumerable<CommandConfig> Create_ItemsSource(Action action, IEnumerable<EditField> fields)
        {
            foreach (var field in fields.Where(f=>(!string.IsNullOrEmpty(f.ForeignSqlName))))
            {
                var command = CreateCommand(action, ActionCommands.QuerySqlCommand, field.ItemsSource, string.Format("自动创建的数据源[{0}]", field.ItemsSource));
                if (command == null)
                {
                    yield break;
                }

                var config = command.Config;

                if (config != null && config is SqlCommand)
                {
                    var commandConfig = config as SqlCommand;

                    commandConfig.GroupName = field.ForeignGroup;
                    commandConfig.TableName = field.ForeignTable; 

                    string sqlName = field.ForeignSqlName;
                    //TODO:判断命令存在与否，不存在则创建 
                    var sqlItem = GetOrCreateSqlItem(commandConfig.GroupName, commandConfig.TableName, sqlName, () =>
                    {
                        var sql = new SqlItem
                        {
                            Name = sqlName,
                            SqlType = SqlType.Query,
                            CommandText = string.Format("SELECT [{0}],[{1}] FROM {2}", field.ValueMember, field.DisplayMember, commandConfig.TableName)
                        };
                        return sql;
                    });
                    commandConfig.CommandName = sqlName;
                }
                yield return command;
            }
            yield break;
        }

        #region 其它未分类

        private void UpdateQueryCommandParameters(SqlCommand config)
        {
            if (config.SqlItem != null)
            {
                var sql = config.SqlItem.CommandText;
                var parameters = sql.GetParameters();
                foreach (var p in parameters)
                {
                    if (p.Length > 0 && Char.IsLower(p[0]))
                    {
                        continue;
                    }
                    var oldItem = config.Parameters.FirstOrDefault(f => f.Name.Equals(p, StringComparison.OrdinalIgnoreCase));
                    if (oldItem == null)
                    {
                        oldItem = new CommandParameter { Name = p };
                        switch (p.ToLower())
                        {
                            case "createduser":
                                oldItem.Type = CommandParameterType.SystemParameter;
                                oldItem.Path = "CurrentUserID";
                                break;
                            case "createdtime":
                                oldItem.Type = CommandParameterType.SystemParameter;
                                oldItem.Path = "CurrentDateTime";
                                break;
                            case "modifieduser":
                                oldItem.Type = CommandParameterType.SystemParameter;
                                oldItem.Path = "CurrentUserID";
                                break;
                            case "modifiedtime":
                                oldItem.Type = CommandParameterType.SystemParameter;
                                oldItem.Path = "CurrentDateTime";
                                break;
                            default:
                                oldItem.Type = CommandParameterType.Parameter;
                                oldItem.Path = string.Format("Request[{0}]", p);

                                var field = EditSource.Fields.FirstOrDefault(f => f.Name.Equals(p, StringComparison.OrdinalIgnoreCase));
                                if (field != null)
                                {
                                    switch (field.DataType.ToLower())
                                    {
                                        case "int":
                                            oldItem.DefaultValue = "0";
                                            break;
                                        case "datetime":
                                            oldItem.DefaultValue = "1900-01-01";
                                            break;
                                    }
                                }
                                break;
                        }
                        config.Parameters.Add(oldItem);
                    }
                }
            }
        }

        private Action GetOrCreateAction(string name, int actionValue, string title)
        {
            var action = Actions.FirstOrDefault(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (action == null)
            {
                action = AppConfig.Current.CreateAction(this.Page);
                action.Name = name;
                action.ActionValue = actionValue;
                action.Title = title;
            }
            else
            {
                action.Commands.Clear();
            }
            return action;
        }

        private CommandConfig CreateCommand(Action action, string commandType, string commandName = "", string description = "")
        {
            var command = AppConfig.Current.CreateCommand(action);
            command.CommandType = commandType;
            command.Name = string.IsNullOrEmpty(commandName) ? commandType : commandName;
            command.Description = description;
            return command;
        }

        #endregion

        #region 取表结构
        private TableSchema GetTable()
        {
            if (EditSource == null || string.IsNullOrEmpty(EditSource.GroupName) || string.IsNullOrEmpty(EditSource.TableName))
            {
                return null;
            }

            var group = SqlData.Current.Groups[EditSource.GroupName];
            if (group == null)
            {
                return null;
            }

            var table = group.Items[EditSource.TableName];
            if (table == null)
            {
                return null;
            }

            var conn = SqlData.Current.Connections[table.ConnectionName];
            var tableSchema = conn.Tables[table.TableName];
            return tableSchema;
        }
        #endregion

        #region 创建数据界面

        #region 新增
        private UIForm Create_CreateForm()
        {
            var editForm = new UIForm();
            editForm.InitSettings();
            EditSource.Fields.Where(f => f.ShowInCreate)
                .ForEach(f => editForm.Fields.Add(
                    CreateUIField(f, true)));

            //添加按钮
            editForm.ToolBar.Buttons.Clear();
            editForm.ToolBar.PageID = PageID;
            CreateUIButtons("btn-add", "btn-cancel")
                .AddToCollection(editForm.ToolBar.Buttons);

            return editForm;
        }
        #endregion

        #region 编辑
        private UIForm Create_EditForm()
        {
            var editForm = new UIForm();
            editForm.InitSettings();
            EditSource.Fields.Where(f => f.ShowInEdit || f.Name.Equals(EditSource.Key))
                .ForEach(f => editForm.Fields.Add(
                    CreateUIField(f, true)));

            //添加按钮
            editForm.ToolBar.Buttons.Clear();
            editForm.ToolBar.PageID = PageID;
            CreateUIButtons("btn-update", "btn-cancel")
                .AddToCollection(editForm.ToolBar.Buttons);

            return editForm;
        }
        #endregion

        #region 详细
        private UIForm Create_DetailForm()
        {
            var editForm = new UIForm();
            editForm.InitSettings();
            EditSource.Fields.Where(f => f.ShowInDetail)
                .ForEach(f => editForm.Fields.Add(
                    CreateUIField(f, false)));

            //添加按钮
            editForm.ToolBar.Buttons.Clear();
            editForm.ToolBar.PageID = PageID;
            CreateUIButtons("btn-close")
                .AddToCollection(editForm.ToolBar.Buttons);

            return editForm;
        }
        #endregion

        #region 搜索
        //TODO:搜索页面需要细化，不能简单呈现，需要完成比较符（大于、小于、等于、包含、区间）的动态选取
        private UIForm Create_SearchForm()
        {
            var editForm = new UIForm();
            editForm.InitSettings();
            EditSource.Fields.Where(f => f.ShowInSearch)
                .ForEach(f => editForm.Fields.Add(
                    CreateUIField(f, true)));

            //添加按钮
            editForm.ToolBar.Buttons.Clear();
            editForm.ToolBar.PageID = PageID;
            CreateUIButtons("btn-query", "btn-cancel")
                .AddToCollection(editForm.ToolBar.Buttons);

            return editForm;
        }
        #endregion

        #region 创建一般字段控件
        private UIField CreateUIField(EditField item, bool canEdit = true)
        {
            var editField = new UIField
            {
                TitleClass = Layout.EditLabelClass,
                Title = item.Title,
                ContentClass = Layout.EditContentClass,
                FieldType = item.Name.Equals(EditSource.Key) ? UIFieldType.Hidden : item.EditBox
            };

            UIInput input = editField.Input;
            if (input != null)
            {
                editField.ResetInput();
            }

            if (input == null)
            {
                return editField;
            }

            editField.Input = input;
            input.Binding = item.Name;
            input.Name = item.Name;
            input.Class = Layout.EditInputClass;
            input.IsReadOnly = !canEdit;
            input.IsRequired = item.IsRequired;

            if (input is UIComboBox)
            {
                var combobox = input as UIComboBox;
                combobox.InitSettings();
                if (string.IsNullOrEmpty(item.DisplayMember) && string.IsNullOrEmpty(item.ValueMember))
                {
                    combobox.ItemsSource = item.ItemsSource;
                }
                else
                {
                    combobox.ItemsSource = string.Format("/Parameters[{0}]", item.ItemsSource);
                    combobox.DisplayMember = item.DisplayMember;
                    combobox.ValueMember = item.ValueMember;
                }
                input.Class = "easyui-combobox edit-input";
            }
            //TextBox:文本框|ComboBox:选择框|CheckBox:复选框|DateBox:日期 

            if (editField.FieldType == UIFieldType.PasswordBox)
            {
                input.InputType = "password";
            }
            else
            {
                input.InputType = "";
            }

            input.Length = item.Length;

            return editField;
        }
        #endregion

        #region 根据ID创建按钮系列
        private IEnumerable<UIButton> CreateUIButtons(params string[] btns)
        {
            foreach (var id in btns)
            {
                var item = Buttons.FirstOrDefault(b => b.ID.Equals(id, StringComparison.OrdinalIgnoreCase));
                if (item != null)
                {
                    yield return CreateUIButton(item);
                }
            }
        }
        #endregion

        #region 创建按钮
        private UIButton CreateUIButton(EditButton item)
        {
            return new UIButton
            {
                ID = item.ID,
                Title = item.Title,
                ActionValue = item.ActionValue,
                Action = item.Action,
                Class = item.Class,
                Options = item.Option
            };
        }
        #endregion

        #endregion

        #region 创建SQL

        #region 创建SQL命令
        private SqlItem GetOrCreateSqlItem(string groupName, string tableName, string sqlName, Func<SqlItem> createSql = null)
        {
            try
            {
                var group = SqlData.Current.Groups[groupName];
                if (group == null)
                {
                    throw new Exception(string.Format("SQL模块不存在——[{0}]", groupName));
                }
                var table = group.Items[tableName];
                if (table == null)
                {
                    throw new Exception(string.Format("SQL表不存在——[{0}]", tableName));
                }
                var sqlItem = table.Items[sqlName];
                if (sqlItem == null)
                {
                    if (createSql != null)
                    {
                        sqlItem = createSql();
                        if (sqlItem != null)
                        {
                            sqlItem.SqlType = SqlType.Query;
                            sqlItem.ConnectionName = table.ConnectionName;
                            table.Items.Add(sqlItem);

                            _SqlNotSave = true;
                        }
                    }
                }
                return sqlItem;
            }
            catch (Exception ex)
            {
                throw new Exception("创建SQL出错" + ex.Message);
            }
        }
        #endregion

        #region 增加列表命令
        private bool AppendSql_SelectIndexCount(StringBuilder sb)
        {
            return AppendSelectCountWithJoin(sb, EditSource.Fields
                .Where(f => f.ShowInGrid || f.Name.Equals(EditSource.Key, StringComparison.OrdinalIgnoreCase)));
        }
        /// <summary>
        /// 增加列表命令，不包括where条件部分
        /// </summary>
        /// <param name="sb"></param>
        private bool AppendSql_SelectIndex(StringBuilder sb)
        {
            return AppendSelectWithJoin(sb, EditSource.Fields
                .Where(f => f.ShowInGrid || f.Name.Equals(EditSource.Key, StringComparison.OrdinalIgnoreCase)));
        }
        #endregion

        private string CreateSql_QueryCount()
        {
            StringBuilder sb = new StringBuilder();
            var searchColumns = EditSource.Fields.Where(f => f.ShowInSearch).ToList();
            if (searchColumns.Count == 0)
            {
                return "";
            }

            sb.Append("DECLARE @sql varchar(max);");
            sb.Append(@"
DECLARE @where varchar(2000);
SET @where='';");

            //TODO:这里漏掉了AND
            foreach (var col in searchColumns)
            {
                sb.AppendFormat(@"
IF(len(@{0})>0)    
  BEGIN
    IF(LEN(@where)>0)
      SET @where = @where + ' AND'
      set @where= @where+' (' + @{0} + ')';
  END
", col.Name);
            }

            sb.Append("\r\nSET @sql='");
            var hasJoin = AppendSql_SelectIndexCount(sb);
            sb.Append("';\r\n");

            sb.AppendFormat(@"
IF(LEN(@where)>0)
  BEGIN
    SET @sql=@sql+' WHERE' + @where;
  END  
execute(@sql);", EditSource.Key);

            return sb.ToString();
        }

        private string CreateSql_QueryPage()
        {
            StringBuilder sb = new StringBuilder();
            var searchColumns = EditSource.Fields.Where(f => f.ShowInSearch).ToList();
            if (searchColumns.Count == 0)
            {
                return "";
            }

            sb.Append("DECLARE @sql varchar(max);");
            sb.Append(@"
DECLARE @where varchar(2000);
SET @where='';");

            //TODO:这里漏掉了AND
            foreach (var col in searchColumns)
            {
                sb.AppendFormat(@"
IF(len(@{0})>0)    
  BEGIN
    IF(LEN(@where)>0)
      SET @where = @where + ' AND'
      set @where= @where+' (' + @{0} + ')';
  END
", col.Name);
            }

            sb.Append("\r\nSET @sql='");
            var hasJoin = AppendSql_SelectIndex(sb);
            sb.Append("';\r\n");

            #region 默认排序列
            string orderKey = "";
            var foreignFields = EditSource.Fields.Where(f => f.ShowInGrid && f.IsForeignColumn).GroupBy(f => f.TargetForeignKey).ToList();
            if (foreignFields.Count == 0)
            {
                orderKey = EditSource.Key;
            }
            else
            { 
                var keys = EditSource.Key.Split(',');
                if(keys.Length==0)
                {
                    throw new Exception("没有主键不能自动创建排序列");
                }
                if(keys.Length==1)
                {
                    orderKey = "M.["+ keys[0] +"]";
                }
                else
                {
                    foreach(var key in keys)
                    {
                        if(orderKey.Length>0)
                        {
                            orderKey += ",";
                        }
                        orderKey += "M.[" + key+"]";
                    }
                }
            }
        #endregion

            sb.AppendFormat(@"
IF(LEN(@where)>0)
  BEGIN
    SET @sql=@sql+' WHERE' + @where;
  END

IF(LEN(ISNULL(@OrderBy,''))=0)
  BEGIN
    SET @sql=@sql+' ORDER BY {0}';
  END
ELSE
  BEGIN
    SET @sql=@sql+' ORDER BY ' + @OrderBy;
  END

SET @sql = @sql + '
	OFFSET ('+ CAST((@Page - 1) * @Rows AS varchar(10)) +') ROWS
    FETCH NEXT ' + CAST(@Rows AS varchar(10)) +' ROWS ONLY';
 
execute(@sql);", orderKey);

            return sb.ToString();
        }

        #region 创建搜索的SQL语句
        private string CreateSql_SearchText()
        {
            StringBuilder sb = new StringBuilder();
            var searchColumns = EditSource.Fields.Where(f => f.ShowInSearch).ToList();
            if (searchColumns.Count == 0)
            {
                return "";
            }

            sb.Append("DECLARE @sql varchar(max);");
            sb.Append(@"
DECLARE @where varchar(2000);
SET @where='';");

            //TODO:这里漏掉了AND
            foreach (var col in searchColumns)
            {
                sb.AppendFormat(@"
IF(len(@{0})>0)    
  BEGIN
    IF(LEN(@where)>0)
      SET @where = @where + ' AND'
      set @where= @where+' (' + @{0} + ')';
  END
", col.Name);
            }

            sb.Append("\r\nSET @sql='");
            var hasJoin = AppendSql_SelectIndex(sb);
            sb.Append("';\r\n");

            sb.Append(@"
IF(LEN(@where)>0)
  BEGIN
    SET @sql=@sql+' WHERE' + @where;
  END
execute(@sql);");

            return sb.ToString();
            #region
            /*
             * 
declare @sql varchar(2000);
declare @where varchar(max);
DECLARE @role varchar(50);
declare @page varchar(50);

set @where='';
set @role='11';
set @page ='01';
set @sql = 'select * from sys_RolePage';
 
if(len(@role)>0)
begin
	if(len(@where)>0)
		begin
		set @where = @where + ' AND ';
		end
	set @where = @where + 'RoleID like ''%' + @role + '%''';
end

if(len(@page)>0)
begin
	if(len(@where)>0)
		begin
		set @where = @where + ' AND ';
		end
	set @where = @where + 'PageID like ''%' + @page + '%''';
end

if(len(@where)>0)
	begin
	set @sql=@sql+' WHERE ' + @where;
	end	 
--select @sql;
execute(@sql);
            */
            #endregion
        }
        #endregion

        #region 创建新增语句
        private string CreateSql_AddText()
        {
            StringBuilder sb = new StringBuilder();

            var columns = EditSource.Fields.Where(f => f.ShowInCreate || f.IsSerialCode
                || f.Name.Equals("CreatedUser")
                || f.Name.Equals("CreatedTime")
                || f.Name.Equals("ModifiedUser")
                || f.Name.Equals("ModifiedTime")).ToList();
            if (columns.Count == 0)
            {
                return "";
            }

            var table = GetTable();
            string tableName = "";
            int indexSchema = table.Name.IndexOf('_');
            if (indexSchema > 0)
            {
                tableName = table.Name.Substring(indexSchema + 1);
            }
            else
            {
                tableName = table.Name;
            }

            var autoKeyColumn = table.Columns.FirstOrDefault(c => c.IsAutoIncrement);
            var hasAutoKey = autoKeyColumn != null;

            string insertColumns = columns.Select(c => c.Name).ToMergeString(",", "[{0}]");
            string insertParameters = columns.Select(c => c.IsSerialCode ? c.Name.ToLower() : c.Name).ToMergeString(",", "@{0}");

            StringBuilder sbwhere = new StringBuilder();
            table.Columns.Where(c => c.IsKey).ForEachWithFirst(
                (c) => { sbwhere.AppendFormat("[{0}]=@{0}", c.Name); },
                (c) => { sbwhere.AppendFormat(" AND [{0}]=@{0}", c.Name); });
            string wherebykey = sbwhere.ToString();

            var displayColumns = EditSource.Fields
                .Where(f => f.ShowInGrid || f.Name.Equals(EditSource.Key, StringComparison.OrdinalIgnoreCase))
                .Select(c => c.Name)
                .ToMergeString(",", "[{0}]");

            var serailField = EditSource.Fields.FirstOrDefault(f => f.IsSerialCode);
            if (serailField != null && (!string.IsNullOrEmpty(serailField.SerialRule)))
            {
                var ruleParas = serailField.SerialRule.Split(',');
                if (ruleParas.Length == 3)
                {
                    sb.AppendFormat("DECLARE @{0} varchar(50);\r\n EXEC sp_CreateSerialCode '{1}','{2}',{3},@{0} output;\r\n",
                        serailField.Name.ToLower(), ruleParas[0], ruleParas[1], ruleParas[2]);
                }
            }

            if (hasAutoKey)
            {
                sb.AppendFormat("INSERT INTO [{0}]\r\n({1}) \r\nVALUES({2});\r\n"
                            , table.Name, insertColumns, insertParameters);
                var hasJoin = AppendSql_SelectIndex(sb);
                sb.AppendFormat("\r\nWHERE {1}[{0}]=scope_identity()", autoKeyColumn.Name, hasJoin ? "M." : "");
            }
            else
            {
                sb.AppendFormat("INSERT INTO [{0}]\r\n({1}) \r\nVALUES({2});\r\n"
                            , table.Name, insertColumns, insertParameters);
                var hasJoin = AppendSql_SelectIndex(sb);

                sb.AppendFormat("\r\nWHERE {1}[{0}]=@{0}", EditSource.Key, hasJoin ? "M." : "");
            }

            return sb.ToString();
        }
        #endregion

        #region 创建Update更新语句
        private string CreateSql_UpdateText()
        {
            var table = GetTable();
            StringBuilder sbUpdates = new StringBuilder();
            EditSource.Fields
                .Where(f => f.ShowInEdit
                    || f.Name.Equals("ModifiedUser")
                    || f.Name.Equals("ModifiedTime"))
                .Select(c => c.Name).ForEachWithFirst(
                        (c) => { sbUpdates.AppendFormat(" [{0}]=@{0}", c); },
                        (c) => { sbUpdates.AppendFormat(",[{0}]=@{0}", c); }
                );

            string wherebykey = string.Format("[{0}]=@{0}", EditSource.Key);

            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat("UPDATE [{0}]\r\n SET {1} WHERE {2};\r\n"
                        , table.Name, sbUpdates.ToString(), wherebykey);

            var hasJoin = AppendSql_SelectIndex(sbSql);
            sbSql.AppendFormat("\r\nWHERE {1}[{0}]=@{0}", EditSource.Key, hasJoin ? "M." : "");

            return sbSql.ToString();
        }
        #endregion

        #region 创建Delete删除语句
        private string CreateSql_DeleteText()
        {
            var table = GetTable();

            string key = "Keys";
            string wherebykey = string.Format("[{0}]=@{1}", EditSource.Key, key);
            string wherebykeys = string.Format("[{0}] in (''' + REPLACE(@{1},',',''',''') + ''')'", EditSource.Key, key);

            StringBuilder sbSql = new StringBuilder();
            sbSql.Append(
@"DECLARE @sql varchar(max);
BEGIN TRY
");

            sbSql.AppendFormat("  IF(CHARINDEX(',',@{0},0)<=0)\r\n", key);
            sbSql.Append("  BEGIN\r\n");
            //删除子表查询
            AppendSql_ChildTableChecking(sbSql, false);

            if (string.IsNullOrEmpty(EditSource.LogicDeleteFlag))
            {
                sbSql.AppendFormat("    DELETE FROM [{0}]\r\n    WHERE {1};",
                    table.Name, wherebykey);
            }
            else
            {
                sbSql.AppendFormat("    UPDATE [{0}] SET {1}\r\n    WHERE {2};",
                    table.Name, EditSource.LogicDeleteFlag, wherebykey);
            }
            sbSql.Append("\r\n  END");
            sbSql.Append("\r\n  ELSE\r\n");
            sbSql.Append("  BEGIN\r\n");

            //删除子表查询
            AppendSql_ChildTableChecking(sbSql, true);

            if (string.IsNullOrEmpty(EditSource.LogicDeleteFlag))
            {
                sbSql.AppendFormat("    SET @sql= 'DELETE FROM [{0}] WHERE [{1}] in (''' + REPLACE(@{2},',',''',''') + ''')'; ",
                    table.Name, EditSource.Key, key);
            }
            else
            {
                sbSql.AppendFormat("    SET @sql= 'UPDATE [{0}] SET {1} WHERE [{2}] in (''' + REPLACE(@{3},',',''',''') + ''')'; ",
                    table.Name, EditSource.LogicDeleteFlag, EditSource.Key, key);
            }
            sbSql.Append("\r\n    EXEC(@sql);");
            sbSql.Append("\r\n  END");

            sbSql.AppendFormat(@"
END TRY
BEGIN CATCH
    DECLARE @error varchar(2000);
    SET @error = ERROR_MESSAGE();
    RAISERROR(@error ,11,1);
END CATCH");
            return sbSql.ToString();
        }

        private void AppendSql_ChildTableChecking(StringBuilder sbSql, bool mutiKey = false)
        {
            if (EditSource.ChildTables.Count == 0)
            {
                return;
            }

            var field = EditSource.Fields.FirstOrDefault(f => f.Name.Equals(EditSource.Key, StringComparison.OrdinalIgnoreCase));
            if (field == null)
            {
                throw new Exception("主键不存在");
            }

            var keyIsNumber = field.DataType.Equals("int", StringComparison.OrdinalIgnoreCase)
                || field.DataType.Equals("bigint", StringComparison.OrdinalIgnoreCase);

            foreach (var t in EditSource.ChildTables)
            {
                sbSql.AppendFormat(@"
SET @sql='
IF(EXISTS(SELECT * FROM {0} A 
        INNER JOIN {1} M 
        ON A.[{2}]=M.[{3}] 
        WHERE '
", t.TableName, EditSource.TableName, t.TableKey, t.ReferenceKey);
                if (mutiKey)
                {
                    if (keyIsNumber)
                    {
                        sbSql.AppendFormat(@" + 'A.[{0}] in (' +@{0} +')))",
                            EditSource.Key);
                    }
                    else
                    {
                        sbSql.AppendFormat(@" + 'A.[{0}] in (''' + REPLACE(@{0},',',''',''') +''')))",
                            EditSource.Key);
                    }
                }
                else
                {
                    if (keyIsNumber)
                    {
                        sbSql.AppendFormat(@" + 'A.[{0}] = ' +@{0} +'))",
                            EditSource.Key);
                    }
                    else
                    {
                        sbSql.AppendFormat(@" + 'A.[{0}] = ''' + @{0} +'''))",
                            EditSource.Key);
                    }
                }
                sbSql.AppendFormat(@"
	RAISERROR(''{1}'',11,1);
';
EXEC @sql;
", t.DeleteCheckError);
            }
        }

        #endregion

        #region 创建取数据明细语句
        private string CreateSql_GetDetailText()
        {
            StringBuilder sbSql = new StringBuilder();


            var hasJoin = AppendSelectWithJoin(sbSql, EditSource.Fields
                .Where(f => f.ShowInDetail));

            sbSql.AppendFormat("\r\nWHERE {1}[{0}]=@{0}", EditSource.Key, hasJoin ? "M." : "");
            return sbSql.ToString();
        }
        #endregion

        #region 创建取数据编辑语句
        private string CreateSql_GetEditText()
        {
            StringBuilder sbSql = new StringBuilder();

            var hasJoin = AppendSelectWithJoin(sbSql, EditSource.Fields
                .Where(f => f.ShowInEdit || f.Name.Equals(EditSource.Key, StringComparison.OrdinalIgnoreCase)));

            sbSql.AppendFormat("\r\nWHERE {1}[{0}]=@{0}", EditSource.Key, hasJoin ? "M." : "");
            return sbSql.ToString();
        }
        #endregion

        #region 绑定外键字段的连接查询
        private bool AppendSelectCountWithJoin(StringBuilder sbSql, IEnumerable<EditField> fields)
        {
            var mainTable = "M";

            var foreignFields = fields.Where(f => f.IsForeignColumn).GroupBy(f => f.TargetForeignKey).ToList();
            if (foreignFields.Count == 0)
            {
                sbSql.Append("SELECT COUNT(*)"); 
                sbSql.Append("\r\n FROM ");
                sbSql.AppendFormat("[{0}]", EditSource.TableName);
                return false;
            }
            else
            {
                sbSql.Append("SELECT COUNT(*)"); 
                sbSql.Append("\r\n FROM ");
                sbSql.AppendFormat("[{1}] {0}", mainTable, EditSource.TableName);

                for (int i = 0; i < foreignFields.Count; i++)
                {
                    var targetField = EditSource.Fields.FirstOrDefault(f => f.Name.Equals(foreignFields[i].Key, StringComparison.OrdinalIgnoreCase));
                    if (targetField == null)
                    {
                        throw new Exception(string.Format("列[{0}]的外键列没有找到", foreignFields[i].Key));
                    }
                    var tableName = targetField.ForeignTable;

                    var shortTableName = EditSource.GetFreignTableShortName(targetField.Name);

                    sbSql.AppendFormat("\r\nleft outer join [{0}] {1} on M.[{2}] = {1}.[{3}]",
                        targetField.ForeignTable, shortTableName, targetField.Name, targetField.ValueMember);
                }
                return true;
            }
        }
        /// <summary>
        /// 如果有外键列，返回true
        /// </summary>
        /// <param name="sbSql"></param>
        /// <param name="fields"></param>
        /// <returns>如果有外键列，返回true</returns>
        private bool AppendSelectWithJoin(StringBuilder sbSql, IEnumerable<EditField> fields)
        {
            var mainTable = "M";

            var foreignFields = fields.Where(f => f.IsForeignColumn).GroupBy(f => f.TargetForeignKey).ToList();
            if (foreignFields.Count == 0)
            {
                sbSql.Append("SELECT ");
                fields.ForEachWithFirst(
                    (c) => { sbSql.AppendFormat("[{0}]", string.IsNullOrEmpty(c.FieldName) ? c.Name : c.FieldName); },
                            (c) => { sbSql.AppendFormat(",[{0}]", string.IsNullOrEmpty(c.FieldName) ? c.Name : c.FieldName); }
                    );
                sbSql.Append("\r\n FROM ");
                sbSql.AppendFormat("[{0}]", EditSource.TableName);
                return false;
            }
            else
            {
                Action<EditField> appendName = (c) =>
                {
                    if (c.IsForeignColumn)
                    {
                        if (string.IsNullOrEmpty(c.FieldName))
                        {
                            var shortTableName = EditSource.GetFreignTableShortName(c);
                            sbSql.AppendFormat("{0}.[{1}] as {2}", shortTableName, c.ForeignColumnName, c.Name);
                        }
                        else
                        {
                            sbSql.AppendFormat("{0} as {1}", c.FieldName, c.Name);
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(c.FieldName))
                        {
                        sbSql.AppendFormat("{0}.[{1}] as {2}", mainTable, c.Name, c.Name);
                    }
                        else
                        {
                            sbSql.AppendFormat("{0} as {1}", c.FieldName, c.Name);
                        }
                    }
                };

                sbSql.Append("SELECT ");
                fields.ForEachWithFirst(
                            (c) => { appendName(c); },
                            (c) => { sbSql.Append(","); appendName(c); }
                    );
                sbSql.Append("\r\n FROM ");
                sbSql.AppendFormat("[{1}] {0}", mainTable, EditSource.TableName);

                for (int i = 0; i < foreignFields.Count; i++)
                {
                    var targetField = EditSource.Fields.FirstOrDefault(f => f.Name.Equals(foreignFields[i].Key, StringComparison.OrdinalIgnoreCase));
                    if (targetField == null)
                    {
                        throw new Exception(string.Format("列[{0}]的外键列没有找到", foreignFields[i].Key));
                    }
                    var tableName = targetField.ForeignTable;

                    var shortTableName = EditSource.GetFreignTableShortName(targetField.Name);

                    sbSql.AppendFormat("\r\nleft outer join [{0}] {1} on M.[{2}] = {1}.[{3}]",
                        targetField.ForeignTable, shortTableName, targetField.Name, targetField.ValueMember);
                }
                return true;
            }
        }
        #endregion
        #endregion
    }

    [Flags]
    public enum ActionType
    {
        None = 0,
        /// <summary>
        /// 列表
        /// </summary>
        Index = 1,
        /// <summary>
        /// 搜索页面
        /// </summary>
        Search = 2,
        /// <summary>
        /// 搜索查询
        /// </summary>
        Query = 4,
        /// <summary>
        /// 详细
        /// </summary>
        Detail = 8,
        /// <summary>
        /// 新增页面
        /// </summary>
        Create = 16,
        /// <summary>
        /// 新增命令
        /// </summary>
        Add = 32,
        /// <summary>
        /// 修改页面
        /// </summary>
        Edit = 64,
        /// <summary>
        /// 更新
        /// </summary>
        Update = 128,
        /// <summary>
        /// 删除
        /// </summary>
        Delete = 256,
        /// <summary>
        /// 分页查询
        /// </summary>
        QueryIndex = 512,

        All = 0xFFFF
    }
}
