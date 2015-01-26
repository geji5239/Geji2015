using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panasia.Gemini.Web.Models.Helpers
{
    /// <summary>
    /// 提供了对于数据操作前 检验数据完整性的基本方法
    /// </summary>
  public static class DBOperateCheck
    {
        /// <summary>
        /// 唯一性验证
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnGroups"></param>
        /// <returns></returns>
        public static CheckUniqueResult CheckUnique(int autoKey,string tableName, List<ColumnGroup> columnGroups)
        {
            CheckUniqueResult result = new CheckUniqueResult();
            foreach (ColumnGroup columnGroup in columnGroups)
            {
                string queryString = string.Format("select count(*) from {0} where AutoKey<>@AutoKey", tableName);
                List<object> queryparameters = new List<object>();
                queryparameters.Add("AutoKey");
                queryparameters.Add(autoKey);
                for (int i = 0; i < columnGroup.Columns.Count; i++)
			    {
			        var column = columnGroup.Columns[i];
                    string parameterName = "ColumnValue" + i.ToString();
                    queryString += string.Format(" and {0} = @{1}", column.ColumnName,parameterName);
                    queryparameters.Add(parameterName);
                    queryparameters.Add(column.ColumnValue);
			    }
                object count = DataCenter.ExecuteScalar(queryString, queryparameters.ToArray());
                if (count.ToInt32Value() > 0)
                {
                    result.HasError = true;
                    result.ColumnsGroup = columnGroup;
                }
            }
            return result;
        }

        /// <summary>
        /// 外键验证
        /// </summary>
        /// <param name="checkItems"></param>
        /// <returns></returns>
        public static CheckForeignKeyResult CheckForeignKeyExist(List<ForeignKeyCheckItem> checkItems)
        {
            CheckForeignKeyResult result = new CheckForeignKeyResult();
            foreach (ForeignKeyCheckItem checkItem in checkItems)
            {
                if (string.IsNullOrEmpty(checkItem.ColumnItem.ColumnValue.ToString()))
                {
                    continue;
                }
                string queryString = string.Format("select count(*) from {0} where {1} = @ColumnValue", checkItem.TableName,checkItem.TargetColumnName);
                object count = DataCenter.ExecuteScalar(queryString, "ColumnValue", checkItem.ColumnItem.ColumnValue);
                if (count.ToInt32Value() == 0)
                {
                    result.HasError = true;
                    result.ForeignKeyItem = checkItem;
                    return result;
                }
            }
            return result;
        }


        /// <summary>
        /// 外键验证
        /// </summary>
        /// <param name="checkItems"></param>
        /// <returns></returns>
        public static CheckDependencyResult CheckDeleteDependency(List<DependencyCheckItem> checkItems)
        {
            CheckDependencyResult result = new CheckDependencyResult();
            foreach (DependencyCheckItem checkItem in checkItems)
            {
                string queryString = string.Format("select count(*) from {0} where {1} = @ColumnValue", checkItem.TableName, checkItem.TargetColumnName);
                object count = DataCenter.ExecuteScalar(queryString, "ColumnValue", checkItem.ColumnItem.ColumnValue);
                if (count.ToInt32Value() > 0)
                {
                    result.HasError = true;
                    result.DependencyItem = checkItem;
                    return result;
                }
            }
            return result;
        }

        /// <summary>
        /// 数据是否更改验证
        /// </summary>
        /// <param name="rowID"></param>
        /// <param name="autoKey"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static CheckRecordChangedResult CheckRecordChanged(Guid rowID, int autoKey, string tableName)
        {
            CheckRecordChangedResult result = CheckRecordChangedResult.NotChanged;
            string queryString = string.Format("select RowID from {0} where AutoKey = @AutoKey", tableName);
            RecordItem record = DataCenter.ExecuteOneRecord<RecordItem>(queryString, "AutoKey", autoKey);
            if (record == null)
            {
                result = CheckRecordChangedResult.RecordNotFound;
            }
            else if (!string.Equals(record["RowID"].ToString(), rowID.ToString(),StringComparison.OrdinalIgnoreCase))
            {
                result = CheckRecordChangedResult.Changed;
            }
            return result;
        }

        public static List<ColumnNameValueItem> GetFields(this List<ColumnNameValueItem> columns)
        {
            return columns;
        }
    }



    public class ColumnGroup
    {
        public ColumnGroup()
        {
 
        }

        public ColumnGroup(params object[] paras)
        {
            for (int i = 0; i < paras.Count(); i++)
            {
                Columns.Add(new ColumnNameValueItem(paras[i].ToString()));
            }
        }

        private List<ColumnNameValueItem> _Columns = null;
        public List<ColumnNameValueItem> Columns
        {
            get 
            {
                if (_Columns == null)
                {
                    _Columns = new List<ColumnNameValueItem> ();
                }
                return _Columns;
            }
            set
            {
                _Columns = value;
            }
        }
    }

    public class ColumnNameValueItem
    {
        public ColumnNameValueItem()
        {
 
        }

        public ColumnNameValueItem(string columnName)
        {
            ColumnName = columnName;
        }

        public ColumnNameValueItem(string columnName, object columnValue)
        {
            ColumnName = columnName;
            ColumnValue = columnValue;
        }
        
        public string ColumnName { get; set; }

        public object ColumnValue { get; set; }
    }

    public class CheckUniqueResult
    {
        public bool HasError { get; set; }

        public ColumnGroup ColumnsGroup { get; set; }
    }

    public class ForeignKeyCheckItem
    {
        public ForeignKeyCheckItem()
        { 
        }

        public ForeignKeyCheckItem(string tableName, string columnName, string targetColumnName)
        {
            TableName = tableName;
            TargetColumnName = targetColumnName;
            ColumnItem = new ColumnNameValueItem(columnName);
        }

        public ForeignKeyCheckItem(string tableName, string columnName,string targetColumnName, object columnValue)
        {
            TableName = tableName;
            TargetColumnName = targetColumnName;
            ColumnItem = new ColumnNameValueItem(columnName, columnValue);
        }

        public string TableName { get; set; }
        public string TargetColumnName { get; set; }
        public ColumnNameValueItem ColumnItem { get; set; }
    }

    public class DependencyCheckItem
    {
        public DependencyCheckItem()
        { 
        }

        public DependencyCheckItem(string tableName,string columnName,string targetColumnName)
        {
            TableName = tableName;
            TargetColumnName = targetColumnName;
            ColumnItem = new ColumnNameValueItem(columnName);
        }

        public DependencyCheckItem(string tableName, string columnName, string targetColumnName, object columnValue)
        {
            TableName = tableName;
            TargetColumnName = targetColumnName;
            ColumnItem = new ColumnNameValueItem(columnName, columnValue);
        }

        public string TableName { get; set; }
        public string TargetColumnName { get; set; }
        public ColumnNameValueItem ColumnItem { get; set; }
    }

    public class CheckForeignKeyResult
    {
        public bool HasError { get; set; }

        public ForeignKeyCheckItem ForeignKeyItem { get; set; }
    }

    public class CheckDependencyResult
    {
        public bool HasError { get; set; }

        public DependencyCheckItem DependencyItem { get; set; }
    }

    public enum CheckRecordChangedResult
    {
        RecordNotFound,
        Changed,
        NotChanged
    }
}
