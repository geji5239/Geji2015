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
    public class ResumeImportHelpers
    {
        string conString = ConfigurationManager.ConnectionStrings["PASys"].ConnectionString;
        string UserID = Panasia.Core.Sys.SysService.GetCurrentUser().UserID;
        SqlHelpers hb = new SqlHelpers();
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

        DataTable tableModel()
        {
            DataTable newtable = new DataTable();
            newtable.Columns.Add("系列号", typeof(string));
            newtable.Columns.Add("姓名", typeof(string));
            newtable.Columns.Add("性别", typeof(string));
            newtable.Columns.Add("应聘职位", typeof(string));
            newtable.Columns.Add("工作年限", typeof(string));
            newtable.Columns.Add("出生日期", typeof(string));
            newtable.Columns.Add("户口", typeof(string));
            newtable.Columns.Add("目前居住地", typeof(string));
            newtable.Columns.Add("移动电话", typeof(string));
            newtable.Columns.Add("电子邮箱", typeof(string));
            newtable.Columns.Add("通讯地址", typeof(string));
            newtable.Columns.Add("学历", typeof(string));
            newtable.Columns.Add("学校名称", typeof(string));
            newtable.Columns.Add("专业名称", typeof(string));
            newtable.Columns.Add("期望薪水", typeof(string));
            newtable.Columns.Add("最近一家公司", typeof(string));
            newtable.Columns.Add("投递时间", typeof(string));
            newtable.Columns.Add("政治面貌", typeof(string));
            newtable.Columns.Add("婚姻状况", typeof(string));
            newtable.Columns.Add("期望工作性质", typeof(string));
            newtable.Columns.Add("期望从事行业", typeof(string));
            newtable.Columns.Add("期望从事职业", typeof(string));
            newtable.Columns.Add("期望工作地区", typeof(string));
            newtable.Columns.Add("求职状态", typeof(string));
            newtable.Columns.Add("自我评价", typeof(string));
            newtable.Columns.Add("工作经验", typeof(string));
            newtable.Columns.Add("项目经验", typeof(string));
            newtable.Columns.Add("教育经历", typeof(string));
            newtable.Columns.Add("培训经历", typeof(string));
            newtable.Columns.Add("证书", typeof(string));
            newtable.Columns.Add("在校学习情况", typeof(string));
            newtable.Columns.Add("在校实践经验", typeof(string));
            newtable.Columns.Add("语言能力", typeof(string));
            newtable.Columns.Add("专业技能", typeof(string));

            newtable.Columns.Add("导入时间", typeof(string));
            newtable.Columns.Add("预约", typeof(string));
            newtable.Columns.Add("创建者", typeof(string));
            newtable.Columns.Add("创建时间", typeof(string));
            newtable.Columns.Add("修改者", typeof(string));
            newtable.Columns.Add("修改时间", typeof(string));

            return newtable;
        }

        //导入
        bool ImportFromExcel(string filePath, string s)
        {
            try
            {
                string sheetCoding;//系列号
                string S;//系列号前缀
                string tableName;//系列号前缀
                DataSet dsRead = ReadExlToDt(filePath);//读出EXCEL所有表数据
                #region 检测excel表内容是否正确
                //if (dsRead.Tables.Count-1!= dsRead.Tables[0].Rows.Count)
                //{
                //    GetError( "文件数据不正确");
                //    return false;
                //}

                switch (s)//根据主表名判断表来源
                {
                    case "51job":
                        S = "w";
                        tableName = "UserList";
                        break;
                    case "智联招聘":
                        tableName = "下载简历列表";
                        S = "z";
                        break;
                    case "通用":
                        tableName = "通用简历列表";
                        S = "w";
                        break;
                    default:
                        GetError("请选择文件来源");
                        return false;
                }
                if (dsRead.Tables[0].TableName.ToString() != tableName)
                {
                    GetError("请选择正确的文件来源");
                    return false;
                }
                #endregion
                DataTable dtRead = DelNullRow(dsRead.Tables[0]);//取主表
                dtRead.Columns.Add("系列号").SetOrdinal(0);//首列插入系列号
                if (!dtRead.Columns.Contains("投递(收藏)时间"))
                    dtRead.Columns.Add("投递(收藏)时间");
                DataTable dtMerge = new DataTable();//存放子表信息
                #region 读取子表信息存入dtMerge
                for (int i = 0; i < dtRead.Rows.Count; i++)
                {
                    DataTable dtsheet = TableTrans(dsRead.Tables[i+1], S);//读子表，并转置
                    if (dtsheet==null||dtsheet.Columns.Count <= 1)
                    {
                        GetError("表'" + dsRead.Tables[i + 1].TableName + "'没有数据");
                        return false;
                    }
                    dtsheet.Columns.Add("系列号").SetOrdinal(0);
                    sheetCoding = S + DateTime.Now.ToString("yyMMddHHmmss")+i.ToString("000");//生成系列号
                    dtRead.Rows[i][0] = dtsheet.Rows[0][0] = sheetCoding;//给主表、子表添加系列号信息
                    dtMerge = MergeTable_RowAdd(dtMerge, dtsheet);//子表信息合并
                }
                #endregion
                dtRead = DrawTable(dtRead, dtMerge, S);//主表、子表合并，S为文件来源标记
                dtRead = DataTrans(dtRead);//格式化表中数据
                dtRead = AddInfo(dtRead);//添加导入信息
                string[] strsInsert = DtToStrings(dtRead);//生成SQL执行语句
                string resultTran = hb.ExecSqlByTran(strsInsert);
                if (string.IsNullOrEmpty(resultTran))
                {
                    result.HasError = false;
                    result.Data = resultCount;
                    return true;//导入成功 
                }
                GetError("导入失败！错误信息："+resultTran);
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
        //提取部分列组成新表
         DataTable DrawTable(DataTable dt_zhu, DataTable dt_zi, string S)
        {
            DataTable dt;

            switch (S)//判断表来源
            {
                case "w":
                    dt_zhu = dt_zhu.DefaultView.ToTable(false, new string[] { "系列号", "姓名", "性别", "应聘职位", "工作年限", "出生日期", "户口", "目前居住地", "联系电话", "电子邮件", "地址", "学历", "毕业学校", "专业", "期望薪水", "最近一家公司", "应聘日期" });
                    dt_zi = dt_zi.DefaultView.ToTable(false, new string[] { "政治面貌", "婚姻状况", "工作性质", "希望行业", "目标职能", "目标地点", "求职状态", "自我评价", "工作经验", "项目经验", "教育经历", "培训经历", "证书", "所获奖励", "社会实践", "语言能力", "IT技能" });
                    break;
                case "z":
                    dt_zhu = dt_zhu.DefaultView.ToTable(false, new string[] { "系列号", "姓名", "性别", "应聘职位", "工作年限", "出生日期", "户口", "目前居住地", "移动电话", "电子邮件", "通讯地址", "最高学历", "学校名称", "专业名称", "期望月薪(税前)", "现在单位", "投递(收藏)时间" });
                    dt_zi = dt_zi.DefaultView.ToTable(false, new string[] { "政治面貌", "婚姻状况", "期望工作性质", "期望从事行业", "期望从事职业", "期望工作地区", "目前状况", "自我评价", "工作经历", "项目经验", "教育经历", "培训经历", "证书", "在校学习情况", "在校实践经验", "语言能力", "专业技能" });
                    break;
                default: return null;
            }

            dt = MergeTable_ColAdd(dt_zhu, dt_zi);
            return dt;
        }
         //生成SQL执行语句
         int[] resultCount = { 0, 0, 0, 0, 0 ,0};//导入，更新，无姓名，无应聘职位，无联系方式
         string[] DtToStrings(DataTable dt)
        {
            string[] strSql = new string[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string[] strsel = hb.SelectReturnR_Strs(string.Format("select [RID],[CV_GetTime],[Done],[CreatedUser],[CreatedTime] from rc_ResumeInfoes where name='{0}' and Position='{1}' and (MobilePhone='{2}' or Email='{3}')", dt.Rows[i][1].ToString(), dt.Rows[i][3].ToString(), dt.Rows[i][8].ToString(), dt.Rows[i][9].ToString())); //根据姓名,应聘职位,移动电话,邮箱 判断简历是否存在

                if (dt.Rows[i][1].ToString().Trim() == "")
                {
                    strSql[i] = "";
                    resultCount[2] += 1;
                }
                else if (dt.Rows[i][3].ToString().Trim() == "")
                {
                    strSql[i] = "";
                    resultCount[3] += 1;
                }
                else if (dt.Rows[i][8].ToString().Trim() == "")
                {
                    strSql[i] = "";
                    resultCount[4] += 1;
                }
                else if (dt.Rows[i][9].ToString().Trim() == "")
                {
                    strSql[i] = "";
                    resultCount[5] += 1;
                }
                else if (strsel==null)
                {
                    string str = "";
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (j != 0)
                        {
                            str += ",";
                        }
                        str += "'" + dt.Rows[i][j].ToString().Replace("'", "\"") + "'";
                    }
                    strSql[i] = @"insert into rc_ResumeInfoes([RID]
                            ,[Name]
                            ,[Sex]
                            ,[Position]
                            ,[YearsOfWorking]
                            ,[BirthDate]
                            ,[HuKou]
                            ,[CurrentResidency]
                            ,[MobilePhone]
                            ,[Email]
                            ,[Address]
                            ,[Degree]
                            ,[School]
                            ,[Professional]
                            ,[SalaryExpected]
                            ,[LastCompant]
                            ,[CV_GetTime]
                            ,[PoliticsStatus]
                            ,[MaritalStatus]
                            ,[TypeOfEmployment]
                            ,[IndustryExpected]
                            ,[JobFunctionExpected]
                            ,[WorkingPlaceExpected]
                            ,[CurrentSituation]
                            ,[SelfAssessment]
                            ,[WorkingExperience]
                            ,[ProjectExperience]
                            ,[Education]
                            ,[Training]
                            ,[Certifications]
                            ,[GraduatesInfo]
                            ,[SocialEvents]
                            ,[Language]
                            ,[ProfessionalSkill]
                            ,[ImportTime]
                            ,[Done]
                            ,[CreatedUser]
                            ,[CreatedTime]
                            ,[ModifiedUser]
                            ,[ModifiedTime]) values(" + str + ")";
                    resultCount[0] += 1;
                }
                else
                {
                    string str = "";
                    dt.Rows[i][0] = strsel[0];
                    dt.Rows[i][16] = strsel[1];
                    dt.Rows[i][35] = strsel[2];
                    dt.Rows[i][36] = strsel[3];
                    dt.Rows[i][37] = strsel[4];
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (j != 0)
                        {
                            str += ",";
                        }
                        str += "'" + dt.Rows[i][j].ToString().Replace("'", "\"") + "'";
                    }
                    strSql[i] = "delete from rc_ResumeInfoes where [RID]='" + dt.Rows[i][0] + "';";
                    strSql[i] += @"insert into rc_ResumeInfoes([RID]
                                ,[Name]
                                ,[Sex]
                                ,[Position]
                                ,[YearsOfWorking]
                                ,[BirthDate]
                                ,[HuKou]
                                ,[CurrentResidency]
                                ,[MobilePhone]
                                ,[Email]
                                ,[Address]
                                ,[Degree]
                                ,[School]
                                ,[Professional]
                                ,[SalaryExpected]
                                ,[LastCompant]
                                ,[CV_GetTime]
                                ,[PoliticsStatus]
                                ,[MaritalStatus]
                                ,[TypeOfEmployment]
                                ,[IndustryExpected]
                                ,[JobFunctionExpected]
                                ,[WorkingPlaceExpected]
                                ,[CurrentSituation]
                                ,[SelfAssessment]
                                ,[WorkingExperience]
                                ,[ProjectExperience]
                                ,[Education]
                                ,[Training]
                                ,[Certifications]
                                ,[GraduatesInfo]
                                ,[SocialEvents]
                                ,[Language]
                                ,[ProfessionalSkill]
                                ,[ImportTime]
                                ,[Done]
                                ,[CreatedUser]
                                ,[CreatedTime]
                                ,[ModifiedUser]
                                ,[ModifiedTime]) values(" + str + ")";
                    resultCount[1] += 1;
                }
            }
            return strSql;
        }
        //格式化投递(收藏)时间【 08-01 02:13:25 --> 2014-08-01】
         DataTable DataTrans(DataTable Table)
        {
            try
            {
                for (int i = 0; i < Table.Rows.Count; i++)
                {
                    int l;
                    string y = Table.Rows[i]["出生日期"].ToString().Substring(0, 4);
                    string m = Table.Rows[i]["出生日期"].ToString().Substring(5);
                    Table.Rows[i]["出生日期"] = string.Format("{0}-{1}-01", y, m);
                    l = Table.Rows[i]["证书"].ToString().Length;
                    Table.Rows[i]["证书"] = Table.Rows[i]["证书"].ToString().Substring(0, l > 30 ? 30 : l);
                    l = Table.Rows[i]["语言能力"].ToString().Length;
                    Table.Rows[i]["语言能力"] = Table.Rows[i]["语言能力"].ToString().Substring(0, l > 30 ? 30 : l);
                    l = Table.Rows[i]["期望从事行业"].ToString().Length;
                    Table.Rows[i]["期望从事行业"] = Table.Rows[i]["期望从事行业"].ToString().Substring(0, l > 30 ? 30 : l);
                    l = Table.Rows[i]["期望从事职业"].ToString().Length;
                    Table.Rows[i]["期望从事职业"] = Table.Rows[i]["期望从事职业"].ToString().Substring(0, l > 30 ? 30 : l);
                    l = Table.Rows[i]["自我评价"].ToString().Length;
                    Table.Rows[i]["自我评价"] = Table.Rows[i]["自我评价"].ToString().Substring(0, l > 500 ? 500 : l);
                    Table.Rows[i]["移动电话"] = Table.Rows[i]["移动电话"].ToString().Replace("086-1","1");
                    string datastring = Table.Rows[i]["投递时间"].ToString();
                    if (datastring.Contains(" "))
                    {
                        string date = datastring.Substring(0, datastring.IndexOf(" "));
                        if (date.Trim().Length < 10)
                        {
                            if (int.Parse(date.Substring(0, date.IndexOf("-")).Trim()) > DateTime.Now.Month)
                            {
                                Table.Rows[i]["投递时间"] = (DateTime.Now.Year - 1).ToString() + "-" + date.Trim();
                            }
                            else Table.Rows[i]["投递时间"] = DateTime.Now.Year.ToString() + "-" + date.Trim();
                        }
                    }
                    Table.Rows[i]["出生日期"] = IsDate(Table.Rows[i]["出生日期"].ToString());
                    Table.Rows[i]["投递时间"] = IsDate(Table.Rows[i]["投递时间"].ToString());
                }
                return Table;
            }
            catch (Exception ex)
            {
                GetError(ex.Message);
                return Table;
            }
        }
         public string IsDate(string strDate)
         {
             try
             {
                 DateTime.Parse(strDate);
                 return strDate;
             }
             catch
             {
                 return DateTime.Now.ToString("yyyy-MM-dd");
             }
         }
         //添加导入信息
         DataTable AddInfo(DataTable Table)
        {
            try
            {
                for (int i = 0; i < Table.Rows.Count; i++)
                {
                    Table.Rows[i][35] = 0;
                    Table.Rows[i][34] = Table.Rows[i][37] = Table.Rows[i][39] = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                    Table.Rows[i][36] = Table.Rows[i][38] = UserID;
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
         //去除空行
         DataTable DelNullRow(DataTable dt)
         {
             List<DataRow> removelist = new List<DataRow>();
             for (int i = 0; i < dt.Rows.Count; i++)
             {
                 bool rowdataisnull = true;
                 for (int j = 0; j < dt.Columns.Count; j++)
                 {

                     if (!string.IsNullOrEmpty(dt.Rows[i][j].ToString().Trim()))
                     {

                         rowdataisnull = false;
                     }

                 }
                 if (rowdataisnull)
                 {
                     removelist.Add(dt.Rows[i]);
                 }

             }
             for (int i = 0; i < removelist.Count; i++)
             {
                 dt.Rows.Remove(removelist[i]);
             }
             return dt;
         }
        //把表dt2添加到表dt1，具有相同列
         DataTable MergeTable_RowAdd(DataTable dt1, DataTable dt2)
        {
            if (dt1 == null || dt1.Rows.Count == 0)
                dt1 = dt2.Clone();
            if (dt2.Columns.Count > dt1.Columns.Count)
            {
                int n = dt2.Columns.Count - dt1.Columns.Count;
                for (int i = 0; i < n; i++)
                {
                    dt2.Columns.RemoveAt(dt1.Columns.Count);
                }
            }
            object[] obj = new object[dt1.Columns.Count];
            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                dt2.Rows[i].ItemArray.CopyTo(obj, 0);
                dt1.Rows.Add(obj);
            }
            return dt1;
        }
        //把表dt1和dt2合并，列合并
         DataTable MergeTable_ColAdd(DataTable dt1, DataTable dt2)
        {

            DataTable dt3 = tableModel();

            object[] obj = new object[dt3.Columns.Count];

            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                dt1.Rows[i].ItemArray.CopyTo(obj, 0);
                dt3.Rows.Add(obj);
            }

            if (dt1.Rows.Count >= dt2.Rows.Count)
            {
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    for (int j = 0; j < dt2.Columns.Count; j++)
                    {
                        dt3.Rows[i][j + dt1.Columns.Count] = dt2.Rows[i][j].ToString();
                    }
                }
            }
            else
            {
                DataRow dr3;
                for (int i = 0; i < dt2.Rows.Count - dt1.Rows.Count; i++)
                {
                    dr3 = dt3.NewRow();
                    dt3.Rows.Add(dr3);
                }
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    for (int j = 0; j < dt2.Columns.Count; j++)
                    {
                        dt3.Rows[i][j + dt1.Columns.Count] = dt2.Rows[i][j].ToString();
                    }
                }
            }
            return dt3;
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