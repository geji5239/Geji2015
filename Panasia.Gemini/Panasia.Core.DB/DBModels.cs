/*
 * 数据的存储结构
 * 1、先实现
*/
namespace Panasia.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.Common;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization; 

    public enum UpdateMode
    {
        /// <summary>
        /// 不更新
        /// </summary>
        Default,
        /// <summary>
        /// 一直更新
        /// </summary>
        KeepUpdate,
        /// <summary>
        /// 更新一次后自动改回Default
        /// </summary>
        UpdateOnce,
        /// <summary>
        /// 保留，还没实现
        /// </summary>
        UpdateToDB
    }

    public class AppModuleCollection : CollectionBase<AppModule>
    {

    }

    [XmlRoot("AppModule")]
    public class AppModule : Entity
    {
        private Dictionary<string, Table> _TableDictionary = new Dictionary<string, Table>();

        private Dictionary<string, SqlCommand> _SqlCommands = new Dictionary<string, SqlCommand>();

        #region sqlbuilder
        SqlStringBuilder _SqlBuilder = null;
        internal SqlStringBuilder SqlBuilder
        {
            get
            {
                if (_SqlBuilder == null)
                {
                    var dbsetting = CodeSetting.Current.DBSettings.Where(db => db.ProviderName.Equals(this.Connection.ProviderName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    if (dbsetting == null)
                    {
                        throw new Exception("没有对应的数据库设置:" + this.Connection.ProviderName);
                    }
                    _SqlBuilder = new SqlStringBuilder(dbsetting);
                }
                return _SqlBuilder;
            }
        }
        #endregion

        #region 属性 Name
        [XmlAttribute("Name")]
        public string Name
        {
            get
            {
                return GetFieldValue<string>("Name", string.Empty);
            }
            set
            {
                SetFieldValue("Name", value);
            }
        }
        #endregion

        #region 属性 Filter
        [XmlAttribute("Filter")]
        public string Filter
        {
            get
            {
                return GetFieldValue<string>("Filter", string.Empty);
            }
            set
            {
                SetFieldValue("Filter", value);
            }
        }
        #endregion

        #region 属性 Connection
        private Connection _Connection = null;
        [XmlElement("Connection")]
        public Connection Connection
        {
            get
            {
                if (_Connection == null)
                {
                    _Connection = new Connection();
                }
                return _Connection;
            }
            set
            {
                _Connection = value;
                RaisePropertyChanged("Connection");
            }
        }
        #endregion

        #region 属性 Tables
        private TableCollection _Tables = null;
        [XmlArray("Tables"), XmlArrayItem("Table", typeof(Table))]
        public TableCollection Tables
        {
            get
            {
                if (_Tables == null)
                {
                    _Tables = new TableCollection();
                    _Tables.CollectionChanged += _Tables_CollectionChanged;
                }
                return _Tables;
            }
            set
            {
                if(_Tables!=null)
                {
                    _Tables.CollectionChanged -= _Tables_CollectionChanged;
                }
                _Tables = value;
                if(_Tables!=null)
                {
                    _Tables.CollectionChanged += _Tables_CollectionChanged;
                }
                RaisePropertyChanged("Tables");
            }
        }

        void _Tables_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach(Table table in e.NewItems)
                    {
                        table.Module = this;
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach(Table table in e.OldItems)
                    {
                        table.Module = null;
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        } 
        #endregion

        #region 属性 SystemColumns
        private CollectionBase<SystemColumn> _SystemColumns = null;
        [XmlArray("SystemColumns"), XmlArrayItem("Column", typeof(SystemColumn))]
        public CollectionBase<SystemColumn> SystemColumns
        {
            get
            {
                if (_SystemColumns == null)
                {
                    _SystemColumns = new CollectionBase<SystemColumn>();
                }
                return _SystemColumns;
            }
            set
            {
                _SystemColumns = value;
                RaisePropertyChanged("SystemColumns");
            }
        }
        #endregion

        #region 属性 LogicKeys
        private CollectionBase<LogicKey> _LogicKeys = null;

        /// <summary>
        /// 逻辑主键，因为表结构是动态获取，逻辑主键也设计为注入的方式，虽然隶属于模块配置定义，但属于表处理逻辑
        /// </summary>
        [XmlArray("LogicKeys"), XmlArrayItem("Add", typeof(LogicKey))]
        public CollectionBase<LogicKey> LogicKeys
        {
            get
            {
                if (_LogicKeys == null)
                {
                    _LogicKeys = new CollectionBase<LogicKey>();
                }
                return _LogicKeys;
            }
            set
            {
                _LogicKeys = value;
                RaisePropertyChanged("LogicKeys");
            }
        }
        #endregion

        #region 更新所有的表结构
        public void Refresh()
        {
            var items = this.LoadTables().ToList();
            try
            {
                Tables.UpdateWithMerge(items, (t, w) => t.Name.Equals(w.Name, StringComparison.OrdinalIgnoreCase),
                    (sourceTable, newTable) =>
                    {
                        sourceTable.Columns.UpdateWithMerge(newTable.Columns, (sc, nc) => sc.Name.Equals(nc.Name, StringComparison.OrdinalIgnoreCase),
                            (sc, nc) =>
                            {
                                sc.ColumnOrdinal=nc.ColumnOrdinal;
                                sc.ColumnType = nc.ColumnType;
                                sc.DataType = nc.DataType;
                                sc.DbType = nc.DbType;
                                sc.DefaultValue = nc.DefaultValue;
                                sc.IsAutoIncrement = nc.IsAutoIncrement;
                                sc.IsKey = nc.IsKey;
                                sc.IsReadOnly = nc.IsReadOnly;
                                sc.IsSystem = nc.IsSystem;
                                sc.Length = nc.Length; 
                                sc.NullAble = nc.NullAble;
                                sc.SystemParameter = sc.SystemParameter; 

                                //sc.PropertyName = nc.PropertyName;
                                sc.PropertyType = nc.PropertyType;
                                //sc.Remark = nc.Remark;
                            });
                    });
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        internal void Init()
        {
            Tables.ForEach(table =>
                {
                    table.Commands.ForEach((command) => {
                        AddCommand(table, command);
                    });
                });
            //Tables.ForEach(table =>
            //    {
            //        this.CreateDefaultCommands(table).ForEach(command =>
            //        {
            //            table.Commands.Add(command);
            //            AddCommand(table, command);
            //        });
            //    });

            ////处理附加命令
            //foreach (var command in Commands)
            //{
            //    this.AddCommand(command.Table, command);
            //}

            ////处理逻辑键
            //foreach (var item in LogicKeys)
            //{
            //    var table = Tables.FirstOrDefault(t => t.Name.Equals(item.Table, StringComparison.OrdinalIgnoreCase));
            //    if (table != null)
            //    {
            //        table.LogicKeys.Add(item);

            //        StringBuilder sb = new StringBuilder();

            //        item.ColumnName.Split(',').ForEachWithFirst(
            //            (c) => { sb.AppendFormat("{0}={1}", SqlBuilder.GetName(c), SqlBuilder.GetParameter(c)); },
            //            (c) => { sb.AppendFormat(",{0}={1}", SqlBuilder.GetName(c), SqlBuilder.GetParameter(c)); });
            //        string where = sb.ToString();

            //        SqlCommand existsCommand = new SqlCommand
            //        {
            //            Name = string.Format("ExistsKey_{0}", item.ColumnName.Replace(",", "_")),
            //            CommandType = CommandType.Count,
            //            Where = where
            //        };

            //        SqlCommand countCommand = new SqlCommand
            //        {
            //            Name = string.Format("CountKey_{0}", item.ColumnName.Replace(",", "_")),
            //            CommandType = CommandType.Count,
            //            Where = where
            //        };

            //        SqlCommand selectKeyCommand = new SqlCommand
            //        {
            //            Name = string.Format("SelectByKey_{0}", item.ColumnName.Replace(",", "_")),
            //            Columns = "*",
            //            CommandType = CommandType.Select,
            //            Where = where
            //        };

            //        this.AddCommand(table, existsCommand);
            //        this.AddCommand(table, countCommand);
            //        this.AddCommand(table, selectKeyCommand);
            //    }
            //}
            //增加方法触发器

        }

        internal void AddCommand(string table, SqlCommand command)
        {
            AddCommand(Tables.FirstOrDefault(t => t.Name.Equals(table, StringComparison.OrdinalIgnoreCase)), command);
        }

        internal void AddCommand(Table table, SqlCommand command)
        {
            CompileCommand(table, command);

            string address = string.Format("{0}.{1}.{2}",
                this.Name.ToLower(), table.Name.ToLower(), command.Name.ToLower());
            _SqlCommands[address] = command;
        }

        /// <summary>
        /// 根据command创建
        /// </summary>
        /// <param name="table"></param>
        /// <param name="command"></param>
        internal void CompileCommand(Table table, SqlCommand command)
        {
            SqlBuilder.CreateCommandText(table, command);
            SqlBuilder.CreateParameters(table, command);

            command.Table = table.Name;
            command.Module = this.Name;
        }

        public IEnumerable<KeyValuePair<string, SqlCommand>> GetCommands()
        {
            return _SqlCommands;
        }

        public SqlCommand GetCommand(string address, SqlCommand defaultCommand = null)
        {
            SqlCommand sqlcommand = null;
            if (_SqlCommands.TryGetValue(address.ToLower(), out sqlcommand))
            {
                return sqlcommand;
            }
            if (defaultCommand != null)
            {
                AddCommand(defaultCommand.Table, defaultCommand);
            }
            return null;
        }

        public Table GetTable(string name)
        {
            Table table = null;
            if (Tables.Count == 0)
            {
                if (Tables.Count == 0)
                {
                    return table;
                }
            }
            if (!_TableDictionary.TryGetValue(name.ToLower(), out table))
            {
                table = Tables.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
                    || t.Name.Equals(string.Format("{0}{1}", Filter, name), StringComparison.OrdinalIgnoreCase)
                    );
                if (table != null)
                {
                    _TableDictionary[name.ToLower()] = table;
                }
            }
            return table;
        }
    }

    public class TableCommandItem : CommandItem
    {
        #region 属性 LogicRule
        [XmlElement("LogicRule")]
        public LogicRule LogicRule
        {
            get
            {
                return GetFieldValue<LogicRule>("LogicRule", null);
            }
            set
            {
                SetFieldValue("LogicRule", value);
            }
        }
        #endregion
    }

    public class CommandItem : SqlCommand
    {
    }

    /*
     * 逻辑键处理
     * 如果一个
     */
    public class LogicKey : Entity
    {
        #region 属性 Table
        [XmlAttribute("Table")]
        public string Table
        {
            get
            {
                return GetFieldValue<string>("Table", string.Empty);
            }
            set
            {
                SetFieldValue("Table", value);
            }
        }
        #endregion

        #region 属性 ColumnName
        [XmlAttribute("ColumnName")]
        public string ColumnName
        {
            get
            {
                return GetFieldValue<string>("ColumnName", string.Empty);
            }
            set
            {
                SetFieldValue("ColumnName", value);
            }
        }
        #endregion

        #region 属性 References
        private CollectionBase<ReferenceTable> _References = null;
        [XmlArray("References"), XmlArrayItem("Add", typeof(ReferenceTable))]
        public CollectionBase<ReferenceTable> References
        {
            get
            {
                if (_References == null)
                {
                    _References = new CollectionBase<ReferenceTable>();
                }
                return _References;
            }
            set
            {
                _References = value;
                RaisePropertyChanged("References");
            }
        }
        #endregion
    }

    public class ReferenceTable : Entity
    {
        #region 属性 Table
        [XmlAttribute("Table")]
        public string Table
        {
            get
            {
                return GetFieldValue<string>("Table", string.Empty);
            }
            set
            {
                SetFieldValue("Table", value);
            }
        }
        #endregion

        #region 属性 ColumnName
        [XmlAttribute("ColumnName")]
        public string ColumnName
        {
            get
            {
                return GetFieldValue<string>("ColumnName", string.Empty);
            }
            set
            {
                SetFieldValue("ColumnName", value);
            }
        }
        #endregion
    }

    public class TableCollection : CollectionBase<Table>
    {

    }

    [XmlRoot("Connection")]
    public class Connection : Entity
    {
        #region 属性 Name
        [XmlAttribute("Name")]
        public string Name
        {
            get
            {
                return GetFieldValue<string>("Name", string.Empty);
            }
            set
            {
                SetFieldValue("Name", value);
            }
        }
        #endregion

        #region 属性 Server
        [XmlAttribute("Server")]
        public string Server
        {
            get
            {
                return GetFieldValue<string>("Server", string.Empty);
            }
            set
            {
                SetFieldValue("Server", value);
            }
        }
        #endregion

        #region 属性 Port
        [XmlAttribute("Port")]
        public int Port
        {
            get
            {
                return GetFieldValue<int>("Port", 0);
            }
            set
            {
                SetFieldValue("Port", value);
            }
        }
        #endregion

        #region 属性 DBName
        [XmlAttribute("DBName")]
        public string DBName
        {
            get
            {
                return GetFieldValue<string>("DBName", string.Empty);
            }
            set
            {
                SetFieldValue("DBName", value);
            }
        }
        #endregion

        #region 属性 DBUser
        [XmlAttribute("DBUser")]
        public string DBUser
        {
            get
            {
                return GetFieldValue<string>("DBUser", string.Empty);
            }
            set
            {
                SetFieldValue("DBUser", value);
            }
        }
        #endregion

        #region 属性 DBPassword
        [XmlAttribute("DBPassword")]
        public string DBPassword
        {
            get
            {
                return GetFieldValue<string>("DBPassword", string.Empty);
            }
            set
            {
                SetFieldValue("DBPassword", value);
            }
        }
        #endregion

        #region 属性 ConnectionString
        [XmlAttribute("ConnectionString")]
        public string ConnectionString
        {
            get
            {
                return GetFieldValue<string>("ConnectionString", string.Empty);
            }
            set
            {
                SetFieldValue("ConnectionString", value);
            }
        }
        #endregion

        #region 属性 ProviderName
        [XmlAttribute("ProviderName")]
        public string ProviderName
        {
            get
            {
                return GetFieldValue<string>("ProviderName", string.Empty);
            }
            set
            {
                SetFieldValue("ProviderName", value);
            }
        }
        #endregion

        #region 属性 UseLongConnection
        [XmlAttribute("UseLongConnection")]
        public bool UseLongConnection
        {
            get
            {
                return GetFieldValue<bool>("UseLongConnection", true);
            }
            set
            {
                SetFieldValue("UseLongConnection", value);
            }
        }
        #endregion
    }

    [XmlRoot("Table")]
    public class Table : TableSchema
    {
        #region 属性 IsFromDB
        [XmlAttribute("IsFromDB")]
        public bool IsFromDB
        {
            get
            {
                return GetFieldValue<bool>("IsFromDB", false);
            }
            set
            {
                SetFieldValue("IsFromDB", value);
            }
        }
        #endregion
        
        #region 属性 Commands
        private CollectionBase<CommandItem> _Commands = null;
        [XmlArray("Commands"),
        XmlArrayItem("Command", typeof(CommandItem)),
        XmlArrayItem("TableCommand", typeof(TableCommandItem))]
        public CollectionBase<CommandItem> Commands
        {
            get
            {
                if (_Commands == null)
                {
                    _Commands = new CollectionBase<CommandItem>();
                }
                return _Commands;
            }
            set
            {
                _Commands = value;
                RaisePropertyChanged("Commands");
            }
        }
        #endregion

        #region 属性 Module
        private AppModule _Module = null;
        [XmlIgnore()]
        public AppModule Module
        {
            get
            {
                return _Module;
            }
            set
            {
                _Module = value;
                RaisePropertyChanged("Module");
            }
        }
        #endregion

        #region 属性 CurrentColumn
        [XmlIgnore()]
        public Column CurrentColumn
        {
            get
            {
                return GetFieldValue<Column>("CurrentColumn", null);
            }
            set
            {
                SetFieldValue("CurrentColumn", value);
            }
        }
        #endregion

        #region 属性 CurrentCommand
        [XmlIgnore()]
        public SqlCommand CurrentCommand
        {
            get
            {
                return GetFieldValue<SqlCommand>("CurrentCommand", null);
            }
            set
            {
                SetFieldValue("CurrentCommand", value);
            }
        }
        #endregion

        #region 属性 LogicKeys
        private CollectionBase<LogicKey> _LogicKeys = null;

        /// <summary>
        /// 逻辑主键
        /// </summary>
        [XmlIgnore()]
        public CollectionBase<LogicKey> LogicKeys
        {
            get
            {
                if (_LogicKeys == null)
                {
                    _LogicKeys = new CollectionBase<LogicKey>();
                }
                return _LogicKeys;
            }
            set
            {
                _LogicKeys = value;
                RaisePropertyChanged("LogicKeys");
            }
        }
        #endregion

        #region 属性 Key
        [XmlAttribute("Key")]
        public string Key
        {
            get
            {
                return GetFieldValue<string>("Key", string.Empty);
            }
            set
            {
                SetFieldValue("Key", value);
            }
        }
        #endregion
    }

    public class TableSchema : Entity
    {
        #region 属性 Name
        [XmlAttribute("Name")]
        public string Name
        {
            get
            {
                return GetFieldValue<string>("Name", string.Empty);
            }
            set
            {
                SetFieldValue("Name", value);
            }
        }
        #endregion

        #region 属性 Schema
        [XmlAttribute("Schema")]
        public string Schema
        {
            get
            {
                return GetFieldValue<string>("Schema", string.Empty);
            }
            set
            {
                SetFieldValue("Schema", value);
            }
        }
        #endregion

        #region 属性 CodeName
        [XmlAttribute("CodeName")]
        public string CodeName
        {
            get
            {
                return GetFieldValue<string>("CodeName", string.Empty);
            }
            set
            {
                SetFieldValue("CodeName", value);
            }
        }
        #endregion

        #region 属性 Columns
        private ColumnCollection _Columns = null;
        [XmlArray("Columns"), XmlArrayItem("Column", typeof(Column))]
        public ColumnCollection Columns
        {
            get
            {
                if (_Columns == null)
                {
                    _Columns = new ColumnCollection();
                }
                return _Columns;
            }
            set
            {
                _Columns = value;
                RaisePropertyChanged("Columns");
            }
        }
        #endregion
    }

    public class SqlCommandCollection : CollectionBase<SqlCommand>
    {
    }

    public class SqlCommand : Entity
    {
        #region 属性 Module
        [XmlAttribute("Module")]
        public string Module
        {
            get
            {
                return GetFieldValue<string>("Module", string.Empty);
            }
            set
            {
                SetFieldValue("Module", value);
            }
        }
        #endregion

        #region 属性 Table
        [XmlAttribute("Table")]
        public string Table
        {
            get
            {
                return GetFieldValue<string>("Table", string.Empty);
            }
            set
            {
                SetFieldValue("Table", value);
            }
        }
        #endregion

        #region 属性 Name
        [XmlAttribute("Name")]
        public string Name
        {
            get
            {
                return GetFieldValue<string>("Name", string.Empty);
            }
            set
            {
                SetFieldValue("Name", value);
            }
        }
        #endregion

        #region 属性 CommandType
        [XmlAttribute("CommandType")]
        public CommandType CommandType
        {
            get
            {
                return GetFieldValue<CommandType>("CommandType", CommandType.Select);
            }
            set
            {
                SetFieldValue("CommandType", value);
            }
        }
        #endregion

        #region 属性 CommandText
        private string _CommandText = string.Empty;
        [XmlIgnore()]
        public string CommandText
        {
            get
            {
                return _CommandText;
            }
            set
            {
                _CommandText = value;
            }
        }
        #endregion

        #region 属性 CustomText
        [XmlAttribute("CustomText")]
        public string CustomText
        {
            get
            {
                return GetFieldValue<string>("CustomText", string.Empty);
            }
            set
            {
                SetFieldValue("CustomText", value);
            }
        }
        #endregion

        #region 属性 Columns
        [XmlAttribute("Columns")]
        public string Columns
        {
            get
            {
                return GetFieldValue<string>("Columns", string.Empty);
            }
            set
            {
                SetFieldValue("Columns", value);
            }
        }
        #endregion

        #region 属性 Where
        [XmlAttribute("Where")]
        public string Where
        {
            get
            {
                return GetFieldValue<string>("Where", string.Empty);
            }
            set
            {
                SetFieldValue("Where", value);
            }
        }
        #endregion

        #region 属性 Parameters
        private SqlParameterCollection _Parameters = null;
        [XmlArray("Parameters"), XmlArrayItem("Parameter", typeof(SqlParameter))]
        public SqlParameterCollection Parameters
        {
            get
            {
                if (_Parameters == null)
                {
                    _Parameters = new SqlParameterCollection();
                }
                return _Parameters;
            }
            set
            {
                _Parameters = value;
                RaisePropertyChanged("Parameters");
            }
        }
        #endregion

        #region 属性 Remark
        [XmlAttribute("Remark"), DefaultValue("")]
        public string Remark
        {
            get
            {
                return GetFieldValue<string>("Remark", string.Empty);
            }
            set
            {
                SetFieldValue("Remark", value);
            }
        }
        #endregion

        #region 属性 ReloadData
        [XmlAttribute("ReloadData"), DefaultValue(false)]
        public bool ReloadData
        {
            get
            {
                return GetFieldValue<bool>("ReloadData", false);
            }
            set
            {
                SetFieldValue("ReloadData", value);
            }
        }
        #endregion

        #region 属性 ReloaBykeys
        [XmlAttribute("ReloaBykeys"), DefaultValue(false)]
        public bool ReloaBykeys
        {
            get
            {
                return GetFieldValue<bool>("ReloaBykeys", false);
            }
            set
            {
                SetFieldValue("ReloaBykeys", value);
            }
        }
        #endregion

        #region 属性 OrderBy
        [XmlAttribute("OrderBy"), DefaultValue("")]
        public string OrderBy
        {
            get
            {
                return GetFieldValue<string>("OrderBy", string.Empty);
            }
            set
            {
                SetFieldValue("OrderBy", value);
            }
        }
        #endregion

        #region 属性 Limit
        [XmlAttribute("Limit"), DefaultValue(0)]
        public int Limit
        {
            get
            {
                return GetFieldValue<int>("Limit", 0);
            }
            set
            {
                SetFieldValue("Limit", value);
            }
        }
        #endregion

        #region 属性 Start
        [XmlAttribute("Start"), DefaultValue(0)]
        public int Start
        {
            get
            {
                return GetFieldValue<int>("Start", 0);
            }
            set
            {
                SetFieldValue("Start", value);
            }
        }
        #endregion

    }

    [Flags]
    public enum CommandType
    {
        Insert,
        Delete,
        Update,
        Select,
        Exists,
        Scalar,
        Count
    }

    public class ColumnCollection : CollectionBase<Column>
    {
    }

    public class SystemColumn : Entity
    {
        #region 属性 Name
        [XmlAttribute("Name")]
        public string Name
        {
            get
            {
                return GetFieldValue<string>("Name", string.Empty);
            }
            set
            {
                SetFieldValue("Name", value);
            }
        }
        #endregion

        #region 属性 IsReadOnly
        [XmlAttribute("IsReadOnly")]
        public bool IsReadOnly
        {
            get
            {
                return GetFieldValue<bool>("IsReadOnly", false);
            }
            set
            {
                SetFieldValue("IsReadOnly", value);
            }
        }
        #endregion
        
        #region 属性 SystemParameter
        [XmlAttribute("SystemParameter")]
        public string SystemParameter
        {
            get
            {
                return GetFieldValue<string>("SystemParameter", string.Empty);
            }
            set
            {
                SetFieldValue("SystemParameter", value);
            }
        }
        #endregion

        #region 属性 CreateInclude 
        [XmlAttribute("CreateInclude"), DefaultValue(false)]
        public bool CreateInclude
        {
            get
            {
                return GetFieldValue<bool>("CreateInclude", false);
            }
            set
            {
                SetFieldValue("CreateInclude", value);
            } 
        }
        #endregion

        #region 属性 UpdateInclude 
        [XmlAttribute("UpdateInclude"), DefaultValue(false)]
        public bool UpdateInclude
        {
            get
            {
                return GetFieldValue<bool>("UpdateInclude", false);
            }
            set
            {
                SetFieldValue("UpdateInclude", value);
            } 
        }
        #endregion
    }

    public class Column : Entity
    {
        #region 属性 Name
        [XmlAttribute("Name")]
        public string Name
        {
            get
            {
                return GetFieldValue<string>("Name", string.Empty);
            }
            set
            {
                SetFieldValue("Name", value);
            }
        }
        #endregion

        #region 属性 ColumnOrdinal
        [XmlAttribute("ColumnOrdinal")]
        public int ColumnOrdinal
        {
            get
            {
                return GetFieldValue<int>("ColumnOrdinal", 0);
            }
            set
            {
                SetFieldValue("ColumnOrdinal", value);
            }
        }
        #endregion

        #region 属性 ColumnType
        /// <summary>
        /// 数据库的字段类型
        /// </summary>
        [XmlAttribute("ColumnType")]
        public string ColumnType
        {
            get
            {
                return GetFieldValue<string>("ColumnType", string.Empty);
            }
            set
            {
                SetFieldValue("ColumnType", value);
            }
        }
        #endregion

        #region 属性 DataType
        /// <summary>
        /// 字段的值类型
        /// </summary>
        [XmlAttribute("DataType")]
        public string DataType
        {
            get
            {
                return GetFieldValue<string>("DataType", string.Empty);
            }
            set
            {
                SetFieldValue("DataType", value);
            }
        }
        #endregion

        #region 属性 DbType
        /// <summary>
        /// 对应标准的DbType
        /// </summary>
        [XmlAttribute("DbType")]
        public System.Data.DbType DbType
        {
            get
            {
                return GetFieldValue<System.Data.DbType>("DbType", System.Data.DbType.String);
            }
            set
            {
                SetFieldValue("DbType", value);
            }
        }
        #endregion

        #region 属性 PropertyType
        /// <summary>
        /// 实体类的字段类型
        /// </summary>
        [XmlAttribute("PropertyType")]
        public string PropertyType
        {
            get
            {
                return GetFieldValue<string>("PropertyType", string.Empty);
            }
            set
            {
                SetFieldValue("PropertyType", value);
            }
        }
        #endregion

        #region 属性 PropertyName
        [XmlAttribute("PropertyName")]
        public string PropertyName
        {
            get
            {
                return GetFieldValue<string>("PropertyName", string.Empty);
            }
            set
            {
                SetFieldValue("PropertyName", value);
            }
        }
        #endregion

        #region 属性 Remark
        [XmlAttribute("Remark")]
        public string Remark
        {
            get
            {
                return GetFieldValue<string>("Remark", string.Empty);
            }
            set
            {
                SetFieldValue("Remark", value);
            }
        }
        #endregion

        #region 属性 DefaultValue
        [XmlAttribute("DefaultValue")]
        public string DefaultValue
        {
            get
            {
                return GetFieldValue<string>("DefaultValue", string.Empty);
            }
            set
            {
                SetFieldValue("DefaultValue", value);
            }
        }
        #endregion

        #region 属性 Length
        [XmlAttribute("Length")]
        public long Length
        {
            get
            {
                return GetFieldValue<long>("Length", 0);
            }
            set
            {
                SetFieldValue("Length", value);
            }
        }
        #endregion

        #region 属性 NullAble
        [XmlAttribute("NullAble")]
        public bool NullAble
        {
            get
            {
                return GetFieldValue<bool>("NullAble", false);
            }
            set
            {
                SetFieldValue("NullAble", value);
            }
        }
        #endregion

        #region 属性 IsAutoIncrement
        [XmlAttribute("IsAutoIncrement")]
        public bool IsAutoIncrement
        {
            get
            {
                return GetFieldValue<bool>("IsAutoIncrement", false);
            }
            set
            {
                SetFieldValue("IsAutoIncrement", value);
            }
        }
        #endregion

        #region 属性 IsReadOnly
        /// <summary>
        /// 只有新增时才可以赋值，不能更新
        /// </summary>
        [XmlAttribute("IsReadOnly")]
        public bool IsReadOnly
        {
            get
            {
                return GetFieldValue<bool>("IsReadOnly", false);
            }
            set
            {
                SetFieldValue("IsReadOnly", value);
            }
        }
        #endregion

        #region 属性 IsKey
        [XmlAttribute("IsKey")]
        public bool IsKey
        {
            get
            {
                return GetFieldValue<bool>("IsKey", false);
            }
            set
            {
                SetFieldValue("IsKey", value);
            }
        }
        #endregion

        #region 属性 IsSystem
        [XmlAttribute("IsSystem")]
        public bool IsSystem
        {
            get
            {
                return GetFieldValue<bool>("IsSystem", false);
            }
            set
            {
                SetFieldValue("IsSystem", value);
            }
        }
        #endregion

        #region 属性 SystemParameter
        [XmlAttribute("SystemParameter")]
        public string SystemParameter
        {
            get
            {
                return GetFieldValue<string>("SystemParameter", string.Empty);
            }
            set
            {
                SetFieldValue("SystemParameter", value);
            }
        }
        #endregion
    }


    [System.Xml.Serialization.XmlRoot("CodeSetting")]
    public class CodeSetting : ConfigFile
    {
        private CodeSetting()
            : base()
        {
        } 

        static CodeSetting _Current = null;
        public static CodeSetting Current
        {
            get
            {
                if (_Current == null)
                {
                    _Current = ConfigFile.Load<CodeSetting>("codesettings.xml");
                }
                return _Current;
            }
        }

        public TypeMapping GetTypeMapping(string columnType)
        {
            var tm = TypeMappings.FirstOrDefault(t => t.DataType.Equals(columnType, StringComparison.OrdinalIgnoreCase));
            if (tm == null)
            {
                throw new Exception("类型没有对应的转换" + columnType);
            }
            return tm;
        }
        #region 属性 TypeMapping
        private TypeMappingCollection _TypeMappings = null;
        [XmlArray("TypeMappings"), XmlArrayItem("Type", typeof(TypeMapping))]
        public TypeMappingCollection TypeMappings
        {
            get
            {
                if (_TypeMappings == null)
                {
                    _TypeMappings = new TypeMappingCollection();
                }
                return _TypeMappings;
            }
            set
            {
                _TypeMappings = value;
                RaisePropertyChanged("TypeMappings");
            }
        }
        #endregion

        #region 属性 DBSettings
        private DBSettingCollection _DBSettings = null;
        [XmlArray("DBSettings"), XmlArrayItem("DBSetting", typeof(DBSetting))]
        public DBSettingCollection DBSettings
        {
            get
            {
                if (_DBSettings == null)
                {
                    _DBSettings = new DBSettingCollection();
                }
                return _DBSettings;
            }
            set
            {
                _DBSettings = value;
                RaisePropertyChanged("DBSettings");
            }
        }
        #endregion
    }

    public class DBSettingCollection : CollectionBase<DBSetting>
    {
    }
    public class DBSetting : Entity
    {
        #region 属性 ProviderName
        [XmlAttribute("ProviderName")]
        public string ProviderName
        {
            get
            {
                return GetFieldValue<string>("ProviderName", string.Empty);
            }
            set
            {
                SetFieldValue("ProviderName", value);
            }
        }
        #endregion

        #region 属性 SqlFieldNameFormat
        [XmlAttribute("SqlFieldNameFormat")]
        public string SqlFieldNameFormat
        {
            get
            {
                return GetFieldValue<string>("SqlFieldNameFormat", string.Empty);
            }
            set
            {
                SetFieldValue("SqlFieldNameFormat", value);
            }
        }
        #endregion

        #region 属性 SqlParameterFormat
        [XmlAttribute("SqlParameterFormat")]
        public string SqlParameterFormat
        {
            get
            {
                return GetFieldValue<string>("SqlParameterFormat", string.Empty);
            }
            set
            {
                SetFieldValue("SqlParameterFormat", value);
            }
        }
        #endregion

        #region 属性 SqlIdentityFunc
        [XmlAttribute("SqlIdentityFunc")]
        public string SqlIdentityFunc
        {
            get
            {
                return GetFieldValue<string>("SqlIdentityFunc", string.Empty);
            }
            set
            {
                SetFieldValue("SqlIdentityFunc", value);
            }
        }
        #endregion

    }

    public class TypeMappingCollection : CollectionBase<TypeMapping>
    {
    }

    public class TypeMapping : Entity
    {
        #region 属性 Name
        [XmlAttribute("Name")]
        public string Name
        {
            get
            {
                return GetFieldValue<string>("Name", string.Empty);
            }
            set
            {
                SetFieldValue("Name", value);
            }
        }
        #endregion

        #region 属性 DataType
        [XmlAttribute("DataType")]
        public string DataType
        {
            get
            {
                return GetFieldValue<string>("DataType", string.Empty);
            }
            set
            {
                SetFieldValue("DataType", value);
            }
        }
        #endregion

        #region 属性 DbType
        [XmlAttribute("DbType")]
        public System.Data.DbType DbType
        {
            get
            {
                return GetFieldValue<System.Data.DbType>("DbType", System.Data.DbType.String);
            }
            set
            {
                SetFieldValue("DbType", value);
            }
        }
        #endregion

        #region 属性 ModelType
        [XmlAttribute("ModelType")]
        public string ModelType
        {
            get
            {
                return GetFieldValue<string>("ModelType", string.Empty);
            }
            set
            {
                SetFieldValue("ModelType", value);
            }
        }
        #endregion

        #region 属性 Converter
        [XmlAttribute("Converter")]
        public string Converter
        {
            get
            {
                return GetFieldValue<string>("Converter", string.Empty);
            }
            set
            {
                SetFieldValue("Converter", value);
            }
        }
        #endregion

        #region 属性 ConvertBack
        [XmlAttribute("ConvertBack")]
        public string ConvertBack
        {
            get
            {
                return GetFieldValue<string>("ConvertBack", string.Empty);
            }
            set
            {
                SetFieldValue("ConvertBack", value);
            }
        }
        #endregion

        #region 属性 DefaultValue
        [XmlAttribute("DefaultValue")]
        public string DefaultValue
        {
            get
            {
                return GetFieldValue<string>("DefaultValue", string.Empty);
            }
            set
            {
                SetFieldValue("DefaultValue", value);
            }
        }
        #endregion

        #region 属性 Remark
        [XmlAttribute("Remark")]
        public string Remark
        {
            get
            {
                return GetFieldValue<string>("Remark", string.Empty);
            }
            set
            {
                SetFieldValue("Remark", value);
            }
        }
        #endregion
    }

    public class SqlParameterCollection : CollectionBase<SqlParameter>
    {
    }

    public class SqlParameter : Entity
    {
        #region 属性 Name
        /// <summary>
        /// 参数名称
        /// </summary>
        [XmlAttribute("Name")]
        public string Name
        {
            get
            {
                return GetFieldValue<string>("Name", string.Empty);
            }
            set
            {
                SetFieldValue("Name", value);
            }
        }
        #endregion

        #region 属性 Value
        [XmlAttribute("Value")]
        public string Value
        {
            get
            {
                return GetFieldValue<string>("Value", string.Empty);
            }
            set
            {
                SetFieldValue("Value", value);
            }
        }
        #endregion

        #region 属性 Converter
        [XmlAttribute("Converter")]
        public string Converter
        {
            get
            {
                return GetFieldValue<string>("Converter", string.Empty);
            }
            set
            {
                SetFieldValue("Converter", value);
            }
        }
        #endregion

        #region 属性 DbType
        [XmlAttribute("DbType")]
        public System.Data.DbType DbType
        {
            get
            {
                return GetFieldValue<System.Data.DbType>("DbType", System.Data.DbType.String);
            }
            set
            {
                SetFieldValue("DbType", value);
            }
        }
        #endregion

        #region 属性 Length
        [XmlAttribute("Length")]
        public long Length
        {
            get
            {
                return GetFieldValue<long>("Length", 0);
            }
            set
            {
                SetFieldValue("Length", value);
            }
        }
        #endregion

        #region 属性 ValueType
        [XmlAttribute("ValueType")]
        public ValueType ValueType
        {
            get
            {
                return GetFieldValue<ValueType>("ValueType", ValueType.Property);
            }
            set
            {
                SetFieldValue("ValueType", value);
            }
        }
        #endregion

        #region 属性 ValueName
        /// <summary>
        /// 值的名称
        /// </summary>
        [XmlAttribute("ValueName")]
        public string ValueName
        {
            get
            {
                return GetFieldValue<string>("ValueName", string.Empty);
            }
            set
            {
                SetFieldValue("ValueName", value);
            }
        }
        #endregion

    }

    public class ColumnValue : Entity
    {
        #region 属性 ColumnName
        [XmlAttribute("ColumnName")]
        public string ColumnName
        {
            get
            {
                return GetFieldValue<string>("ColumnName", string.Empty);
            }
            set
            {
                SetFieldValue("ColumnName", value);
            }
        }
        #endregion

        #region 属性 Value
        [XmlAttribute("Value")]
        public string Value
        {
            get
            {
                return GetFieldValue<string>("Value", string.Empty);
            }
            set
            {
                SetFieldValue("Value", value);
            }
        }
        #endregion

        #region 属性 ValueType
        [XmlAttribute("ValueType")]
        public ValueType ValueType
        {
            get
            {
                return GetFieldValue<ValueType>("ValueType", ValueType.Property);
            }
            set
            {
                SetFieldValue("ValueType", value);
            }
        }
        #endregion
    }

    public enum ValueType
    {
        /// <summary>
        /// 值是常数
        /// </summary>
        Constant = 0,
        /// <summary>
        /// 值是实体属性
        /// </summary>
        Property = 1,
        /// <summary>
        /// 值是系统变量
        /// </summary>
        System = 2,
    }
}
