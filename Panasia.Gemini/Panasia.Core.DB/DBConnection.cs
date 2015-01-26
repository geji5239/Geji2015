using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using System.Text.RegularExpressions;

namespace Panasia.Core
{
    public interface ISystemParameter
    {
        string Name { get; }
    }

    public interface IValueConverter
    {
        string Name { get; }
    }

    class SystemFunctions
    {
#pragma warning disable 0649
        private IEnumerable<Lazy<Func<string, object>, ISystemParameter>> _SystemParameterFuncs;

        private IEnumerable<Lazy<Func<object, object>, IValueConverter>> _ValueConverters;
#pragma warning restore 0649

        
        public SystemFunctions()
        {
            var container = ServiceLocator.Current.GetContainer();
            _SystemParameterFuncs = container.GetExports<Func<string, object>, ISystemParameter>(SystemParameterFunctionAttribute.ExportContractName);
            _ValueConverters = container.GetExports<Func<object, object>, IValueConverter>("ValueConverter");
        }

        public object GetSystemParameterValue(string funname, string parameter = "")
        {
            if (_SystemParameterFuncs == null)
            {
                throw new ArgumentException(string.Format("参数方法[{0}]没有对应", funname));
            }

            var func = _SystemParameterFuncs.FirstOrDefault(f => f.Metadata.Name.Equals(funname, StringComparison.OrdinalIgnoreCase));
            if (func == null)
            {
                throw new ArgumentException(string.Format("参数方法[{0}]没有对应", funname));
            }
            return func.Value(parameter);
        }

        public object GetConverterValue(string funName, object value)
        {
            if (_ValueConverters == null)
            {
                throw new ArgumentException(string.Format("参数方法[{0}]没有对应", funName));
            }

            var func = _ValueConverters.FirstOrDefault(f => f.Metadata.Name.Equals(funName, StringComparison.OrdinalIgnoreCase));
            if (func == null)
            {
                throw new ArgumentException(string.Format("参数方法[{0}]没有对应", funName));
            }
            return func.Value(value);
        }

    }

    public class ValueConverters
    {
        [Export("ValueConverter", typeof(Func<object, object>))]
        [ExportMetadata("Name", "GuidToBinary")]
        public object GuidToBinary(object guid)
        {
            if (guid is Guid)
            {
                return ((Guid)guid).ToByteArray();
            }
            if (guid is string)
            {
                return (new Guid(guid.ToString())).ToByteArray();
            }
            return guid;
        }

        [Export("ValueConverter", typeof(Func<object, object>))]
        [ExportMetadata("Name", "BinaryToGuid")]
        public object BinaryToGuid(object value)
        {
            if (value == null)
            {
                return Guid.Empty;
            }
            if (value is byte[])
            {
                return new Guid((byte[])value);
            }
            return value;
        }
        [Export("ValueConverter", typeof(Func<object, object>))]
        [ExportMetadata("Name", "BinaryToString")]
        public object BinaryToString(object value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            if (value is byte[])
            {
                return Convert.ToBase64String((byte[])value);
            }
            return value;
        }

        [Export("ValueConverter", typeof(Func<object, object>))]
        [ExportMetadata("Name", "StringToBinary")]
        public object StringToBinary(object value)
        {
            if (value == null)
            {
                return new byte[0];
            }
            if (value is string)
            {
                return Convert.FromBase64String((string)value);
            }
            return value;
        }

        [Export("ValueConverter", typeof(Func<object, object>))]
        [ExportMetadata("Name", "Md5")]
        public object Md5Encryption(object value)
        {
            if (value is string)
            {
                return PasswordEncryption.GetMd5_32(value as string);
            }
            return value;
        }
    }


    public static class DBConn
    {
        private static SystemFunctions _SystemFunctions = new SystemFunctions();

        private static Dictionary<DbType, object> _DefaultValues = new Dictionary<DbType, object>();

        private static Dictionary<DbType, Func<string, object>> _DefaultValueConverters = new Dictionary<DbType, Func<string, object>>();

        static DBConn()
        {
            _DefaultValues.Add(DbType.AnsiString, string.Empty);
            _DefaultValues.Add(DbType.AnsiStringFixedLength, string.Empty);
            _DefaultValues.Add(DbType.Binary, null);
            _DefaultValues.Add(DbType.Boolean, false);
            _DefaultValues.Add(DbType.Byte, 0);
            _DefaultValues.Add(DbType.Currency, 0);
            _DefaultValues.Add(DbType.Date, DateHelper.MinDate);
            _DefaultValues.Add(DbType.DateTime, DateHelper.MinDate);
            _DefaultValues.Add(DbType.DateTime2, DateHelper.MinDate);
            _DefaultValues.Add(DbType.DateTimeOffset, DateHelper.MinDate);
            _DefaultValues.Add(DbType.Decimal, 0);
            _DefaultValues.Add(DbType.Guid, Guid.Empty);
            _DefaultValues.Add(DbType.Int16, 0);
            _DefaultValues.Add(DbType.Int32, 0);
            _DefaultValues.Add(DbType.Int64, 0);
            _DefaultValues.Add(DbType.Object, null);
            _DefaultValues.Add(DbType.SByte, 0);
            _DefaultValues.Add(DbType.Single, 0);
            _DefaultValues.Add(DbType.String, string.Empty);
            _DefaultValues.Add(DbType.StringFixedLength, string.Empty);
            _DefaultValues.Add(DbType.Time, DateHelper.MinDate);
            _DefaultValues.Add(DbType.UInt16, 0);
            _DefaultValues.Add(DbType.UInt32, 0);
            _DefaultValues.Add(DbType.UInt64, 0);
            _DefaultValues.Add(DbType.VarNumeric, 0);
            _DefaultValues.Add(DbType.Xml, string.Empty);
        }

        public static object GetDBValue(DbType dbtype, object value)
        {
            if (value == null || value.ToString().Length == 0)
            {
                return _DefaultValues[dbtype];
            }
            return value;
        }

        public static object ExecuteScalar(DbConnection db, SqlCommand sql, params object[] nameParas)
        {
            var cmd = db.CreateDBCommand(sql, nameParas);
            var result = cmd.ExecuteScalar();
            return result;
        }

        public static int ExecuteNoneQuery(DbConnection db, SqlCommand sql, params object[] nameParas)
        {
            lock (db)
            {
                var cmd = db.CreateDBCommand(sql, nameParas);
                var count = cmd.ExecuteNonQuery();
                return count;
            }
        }

        public static IEnumerable<T> ExecuteQuery<T>(DbConnection db, SqlCommand sql, params object[] nameParas) where T : IEntityBase, new()
        {
            lock (db)
            {
                var cmd = db.CreateDBCommand(sql, nameParas);
                var reader = cmd.ExecuteReader();
                var items = reader.ToItems<T>().ToList();
                reader.Close();
                reader.Dispose();
                return items;
            }
        }

        public static T ExecuteCommand<T>(DbConnection db, SqlCommand sql, Func<DbCommand, T> funcResult, params object[] nameParas)
        {
            var cmd = CreateDBCommand(db, sql, nameParas);
            return funcResult(cmd);
        }

        private static DbCommand CreateDBCommand(this DbConnection db, SqlCommand sql, object[] nameParas)
        {
            string result = string.Empty;
            var cmd = db.CreateCommand();
            cmd.CommandText = sql.CommandText;
            var dic = nameParas.ToDictionary(true);

            Func<SqlParameter, object> getParameterValue = (p) =>
            {
                switch (p.ValueType)
                {
                    case ValueType.Constant:
                        return p.Value;
                    case ValueType.System:
                        return GetSystemValue(p.ValueName);
                    case ValueType.Property:
                    default:
                        var obj = dic.GetDictionaryValue(p.ValueName.IsNullOrEmpty() ? p.Name.ToLower() : p.ValueName.ToLower(), p.Value);
                        return obj;
                }
            };

            foreach (var sqlParameter in sql.Parameters)
            {
                DbParameter para = cmd.CreateParameter();
                para.ParameterName = sqlParameter.Name;
                para.DbType = sqlParameter.DbType;

                if (sqlParameter.Converter.IsNullOrEmpty())
                {
                    para.Value = GetDBValue(para.DbType, getParameterValue(sqlParameter));
                }
                else
                {
                    para.Value = _SystemFunctions.GetConverterValue(sqlParameter.Converter, getParameterValue(sqlParameter));
                }
                cmd.Parameters.Add(para);
            }
            return cmd;
        }

        public static object GetSystemValue(string name, string parameter = "")
        {
            return _SystemFunctions.GetSystemParameterValue(name, parameter);
        }

        private static IEnumerable<T> ToItems<T>(this IDataReader reader) where T : IEntityBase, new()
        {
            while (reader.Read())
            {
                T newItem = new T();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var vv = reader.GetValue(i);
                    if (vv is DBNull)
                    {
                        continue;
                    }
                    newItem.SetValue(reader.GetName(i), vv);
                }
                yield return newItem;
            }
        }
    }

    public static class SchemaExtension
    {
        private static ConcurrentDictionary<string, DbConnection> _Connections = new ConcurrentDictionary<string, DbConnection>();
        private static ConcurrentDictionary<DbConnection, string> _ConnectionNames = new ConcurrentDictionary<DbConnection, string>();

        public static DbConnection OpenConnection(this AppModule module)
        {
            DbConnection conn = null;
            if (module.Connection.UseLongConnection)
            {
                conn = GetExistsConnection(module.Name);
                if (conn == null)
                {
                    lock (module)
                    {
                        conn = GetExistsConnection(module.Name);
                        if (conn == null)
                        {
                            conn = OpenNewConnection(module);
                        }

                        _Connections.TryAdd(module.Name, conn);
                        _ConnectionNames.TryAdd(conn, module.Name);
                    }
                }
            }
            else
            {
                conn = OpenNewConnection(module);
            }

            return conn;
        }

        private static DbConnection GetExistsConnection(string name)
        {
            DbConnection conn = null;
            if (_Connections.TryGetValue(name, out conn))
            {
                if (conn.State == ConnectionState.Open)
                {
                    return conn;
                }
                else
                {
                    _Connections.TryRemove(name, out conn);
                    _ConnectionNames.TryRemove(conn, out name);
                }
            }
            return null;
        }
        private static DbConnection OpenNewConnection(AppModule module)
        {
            var factory = DbProviderFactories.GetFactory(
                string.IsNullOrWhiteSpace(module.Connection.ProviderName) ? "System.Data.SqlClient" : module.Connection.ProviderName);
            var conn = factory.CreateConnection();
            conn.ConnectionString = module.Connection.ConnectionString;
            conn.Open();

            //conn.Log(string.Format("新打开模块数据库连接[{0}]", module.Name), Category.Debug);
            return conn;
        }

        /// <summary>
        /// 关闭连接：如果这个连接是长连接，不关闭
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="forceColse">如果指定为强制关闭，则关闭</param>
        public static void CloseConnection(this DbConnection conn, bool forceColse = false)
        {
            try
            {
                if (_Connections.Values.Contains(conn))
                {
                    if (forceColse)
                    {
                        DbConnection db = null;
                        string name = "";
                        _ConnectionNames.TryRemove(conn, out name);
                        _Connections.TryRemove(name, out db);
                        conn.Close();
                        conn.Dispose();
                    }
                }
                else
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            catch
            {
            }
        }

        #region LoadTables
        public static IEnumerable<Table> LoadTables(this AppModule module)
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
            var conn = module.OpenConnection();
            string[] restrictions = new string[4];
            restrictions[3] = "BASE TABLE";
            DataTable stable = conn.GetSchema("Tables", restrictions);
            foreach (DataRow row in stable.Rows)
            {
                Table table = null;
                try
                {
                    string tableName = row[1].ToString().Equals("dbo", StringComparison.OrdinalIgnoreCase) ? row[2].ToString() : string.Format("{0}.{1}", row[1], row[2]);
                    string name = row[2].ToString();
                    if (module.Filter.Length > 0 && (!name.StartsWith(module.Filter, StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }
                    table = new Table
                    {
                        Name = row[2].ToString(),
                        Schema = row[1].ToString(),
                        CodeName = row[2].ToString().ToFormatedName(),
                        IsFromDB = true
                    };
                    GetColumns(module, row[1].ToString(), row[2].ToString()).AddToCollection(table.Columns);

                    var keycolumns = table.Columns.Where(c => c.IsKey).Select(c => c.Name).ToMergeString(",");
                    table.Key = keycolumns;

                }
                catch (Exception)
                {
                    throw;
                }
                yield return table;
                conn.CloseConnection();
            }
        }
        #endregion

        #region GetColumns

        public static IEnumerable<Column> GetColumns(this AppModule module, string schema, string table)
        {
            IEnumerable<Column> columns = null;
            var conn = module.OpenConnection();
            columns = GetColumns(conn, schema,table,module.SqlBuilder.GetName(table)).ToList();
            conn.CloseConnection();
            foreach (var column in columns)
            {
                var sysColumn = module.SystemColumns.FirstOrDefault(s => s.Name.Equals(column.Name, StringComparison.OrdinalIgnoreCase));
                if (sysColumn != null)
                {
                    column.IsSystem = true;
                    column.IsReadOnly = sysColumn.IsReadOnly;
                    column.SystemParameter = sysColumn.SystemParameter;
                }

                yield return column;
            }
        }

        private static IEnumerable<Column> GetColumns(DbConnection conn, string schema, string table,string tableName)
        {
            string[] restrictions = new string[4];
            restrictions[1] = schema;
            restrictions[2] = table;

            DataTable ctable = conn.GetSchema("Columns", restrictions);
            string sql = string.Format("select * from {0}", schema.Length > 0 ? (schema + "." + tableName) : tableName);
            var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            var reader = cmd.ExecuteReader(CommandBehavior.KeyInfo | CommandBehavior.SchemaOnly);
            DataTable stable = reader.GetSchemaTable();
            reader.Close();
            reader.Dispose();

            int index = 0;
            foreach (DataRow row in stable.Rows)
            {
                Column col = null;
                try
                {
                    string columnType = ctable.Rows[index]["DATA_TYPE"].ToString();

                    var tm = CodeSetting.Current.GetTypeMapping(columnType);
                    col = new Column
                    {
                        Name = row["ColumnName"].ToString(),
                        ColumnType = columnType,
                        DataType = row["DataType"].ToString(),
                        Length = ctable.Rows[index]["CHARACTER_MAXIMUM_LENGTH"].ToInt64(),
                        IsKey = (bool)row["IsKey"],
                        NullAble = (bool)row["AllowDBNull"],
                        IsAutoIncrement = (bool)row["IsAutoIncrement"],
                        ColumnOrdinal = (int)row["ColumnOrdinal"],
                        DbType = tm.DbType,
                        PropertyType = tm.ModelType,
                        DefaultValue = tm.DefaultValue,
                        PropertyName = row["ColumnName"].ToString().ToFirstUpper(),
                        IsReadOnly = (bool)row["IsAutoIncrement"]
                    };
                    index++;

                }
                catch (Exception)
                {
                    throw;
                }
                yield return col;
            }
        }

        public static string ToFirstUpper(this string name)
        {
            if (name.IsNullOrEmpty())
            {
                return name;
            }
            if (char.IsUpper(name[0]))
            {
                return name;
            }
            if (name.Length > 1)
            {
                return name.Substring(0, 1).ToUpper() + name.Substring(1);
            }
            return name.ToUpper();
        }

        public static string ToFormatedName(this string name)
        {
            if (name.Length > 1)
            {
                if (name.IndexOf('_') < 0)
                {
                    return name.ToFirstUpper();
                }

                StringBuilder sb = new StringBuilder();
                foreach (var s in name.Split('_'))
                {
                    sb.Append(s.ToFirstUpper());
                }
                return sb.ToString();
            }
            return name.ToFirstUpper();
        }
        #endregion

        public static IEnumerable<CommandItem> CreateDefaultCommands(this AppModule module, Table table)
        {
            var dbsetting = CodeSetting.Current.DBSettings.Where(db => db.ProviderName.Equals(module.Connection.ProviderName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (dbsetting == null)
            {
                yield break;
            }

            SqlStringBuilder sqlbuilder = new SqlStringBuilder(dbsetting);
            foreach (var item in sqlbuilder.CreateDefaultCommands(table))
            {
                yield return item;
            }
        }
    }

    public class SqlStringBuilder
    {
        DBSetting Setting = null;

        public SqlStringBuilder(DBSetting setting)
        {
            Setting = setting;
        }

        public string GetName(string name)
        {
            return string.Format(Setting.SqlFieldNameFormat, name);
        }
        public string GetParameter(string name)
        {
            return string.Format(Setting.SqlParameterFormat, name);
        }

        private void AppendNames(StringBuilder sb, IEnumerable<string> names)
        {
            names.ForEachWithFirst((c) => { sb.AppendFormat("{0}", GetName(c)); },
                (c) => { sb.AppendFormat(",{0}", GetName(c)); });
        }
        private void AppendParameters(StringBuilder sb, IEnumerable<string> names)
        {
            names.ForEachWithFirst((c) => { sb.AppendFormat("{0}", GetParameter(c)); },
                (c) => { sb.AppendFormat(",{0}", GetParameter(c)); });
        }
        private void AppendNameParameters(StringBuilder sb, IEnumerable<string> names)
        {
            names.ForEachWithFirst(
                (c) => { sb.AppendFormat("{0}={1}", GetName(c), GetParameter(c)); },
                (c) => { sb.AppendFormat(",{0}={1}", GetName(c), GetParameter(c)); });
        }


        private IEnumerable<Column> GetColumns(Table table, IEnumerable<string> names)
        {
            foreach (var n in names)
            {
                var col = table.Columns.FirstOrDefault(c => c.Name.Equals(n, StringComparison.OrdinalIgnoreCase));
                yield return col;
            }
        }
        private Column GetColumn(Table table, string name)
        {
            var col = table.Columns.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return col;
        }

        private SqlParameter CreateParameter(Table table, string name)
        {
            var column = GetColumn(table, name);
            SqlParameter p = new SqlParameter { Name = name };
            if (column.IsSystem)
            {
                p.DbType = column.DbType;
                if (column.Length > 0) { p.Length = column.Length; }
                p.ValueType = ValueType.System;
                p.ValueName = column.SystemParameter;
            }
            else
            {
                p.DbType = column.DbType;
                p.Value = column.DefaultValue.Equals("string.empty", StringComparison.OrdinalIgnoreCase) ? string.Empty : column.DefaultValue;
                if (column.Length > 0) { p.Length = column.Length; }
                p.ValueType = ValueType.Property;
                p.ValueName = column.PropertyName;
            }
            return p;
        }
        private SqlParameter CreateDefaultParameter(string name)
        {
            SqlParameter p = new SqlParameter { Name = name };
            p.ValueType = ValueType.Property;
            p.DbType = DbType.String;
            p.ValueName = name;
            return p;
        }

        public IEnumerable<CommandItem> CreateDefaultCommands(Table table)
        {
            string columns = table.Columns.Select(c => c.Name).ToMergeString();
            string insertColumns = table.Columns.Where(c => c.IsAutoIncrement == false).Select(c => c.Name).ToMergeString();
            string updateColumns = table.Columns.Where(c => c.IsReadOnly == false).Select(c => c.Name).ToMergeString();
            StringBuilder sb = new StringBuilder();
            table.Columns.Where(c => c.IsKey).ForEachWithFirst(
                (c) => { sb.AppendFormat("{0}={1}", GetName(c.Name), GetParameter(c.Name)); },
                (c) => { sb.AppendFormat(" AND {0}={1}", GetName(c.Name), GetParameter(c.Name)); });
            string wherebykey = sb.ToString();

            yield return new CommandItem { Name = "Insert", CommandType = CommandType.Insert, Columns = insertColumns };
            yield return new CommandItem { Name = "Update", CommandType = CommandType.Update, Columns = updateColumns, Where = wherebykey };
            yield return new CommandItem { Name = "Delete", CommandType = CommandType.Delete, Where = wherebykey };
            yield return new CommandItem { Name = "SelectAll", CommandType = CommandType.Select, Columns = columns };
            yield return new CommandItem { Name = "SelectByKeys", CommandType = CommandType.Select, Columns = columns, Where = wherebykey };
        }

        public string CheckingExistsOrCreate(Table table)
        {
            return string.Empty;
        }

        public void CreateParameters(Table table, SqlCommand sqlcmd)
        {
            List<string> result = new List<string>();
            Regex paramReg = new Regex(@"(?<!@)[^\w$#@]@(?!@)[\w$#@]+");
            MatchCollection matches = paramReg.Matches(sqlcmd.CommandText);
            foreach (Match m in matches)
            {
                result.Add(m.Groups[0].Value.Substring(m.Groups[0].Value.IndexOf("@") + 1));
            }
            result.ForEach(r =>
            {
                if (sqlcmd.Parameters.FirstOrDefault(p => p.Name.Equals(r, StringComparison.OrdinalIgnoreCase)) == null)
                {
                    if (table.Columns.FirstOrDefault(f => f.Name.Equals(r, StringComparison.OrdinalIgnoreCase)) != null)
                    {
                        var p = CreateParameter(table, r);
                        sqlcmd.Parameters.Add(p);
                    }
                    else
                    {
                        var p = CreateDefaultParameter(r);
                        sqlcmd.Parameters.Add(p);
                    }
                }
            });
        }

        public void CreateCommandText(Table table, SqlCommand sqlcmd)
        {
            if (sqlcmd.CustomText.IsNullOrEmpty())
            {
                switch (sqlcmd.CommandType)
                {
                    case CommandType.Select:
                        sqlcmd.CommandText = CreateSelectText(table, sqlcmd);
                        break;
                    case CommandType.Insert:
                        sqlcmd.CommandText = CreateInsertText(table, sqlcmd);
                        break;
                    case CommandType.Delete:
                        sqlcmd.CommandText = CreateDeleteText(table, sqlcmd);
                        break;
                    case CommandType.Update:
                        sqlcmd.CommandText = CreateUpdateText(table, sqlcmd);
                        break;
                    case CommandType.Exists:
                        sqlcmd.CommandText = CreateExistsText(table, sqlcmd);
                        break;
                    case CommandType.Count:
                        sqlcmd.CommandText = CreateCountText(table, sqlcmd);
                        break;
                }
            }
            else
            {
                sqlcmd.CommandText = sqlcmd.CustomText;
            }
        }

        private string CreateExistsText(Table table, SqlCommand sqlcmd)
        {
            return string.Format("select exists(select * from {0} {1})", GetName(table.Name), sqlcmd.Where.Length > 0 ? string.Format("where {0}", sqlcmd.Where) : "");
        }

        private string CreateCountText(Table table, SqlCommand sqlcmd)
        {
            return string.Format("select count(*) from {0} {1}", GetName(table.Name), sqlcmd.Where.Length > 0 ? string.Format("where {0}", sqlcmd.Where) : "");
        }

        private string CreateSelectText(Table table, SqlCommand sqlcmd)
        {
            return CreateSelectText(table, sqlcmd.Columns, sqlcmd.Where, sqlcmd.OrderBy, sqlcmd.Limit, sqlcmd.Start);
        }

        private string CreateSelectText(Table table, string cols, string where, string orderby = "", int limit = 0, int start = 0)
        {
            StringBuilder sb = new StringBuilder();
            var columns = (cols.IsNullOrEmpty() || cols == "*") ?
                table.Columns.Select(c => c.Name).ToArray() : cols.Split(',');

            sb.Append(" SELECT ");

            if (limit > 0 && (!Setting.ProviderName.Equals("MySql.Data.MySqlClient", StringComparison.OrdinalIgnoreCase)))
            {
                if (limit > 0)
                {
                    sb.AppendFormat("top {0} ", limit);
                }
            }
            columns.ForEachWithFirst(
                (c) => { sb.AppendFormat("{0}", GetName(c)); },
                (c) => { sb.AppendFormat(",{0}", GetName(c)); });
            sb.AppendFormat(" FROM {0} ", GetName(table.Name));
            if (!where.IsNullOrEmpty())
            {
                sb.AppendFormat(" WHERE {0}", where);
            }
            if (!orderby.IsNullOrEmpty())
            {
                sb.AppendFormat(" ORDER BY {0}", orderby);
            }

            if (limit > 0 && Setting.ProviderName.Equals("MySql.Data.MySqlClient", StringComparison.OrdinalIgnoreCase))
            {
                sb.AppendFormat(" LIMIT {0},{1} ", start, limit);
            }

            return sb.ToString();
        }


        private string CreateInsertText(Table table, SqlCommand sqlcmd)
        {
            StringBuilder sb = new StringBuilder();
            var columns = (sqlcmd.Columns.IsNullOrEmpty() || sqlcmd.Columns == "*") ?
                table.Columns.Where(c => c.IsAutoIncrement == false).Select(c => c.Name).ToArray() : sqlcmd.Columns.Split(',');

            sb.AppendFormat("INSERT INTO {0}(", GetName(table.Name));
            AppendNames(sb, columns);
            sb.Append(")");
            sb.Append("VALUES(");
            AppendParameters(sb, columns);
            sb.Append(")");

            if (!sqlcmd.Where.IsNullOrEmpty())
            {
                sb.AppendFormat(" WHERE {0}", sqlcmd.Where);
            }
            if (sqlcmd.ReloadData)
            {
                sb.Append(";\r\n");
                sb.Append(CreateReloadInsert(table, sqlcmd));
            }
            return sb.ToString();
        }

        private string CreateDeleteText(Table table, SqlCommand sqlcmd)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" DELETE ");
            sb.AppendFormat(" FROM {0} ", GetName(table.Name));
            if (!sqlcmd.Where.IsNullOrEmpty())
            {
                sb.AppendFormat(" WHERE {0}", sqlcmd.Where);
            }
            return sb.ToString();
        }

        private string CreateUpdateText(Table table, SqlCommand sqlcmd)
        {
            var columns = (sqlcmd.Columns.IsNullOrEmpty() || sqlcmd.Columns == "*") ?
                table.Columns.Where(c => c.IsAutoIncrement == false && c.IsReadOnly == false).Select(c => c.Name).ToArray() : sqlcmd.Columns.Split(',');

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("UPDATE {0} SET ", GetName(table.Name));

            columns.ForEachWithFirst(
                (c) => { sb.AppendFormat("{0}={1}", GetName(c), GetParameter(c)); },
                (c) => { sb.AppendFormat(",{0}={1}", GetName(c), GetParameter(c)); });
            if (!sqlcmd.Where.IsNullOrEmpty())
            {
                sb.AppendFormat(" WHERE {0}", sqlcmd.Where);
            }

            if (sqlcmd.ReloadData)
            {
                sb.Append(";\r\n");
                if (sqlcmd.ReloaBykeys)
                {
                    sb.Append(CreateSelectByKeys(table));
                }
                else
                {
                    sb.Append(CreateSelect(table, sqlcmd.Where));
                }
            }

            return sb.ToString();
        }

        private string CreateReloadInsert(Table table, SqlCommand sqlcmd)
        {
            var col = table.Columns.FirstOrDefault(c => c.IsAutoIncrement);
            if (col != null)
            {
                return CreateSelectText(table, "*", string.Format(" {0}={1}", col.Name, Setting.SqlIdentityFunc));
            }
            else
            {
                return CreateSelectByKeys(table);
            }
        }

        public SqlCommand CreateSelect(string name, Table table, IEnumerable<string> columns, string where)
        {
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.Name = name;
            sqlcmd.CommandType = CommandType.Insert;

            StringBuilder sb = new StringBuilder();
            sb.Append(" SELECT ");
            columns.ForEachWithFirst(
                (c) => { sb.AppendFormat("{0}", GetName(c)); },
                (c) => { sb.AppendFormat(",{0}", GetName(c)); });
            sb.AppendFormat(" FROM {0} ", GetName(table.Name));
            sb.AppendFormat(" WHERE {0}", where);
            return sqlcmd;
        }

        public SqlCommand CreateDelete(string name, Table table, string where)
        {
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.Name = name;
            sqlcmd.CommandType = CommandType.Insert;

            StringBuilder sb = new StringBuilder();
            sb.Append(" DELETE ");
            sb.AppendFormat(" FROM {0} ", GetName(table.Name));
            sb.AppendFormat(" WHERE {0}", where);
            return sqlcmd;
        }

        public SqlCommand CreateInsert(string name, Table table, IEnumerable<string> columns)
        {
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.Name = name;
            sqlcmd.CommandType = CommandType.Insert;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("INSERT INTO {0}(", GetName(table.Name));
            AppendNames(sb, columns);
            sb.Append(")");
            sb.Append("VALUES(");
            AppendParameters(sb, columns);
            sb.Append(")");

            sqlcmd.CommandText = sb.ToString();
            columns.ForEach(c =>
            {
                SqlParameter p = CreateParameter(table, c);
                sqlcmd.Parameters.Add(p);
            });

            return sqlcmd;
        }

        public SqlCommand CreateUpdate(string name, Table table, IEnumerable<string> columns, string where)
        {
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.Name = name;
            sqlcmd.CommandType = CommandType.Update;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("UPDATE {0} SET ", GetName(table.Name));

            columns.ForEachWithFirst(
                (c) => { sb.AppendFormat("{0}={1}", GetName(c), GetParameter(c)); },
                (c) => { sb.AppendFormat(",{0}={1}", GetName(c), GetParameter(c)); });
            sb.AppendFormat(" WHERE {0}", where);

            columns.ForEach(c =>
            {
                SqlParameter p = CreateParameter(table, c);
                sqlcmd.Parameters.Add(p);
            });

            return sqlcmd;
        }


        public string CreateInsert(Table table)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("INSERT INTO {0}(", GetName(table.Name));
            AppendNames(sb, table.Columns.Select(s => s.Name));
            sb.Append(")");
            sb.Append("VALUES(");
            AppendParameters(sb, table.Columns.Select(s => s.Name));
            sb.Append(")");
            return sb.ToString();
        }

        public string CreateUpdate(Table table)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("UPDATE {0} SET ", GetName(table.Name));
            table.Columns.Where(c => !c.IsKey).ForEachWithFirst(
                (c) => { sb.AppendFormat("{0}={1}", GetName(c.Name), GetParameter(c.Name)); },
                (c) => { sb.AppendFormat(",{0}={1}", GetName(c.Name), GetParameter(c.Name)); });
            sb.Append(" WHERE ");
            table.Columns.Where(c => c.IsKey).ForEachWithFirst(
                (c) => { sb.AppendFormat("{0}={1}", GetName(c.Name), GetParameter(c.Name)); },
                (c) => { sb.AppendFormat(" AND {0}={1}", GetName(c.Name), GetParameter(c.Name)); });
            return sb.ToString();
        }
        public string CreateDelete(Table table)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("DELETE FROM {0} ", GetName(table.Name));
            sb.Append(" WHERE ");
            table.Columns.Where(c => c.IsKey).ForEachWithFirst(
                (c) => { sb.AppendFormat("{0}={1}", GetName(c.Name), GetParameter(c.Name)); },
                (c) => { sb.AppendFormat(" AND {0}={1}", GetName(c.Name), GetParameter(c.Name)); });
            return sb.ToString();
        }

        public string CreateSelect(Table table)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" SELECT ");
            table.Columns.ForEachWithFirst(
                (c) => { sb.AppendFormat("{0}", GetName(c.Name)); },
                (c) => { sb.AppendFormat(",{0}", GetName(c.Name)); });
            sb.AppendFormat(" FROM {0} ", GetName(table.Name));
            return sb.ToString();
        }

        public string CreateSelect(Table table, string where)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" SELECT ");
            table.Columns.ForEachWithFirst((c) => { sb.AppendFormat("{0}", GetName(c.Name)); }, (c) => { sb.AppendFormat(",{0}", GetName(c.Name)); });
            sb.AppendFormat(" FROM {0} ", GetName(table.Name));
            sb.Append(" WHERE ");
            sb.Append(where);

            return sb.ToString();
        }

        public string CreateSelectByKeys(Table table)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" SELECT ");
            table.Columns.ForEachWithFirst((c) => { sb.AppendFormat("{0}", GetName(c.Name)); }, (c) => { sb.AppendFormat(",{0}", GetName(c.Name)); });
            sb.AppendFormat(" FROM {0} ", GetName(table.Name));
            sb.Append(" WHERE ");
            table.Columns.Where(c => c.IsKey).ForEachWithFirst((c) => { sb.AppendFormat("{0}={1}", GetName(c.Name), GetParameter(c.Name)); }, (c) => { sb.AppendFormat(" AND {0}={1}", GetName(c.Name), GetParameter(c.Name)); });
            return sb.ToString();
        }
    }
}
