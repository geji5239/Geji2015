using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Text;
using System.Drawing;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.IO;
using System.Diagnostics;
using System.Xml;
using System.Management;
using System.Configuration;


namespace Panasia.Gemini.Web.Models.Helpers
{
    public class SqlHelpers
    {
        public static string strConnection = ConfigurationManager.ConnectionStrings["PASys"].ConnectionString;
        //public static string strPath = Process.GetCurrentProcess().MainModule.FileName;
        public static string strPath;
        public static string strUserZH;
        public static string SQLName;
        public static SqlConnection Myconnection;
        public static SqlConnection MyconnectionT;//测试链接
        public static SqlDataAdapter Myadapter;
        public static SqlCommand Mycommand;
        /// <summary>通过连接语句strConnection连接数据库，打开并返回数据源
        /// 
        /// </summary>
        /// <returns></returns>
        public static SqlConnection getconnection()
        {
            try
            {
                if (Myconnection == null)
                {
                    Myconnection = new SqlConnection(strConnection);
                }
                else if (Myconnection.State == ConnectionState.Open)
                {
                    Myconnection.Close();
                }
                Myconnection.Open();
                return Myconnection;
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>测试数据库连接是否正确
        /// 
        /// </summary>
        /// <param name="strConStringTest"></param>
        /// <returns></returns>
        public static bool getconnectionT(string strConStringTest)
        {
            try
            {
                if (MyconnectionT == null)
                {
                    MyconnectionT = new SqlConnection(strConStringTest);
                    MyconnectionT.Open();
                    MyconnectionT.Close();
                    return true;
                }
                if (MyconnectionT.State != ConnectionState.Open)
                {
                    MyconnectionT.ConnectionString = strConStringTest;
                    MyconnectionT.Open();
                    MyconnectionT.Close();
                    return true;
                }
                else
                {
                    MyconnectionT = null;
                    return true;
                }

            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>执行语句，更新数据库
        /// 
        /// </summary>
        /// <param name="str"></param>
       public void CmdExec(string str)
        {
            try
            {
                Mycommand = new SqlCommand(str, getconnection());
                Mycommand.ExecuteNonQuery();
            }
            catch (Exception)
            {
                return;
                throw;
            }
        }
        /// <summary>执行语句，返回是否更新数据库成功
        /// 
        /// </summary>
        /// <param name="str"></param>
        public static Boolean CmdExecbool(string str)
        {
            try
            {
                SqlConnection co = getconnection();
                if (co == null)
                {
                    return false;
                }
                Mycommand = new SqlCommand(str, co);
                if (Mycommand.ExecuteNonQuery() >= 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        /// <summary>执行语句,返回reader
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static SqlDataReader CmdExecReturnReader(string str)
        {
            try
            {
                Mycommand = new SqlCommand(str, getconnection());
                SqlDataReader Myreader = Mycommand.ExecuteReader();
                return Myreader;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        /// <summary>执行查询，导出数据表
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static DataSet SelectReturnDataSet(string str)
        {
            try
            {
                DataSet Mydataset = new DataSet();
                Myadapter = new SqlDataAdapter(str, getconnection());
                Myadapter.Fill(Mydataset);
                return Mydataset;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        /// <summary>执行查询，导出数据表
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public DataTable SelectReturnDataTable(string str)
        {
            try
            {
                DataSet Mydataset = new DataSet();
                Myadapter = new SqlDataAdapter(str, getconnection());
                Myadapter.Fill(Mydataset);
                return Mydataset.Tables[0];
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }

        /// <summary>搜索符合条件内容，若为零，返回true
        /// 
        /// </summary>
        /// <param name="select_str"></param>
        /// <param name="datagridview_name"></param>
        /// <returns></returns>
        public bool SelectResultZero(string select_str)
        {
            try
            {
                DataSet dataset = SelectReturnDataSet(select_str);
                if (dataset.Tables[0].Rows.Count == 0)
                    return true;
                else return false;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        /// <summary>执行搜索语句，返回字符串string
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string SelectReturnStr(string str)
        {
            try
            {
                str = SelectReturnDataSet(str).Tables[0].Rows[0][0].ToString();
                return str;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        public static string SelectReturnStr(string str,int r,int c)
        {
            try
            {
                str = SelectReturnDataSet(str).Tables[0].Rows[r][c].ToString();
                return str;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        /// 执行搜索语句(1列)，返回字符组string[] strs
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string[] SelectReturnC_Strs(string str)
        {
            try
            {
                if (SelectResultZero(str))
                    return null;
                else
                {
                    int len = SelectReturnDataSet(str).Tables[0].Rows.Count;//行数
                    string[] strs = new string[len];
                    for (int i = 0; i < len; i++)
                        strs[i] = SelectReturnDataSet(str).Tables[0].Rows[i][0].ToString();
                    return strs;
                }
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }

        //执行搜索语句(1行)，返回字符组string[] strs
        public string[] SelectReturnR_Strs(string str)
        {
            try
            {
                if (SelectResultZero(str))
                    return null;
                else
                {
                    int len = SelectReturnDataSet(str).Tables[0].Columns.Count;//列数
                    string[] strs = new string[len];
                    for (int i = 0; i < len; i++)
                        strs[i] = SelectReturnDataSet(str).Tables[0].Rows[0][i].ToString();
                    return strs;
                }
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        /// <summary>输入字符串组，返回最大值
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ReturnMax(string[] strs)
        {
            try
            {
                int n = int.Parse(strs[0].ToString());
                for (int i = 1; i < strs.Length; i++)
                    n = (n > int.Parse(strs[i].ToString())) ? n : int.Parse(strs[i].ToString());
                return n;
            }
            catch (Exception)
            {
                return 0;
                throw;
            }
        }
        /// <summary>对DataSet其中一列求和，结果显示在该列最下面
        /// 
        /// </summary>
        /// <param name="myds"></param>
        /// <param name="columnName"></param>
        public static void DataSetColumnSum(DataSet myds, string columnName)
        {
            try
            {
                float S = 0;
                int N = myds.Tables[0].Rows.Count;
                for (int i = 0; i < N; i++)
                    S += float.Parse(myds.Tables[0].Rows[i][columnName].ToString());
                myds.Tables[0].Rows.Add();
                myds.Tables[0].Rows[N][columnName] = S.ToString();
            }
            catch (Exception)
            {
                return;
                throw;
            }
        }
        //public static void datatoExcel(DataTable data)
        //{
        //    Microsoft.Office.Interop.Excel.Application objExcel = null;
        //    Microsoft.Office.Interop.Excel.Workbook objWorkbook = null;
        //    Microsoft.Office.Interop.Excel.Worksheet objWorksheet = null;
        //    objExcel = new Microsoft.Office.Interop.Excel.Application();
        //    objWorkbook = objExcel.Workbooks.Add(Missing.Value);
        //    objWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)objWorkbook.ActiveSheet;
        //    SaveFileDialog dlg = new SaveFileDialog();
        //    try
        //    {

        //        dlg.DefaultExt = "xls";
        //        dlg.Filter = "Excel 97-2003 工作簿(.xls)|*.xls|Excel 2007 工作簿(.xlsx)|*.xlsx";
        //        dlg.InitialDirectory = Directory.GetCurrentDirectory();
        //        if (dlg.ShowDialog() == DialogResult.Cancel)
        //        {
        //            return;
        //        }
        //        string sfilename = dlg.FileName;
        //        if (!sfilename.EndsWith(".xls"))
        //        {
        //            sfilename = sfilename + ".xls";
        //        }
        //        if (sfilename.Trim() == "")
        //        {
        //            return;
        //        }
        //        if (data.Rows.Count <= 0)
        //        {
        //            MessageBox.Show("没有数据可供保存！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //            return;
        //        }
        //        if (data.Rows.Count > 65536)
        //        {
        //            MessageBox.Show("数据太多，不能保存", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //            return;
        //        }
        //        if (data.Columns.Count > 255)
        //        {
        //            MessageBox.Show("数据太多，不能保存", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //            return;
        //        }
        //        FileInfo file = new FileInfo(sfilename);
        //        if (file.Exists)
        //        {
        //            try
        //            {
        //                file.Delete();
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show(ex.Message, "删除失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //                return;
        //            }
        //        }


        //        objExcel.Visible = false;
        //        //想excel中写入表格的表头
        //        int displayColumnsCount = 1;
        //        //for (int i = 0; i < data.Columns.Count; i++)
        //        //{
        //        //    //if (data.Columns[i].Visible == true)
        //        //    //{
        //        //        objExcel.Cells[1, displayColumnsCount] = data.Columns[i].ColumnName;
        //        //        displayColumnsCount++;
        //        //    //}
        //        //}
        //        foreach (DataColumn item in data.Columns)
        //        {

        //            objExcel.Cells[1, displayColumnsCount] = item.ColumnName;
        //            displayColumnsCount++;
        //        }
        //        //向EXCEL中逐行逐列的添加数据
        //        for (int row = 0; row < data.Rows.Count; row++)
        //        {
        //            displayColumnsCount = 1;
        //            for (int col = 0; col < data.Columns.Count; col++)
        //            {
        //                //if (data.Columns[col].Visible == true)
        //                //{
        //                try
        //                {
        //                    objExcel.Cells[row + 2, displayColumnsCount] = data.Rows[row][col].ToString().Trim();
        //                    displayColumnsCount++;
        //                }
        //                catch (Exception)
        //                {
        //                    continue;
        //                }
        //                //}
        //            }
        //        }
        //        //int FormatNum;//保存excel文件的格式
        //        //string Version;//excel版本号
        //        //Microsoft.Office.Interop.Excel.Application Application = new Microsoft.Office.Interop.Excel.Application();
        //        //Microsoft.Office.Interop.Excel.Workbook workbook = (Microsoft.Office.Interop.Excel.Workbook)Application.Workbooks.Add(Missing.Value);//激活工作簿
        //        //Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets.Add(true);//给工作簿添加一个sheet
        //        //Version = Application.Version;//获取你使用的excel 的版本号
        //        //if (Convert.ToDouble(Version) < 12)//You use Excel 97-2003
        //        //{
        //        //    FormatNum = -4143;
        //        //}
        //        //else//you use excel 2007 or later
        //        //{
        //        //    FormatNum = 56;
        //        //}

        //        //保存文件
        //        objWorkbook.SaveAs(sfilename.Trim(), -4143);
        //    }
        //    catch (Exception ex)
        //    {

        //        MessageBox.Show(ex.Message, "警告 ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }
        //    finally
        //    {
        //        //关闭Excel应用    
        //        if (objWorkbook != null) objWorkbook.Close(Missing.Value, Missing.Value, Missing.Value);
        //        if (objExcel.Workbooks != null) objExcel.Workbooks.Close();
        //        if (objExcel != null) objExcel.Quit();

        //        objWorksheet = null;
        //        objWorkbook = null;
        //        objExcel = null;
        //    }
        //    MessageBox.Show(dlg.FileName.Trim() + "\n\n导出完毕! ", "提示 ", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //}

        /// <summary>用事务批处理多条sql语句
        /// 
        /// </summary>
        /// <param name="paraSql"></param>
        /// <returns></returns>
        public string ExecSqlByTran(string[] paraSql)
        {

            try
            {
                using (SqlConnection conn = new SqlConnection(strConnection))
                {
                    conn.Open();
                    SqlCommand sqlcmd = new SqlCommand("", conn);
                    SqlTransaction tran;
                    tran = conn.BeginTransaction();
                    sqlcmd.Transaction = tran;
                    foreach (string sql in paraSql)
                    {
                        if (sql == null || sql == "")
                        {
                            continue;
                        }
                        sqlcmd.CommandText = sql;

                        try
                        {
                            sqlcmd.ExecuteNonQuery();
                            sqlcmd.Parameters.Clear();
                        }
                        catch (Exception e)
                        {
                            tran.Rollback();
                            return e.Message;
                        }
                    }
                    tran.Commit();
                    return null;
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }



        //public DataSet ReadExcelFile(string filePath)
        //{
        //    Page pg = new Page();
        //    string sArgs;
        //    //string filePath = "";
        //    string fileExtend = "";//文件扩展名
        //    int fileSize = 0;//文件大小

        //    //filePath = this.fileDaoRu.PostedFile.FileName.ToLower().Trim();

        //    //取得上传前的文件(存在于客户端)的文件或文件夹的名称,组成数组.例如:C:aaaa.txt,那么Names.lenth就为3
        //    string[] names = filePath.Split('\\');
        //    //取得文件名
        //    string name = names[names.Length - 1];
        //    string serverPath = HttpContext.Current.Server.MapPath("\\");//获得服务器端的根目录

        //    //判断是否有该目录
        //    if (!Directory.Exists(serverPath + "file"))
        //    {
        //        Directory.CreateDirectory(serverPath + "file");
        //        serverPath = serverPath + "file";
        //    }
        //    filePath = serverPath + name;
        //    //如果存在,删除文件
        //    if (File.Exists(filePath))
        //    {
        //        File.Delete(filePath);
        //    }
        //    //上传文件
        //    this.fileDaoRu.PostedFile.SaveAs(filePath);

        //    //得到文件的大小
        //    //fileSize = this.fileDaoRu.PostedFile.ContentLength;
        //    //得到扩展名
        //    fileExtend = filePath.Substring(filePath.IndexOf("."));
        //    if (fileSize == 0)
        //    {
        //        sArgs = @"<script language=javascript>window.alert( '找不到该文件！' );</script>";
        //        pg.Response.Write(sArgs);
        //        return null;
        //    }
        //    if (fileExtend != ".xls")
        //    {
        //        sArgs = @"<script language=javascript>window.alert( '请确认您所导入的文件是否EXCEL文件！！' );</script>";
        //        pg.Response.Write(sArgs);
        //        return null;
        //    }
        //    DataSet ds = new DataSet();

        //    try
        //    {
        //        OleDbConnection conn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Extended Properties=Excel 8.0;Data Source=" + filePath);
        //        if (conn.State != ConnectionState.Open)
        //        {
        //            conn.Open();
        //        }
        //        else
        //        {
        //            sArgs = @"<script language=javascript>window.alert( 'EXCEL文件正在使用！' );</script>";
        //            pg.Response.Write(sArgs);
        //            return null;
        //        }
        //        OleDbCommand com = conn.CreateCommand();
        //        com.CommandText = "select * from [sheet1$]";
        //        com.CommandType = CommandType.Text;
        //        OleDbDataAdapter adapter = new OleDbDataAdapter(com);


        //        adapter.Fill(ds);
        //        adapter.Dispose();
        //        conn.Close();
        //        conn.Dispose();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }

        //    return ds;
        //}
        public int DateDiff(DateTime DT1, DateTime DT2)
        {
            try
            {
                TimeSpan ts = DT1.Subtract(DT2); //时间差： DT1-DT2 （DT1.Subtract(DT2).Duration()绝对值）
                return ts.Days;
            }
            catch (Exception)
            {
                return 0;
                throw;
            }
        }
    }
    
}