using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panasia.Gemini.Web.Models.Helpers
{
    public class SearchParameter
    {
        public SearchParameter()
        {
 
        }

        public SearchParameter(int currentPage)
        {
            Page = currentPage; 
        }

        public SearchParameter(int currentPage, string orderby, params string[] keyvalues)
        {
            Page = currentPage;
            OrderBy = new List<OrderByItem>();
            OrderBy.Add(new OrderByItem(orderby));
            SearchKeyValues = new List<ColumnNameValueItem>();
            if (keyvalues != null && keyvalues.Count() > 0)
            {
                for (int i = 0; i < keyvalues.Count(); i = i + 2)
                {
                    if (i + 1 < keyvalues.Count())
                    {
                        ColumnNameValueItem item = new ColumnNameValueItem(keyvalues[i], keyvalues[i + 1]);
                        SearchKeyValues.Add(item);
                    }

                }
            }
        }

        public SearchParameter(string searchKey, string tableName, List<string> mappingColumn, List<OrderByItem> orderby, int page = 1, int pageSize = -1)
        {
            if (pageSize != -1)
            {
                PageSize = pageSize;
            }
            SearchKey = searchKey;
            TableName = tableName;
            MappingColumns = mappingColumn;
            OrderBy = orderby;
            Page = page;
        }

        public string TableName { get; set; }
        
        public string SearchKey { get; set; }

        public List<string> MappingColumns { get; set; }

        public List<OrderByItem> OrderBy { get; set; }

        public List<ColumnNameValueItem> SearchKeyValues { get; set; }

        // 这个值从配置中取出
        private int _PageSize = SystemCodes.DefaultPageSize;
        public int PageSize
        {
            get { return _PageSize; }
            set { _PageSize = value; }
        }

        private int _Page = 1;
        public int Page
        {
            get { return _Page; }
            set { _Page = value; }
        }
    }

    public class FindParameter
    {
        public FindParameter()
        {
 
        }

        public FindParameter(string searchKey, List<string> mappingColumns,string tableName)
        {
            SearchKey = searchKey;
            MappingColumns = mappingColumns;
            TableName = tableName;
        }

        public string SearchKey { get; set; }

        public List<string> MappingColumns { get; set; }

        public string TableName { get; set; }
    }

    public class OrderByItem
    {
        public OrderByItem()
        {
 
        }

        public OrderByItem(string columnName, OrderBySequence sequence = OrderBySequence.Asc)
        {
            ColumnName = columnName;
            Sequence = sequence;
        }

        public string ColumnName { get; set; }

        private OrderBySequence _Sequence = OrderBySequence.Asc;

        public OrderBySequence Sequence
        {
            get { return _Sequence; }
            set { _Sequence = value; }
        }

    }

    public enum OrderBySequence
    {
        Asc,
        Desc
    }
}
