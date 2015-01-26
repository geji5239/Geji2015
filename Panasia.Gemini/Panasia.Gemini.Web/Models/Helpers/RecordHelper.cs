using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Panasia.Gemini.Web.Models.Helpers
{
    public class RecordVersionModelHelper
    {
        public static List<T> ExecuteDefaultSearchCmd<T>(this string tableName)
where T : RecordVersionModel, new()
        {
            SearchParameter parameter = new SearchParameter("", tableName, GetSearchDefaultMappingColumn(tableName), GetDefaultOrderBy(tableName));
            string searchConditon = parameter.GetSearchConditon();
            string cmdSql = string.Format("select top {0} * from (select ROW_NUMBER() over (order by {1}) as RowNumber,* from {2} where {4}) A where RowNumber > {3}", parameter.PageSize, GetOrderBy(parameter.OrderBy), parameter.TableName, (parameter.Page - 1) * parameter.PageSize, searchConditon);
            List<T> dataList = DataCenter.ExecuteRecords<T>(cmdSql, "SearchKey", "%" + parameter.SearchKey + "%");
            return dataList;
        }
    }
}