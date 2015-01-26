using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Panasia.Gemini.Web.Models.Helpers;

namespace Panasia.Gemini.Web.Models.Helpers
{
    public class RecordModel:RecordItem
    {
        public RecordModel() : base() { }

        public RecordModel(params string[] names) : base(names) { }        
    }

    public class RecordVersionModel : RecordModel
    {
        public RecordVersionModel() : base() { }

        public RecordVersionModel(params string[] names)
            : base(names.Union(new string[] { "RowLockState", "CreatedUser", "CreatedDate", "ModifiedUser", "ModifiedDate", "RowID", "AutoKey", "RecordVersion" }).ToArray())
        {

        }
                
        #region SetFieldValue
        protected override void SetFieldValue(string name, object value)
        {
            object newValue = null;
            switch (name.ToLower())
            {
                case "rowlockstate":
                    newValue = value.ToValue("int");
                    break;
                case "createduser":
                    newValue = value.ToValue("int");
                    break;
                case "createddate":
                    newValue = value.ToValue("DateTime");
                    break;
                case "modifieduser":
                    newValue = value.ToValue("int");
                    break;
                case "modifieddate":
                    newValue = value.ToValue("DateTime");
                    break;
                case "rowid":
                    newValue = value.ToValue("Guid");
                    break;
                case "autokey":
                    newValue = value.ToValue("int");
                    break;
                case "recordversion":
                    newValue = value.ToValue("string");
                    break;
                default:
                    newValue = value;
                    break;
            }
            base.SetFieldValue(name, newValue);
        }
        #endregion

        #region property RowLockState
        public int RowLockState
        {
            get
            {
                return (int)base["RowLockState"];
            }
            set
            {
                base["RowLockState"] = value;
            }
        }
        #endregion //property RowLockState

        #region property CreatedUser
        public int CreatedUser
        {
            get
            {
                return (int)base["CreatedUser"];
            }
            set
            {
                base["CreatedUser"] = value;
            }
        }
        #endregion //property CreatedUser

        #region property CreatedDate
        public DateTime CreatedDate
        {
            get
            {
                return (DateTime)base["CreatedDate"];
            }
            set
            {
                base["CreatedDate"] = value;
            }
        }
        #endregion //property CreatedDate

        #region property ModifiedUser
        public int ModifiedUser
        {
            get
            {
                return (int)base["ModifiedUser"];
            }
            set
            {
                base["ModifiedUser"] = value;
            }
        }
        #endregion //property ModifiedUser

        #region property ModifiedDate
        public DateTime ModifiedDate
        {
            get
            {
                return (DateTime)base["ModifiedDate"];
            }
            set
            {
                base["ModifiedDate"] = value;
            }
        }
        #endregion //property ModifiedDate

        #region property RowID
        public Guid RowID
        {
            get
            {
                return (Guid)base["RowID"];
            }
            set
            {
                base["RowID"] = value;
            }
        }
        #endregion //property RowID

        #region property AutoKey
        public int AutoKey
        {
            get
            {
                return (int)base["AutoKey"];
            }
            set
            {
                base["AutoKey"] = value;
            }
        }
        #endregion //property AutoKey

        #region property RecordVersion
        public string RecordVersion
        {
            get
            {
                return (string)base["RecordVersion"];
            }
            set
            {
                base["RecordVersion"] = value;
            }
        }
        #endregion //property RecordVersion

        private List<ColumnGroup> _GroupColumns;

        public List<ColumnGroup> GroupColumns
        {
            get { return _GroupColumns; }
            set { _GroupColumns = value; }
        }

        public List<ColumnGroup> GetGroupColumns()
        {
            List<ColumnGroup> groupClns = new List<ColumnGroup>();
            foreach (var groupCln in GroupColumns)
            {
                ColumnGroup gr = new ColumnGroup();
                foreach (var column in groupCln.Columns)
                {
                    gr.Columns.Add(new ColumnNameValueItem(column.ColumnName, this[column.ColumnName]));
                }
                groupClns.Add(gr);
            }
            return groupClns;
        }

        public List<ForeignKeyCheckItem> GetForeignKeys()
        {
            List<ForeignKeyCheckItem> fks = new List<ForeignKeyCheckItem>();
            foreach (var fk in FKeyItems)
            {
                ForeignKeyCheckItem fckItem = new ForeignKeyCheckItem(fk.TableName, fk.ColumnItem.ColumnName,fk.TargetColumnName, this[fk.ColumnItem.ColumnName]);
                fks.Add(fckItem);
            }
            return fks;
        }

        public List<DependencyCheckItem> GetDepencys()
        {
            List<DependencyCheckItem> dps = new List<DependencyCheckItem>();
            foreach (var dp in DependencyItems)
            {
                DependencyCheckItem dpItem = new DependencyCheckItem(dp.TableName, dp.ColumnItem.ColumnName, dp.TargetColumnName, this[dp.ColumnItem.ColumnName]);
                dps.Add(dpItem);
            }
            return dps;
        }

        private string _TableName;

        public string TableName
        {
            get { return _TableName; }
            set { _TableName = value; } 
        }

        private List<ForeignKeyCheckItem> _FKeyItems = null;
        public List<ForeignKeyCheckItem> FKeyItems
        {
            get { return _FKeyItems; }
            set { _FKeyItems = value; }
        }

        private List<DependencyCheckItem> _DependencyItems = null;
        public List<DependencyCheckItem> DependencyItems
        {
            get { return _DependencyItems; }
            set { _DependencyItems = value; }
        }

        private List<string> _InsertColumnNames = null;
        public List<string> InsertColumnNames
        {
            get { return _InsertColumnNames; }
            set { _InsertColumnNames = value; }
        }

        private List<string> _UpdateColumnNames = null;
        public List<string> UpdateColumnNames
        {
            get { return _UpdateColumnNames; }
            set { _UpdateColumnNames = value; }
        }

        public List<OrderByItem> _DefaultOrderBy = null;
        public List<OrderByItem> DefaultOrderBy
        {
            get { return _DefaultOrderBy; }
            set { _DefaultOrderBy = value; }
        }

        public List<string> _SortColumnNames = null;
        public List<string> SortColumnNames
        {
            get { return _SortColumnNames; }
            set { _SortColumnNames = value; }
        }

        public List<string> _SearchColumnNames = null;
        public List<string> SearchColumnNames
        {
            get { return _SearchColumnNames; }
            set { _SearchColumnNames = value; }
        }

        public List<string> _FindColumnNames = null;
        public List<string> FindColumnNames
        {
            get { return _FindColumnNames; }
            set { _FindColumnNames = value; }
        }
    }

    public static class RecordHelper
    {
        public static object ToValue(this object value, string typeName)
        {
            switch (typeName.ToLower())            
            { 
                case "int":
                    return ToInt32Value(value); 
                case "long":
                    return ToInt64Value(value);  
                case "string":
                    return ToStringValue(value);  
                case "double":
                    return ToDoubleValue(value);
                case "decimal":
                    return ToDecimalValue(value); 
                case "datetime":
                    return ToDateTimeValue(value);   
                case "bool":
                    return ToBoolValue(value);    
                case "guid":
                    return ToGuidValue(value);
                default:
                    return value;
            }
        }

        public static void Update(this RecordModel oldItem, RecordItem newItem)
        { 
            foreach(var f in newItem.Fields)
            {
                oldItem[f.Name] = f.Value;
            }
        }

        public static bool ToBoolValue(this object value)
        {
            return value == null ? false : ((value is bool) ? (bool)value : Convert.ToBoolean(value)); 
        }

        public static Guid ToGuidValue(this object value)
        {
            return value == null ? Guid.Empty : new Guid(value.ToString());
        }

        public static long ToInt64Value(this object value)
        {
            long result = 0;
            if (value is byte[])
            {  
                var vv=(byte[])value;
                if (vv.Length > 0)
                {
                    long p = 1;
                    for (int i = vv.Length - 1; i >= 0; i--)
                    {
                        result += vv[i] * p;
                        p = p * 256;
                    }
                }
                return result;
            }
            return value == null ? 0 : ((value is Int64) ? (Int64)value : Convert.ToInt64(value));
        }
        public static int ToInt32Value(this object value)
        {
            return value == null ? 0 : ((value is Int32) ? (Int32)value : Convert.ToInt32(value));
        }

        public static void CopyField(this RecordVersionModel recordTarget,RecordItem recordFrom)
        {
            foreach (var field in recordFrom.Fields)
            {
                recordTarget.AddField(field);
            }
        }

        public static string ToStringValue(this object value)
        {
            return value == null ? string.Empty : ((value is string) ? (string)value : value.ToString());
        }

        public static double ToDoubleValue(this object value)
        {
            return value == null ? 0 : ((value is double) ? (double)value : Convert.ToDouble(value));
        }

        public static decimal ToDecimalValue(this object value)
        {
            return value == null ? 0 : ((value is decimal) ? (decimal)value : Convert.ToDecimal(value));
        }

        public static DateTime ToDateTimeValue(this object value)
        {
            return value == null ? DateHelper.MinDate : ((value is DateTime) ? (DateTime)value : Convert.ToDateTime(value));
        }
    }
}
