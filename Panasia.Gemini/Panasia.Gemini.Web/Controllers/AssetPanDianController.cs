using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Panasia.Core.Auth;
using System.IO;
using Panasia.Gemini.Web.Models.Helpers;
using System.Data;
using Panasia.Core;
using System.Text;
using Panasia.Core.Web;
using System.Data.SqlClient;
using System.Configuration;
using Panasia.Gemini.Web.Models;
using Panasia.Core.Sys;

namespace Panasia.Gemini.Web.Controllers
{
    public class AssetPanDianController : Controller
    {
        //
        // GET: /Interview/FormCollection form
        [HttpPost]
        #region 导出text
        public ActionResult Export()
        {
            DataTable dt = GetData(Request);
            List<String[]> ls = new List<string[]>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string[] iteam = new string[5];
                iteam[0] = SubString(dt.Rows[i][0].ToString(), 20);
                iteam[1] = SubString(dt.Rows[i][1].ToString(), 40);
                iteam[2] = SubString(dt.Rows[i][2].ToString(), 30);
                iteam[3] = SubString(dt.Rows[i][3].ToString(), 20);
                iteam[4] = SubString(dt.Rows[i][4].ToString(), 20);
                ls.Add(iteam);
            }
            //string filepath = HttpRuntime.AppDomainAppPath.ToString() + @"AssetPanDian\alook.txt";
            //WriteCSV(filepath, false, ls);
            //var path = Server.MapPath("alook.txt");
            //return File(path, "application/x-txt-compressed");
            StringBuilder sb = new StringBuilder();
            foreach (String[] strArr in ls)
            {
                int i;
                for (i = 0; i < strArr.Length - 1; i++)
                {
                    sb.Append(strArr[i]);
                    sb.Append(",");
                }
                sb.Append(strArr[i]);
                sb.Append("\r\n");
            }
            byte[] data = Encoding.UTF8.GetBytes(sb.ToString());
            return File(data, "text/plain", "alook.txt");
        }
        public void voidExport()
        {
            DataTable dt = GetData(Request);
            List<String[]> ls = new List<string[]>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string[] iteam = new string[5];
                iteam[0] = SubString(dt.Rows[i][0].ToString(), 20);
                iteam[1] = SubString(dt.Rows[i][1].ToString(), 40);
                iteam[2] = SubString(dt.Rows[i][2].ToString(), 30);
                iteam[3] = SubString(dt.Rows[i][3].ToString(), 20);
                iteam[4] = SubString(dt.Rows[i][4].ToString(), 20);
                ls.Add(iteam);
            }
            ResponseTxt(ls);
        }
        DataTable GetData(HttpRequestBase re)
        {
            SqlHelpers hp = new SqlHelpers();
            try
            {
                string sql = @"SELECT a.[BarCard]
                      ,a.[AssetName]
                      ,a.[SpecificationsModel]
	                  ,IIF(isnull(e.[Name],'')='',IIF(isnull(d.[Name],'')='','无',d.[Name]),e.[Name])as [EName]
	                  ,IIF(isnull(e.[Name],'')='',IIF(isnull(d.[Name],'')='',IIF(isnull(wh.[WHName],'')='','无',wh.[WHName]),d.[Name]),ed.[Name])as [DName]
                      FROM [dbo].[zc_Asset]a
                      left join [zc_INVWarehouse]wh on a.[WhCd]=wh.[WHCd]
                      left join [hr_Department]d on a.[DeptID]=d.[DeptID]
                      left join [hr_Employee]e on a.[EmployeeID]=e.[EmployeeID]
                      left join [hr_Department]ed on e.[DeptID]=ed.[DeptID] where ";
                if (re.Form.Count > 0)
                {
                    string BuyDateBegin = string.IsNullOrEmpty(re.Form["BuyDateBegin"]) ? "1900-01-01" : re.Form["BuyDateBegin"];
                    string BuyDateEnd = string.IsNullOrEmpty(re.Form["BuyDateEnd"]) ? "2100-01-01" : re.Form["BuyDateEnd"];
                    sql += @" a.[BarCard] like '%" + re.Form["BarCard"] + @"%'
                          and a.[AssetName] like '%" + re.Form["AssetName"] + @"%'
                          and a.[CategoryID] like '%" + re.Form["CategoryID"] + @"%'
                          and a.[AddType] like '%" + re.Form["AddType"] + @"%'
                          and a.[CompanyID] like '%" + re.Form["CompanyID"] + @"%'
                          and a.[DeptID] like '%" + re.Form["DeptID"] + @"%'
                          and a.[EmployeeID] like '%" + re.Form["EmployeeID"] + @"%'
                          and a.[WhCd] like '%" + re.Form["WhCd"] + @"%'
                          and a.[SupplierID] like '%" + re.Form["SupplierID"] + @"%'
                          and a.[UseCondition] like '%" + re.Form["UseCondition"] + @"%'
                          and IIF(a.[BuyDate]='','1900-01-01',a.[BuyDate]) between '" + BuyDateBegin + "' and '" + BuyDateEnd + "' and ";
                }
                sql += " a.[IsActive]=1";
                DataTable dt = hp.SelectReturnDataTable(sql);
                return dt;
            }
            catch (Exception)
            {
                return null;
            }
        }
        void WriteCSV(string filePathName, bool append, List<String[]> ls)
        {
            try
            {
                StreamWriter fileWriter = new StreamWriter(filePathName, append, Encoding.Default);
                foreach (String[] strArr in ls)
                {
                    fileWriter.WriteLine(String.Join(",", strArr));//这里换成你实际的分隔符
                }
                fileWriter.Flush();
                fileWriter.Close();
            }
            catch (Exception)
            {
            }
        }
        /// <summary>
        /// 取指定长度的字符串
        /// </summary>
        /// <param name="Source">被格式化字符串</param>
        /// <param name="Length">截取长度</param>
        /// <returns></returns>
        private string SubString(string Source, int Length)
        {
            string result = string.Empty;

            Byte[] tmpByteArr;
            tmpByteArr = System.Text.Encoding.Default.GetBytes(Source);

            if (Length >= tmpByteArr.Length)
            {
                result = Source;

                result = result + new string(Convert.ToChar(" "), Length - tmpByteArr.Length);
            }
            else
            {
                int temp = 0;

                for (int i = 0; i < tmpByteArr.Length; i++)
                {
                    if (tmpByteArr[i] > 127)
                    {
                        temp = temp + 1;
                    }
                }

                if (temp % 2 == 1)
                {
                    result = System.Text.Encoding.Default.GetString(tmpByteArr, 0, Length - 1) + "  ";
                }
                else
                {
                    result = System.Text.Encoding.Default.GetString(tmpByteArr, 0, Length);
                }
            }
            if (Length > result.Length)
            {
                int k = 0;
            }
            tmpByteArr = System.Text.Encoding.Default.GetBytes(result);

            result = System.Text.Encoding.Default.GetString(tmpByteArr, 0, Length);
            return result;
        }
        void ResponseTxt(List<String[]> ls)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                foreach (String[] strArr in ls)
                {
                    int i;
                    for (i = 0; i < strArr.Length - 1; i++)
                    {
                        sb.Append(strArr[i]);
                        sb.Append(",");
                    }
                    sb.Append(strArr[i]);
                    sb.Append("\r\n");
                }
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "GB2312";
                Response.AppendHeader("Content-Disposition", "attachment;filename=alook.txt");
                Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");//设置输出流为简体中文
                Response.ContentType = "text/plain";//设置输出文件类型为txt文件。 
                System.Globalization.CultureInfo myCItrad = new System.Globalization.CultureInfo("ZH-CN", true);
                System.IO.StringWriter oStringWriter = new System.IO.StringWriter(myCItrad);
                Response.Write(sb.ToString());
                Response.End();
            }
            catch (Exception)
            {
            }
        }
        #endregion
        #region 上传

        public ActionResult Upload(FormCollection form)
        {
            HttpRequestBase request = Request;                                    //上传文件夹
            string[] fileTypes = new[] { "txt" }; //文件格式设定

            int iTotal = request.Files.Count;
            if (iTotal <= 1)
            {
                return Json(ResultData.CreateError("缺少对比文档"));
            }
            else
            {
                HttpPostedFileBase fileys = Request.Files[0];
                HttpPostedFileBase filepd = Request.Files[1];
                string fileTypeys = System.IO.Path.GetExtension(fileys.FileName).Substring(1).ToLower(); //扩展名（包含‘.’）
                string fileTypepd = System.IO.Path.GetExtension(filepd.FileName).Substring(1).ToLower(); //扩展名（包含‘.’）
                if (!fileTypes.Contains(fileTypeys))
                {
                    return Json(ResultData.CreateError("原始文档类型错误"), "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
                }
                else if (fileys.ContentLength > 1024 * 1024 * 2)
                {
                    return Json(ResultData.CreateError("原始文档大小不得超过2M"), "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
                }
                else if (!fileTypes.Contains(fileTypepd))
                {
                    return Json(ResultData.CreateError("盘点文档类型错误"), "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
                }
                else if (filepd.ContentLength > 1024 * 1024 * 2)
                {
                    return Json(ResultData.CreateError("盘点文档大小不得超过2M"), "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string filepathys = saveFilePath(fileys);
                    string filepathpd = saveFilePath(filepd);
                    string ysIDs = getIDfromTXT(filepathys);
                    string pdIDs = getIDfromTXT(filepathpd);
                    string pkcountstr = @"select count(y.Value) from [dbo].[fSplitString]('{0}',',',1)y left join  [dbo].[fSplitString]('{1}',',',1)p on y.Value=p.Value ";
                    string sqlstr = @"SELECT distinct
                                    p.Value
                                    ,a.[AssetID]
                                    ,a.[AssetName]
                                    ,a.[CategoryID]
                                    ,a.[SpecificationsModel]
                                    ,case a.[AddType] when 10 then '购买' when 20 then '借用'  when 90 then '其它' end as [AddType]
                                    ,a.[WhCd]
                                    ,a.[LOCNO]
                                    ,a.[UnitID]
                                    ,a.[BarCard]
                                    ,a.[SupplierID]
                                    ,a.[BuyDate]
                                    ,a.[UseYear]
                                    ,(case a.UseCondition when 'RK'then'入库' when 'LY'then'领用'when 'JY'then'借用'when 'GH'then'归还'when 'DB'then'调拨'when 'BX'then'报修'when 'BF'then'报废'when 'QL'then'清理'when 'QT'then'其他' end)as UseCondition
                                    ,a.[CompanyID]
                                    ,a.[DeptID]
                                    ,a.[EmployeeID]
                                    ,a.[QualityDescription]
                                    ,convert(varchar, a.[Price],1) as [Price]
                                    ,a.[Remark]
                                    ,a.[Status]
                                    ,ca.[Category]
                                    ,wh.[WHName]
                                    ,u.[UnitName]
                                    ,s.[SupplierName]
                                    ,ass.[Area]+'('+ass.[AreaCode]+')' as [CName]
                                    ,d.[Name]as [DName]
                                    ,e.[Name]as [EName]
                                    from [dbo].[fSplitString]('{0}',',',1)y 
                                    left join [dbo].[fSplitString]('{1}',',',1)p on y.Value=p.Value 
                                    left join [dbo].[zc_Asset]a on y.Value=a.BarCard
                                    left join [zc_Category]ca on a.[CategoryID]=ca.[CategoryID]
                                    left join [zc_INVWarehouse]wh on a.[WhCd]=wh.[WHCd]
                                    left join [zc_Unit]u on a.[UnitID]=u.[UnitID]
                                    left join [zc_Supplier]s on a.[SupplierID]=s.[SupplierID]
                                    left join [zc_Area]ass on a.[CompanyID]=ass.[CompanyID]
                                    left join [hr_Department]d on a.[DeptID]=d.[DeptID]
                                    left join [hr_Employee]e on a.[EmployeeID]=e.[EmployeeID]
                                    order by p.Value";
                    PanDianData = SqlHelpers.SelectReturnDataSet(string.Format(sqlstr, ysIDs, pdIDs)).Tables[0];
                    string countAll = SqlHelpers.SelectReturnStr(string.Format(pkcountstr, ysIDs, pdIDs), 0, 0);
                    string countPanKui = SqlHelpers.SelectReturnStr(string.Format(pkcountstr+" where p.Id is null", ysIDs, pdIDs), 0, 0);
                    int[] count=new int[2];
                    count[0] = string.IsNullOrEmpty(countAll) ? 0 : int.Parse(countAll);
                    count[1] = string.IsNullOrEmpty(countPanKui) ? 0 : int.Parse(countPanKui);
                    return Json(ResultData.Create(count), "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);//返回相对路径
                }
            }
        }
        static DataTable PanDianData = new DataTable();
        public string PanDianTable()
        {
            return PanDianData.ToJson();
        }
        string saveFilePath(HttpPostedFileBase file) 
        {
            string document = @"\Upload\PanDian";   
            string fileName = file.FileName.Substring(file.FileName.LastIndexOf("\\") + 1);//原文件名（包含扩展名）
            string filePath = string.Format(HttpRuntime.AppDomainAppPath.ToString() + document);//绝对路径文件夹
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            string relativePath = string.Format("{0}\\{1}", document, fileName);//相对路径
            string Path = string.Format("{0}\\{1}", filePath, fileName);//绝对路径
            file.SaveAs(Path);//保存文件到绝对路径
            return Path;
        }
        string getIDfromTXT(string filepath)
        {
            try
            {
                using (StreamReader sr = new StreamReader(filepath, Encoding.Default))
                {
                    StringBuilder ids = new StringBuilder();
                    StringBuilder idsS = new StringBuilder();
                    string line = sr.ReadToEnd();
                    if (line != null)
                    {
                        string[] rows = line.Split(new char[] { '\r', '\n' });
                        for (int i = 0; i < rows.Length; i++)
                        {
                            string ID = rows[i].Split(',')[0].Trim();
                            if (!string.IsNullOrEmpty(ID))
                            {
                                ids.Append(ID);
                                ids.Append(",");
                            }
                        }
                    }
                    return ids.ToString();
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        #endregion
        #region 导出至Excel
        public void ExportPanDian()
        {
            DataTable dt= PanDianData;
            var isValidate = string.Empty;
            var keys = Request.Form.AllKeys;
            foreach (var key in keys)
            {
                isValidate = Request.Form[key];
            }
            List<UserModel> list = SysService.GetUsers("", Boolean.Parse(isValidate), "", "");
            StringBuilder sb = new StringBuilder();
            sb.Append("<style type=\"text/css\">");
            sb.Append("<!--");
            sb.Append(".text");
            sb.Append("{mso-style-parent:style0;");
            sb.Append("font-size:10.0pt;");
            sb.Append("font-family:\"Arial Unicode MS\", sans-serif;");
            sb.Append("mso-font-charset:0;");
            sb.Append(@"mso-number-format:\@;");
            sb.Append("text-align:center;");
            sb.Append("border:.5pt solid black;");
            sb.Append("white-space:normal;}");
            sb.Append("-->");
            sb.Append("</style>");
            sb.Append("<table cellspacing=\"0\" rules=\"all\" border=\"1\" style=\"border-collapse:collapse;\">");
            sb.Append("<tr align=\"Center\" style=\"font-weight:bold;\">");
            sb.Append("<td>编号</td>");
            sb.Append("<td>资产名称</td>");
            sb.Append("<td>资产类别</td>");
            sb.Append("<td>条码编号</td>");
            sb.Append("<td>规格型号</td>");
            sb.Append("<td>单位</td>");
            sb.Append("<td>增加方式</td>");
            sb.Append("<td>所属公司</td>");
            sb.Append("<td>所属部门</td>");
            sb.Append("<td>使用员工</td>");
            sb.Append("<td>仓库名称</td>");
            sb.Append("<td>购入价格</td>");
            sb.Append("<td>购置时间</td>");
            sb.Append("<td>供应商</td>");
            sb.Append("<td>质保说明</td>");
            sb.Append("<td>使用年限</td>");
            sb.Append("<td>使用情况</td>");
            sb.Append("<td>备注</td>");
            sb.Append("</tr>");
            foreach (DataRow dr in dt.Rows)
            {
                //string value = user.IsValid ? "是" : "否";
                sb.Append("<tr align=\"Center\"><td>" + dr["AssetID"] + "</td>");
                sb.Append("<td>" + dr["AssetName"] + "</td>");
                sb.Append("<td>" + dr["Category"] + "</td>");
                sb.Append("<td>" + dr["BarCard"] + "</td>");
                sb.Append("<td>" + dr["SpecificationsModel"] + "</td>");
                sb.Append("<td>" + dr["UnitName"] + "</td>");
                sb.Append("<td>" + dr["AddType"] + "</td>");
                sb.Append("<td>" + dr["CName"] + "</td>");
                sb.Append("<td>" + dr["DName"] + "</td>");
                sb.Append("<td>" + dr["EName"] + "</td>");
                sb.Append("<td>" + dr["WHName"] + "</td>");
                sb.Append("<td>" + dr["Price"] + "</td>");
                sb.Append("<td>" + dr["BuyDate"] + "</td>");
                sb.Append("<td>" + dr["SupplierName"] + "</td>");
                sb.Append("<td>" + dr["QualityDescription"] + "</td>");
                sb.Append("<td>" + dr["UseYear"] + "</td>");
                sb.Append("<td>" + dr["UseCondition"] + "</td>");
                sb.Append("<td>" + dr["Remark"] + "</td></tr>");
            }
            sb.Append("</table>");
            HttpResponseBase response = HttpContext.Response;
            response.Clear();
            response.ContentType = "application/ms-excel";
            if (HttpContext.Request.Browser.Type.StartsWith("Inter"))
            {
                response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode("固定资产盘点.xls", Encoding.GetEncoding("UTF-8")));
            }
            response.AppendHeader("Content-Disposition", "attachment;filename=" + "固定资产盘点.xls");
            response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            response.AddHeader("Pragma", "public");
            response.Write(sb.ToString());
            response.End();
            HttpContext.ApplicationInstance.CompleteRequest();
        }
        #endregion
    }
}
