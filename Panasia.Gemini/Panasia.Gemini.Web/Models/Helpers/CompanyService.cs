using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Panasia.Gemini.Web.Models.Helpers;

namespace Panasia.Gemini.Web.Models.Helpers
{
    public class CompanyService
    {
        public static DataOperateResult<org_Company> SearchDefaultRecord(string tableName)
        {
            DataOperateResult<org_Company> defaultResult = new DataOperateResult<org_Company>(DBOperateType.Search);
            try
            {
                #region 拼接参数，执行查询操作
                List<org_Company> dataList = tableName.ExecuteDefaultSearchCmd<org_Company>();
                if (dataList == null || dataList.Count == 0)
                {
                    return DataOperateResult<org_Company>.NullRecordResult;
                }
                defaultResult.DataList = dataList;
                #endregion
                return defaultResult;
            }
            catch (Exception ex)
            {
                return new DataOperateResult<org_Company>(DBOperateType.Search, ex);
            }
        }
    }
}