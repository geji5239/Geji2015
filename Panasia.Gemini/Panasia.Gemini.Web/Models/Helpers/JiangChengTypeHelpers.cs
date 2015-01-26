using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data.Entity;
using System.Web.Security;
using System.Web.Mvc;
using System.Configuration;
using Excel;
using System.IO;
using Panasia.Core.Auth;

namespace Panasia.Gemini.Web.Models.Helpers
{
    public class JiangChengTypeHelpers
    {
        ResultData result = new ResultData();
        void GetError(string errorMsg) 
        {
            result.HasError = true;
            result.ErrorMessage = errorMsg;
        }
        public ResultData ImportResult(string filePath, string s)
        {
            ImportFromExcel(filePath, s);
            return result;
        }

        static SqlHelpers hb = new SqlHelpers();
        string conString = ConfigurationManager.ConnectionStrings["PASys"].ConnectionString;
        string UserID = Panasia.Core.Sys.SysService.GetCurrentUser().UserID;
     

        //导入
        public bool ImportFromExcel(string filePath, string s)
        {

            try
            {
                DataSet dsRead = ReadExlToDt(filePath);//读出EXCEL所有表数据
                //#region 检测excel表内容是否正确
                //if (dsRead.Tables.Count-1!= dsRead.Tables[0].Rows.Count)
                //{
                //    GetError( "文件数据不正确");
                //    return false;
                //}
                //#endregion
                DataTable dtRead = dsRead.Tables[0];//取主表
                dtRead.Columns.Add("JCID").SetOrdinal(0);//首列插入系列号
                DataTable dtMerge = new DataTable();//存放子表信息

                dtRead = AddInfo(dtRead,s);//添加导入信息
                string[] strsInsert = DtToStrings(dtRead,s);//生成SQL执行语句
                if (string.IsNullOrEmpty(hb.ExecSqlByTran(strsInsert)))
                {
                    result.HasError = false;
                    result.Data = dtRead;
                    return true;//导入成功 
                }
                GetError("将截断字符串或二进制数据");
                return false;
            }
            catch (Exception ex)
            {
                GetError(ex.Message);
                return false;
            }
        }
        //根据路径filePath，读Excel数据，写入表table
        public DataSet ReadExlToDt(string filePath)
        {
            try
            {
                FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
                IExcelDataReader excelReader;
                if (filePath.EndsWith(".xls", StringComparison.OrdinalIgnoreCase))
                {

                    // Reading from a binary Excel file ('97-2003 format; *.xls)
                    excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else
                {
                    // Reading from a OpenXml Excel file (2007 format; *.xlsx)
                    excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                excelReader.IsFirstRowAsColumnNames = true;
                DataSet ds = excelReader.AsDataSet();
                excelReader.Close();
                return ds;
            }
            catch (Exception ex)
            {
                GetError(ex.Message);
                return null;
            }
        }

         //生成SQL执行语句
         string[] DtToStrings(DataTable dt,string s)
        {
            string[] strSql = new string[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //判断条例是否已经存在
                string strsel = string.Format("select * from hr_JiaChengTiaoLi where JCName='{0}' and JCID='{1}'", dt.Rows[i][1].ToString(),s);
                if (!hb.SelectResultZero(strsel) || dt.Rows[i][1].ToString().Trim()=="")
                {
                    strSql[i] = "";
                    continue;
                }
                string str = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (j != 0)
                    {
                        str += ",";
                    }
                    str += "'" + dt.Rows[i][j].ToString().Replace("'", "\"") + "'";
                }
                string dtime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                strSql[i] = "insert into hr_JiaChengTiaoLi values(" + str + ",1,'" + UserID + "','" + dtime + "','" + UserID + "','" + dtime + "')";
            }
            return strSql;
        }
         //添加导入信息
         DataTable AddInfo(DataTable Table,string s)
        {
            try
            {
                for (int i = 0; i < Table.Rows.Count; i++)
                {
                    Table.Rows[i][0] = s;
                    //Table.Rows[i][5] = Table.Rows[i][7]  = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                    //Table.Rows[i][4] = Table.Rows[i][6] = UserID;
                }
                return Table;
            }
            catch (Exception ex)
            {
                GetError(ex.Message);
                return Table;
            }
        }
        //转置表结构，整理表内容
         DataTable TableTrans(DataTable Table, string S)
        {
            try
            {
                DataTable table = DelNullCol(Table);// 此table 只包含两列
                DataTable dt = new DataTable();

                int n = table.Rows.Count; //表中数据行数
                int j = 1;
                string[] header1 = new string[n];
                string[] body1 = new string[n];
                string[] header2 = new string[n];
                string[] body2 = new string[n];

                //把每一列转换成字符串数组1
                for (int i = 0; i < n; i++)
                {
                    header1[i] = table.Rows[i][0].ToString();
                    header1[i] = header1[i].ToString().Replace("：", " ").Trim();
                    header1[i] = header1[i].ToString().Replace(":", " ").Trim();
                    body1[i] = table.Rows[i][1].ToString();
                }

                //取字符串数组1中第一个不为空的列名和内容，放入字符串数组2
                for (int i = 0; i < n; i++)
                {
                    if (header1[i].ToString().Trim() == "")
                        continue;
                    header2[0] = header1[i].ToString();
                    body2[0] = body1[i].ToString();
                    break;
                }
                int flag = 0;
                //去除数组1中列名为空或重名的列，并整合列内容到2
                for (int i = 1; i < n; i++)
                {
                    if (S == "z" && header1[i] == "项目经验")
                        flag = 1;
                    if (S == "z" && header1[i] == "培训经历")
                        flag = 2;
                    if (flag == 1)
                    {
                        if (header1[i] == "项目经验")
                        {
                            header2[j] = header1[i].ToString();
                            body2[j] = body1[i].ToString();
                            j++;
                            continue;
                        }
                        else if (header1[i] == "教育经历")
                        {
                            flag = 0;
                        }
                        else
                        {
                            body2[j - 1] = body2[j - 1].ToString() + header1[i].ToString() + ":" + body1[i].ToString() + "\r\n";
                            continue;
                        }
                    }
                    if (flag == 2)
                    {

                        if (header1[i] == "培训经历")
                        {
                            header2[j] = header1[i].ToString();
                            body2[j] = body1[i].ToString();
                            j++;
                            continue;
                        }
                        else if (header1[i] == "证书")
                        {
                            flag = 0;
                        }
                        else
                        {
                            body2[j - 1] = body2[j - 1].ToString() + header1[i].ToString() + ":" + body1[i].ToString() + "\n";
                            continue;
                        }
                    }
                    if ((header1[i].ToString().Trim() == "") || (header1[i].ToString().Trim() == header2[j - 1].ToString().Trim()))
                    {
                        body2[j - 1] = body2[j - 1].ToString() + body1[i].ToString() + "\r\n";
                    }
                    else
                    {
                        header2[j] = header1[i].ToString();
                        body2[j] = body1[i].ToString();
                        j++;
                    }
                }
                //根据数组2中的列名创建列
                for (int i = 0; i < j; i++)
                {
                    dt.Columns.Add(header2[i].ToString());
                }
                DataRow dr = dt.NewRow();

                //根据数组2中的内容添加列数据
                for (int i = 0; i < j; i++)
                {
                    dr[i] = body2[i].ToString();
                }
                dt.Rows.Add(dr);
                //dt.Columns.Add("Id");
                //dt.Columns["Id"].SetOrdinal(0);
                return dt;


            }
            catch (Exception ex)
            {
                GetError(ex.Message);
                return null;
            }
        }
        //去除空列
         DataTable DelNullCol(DataTable t)
        {
            bool IsNull;
            for (int i = t.Columns.Count - 1; i >= 0; i--)
            {
                IsNull = true;
                foreach (DataRow r in t.Rows)
                {
                    if (!r.IsNull(t.Columns[i]))
                    {
                        IsNull = false;
                        break;
                    }
                }
                if (IsNull)
                    t.Columns.RemoveAt(i);
            }
            return t;
        }




        //导入读取的数据表db 到 tableName（数据库表名)
         bool SuccessImport(DataTable dt, string tableName)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                SqlDataAdapter da = new SqlDataAdapter("select * from " + tableName + "", con);
                DataSet ds = new DataSet();
                da.Fill(ds);
                //ds.Tables[0].PrimaryKey = new DataColumn[] { ds.Tables[0].Columns[0] };//主键
                SqlCommandBuilder cmdb = new SqlCommandBuilder(da);
                da.InsertCommand = cmdb.GetInsertCommand();
                da.UpdateCommand = cmdb.GetUpdateCommand();
                da.DeleteCommand = cmdb.GetDeleteCommand();
                foreach (DataRow dr in dt.Rows)
                {
                    DataRow newdr = ds.Tables[0].NewRow();
                    newdr.ItemArray = dr.ItemArray;
                    ds.Tables[0].Rows.Add(newdr);
                }
                da.Update(ds);
                ds.AcceptChanges();
                return true;
            }
            catch (Exception ex)
            {
                GetError(ex.Message);
                return false;
            }

        }
        //导出
        // void DataTabletoExcel(DataTable tmpDataTable, string strFileName)
        //{

        //    if (tmpDataTable == null)

        //        return;

        //    int rowNum = tmpDataTable.Rows.Count;

        //    int columnNum = tmpDataTable.Columns.Count;

        //    int rowIndex = 1;

        //    int columnIndex = 0;


        //    Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.ApplicationClass();

        //    xlApp.DefaultFilePath = "";

        //    xlApp.DisplayAlerts = true;

        //    xlApp.SheetsInNewWorkbook = 1;

        //    Microsoft.Office.Interop.Excel.Workbook xlBook = xlApp.Workbooks.Add(true);

        //    //将DataTable的列名导入Excel表第一行

        //    foreach (DataColumn dc in tmpDataTable.Columns)
        //    {

        //        columnIndex++;

        //        xlApp.Cells[rowIndex, columnIndex] = dc.ColumnName;

        //    }

        //    //将DataTable中的数据导入Excel中

        //    for (int i = 0; i < rowNum; i++)
        //    {

        //        rowIndex++;

        //        columnIndex = 0;

        //        for (int j = 0; j < columnNum; j++)
        //        {

        //            columnIndex++;

        //            xlApp.Cells[rowIndex, columnIndex] = tmpDataTable.Rows[i][j].ToString();

        //        }

        //    }

        //    //xlBook.SaveCopyAs(HttpUtility.UrlDecode(strFileName, System.Text.Encoding.UTF8));

        //    xlBook.SaveCopyAs(strFileName);

        //}       

    }
}