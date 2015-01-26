using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Maticsoft.DBUtility;

namespace Panasia.Gemini.FL.Data.Common
{
    public class Pager
    {
        public Pager()
        { }
        #region 执行分页操作

        public DataTable ExecutePager(Panasia.Gemini.FL.Data.Models.PagerSet Info)
        {
            string TotalCountSql = "select count(*) from " + Info.TableOrViewName + " " + Info.ConditionString;

            int start = (Info.PageCurr - 1) * Info.PageSize + 1;
            int end = start + Info.PageSize - 1;
            string sql = "(select row_number() over (order by " + Info.SortExpression + " " + Info.SortDirection + ") as rowId," + Info.NeedColumn + " from " + Info.TableOrViewName + " " + Info.ConditionString;
            sql = "select * from " + sql + ") as t where rowId between " + start.ToString() + " and " + end.ToString();

            try
            {
                DataTable Result = DbHelperSQL.Query(sql).Tables[0];
                Info.RecordCount = Convert.ToInt32(DbHelperSQL.GetSingle(TotalCountSql));
                return Result;
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
