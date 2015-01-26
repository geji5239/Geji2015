using System;

namespace Panasia.Gemini.FL.Data.Models
{
    public class PagerSet
    {
        public PagerSet()
        {
            _NeedColumn = "*";
            _ConditionString = "";
        }

        private string _SortExpression;
        private string _SortDirection;
        private int _PageCurr;
        private int _PageSize;
        private string _ConditionString;
        private string _TableOrViewName;
        private string _NeedColumn;

        private int _RecordCount;

        public string SortExpression
        {
            set { _SortExpression = value; }
            get { return _SortExpression; }
        }
        public string SortDirection
        {
            set { _SortDirection = value; }
            get { return _SortDirection; }
        }
        public int PageCurr
        {
            set { _PageCurr = value; }
            get { return _PageCurr; }
        }
        public int PageSize
        {
            set { _PageSize = value; }
            get { return _PageSize; }
        }
        public string ConditionString
        {
            set { _ConditionString = value; }
            get { return _ConditionString; }
        }
        public string TableOrViewName
        {
            set { _TableOrViewName = value; }
            get { return _TableOrViewName; }
        }
        public string NeedColumn
        {
            set { _NeedColumn = value; }
            get { return _NeedColumn; }
        }
        public int RecordCount
        {
            set { _RecordCount = value; }
            get { return _RecordCount; }
        }
    }
}
