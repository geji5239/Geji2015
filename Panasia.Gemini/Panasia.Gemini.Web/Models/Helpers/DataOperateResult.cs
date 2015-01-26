using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Panasia.Gemini.Web.Models;

namespace Panasia.Gemini.Web.Models.Helpers
{
    public class DataOperateResult<T>
    {
        public DataOperateResult()
        {

        }

        public DataOperateResult(DBOperateType operationType)
        {
            OperateType = operationType;
        }

        public DataOperateResult(DBOperateType operationType, Exception ex)
        {
            OperateType = operationType;
            HasError = true;
            ErrorType = DBErrorType.UnKnowError;
            ExceptionMessage = ex.ToString();
          //  ex.AddLog("Error", 0);
        }

        public static readonly DataOperateResult<T> NullRecordResult = new DataOperateResult<T>() { HasError = true, ErrorType = DBErrorType.RecordNotFound };

        private T _DataItem = default(T);

        public T DataItem
        {
            get { return _DataItem; }
            set { _DataItem = value; }
        }

        private List<T> _DataList = null;

        public List<T> DataList
        {
            get { return _DataList; }
            set { _DataList = value; }
        }


        private bool _HasError = false;

        public bool HasError
        {
            get { return _HasError; }
            set { _HasError = value; }
        }

        private DBErrorType _ErrorType = DBErrorType.None;

        public DBErrorType ErrorType
        {
            get { return _ErrorType; }
            set { _ErrorType = value; }
        }

        private List<ColumnNameValueItem> _ErrorFields = null;

        public List<ColumnNameValueItem> ErrorFields
        {
            get
            {
                if (_ErrorFields == null)
                {
                    _ErrorFields = new List<ColumnNameValueItem>();
                }
                return _ErrorFields;
            }
            set { _ErrorFields = value; }
        }


        private DBOperateType _OperateType = DBOperateType.None;

        public DBOperateType OperateType
        {
            get { return _OperateType; }
            set { _OperateType = value; }
        }

        private string _ExceptionMessage = string.Empty;

        public string ExceptionMessage
        {
            get { return _ExceptionMessage; }
            set { _ExceptionMessage = value; }
        }

        public int _DataCount = -1;
        public int DataCount
        {
            get { return _DataCount; }
            set { _DataCount = value; }
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
           
    public enum DBErrorType
    {
        //没有错误
        None,
        //唯一键重复
        UniqueKeyDuplicate,
        //外键不存在
        ForeignKeyNotFound,
        //记录不存在
        RecordNotFound,
        //这笔记录在数据库中已更改 
        RecordChanged,
        //上一笔不存在 
        PreviousItemNotFound,
        //下一笔不存在
        NextItemNotFound,
        //第一笔不存在
        FirstItemNotFound,
        //最后一笔不存在
        LastItemNotFound,
        //查询结果为空
        SearchResultEmpty,
        // 删除外键依赖
        DeleteDependency,
        ItemClosed,
        UnKnowError
    }

    public enum DBOperateType
    {
        None,
        Add,
        Update,
        Delete,
        First,
        Previous,
        Next,
        Last,
        Search,
        ViewDetail,
        Find
    }
} 