using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Panasia.Core.Auth;
using System.IO;
using Panasia.Gemini.Web.Models.Helpers;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data.Entity;
using Panasia.Core;
using System.Text;
using Panasia.Core.Web;
using System.Configuration;
using Panasia.Gemini.Web.Models;
using Panasia.Core.Sys;

namespace Panasia.Gemini.Web.Controllers
{
    public class org_EmployeesController : Controller
    {
        EmployeeImportHelpers EmpImportHp = new EmployeeImportHelpers();
        #region 员工照片上传
        [HttpPost]
        public ActionResult UploadImageFile()
        {
            int iTotal = Request.Files.Count;
            if (iTotal == 0)
            {
                return Json(ResultData.CreateError("没有数据"));
            }
            else
            {
                var supportedTypes = new[] { "jpg", "jpeg", "png", "gif", "bmp" };
                for (int i = 0; i < iTotal; i++)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);

                    Random r = new Random();
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + r.Next(10000) + "." + fileExt;


                    if (!supportedTypes.Contains(fileExt))
                    {
                        return Json(ResultData.CreateError("文件类型错误"), "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
                    }
                    if (file.ContentLength > 0 || !string.IsNullOrEmpty(file.FileName))
                    {
                        //保存文件
                        string filePath = string.Format(HttpRuntime.AppDomainAppPath.ToString() + @"/EmployeeImg/Temp");
                        if (!Directory.Exists(filePath))
                        {
                            Directory.CreateDirectory(filePath);
                        }
                        string filename = Path.GetFileName(fileName);
                        string savepath = string.Format("{0}\\{1}", filePath, filename);
                        file.SaveAs(savepath);
                        //var json = ImportToDb(savepath);
                        return Json(fileName, "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return null;
        }
        #endregion
        class combobox
        {
            public string id;
            public string text;
        }
        static List<combobox> combo = new List<combobox>(); //表名下拉列表
        static DataSet dsRead;//Excel所有表
        static DataTable dtOpen = new DataTable();//打开的表
        DataTable tableModel()
        {
            DataTable newtable = new DataTable();
            newtable.Columns.Add("CompanyID", typeof(string));
            newtable.Columns.Add("DeptID", typeof(string));
            newtable.Columns.Add("JobID", typeof(string));
            newtable.Columns.Add("CardID", typeof(string));
            newtable.Columns.Add("Name", typeof(string));
            newtable.Columns.Add("Sex", typeof(string));
            newtable.Columns.Add("CardNo", typeof(string));
            newtable.Columns.Add("Birthday", typeof(string));
            newtable.Columns.Add("Nation", typeof(string));
            newtable.Columns.Add("Marriage", typeof(string));
            newtable.Columns.Add("Health", typeof(string));
            newtable.Columns.Add("Politics", typeof(string));
            newtable.Columns.Add("Native", typeof(string));
            newtable.Columns.Add("RegisterAddress", typeof(string));
            newtable.Columns.Add("LiveAddress", typeof(string));
            newtable.Columns.Add("MobiPhone", typeof(string));
            newtable.Columns.Add("Telephone", typeof(string));
            newtable.Columns.Add("ExtNum", typeof(string));
            newtable.Columns.Add("Education", typeof(string));
            newtable.Columns.Add("HireDate", typeof(string));
            newtable.Columns.Add("RegularDate", typeof(string));
            newtable.Columns.Add("Terminate", typeof(string));
            newtable.Columns.Add("Email", typeof(string));
            newtable.Columns.Add("ImoID", typeof(string));
            newtable.Columns.Add("Pemail", typeof(string));
            newtable.Columns.Add("EduName", typeof(string));
            newtable.Columns.Add("Major", typeof(string));
            newtable.Columns.Add("EduEndDate", typeof(string));
            newtable.Columns.Add("InsuranceNo", typeof(string));
            newtable.Columns.Add("SocialSecurity", typeof(string));
            newtable.Columns.Add("Convention", typeof(string));
            newtable.Columns.Add("ConventionStartDate", typeof(string));
            newtable.Columns.Add("ConventionEndDate", typeof(string));
            newtable.Columns.Add("ContractOfSale", typeof(string));
            newtable.Columns.Add("ContractStartDate", typeof(string));
            newtable.Columns.Add("ContractEndDate", typeof(string));
            newtable.Columns.Add("State", typeof(string));
            newtable.Columns.Add("JobState", typeof(string));
            newtable.Columns.Add("Remark", typeof(string));
            return newtable;
        }
        class TableCols
        {
            public object Cols { get; set; }
        }
        public ActionResult GetSheetNames()
        {
            return Json(combo, "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
        }
        //获取打开的表
        public string OpenExcelTable(string id)
        {
            int index = int.Parse(id);
            dtOpen =  DelNullRow(dsRead.Tables[index]);
            return dtOpen.ToJson();
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
        //获取打开表的数据
        public string OpenTable()
        {
            return dtOpen.ToJson();
        }
        //获取打开表的列名
        public ActionResult GetTableCols(string id)
        {
            dtOpen = DelNullRow(dsRead.Tables[int.Parse(id)]);
            string[] cols = new string[dtOpen.Columns.Count];
            for (int i = 0; i < cols.Length; i++)
            {
                cols[i] = dtOpen.Columns[i].ColumnName;
            }
            TableCols Cs=new TableCols();
            Cs.Cols = cols;
            return Json(Cs, "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
        }
        //打开excel
        public ActionResult OpenExcel(FormCollection form)
        {
            EmployeeImportHelpers th = new EmployeeImportHelpers();
            HttpRequest request = System.Web.HttpContext.Current.Request;
            HttpFileCollection FileCollect = request.Files;
            if (FileCollect.Count > 0)
            {
                foreach (string str in FileCollect)
                {
                    HttpPostedFile FileSave = FileCollect[str];  //用key获取单个文件对象HttpPostedFile
                    string fileName = FileSave.FileName.Substring(FileSave.FileName.LastIndexOf("\\") + 1);
                    string filePath = string.Format(HttpRuntime.AppDomainAppPath.ToString() + @"Upload");
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    string Path = string.Format("{0}\\{1}_{2}", filePath, DateTime.Now.ToString("yyyyMMddHHmmss"), fileName);
                    FileSave.SaveAs(Path);              //将上传的文件保存
                    //return Json(ResultData.CreateError("维护中，暂不能用!"));
                    dsRead = th.ReadExlToDt(Path);
                    combo.Clear();
                   // Dictionary<string, int> list = new Dictionary<string, int>();
                    for (int i = 0; i < dsRead.Tables.Count; i++)
                    {
                        combobox combobox = new combobox();
                        combobox.id = i.ToString();
                        combobox.text = dsRead.Tables[i].TableName;
                        combo.Add(combobox);
                        //list.Add(dsRead.Tables[i].TableName, i);
                    }
                    return Json(ResultData.Create(null), "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
                }
            }
            return View(form);
        }
        #region 不必要的列（无则添加）colsUnNeed
        string[] colsUnNeed = {
                            "性别",
                            "出生日期",
                            "民族",
                            "婚姻状况",
                            "健康状况",
                            "政治面貌",
                            "籍贯",
                            "户口所在地",
                            "家庭地址",
                            "办公电话",
                            "分机号",
                            "学历",
                            "转正日期",
                            "离职日期",
                            "企业邮箱",
                            "IMO",
                            "个人邮箱",
                            "毕业院校",
                            "专业",
                            "毕业时间",
                            "保险帐号",
                            "社保号码",
                            "实习合同编号",
                            "实习合同日期起",
                            "实习合同日期讫",
                            "正式合同编号",
                            "正式合同日期起",
                            "正式合同日期讫",
                            "员工状态",
                            "任职状态",
                            "备注"
                            };
        #endregion
        #region 要导入的所有列colsImport
        string[] colsImport = { "公司",
                                "部门",
                                "职务",
                                "卡号",
                                "姓名",
                                "性别",
                                "身份证号",
                                "出生日期",
                                "民族",
                                "婚姻状况",
                                "健康状况",
                                "政治面貌",
                                "籍贯",
                                "户口所在地",
                                "家庭地址",
                                "联系电话",
                                "办公电话",
                                "分机号",
                                "学历",
                                "入职时间",
                                "转正日期",
                                "离职日期",
                                "企业邮箱",
                                "IMO",
                                "个人邮箱",
                                "毕业院校",
                                "专业",
                                "毕业时间",
                                "保险帐号",
                                "社保号码",
                                "实习合同编号",
                                "实习合同日期起",
                                "实习合同日期讫",
                                "正式合同编号",
                                "正式合同日期起",
                                "正式合同日期讫",
                                "员工状态",
                                "任职状态",
                                "备注"
                               };
        #endregion
        //导入
        public ActionResult EmpImport()
        {
            ResultData result = new ResultData();
            try
            {
                DataTable dt = GetImportTable();
                int rowsCount = dt.Rows.Count;
                #region 配置
                SqlCommand cmd = new SqlCommand();
                System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings["PASys"].ConnectionString);
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "sp_hr_EmpInfoToEmployee";
                cmd.CommandType = CommandType.StoredProcedure;
                #endregion
                #region 存储过程参数
                cmd.Parameters.Add("@EmpInfoes", SqlDbType.Structured);
                cmd.Parameters["@EmpInfoes"].Value = dt;

                cmd.Parameters.Add("@UserID", SqlDbType.VarChar, 6);
                cmd.Parameters["@UserID"].Value = Panasia.Core.Sys.SysService.GetCurrentUser().UserID;

                cmd.Parameters.Add("@errorMessage", SqlDbType.VarChar, 100);
                cmd.Parameters["@errorMessage"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@InsertCount", SqlDbType.Int);
                cmd.Parameters["@InsertCount"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@noCompany", SqlDbType.VarChar, 8000);
                cmd.Parameters["@noCompany"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@noDept", SqlDbType.VarChar, 8000);
                cmd.Parameters["@noDept"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@noJob", SqlDbType.VarChar, 8000);
                cmd.Parameters["@noJob"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@noName", SqlDbType.VarChar, 8000);
                cmd.Parameters["@noName"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@sameCardNo", SqlDbType.VarChar, 8000);
                cmd.Parameters["@sameCardNo"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@noOrSameCardID", SqlDbType.VarChar, 8000);
                cmd.Parameters["@noOrSameCardID"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@sameImo", SqlDbType.VarChar, 8000);
                cmd.Parameters["@sameImo"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@sameEmail", SqlDbType.VarChar, 8000);
                cmd.Parameters["@sameEmail"].Direction = ParameterDirection.Output;
                #endregion
                cmd.ExecuteNonQuery();
                string[] msg = new string[11];
                #region msg添加导入结果
                msg[0] = cmd.Parameters["@errorMessage"].Value.ToString();
                msg[1] = cmd.Parameters["@noCompany"].Value.ToString();
                msg[2] = cmd.Parameters["@noDept"].Value.ToString();
                msg[3] = cmd.Parameters["@noJob"].Value.ToString();
                msg[4] = cmd.Parameters["@noName"].Value.ToString();
                msg[5] = cmd.Parameters["@sameCardNo"].Value.ToString();
                msg[6] = cmd.Parameters["@noOrSameCardID"].Value.ToString();
                msg[7] = cmd.Parameters["@sameImo"].Value.ToString();
                msg[8] = cmd.Parameters["@sameEmail"].Value.ToString();
                msg[9] = cmd.Parameters["@InsertCount"].Value.ToString();
                msg[10] = (rowsCount - int.Parse(msg[9])).ToString();
                #endregion
                con.Close();
                if (string.IsNullOrEmpty(msg[0]))
                {
                    result.HasError = false; result.Data = msg;
                    return Json(result, "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    result.HasError = true;
                    result.ErrorMessage = msg[0];
                    return Json(result, "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                //ServiceExtensions.Log(null, e.Message);
                result.HasError = true;
                result.ErrorMessage = e.Message;
                return Json(result, "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
            }
        }
        //获取要导入的数据
        private DataTable GetImportTable()
        {
            DataTable dt = new DataTable();
            dt = EmpImportHp.AddColumns(dtOpen, colsUnNeed);
            dt = EmpImportHp.SelectColsToTable(dt, colsImport);
            dt = EmpImportHp.MergeTable_RowAdd(dt, tableModel());
            dt.Columns.Add("id", typeof(int)).SetOrdinal(0);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i][0] = i;
            }
            return dt;
        }

        public DataSet ReadExcel( string str, DataSet ds, HttpFileCollection FileCollect)
        {
            EmployeeImportHelpers th = new EmployeeImportHelpers();

            HttpPostedFile FileSave = FileCollect[str];  //用key获取单个文件对象HttpPostedFile
            string fileName = FileSave.FileName.Substring(FileSave.FileName.LastIndexOf("\\") + 1);
            string filePath = string.Format(HttpRuntime.AppDomainAppPath.ToString() + @"Upload");
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            string Path = string.Format("{0}\\{1}_{2}", filePath, DateTime.Now.ToString("yyyyMMddHHmmss"), fileName);
            FileSave.SaveAs(Path);
            ds = th.ReadExlToDt(Path);
            System.IO.File.Delete(Path);
            return ds;
        }

        public ActionResult ImportCompany(FormCollection form)
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            HttpFileCollection FileCollect = request.Files;
            DataSet ds = new DataSet();

            StringBuilder skipIndex = new StringBuilder();
            StringBuilder rightIndex = new StringBuilder();
            StringBuilder errorIndex = new StringBuilder();
            ResultData res = new ResultData();

            if (FileCollect.Count > 0)
            {
                foreach (string str in FileCollect)
                {
                    #region 读取excel文件
                    ds = ReadExcel(str, ds, FileCollect);
                    //HttpPostedFile FileSave = FileCollect[str];  //用key获取单个文件对象HttpPostedFile
                    //string fileName = FileSave.FileName.Substring(FileSave.FileName.LastIndexOf("\\") + 1);
                    //string filePath = string.Format(HttpRuntime.AppDomainAppPath.ToString() + @"Upload");
                    //if (!Directory.Exists(filePath))
                    //{
                    //    Directory.CreateDirectory(filePath);
                    //}
                    //string Path = string.Format("{0}\\{1}_{2}", filePath, DateTime.Now.ToString("yyyyMMddHHmmss"), fileName);
                    //FileSave.SaveAs(Path);
                    //ds = th.ReadExlToDt(Path);
                    //System.IO.File.Delete(Path);
                    #endregion
                    #region 判断excel表中的数据是否存在
                    string[] cols = new string[] { "事业部", "公司名称", "简称", "负责人", "排序", "网址1", "网址2", "电话1", "电话2", "公司地址", "公司说明", "备注" };
                    for (int j = 0; j < cols.Length; j++)
                    {
                        if (!ds.Tables[0].Columns.Contains(cols[j]))
                        {
                            res.ErrorMessage = cols[j] + "列不存在！";
                            res.HasError = false;
                            return Json(res, "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
                        }
                    }
                    #endregion
 
                    int count = ds.Tables[0].Rows.Count;
                    using (var db = new SysContext())
                    {
                        for (int i = 0; i < count; i++)
                        {
                            #region 预处理
                            string buname, companyname, managerName;
                            int sort = i + 1;
                            buname = ds.Tables[0].Rows[i]["事业部"].ToString().Trim();
                            companyname = ds.Tables[0].Rows[i]["公司名称"].ToString().Trim();
                            managerName = ds.Tables[0].Rows[i]["负责人"].ToString().Trim();
                            #endregion

                            var oldCompany = (from com in db.hr_Companies where com.Name.Equals(companyname) && com.IsActive select com).ToList();
                            if (oldCompany.Count > 0)
                            {
                                skipIndex.Append((i + 1).ToString() + ',');
                            }
                            else
                            {
                                string managerID;
                                var buid = (from bu in db.hr_BUs where bu.Name.Equals(buname) && bu.IsActive select bu.BUID.ToString()).ToList();
                                using (var dbe = new Panasia.Core.Sys.SysContext())
                                {
                                    var manager = (from ma in dbe.hr_Employees where ma.Name.Equals(managerName) && ma.IsActive select ma.EmployeeID.ToString()).ToList();
                                    managerID = manager.Count > 0 ? manager[0] : "";
                                }
                                #region 获取公司ID
                                db.Database.Connection.Open();
                                var cmd = db.Database.Connection.CreateCommand();
                                cmd.CommandText = "declare @demo varchar(max) exec dbo.sp_CreateSerialCode 'Company','C',6,@demo output select @demo";
                                var companyid = cmd.ExecuteScalar();
                                db.Database.Connection.Close();
                                #endregion
                                #region 将公司信息插入数据库并获取异常
                                try
                                {
                                    hr_Company company = new hr_Company
                                    {
                                        CompanyID = companyid.ToString(),
                                        Name = companyname,
                                        BUID = buid.Count > 0 ? buid[0] : "",
                                        ManagerID = managerID,
                                        SortID = ds.Tables[0].Rows[i]["排序"].ToString() == "" ? sort : ds.Tables[0].Rows[i]["排序"].ToInt32(),
                                        IsActive = true,
                                        JC = ds.Tables[0].Rows[i]["简称"].ToString().Trim(),
                                        JM = ds.Tables[0].Rows[i]["简称"].ToString().Trim().GetPYFirsts(),
                                        Url1 = ds.Tables[0].Rows[i]["网址1"].ToString().Trim(),
                                        Url2 = ds.Tables[0].Rows[i]["网址2"].ToString().Trim(),
                                        Tel1 = ds.Tables[0].Rows[i]["电话1"].ToString().Trim(),
                                        Tel2 = ds.Tables[0].Rows[i]["电话2"].ToString().Trim(),
                                        Address = ds.Tables[0].Rows[i]["公司地址"].ToString().Trim(),
                                        Description = ds.Tables[0].Rows[i]["公司说明"].ToString().Trim(),
                                        Remark = ds.Tables[0].Rows[i]["备注"].ToString().Trim()
                                    };
                                    company.ResetCreated();
                                    db.hr_Companies.Add(company);
                                    db.SaveChanges();
                                    rightIndex.Append((i + 1).ToString() + ',');
                                }
                                catch (Exception ex)
                                {
                                    errorIndex.Append((i + 1).ToString() + ',');
                                    res.ErrorMessage = skipIndex + ":" + rightIndex + ":" + errorIndex;
                                    res.HasError = true;
                                    return Json(res, "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
                                }
                                #endregion
                            }
                        }
                    }
                }
            }
            res.ErrorMessage = skipIndex + ":" + rightIndex + ":" + errorIndex;
            res.HasError = true;
            return Json(res, "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
        }

        public ActionResult ImportDept(FormCollection form)
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            HttpFileCollection FileCollect = request.Files;
            DataSet ds = new DataSet();

            StringBuilder skipIndex = new StringBuilder();
            StringBuilder rightIndex = new StringBuilder();
            StringBuilder errorIndex = new StringBuilder();
            ResultData res = new ResultData();

            if (FileCollect.Count > 0)
            {
                foreach (string str in FileCollect)
                {
                    ds = ReadExcel(str, ds, FileCollect);
                    #region 判断excel表中的数据是否存在
                    string[] cols = new string[] { "事业部", "公司", "部门", "子部门", "负责人", "排序", "备注"};
                    for (int j = 0; j < cols.Length; j++)
                    {
                        if (!ds.Tables[0].Columns.Contains(cols[j]))
                        {
                            res.ErrorMessage = cols[j] + "列不存在！";
                            res.HasError = false;
                            return Json(res, "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
                        }
                    }
                    #endregion
                     int count = ds.Tables[0].Rows.Count;
                     using (var db = new SysContext())
                     {
                         for (int i = 0; i < count; i++)
                         {
                             #region 预处理
                             string buname, companyname, managerName, deptName, parentName, managerID, parentID, deptid, companyid;
                             int sort = i + 1;
                             buname = ds.Tables[0].Rows[i]["事业部"].ToString().Trim();
                             companyname = ds.Tables[0].Rows[i]["公司"].ToString().Trim();
                             managerName = ds.Tables[0].Rows[i]["负责人"].ToString().Trim();
                             #region 获取负责人id
                             using (var dbe = new Panasia.Core.Sys.SysContext())
                             {
                                 var manager = (from ma in dbe.hr_Employees where ma.Name.Equals(managerName) && ma.IsActive select ma.EmployeeID.ToString()).ToList();
                                 managerID = manager.Count > 0 ? manager[0] : "";
                             }
                             #endregion

                             var company = (from com in db.hr_Companies where com.Name.Equals(companyname)&& com.IsActive select com.CompanyID.ToString()).ToList();
                             var buid = (from bu in db.hr_BUs where bu.Name.Equals(buname) && bu.IsActive select bu.BUID.ToString()).ToList();
                             #endregion
                             if (company.Count == 0)
                             {
                                 res.ErrorMessage = companyname + "不存在！";
                                 res.HasError = false;
                                 return Json(res, "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
                             }
                             else
                             {
                                 #region 判断部门存在则跳过
                                  companyid = company[0];
                                  var departments=new List<string>();
                                  if (ds.Tables[0].Rows[i]["子部门"].ToString().Trim() == "")
                                  {
                                      deptName = ds.Tables[0].Rows[i]["部门"].ToString().Trim();
                                      departments = (from dept in db.hr_Depts where dept.Name.Equals(deptName) && dept.CompanyID.Equals(companyid) && dept.IsActive select dept.DeptID.ToString()).ToList();
                                  }
                                  else
                                  {
                                      deptName = ds.Tables[0].Rows[i]["子部门"].ToString().Trim();
                                      parentName = ds.Tables[0].Rows[i]["部门"].ToString().Trim();
                                    var  parentid=(from dept1 in db.hr_Depts where dept1.Name.Equals(parentName) && dept1.IsActive select dept1.ParentID.ToString()).ToList().FirstOrDefault();
                                    departments = (from dept in db.hr_Depts where dept.Name.Equals(deptName) && dept.ParentID.Equals(parentid) && dept.IsActive select dept.DeptID).ToList();
                                  }
                                      
                                
                                 if (departments.Count > 0)
                                 {
                                     skipIndex.Append((i+1).ToString() + ",");
                                 }
                                 #endregion
                                 else
                                 {
                                 #region 获取部门ID
                                     try
                                     {
                                         db.Database.Connection.Open();
                                         var cmd = db.Database.Connection.CreateCommand();
                                         cmd.CommandText = "declare @demo varchar(max) exec dbo.sp_CreateSerialCode 'Depart','D',6,@demo output select @demo";
                                         deptid = cmd.ExecuteScalar().ToString();
                                         db.Database.Connection.Close();
                                     }
                                     catch (Exception ex)
                                     {
                                         res.ErrorMessage = "数据库操作出错";
                                         res.HasError = false;
                                         return Json(res, "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
                                     }
                               
                                    #endregion
                                     if (ds.Tables[0].Rows[i]["子部门"].ToString().Trim() == "")
                                     {
                                         #region 新建没有父节点的部门
                                         try
                                         {
                                             hr_Department department = new hr_Department
                                             {
                                                 DeptID = deptid,
                                                 Name = ds.Tables[0].Rows[i]["部门"].ToString().Trim(),
                                                 ManagerID = managerID,
                                                 ParentID = "",
                                                 CompanyID = company[0],
                                                 BUID = buid.Count>0?buid[0]:"",
                                                 Description = ds.Tables[0].Rows[i]["备注"].ToString().Trim(),
                                                 IsActive = true,
                                                 SortID = sort
                                             };
                                             department.ResetCreated();
                                             db.hr_Depts.Add(department);
                                             db.SaveChanges();
                                             rightIndex.Append((i + 1).ToString() + ',');
                                         }
                                         catch (Exception ex)
                                         {
                                             errorIndex.Append((i + 1).ToString() + ',');
                                             res.ErrorMessage = skipIndex + ":" + rightIndex + ":" + errorIndex;
                                             res.HasError = true;
                                             return Json(res, "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
                                         }
                                         #endregion
                                     }
                                     else {
                                         #region 新建有父节点的部门
                                         try
                                         {
                                             deptName = ds.Tables[0].Rows[i]["部门"].ToString().Trim();
                                             var parentid = (from dept in db.hr_Depts where dept.Name.Equals(deptName) && dept.IsActive && dept.CompanyID.Equals(companyid) select dept.DeptID.ToString()).ToList();
                                             hr_Department department = new hr_Department
                                             {
                                                 DeptID = deptid,
                                                 Name = ds.Tables[0].Rows[i]["子部门"].ToString().Trim(),
                                                 ManagerID = managerID,
                                                 ParentID = parentid[0],
                                                 CompanyID = companyid,
                                                 BUID = buid.Count>0?buid[0]:"",
                                                 Description = ds.Tables[0].Rows[i]["备注"].ToString().Trim(),
                                                 IsActive = true,
                                                 SortID = sort
                                             };
                                             department.ResetCreated();
                                             db.hr_Depts.Add(department);
                                             db.SaveChanges();
                                             rightIndex.Append((i + 1).ToString() + ',');
                                         }
                                         catch (Exception ex)
                                         {
                                             errorIndex.Append((i + 1).ToString() + ',');
                                             res.ErrorMessage = skipIndex + ":" + rightIndex + ":" + errorIndex;
                                             res.HasError = true;
                                             return Json(res, "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
                                         }
                                         #endregion
                                     }
                                 }
                             }
                         }
                     }
                     res.ErrorMessage = skipIndex + ":" + rightIndex + ":" + errorIndex;
                     res.HasError = true;
                     return Json(res, "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
                }
            }
            return Json(res, "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
        }

    }
}