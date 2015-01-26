using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using System.Data;
using Panasia.Gemini.Web.Models;
using System.Data.SqlClient;
using Panasia.Core.Sys;
using Panasia.Core.Auth;

namespace Panasia.Gemini.Web.Controllers
{
    public class ApproveController : Controller
    {
        Panasia.Core.Sys.SysContext dbSys = new Core.Sys.SysContext();

        SystemParameterFunctions spf = new Panasia.Core.Sys.SystemParameterFunctions(); //fgh add 2015-01-19

        /// <summary>
        /// 送签
        /// </summary>
        /// <returns></returns>
        public ActionResult IntoApproveFlow()
        {
            bool HasError = false;
            string ErrorMessage = "";
            List<Dictionary<string, string>> Data = new List<Dictionary<string, string>>();
            int ExecuteCount = 0;
            try
            {
                if (string.IsNullOrEmpty(Request.QueryString["table_name"]) || string.IsNullOrEmpty(Request.QueryString["keys"]))
                {
                    HasError = true;
                    ErrorMessage = "未提供必要的参数";
                }
                else
                {
                    string CompanyID = spf.GetCurrentUserProfiles("CompanyID").ToString();
                    string EmployeeID = spf.GetCurrentUserProfiles("EmployeeID").ToString();
                    if (string.IsNullOrEmpty(EmployeeID))
                    {
                        return Json(new { Url="/Home/TimeOut"},JsonRequestBehavior.AllowGet);
                    }
                    string DeptID = spf.GetCurrentUserProfiles("DeptID").ToString();
                    string JobID = spf.GetCurrentUserProfiles("JobID").ToString();
                    string UserID = spf.GetCurrentUserProfiles("UserID").ToString();

                    string TableName = Request.QueryString["table_name"];
                    string[] keys = Request.QueryString["keys"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    SysContext db = new SysContext();

                    fl_Form template = db.fl_Form.FirstOrDefault(t => t.DataFrom == ("fl_" + TableName) && t.Companys.Contains(CompanyID.ToString()));

                    int FormID = 0;
                    if (template != null)
                    {
                        FormID = template.FormID;
                    }
                    else
                    {
                        HasError = true;
                        ErrorMessage = "OA系统不存在此单据的相关配置";
                        return Json(new { HasError = HasError, ErrorMessage = ErrorMessage }, JsonRequestBehavior.AllowGet);
                    }
               
                    //向OA业务表插入相同的数据
                    //获取该表主键
                    DataTable dtKeys = new DataTable();
                    //不知道怎么用ef执行系统存储过程，只好用回ado
                    using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["PASys"].ConnectionString))
                    {
                        SqlCommand cmd = new SqlCommand("dbo.sp_pkeys", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@table_name", "fl_" + TableName));
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        conn.Open();
                        sda.Fill(dtKeys);
                        conn.Close();

                    }
                    string key = dtKeys.AsEnumerable().Where(pkeys => pkeys["COLUMN_NAME"].ToString() != "Version")
                        .OrderBy(pkeys => Convert.ToInt32(pkeys["KEY_SEQ"]))
                        .Select(pkeys => pkeys["COLUMN_NAME"].ToString()).Take(1).First();

                    Assembly assembly = Assembly.Load("Panasia.Core.Sys");
                    int version = 1;

                    //创建实例
                    object entity_fl = assembly.CreateInstance("Panasia.Core.Sys.fl_" + TableName);//OA业务表
                    object entity_Business = assembly.CreateInstance("Panasia.Core.Sys." + TableName);//其他系统业务表
                    //获得2个实例的数据类型

                    Type type_fl = entity_fl.GetType();
                    Type type_Business = entity_Business.GetType();
                    //创建数据访问实例
                    System.Data.Entity.DbSet db_fl = db.Set(type_fl);
                    System.Data.Entity.DbSet db_hr = db.Set(type_Business);
                    //这里的逻辑是，OA业务表拥有和其他系统的对应业务表完全一致的字段，只会多，不会少。如果有多的字段，除了Version，其他肯定是之后在签核过程中使用的，和现在没有关系
                    PropertyInfo[] PropertyList_fl = type_fl.GetProperties();//获得OA业务表的全部字段
                    PropertyInfo[] PropertyList_hr = type_Business.GetProperties();//获得人事业务表的全部字段

                    foreach (string id in keys)
                    {
                        #region 1. 向OA业务表插入业务表数据
                        //先查询version版本号
                        int maxVersion = db.Database.SqlQuery<int>("SELECT ISNULL(MAX([Version]),0) FROM fl_" + TableName + " WHERE " + key + " = '" + id + "'").First();
                        version = maxVersion + 1;//最新version
                      
                        //获得送签的业务表数据
                        entity_Business = db_hr.Find(id);                     

                        fl_MappingForm entity_fl_MappingForm = db.fl_MappingForm.SingleOrDefault(map => map.FormNo == id && map.Table_Name == "fl_" + TableName);
                        if (entity_fl_MappingForm != null && entity_fl_MappingForm.Approved == 1)
                        {
                            //如果已存在状态为1的相关数据，跳过执行（驳回的状态为0）
                            continue;
                        }
                        //如果没有则向映射表插入一条数据，表示进入签核流程
                        if (entity_fl_MappingForm == null)
                        {
                            entity_fl_MappingForm = new fl_MappingForm();
                            entity_fl_MappingForm.FormNo = id;
                            entity_fl_MappingForm.Table_Name = "fl_" + TableName;
                            entity_fl_MappingForm.Approved = 1;
                            entity_fl_MappingForm.ApprovedDate = DateTime.Now;
                            entity_fl_MappingForm.TemplateUrl = "";//这个暂时不用了
                            entity_fl_MappingForm.ResetCreated();
                            db.fl_MappingForm.Add(entity_fl_MappingForm);
                        }
                        else
                        {
                            //如果已经有了则只要把状态变成1就可以了
                            entity_fl_MappingForm.Approved = 1;
                            entity_fl_MappingForm.ApprovedDate = DateTime.Now;
                            entity_fl_MappingForm.ResetUpdated();
                        }



                        //赋值运算
                        foreach (PropertyInfo property_hr in PropertyList_hr)
                        {
                            string column_name = property_hr.Name;
                            if (column_name != "AutoKey")//自增列不需要
                            {
                                object value = property_hr.GetValue(entity_Business, null);//获得该字段的值
                                IEnumerable<PropertyInfo> informations = PropertyList_fl.Where(p => p.Name == column_name);
                                if (informations.Count() == 1)
                                {
                                    informations.First().SetValue(entity_fl, value, null);
                                }
                            }
                        }
                        PropertyList_fl.Where(p => p.Name == "Version").First().SetValue(entity_fl, version, null);//OA业务表比较重要的版本号
                        type_fl.GetMethod("ResetCreated").Invoke(entity_fl, null);//执行SysEntity的重置方法

                        db_fl.Add(entity_fl);//添加数据

                        //特例fl_ReimbursementDetail,是fl_Reimbursement的子表，存放报销明细数据，目前看来各单据只有报销有子表，所以特殊处理
                        if (TableName == "Reimbursement")
                        {
                            //由于是特例，直接使用类来操作就可以了，不用像上面那样兼顾所有类型
                            //不能用linq
                            List<fl_ReimbursementDetail> details_fl = new List<fl_ReimbursementDetail>();
                            IQueryable<hr_ReimbursementDetail> details_hr = db.hr_ReimbursementDetail.Where(rd => rd.RID == id);
                            foreach (hr_ReimbursementDetail detail_hr in details_hr)
                            {
                                details_fl.Add(new fl_ReimbursementDetail
                                {
                                    RID = detail_hr.RID,
                                    Version = version,
                                    ItemNo = detail_hr.ItemNo,
                                    CostCategories = detail_hr.CostCategories,
                                    SubCostCategories = detail_hr.SubCostCategories,
                                    Amount = detail_hr.Amount,
                                    IsActive = detail_hr.IsActive
                                });
                            }
                            details_fl.ForEach(d => d.ResetCreated());
                            db.fl_ReimbursementDetail.AddRange(details_fl);
                        }
                        #endregion

                        #region 2. 查找签核者
                        //向签核流表插入下一个需要审核的人的状态
                        //当fl_MappingForm没有记录时表示初次送签，需要查找签核者送入到签核状态表
                        //if (entity_fl_MappingForm == null)
                        //{
                        bool bl= InsertFlow(EmployeeID, DeptID, JobID, id, "fl_" + TableName, entity_fl_MappingForm, FormID, db, entity_fl, version);


                        if (!bl) //没有设置流程或者没有符合条件的签核者，则结单
                        {
                            //更新映射表的状态，表示已签核完毕，结单
                            entity_fl_MappingForm.Approved = 0;
                            entity_fl_MappingForm.ApprovedDate = DateTime.Now;


                            HasError = true;
                            ErrorMessage = "送签失败：OA系统没有设置流程或者没有符合条件的签核者";

                            //fgh 2014-01-20 add将每次签核的状态都添加到fl_ApproveLog表
                            InsertApproveFlow_Log(version, FormID, id, EmployeeID, 0, DeptID,
                            JobID, "S", "S", DateTime.Now, db, ErrorMessage);
                            continue; 
                        }
                        else
                        {

                            #region 1. 发起者写入签核状态表fl_ApproveState
                            //插入自己的状态，实际上发起信息也应当作为流程之一被记录
                            fl_ApproveState entity_fl_ApproveFlow_mine = new fl_ApproveState();
                            entity_fl_ApproveFlow_mine.FormID = FormID;
                            entity_fl_ApproveFlow_mine.FormNo = id;
                            entity_fl_ApproveFlow_mine.EmployeeID = EmployeeID;//发起者的ID
                            entity_fl_ApproveFlow_mine.Employee_Sort = 0;//发起者的顺序就是0
                            entity_fl_ApproveFlow_mine.DeptID = DeptID;//发起者的部门
                            entity_fl_ApproveFlow_mine.JobID = JobID;//发起者的职位
                            entity_fl_ApproveFlow_mine.Confirm_Class = "S";
                            entity_fl_ApproveFlow_mine.Confirm_Stat = "S";//送签用S
                            entity_fl_ApproveFlow_mine.Confirm_Time = DateTime.Now;
                            entity_fl_ApproveFlow_mine.ApproverID = null;
                            entity_fl_ApproveFlow_mine.ResetCreated();
                            db.fl_ApproveState.Add(entity_fl_ApproveFlow_mine);

                            //fgh 2014-01-20 add将每次签核的状态都添加到fl_ApproveLog表
                            InsertApproveFlow_Log(version, FormID, id, EmployeeID, 0, DeptID,
                            JobID, "S", "S", DateTime.Now, db, string.Empty);
                            #endregion

                        }
                        //}
                        #endregion
                        
                        ExecuteCount++;

                        Dictionary<string, string> row = new Dictionary<string, string>();
                        row.Add("Key", id);
                        row.Add("ApproveStatus", entity_fl_MappingForm.Approved.ToString());
                        Data.Add(row);
                    }
                    db.SaveChanges();
                    db.Database.Connection.Open();
                    var cmds = db.Database.Connection.CreateCommand();
                    foreach (string fno in keys)
                    {
                        cmds.CommandText = "exec sp_fl_ApproveMessageRemind @FormID,@FormNo,@UserID";
                        cmds.Parameters.Add(new SqlParameter("@FormID", TableName));
                        cmds.Parameters.Add(new SqlParameter("@FormNo", fno));
                        cmds.Parameters.Add(new SqlParameter("@UserID", UserID));
                        cmds.ExecuteScalar();
                    }
                    db.Database.Connection.Close();
                }
            }
            catch (Exception e)
            {
                HasError = true;
                ErrorMessage = e.Message;
            }
            return Json(new { HasError = HasError, ErrorMessage = ErrorMessage, ExecuteCount = ExecuteCount, Data = Data }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 送签 (原杨磊版本)
        /// </summary>
        /// <returns></returns>
        public ActionResult IntoApproveFlow1()
        {
            bool HasError = false;
            string ErrorMessage = "";
            List<Dictionary<string,string>> Data = new List<Dictionary<string,string>>();
            int ExecuteCount = 0;
            try
            {
                if (!string.IsNullOrEmpty(Request.QueryString["table_name"]) && !string.IsNullOrEmpty(Request.QueryString["keys"]))
                {
                    string TableName = Request.QueryString["table_name"];
                    string[] keys = Request.QueryString["keys"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    SysContext db = new SysContext();
                    //向OA业务表插入相同的数据
                    //获取该表主键
                    DataTable dtKeys = new DataTable();
                    //不知道怎么用ef执行系统存储过程，只好用回ado
                    using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["PASys"].ConnectionString))
                    {
                        SqlCommand cmd = new SqlCommand("dbo.sp_pkeys", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@table_name", "fl_" + TableName));
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        conn.Open();
                        sda.Fill(dtKeys);
                        conn.Close();
                        
                    }
                    string key = dtKeys.AsEnumerable().Where(pkeys => pkeys["COLUMN_NAME"].ToString() != "Version")
                        .OrderBy(pkeys => Convert.ToInt32(pkeys["KEY_SEQ"]))
                        .Select(pkeys => pkeys["COLUMN_NAME"].ToString()).Take(1).First();

                    Assembly assembly = Assembly.Load("Panasia.Core.Sys");
                    int version = 1;
                    foreach (string id in keys)
                    {

                        //先查询version版本号
                        int maxVersion = db.Database.SqlQuery<int>("SELECT ISNULL(MAX([Version]),0) FROM fl_" + TableName + " WHERE " + key + " = '" + id + "'").First();
                        version = maxVersion + 1;//最新version

                        //创建实例
                        object entity_fl = assembly.CreateInstance("Panasia.Core.Sys.fl_" + TableName);//OA业务表
                        object entity_Business = assembly.CreateInstance("Panasia.Core.Sys." + TableName);//其他系统业务表
                        //获得2个实例的数据类型

                        Type type_fl = entity_fl.GetType();
                        Type type_Business = entity_Business.GetType();
                        //创建数据访问实例
                        System.Data.Entity.DbSet db_fl = db.Set(type_fl);
                        System.Data.Entity.DbSet db_hr = db.Set(type_Business);
                        //获得送签的业务表数据
                        entity_Business = db_hr.Find(id);
                        //这里的逻辑是，OA业务表拥有和其他系统的对应业务表完全一致的字段，只会多，不会少。如果有多的字段，除了Version，其他肯定是之后在签核过程中使用的，和现在没有关系
                        PropertyInfo[] PropertyList_fl = type_fl.GetProperties();//获得OA业务表的全部字段
                        PropertyInfo[] PropertyList_hr = type_Business.GetProperties();//获得人事业务表的全部字段

                        //获得提交单据时的公司ID
                        object CompanyID = PropertyList_hr.Where(p => p.Name == "CompanyID").First().GetValue(entity_Business, null);
                        //2014-12-27 fgh 因为单据与公司有关系，所以下面添加
                        //fl_Form template = db.fl_Form.FirstOrDefault(t => t.DataFrom == ("fl_" + TableName)&&t.CompanyID==CompanyID.ToString());

                        fl_Form template = db.fl_Form.FirstOrDefault(t => t.DataFrom == ("fl_" + TableName) && t.Companys.Contains( CompanyID.ToString()));

                        int TemplateID = 0;
                        if (template != null)
                        {
                            TemplateID = template.FormID;
                        }
                        else
                        {
                            HasError = true;
                            ErrorMessage = "OA系统不存在此单据的相关配置";
                            break;
                        }
                        fl_MappingForm entity_fl_MappingForm = db.fl_MappingForm.SingleOrDefault(map => map.FormNo == id && map.Table_Name == "fl_" + TableName);
                        if (entity_fl_MappingForm != null && entity_fl_MappingForm.Approved == 1)
                        {
                            //如果已存在状态为1的相关数据，跳过执行（驳回的状态为0）
                            continue;
                        }
                        //如果没有则向映射表插入一条数据，表示进入签核流程
                        if (entity_fl_MappingForm == null)
                        {
                            entity_fl_MappingForm = new fl_MappingForm();
                            entity_fl_MappingForm.FormNo = id;
                            entity_fl_MappingForm.Table_Name = "fl_" + TableName;
                            entity_fl_MappingForm.Approved = 1;
                            entity_fl_MappingForm.ApprovedDate = DateTime.Now;
                            entity_fl_MappingForm.TemplateUrl = "";//这个暂时不用了
                            entity_fl_MappingForm.ResetCreated();
                            db.fl_MappingForm.Add(entity_fl_MappingForm);
                        }
                        else
                        {
                            //如果已经有了则只要把状态变成1就可以了
                            entity_fl_MappingForm.Approved = 1;
                            entity_fl_MappingForm.ApprovedDate = DateTime.Now;
                            entity_fl_MappingForm.ResetUpdated();
                        }

                        

                        //赋值运算
                        foreach (PropertyInfo property_hr in PropertyList_hr)
                        {
                            string column_name = property_hr.Name;
                            if (column_name != "AutoKey")//自增列不需要
                            {
                                object value = property_hr.GetValue(entity_Business, null);//获得该字段的值
                                IEnumerable<PropertyInfo> informations = PropertyList_fl.Where(p => p.Name == column_name);
                                if (informations.Count() == 1)
                                {
                                    informations.First().SetValue(entity_fl, value, null);
                                }
                            }
                        }
                        PropertyList_fl.Where(p => p.Name == "Version").First().SetValue(entity_fl, version, null);//OA业务表比较重要的版本号
                        type_fl.GetMethod("ResetCreated").Invoke(entity_fl, null);//执行SysEntity的重置方法

                        db_fl.Add(entity_fl);//添加数据

                        //特例fl_ReimbursementDetail,是fl_Reimbursement的子表，存放报销明细数据，目前看来各单据只有报销有子表，所以特殊处理
                        if (TableName == "Reimbursement")
                        {
                            //由于是特例，直接使用类来操作就可以了，不用像上面那样兼顾所有类型
                            //不能用linq
                            List<fl_ReimbursementDetail> details_fl = new List<fl_ReimbursementDetail>();
                            IQueryable<hr_ReimbursementDetail> details_hr = db.hr_ReimbursementDetail.Where(rd => rd.RID == id);
                            foreach (hr_ReimbursementDetail detail_hr in details_hr)
                            {
                                details_fl.Add(new fl_ReimbursementDetail
                                             {
                                                 RID = detail_hr.RID,
                                                 Version = version,
                                                 ItemNo = detail_hr.ItemNo,
                                                 CostCategories = detail_hr.CostCategories,
                                                 SubCostCategories = detail_hr.SubCostCategories,
                                                 Amount = detail_hr.Amount,
                                                 IsActive = detail_hr.IsActive
                                             });
                            }
                            details_fl.ForEach(d => d.ResetCreated());
                            db.fl_ReimbursementDetail.AddRange(details_fl);
                        }

                        //向签核流表插入下一个需要审核的人的状态
                        InsertFlow_old(id, "fl_" + TableName, entity_fl_MappingForm, TemplateID, db, entity_fl, CompanyID.ToString());
                        ExecuteCount++;

                        Dictionary<string, string> row = new Dictionary<string, string>();
                        row.Add("Key", id);
                        row.Add("ApproveStatus", entity_fl_MappingForm.Approved.ToString());
                        Data.Add(row);
                    }
                    db.SaveChanges();
                    db.Database.Connection.Open();
                    var cmds = db.Database.Connection.CreateCommand();
                    foreach (string fno in keys)
                    {
                    cmds.CommandText = "exec sp_fl_ApproveMessageRemind @FormID,@FormNo,@UserID";
                        cmds.Parameters.Add(new SqlParameter("@FormID", TableName));
                        cmds.Parameters.Add(new SqlParameter("@FormNo", fno));
                        cmds.Parameters.Add(new SqlParameter("@UserID", Panasia.Core.Sys.SysService.GetCurrentUser().UserID));
                        cmds.ExecuteScalar();
                    }
                    db.Database.Connection.Close();
                }
                else
                {
                    HasError = true;
                    ErrorMessage = "未提供必要的参数";
                }
            }
            catch(Exception e)
            {
                HasError = true;
                ErrorMessage = e.Message;
            }
            return Json(new { HasError = HasError, ErrorMessage = ErrorMessage, ExecuteCount = ExecuteCount, Data = Data }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 产品送签
        /// fgh 2014-12-26 add
        /// </summary>
        /// <returns></returns>
        public ActionResult IntoApproveFlowOrder()
        {
            bool HasError = false;
            string ErrorMessage = "";
            List<Dictionary<string, string>> Data = new List<Dictionary<string, string>>();
            int ExecuteCount = 0;
            try
            {
                if (!string.IsNullOrEmpty(Request.QueryString["table_name"]) && !string.IsNullOrEmpty(Request.QueryString["keys"]))
                {
                    string TableName = Request.QueryString["table_name"];
                    string[] keys = Request.QueryString["keys"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    SysContext db = new SysContext();
                    //向OA业务表插入相同的数据
                    //获取该表主键
                    DataTable dtKeys = new DataTable();
                    //不知道怎么用ef执行系统存储过程，只好用回ado
                    using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["PASys"].ConnectionString))
                    {
                        SqlCommand cmd = new SqlCommand("dbo.sp_pkeys", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@table_name", "bi_" + TableName));
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        conn.Open();
                        sda.Fill(dtKeys);
                        conn.Close();
                    }
                    string key = dtKeys.AsEnumerable().Where(pkeys => pkeys["COLUMN_NAME"].ToString() != "Version")
                        .OrderBy(pkeys => Convert.ToInt32(pkeys["KEY_SEQ"]))
                        .Select(pkeys => pkeys["COLUMN_NAME"].ToString()).Take(1).First();

                    Assembly assembly = Assembly.Load("Panasia.Core.Sys");
                    int version = 1;
                    foreach (string orderid in keys)
                    {
                        IList<bi_OrderProduct> ilorder = db.bi_OrderProduct.Where(o => o.OrderID == orderid).ToList();
                        for (int i = 0; i < ilorder.Count; i++)
                        {
                            string id = ilorder[i].OrdPctID;
                            fl_Form template = db.fl_Form.FirstOrDefault(t => t.DataFrom == ("bi_" + TableName));
                            int TemplateID = 0;
                            if (template != null)
                            {
                                TemplateID = template.FormID;
                            }
                            else
                            {
                                HasError = true;
                                ErrorMessage = "OA系统不存在此单据的相关配置";
                                break;
                            }
                            fl_MappingForm entity_fl_MappingForm = db.fl_MappingForm.SingleOrDefault(map => map.FormNo == id && map.Table_Name == "bi_" + TableName);
                            if (entity_fl_MappingForm != null && entity_fl_MappingForm.Approved == 1)
                            {
                                //如果已存在状态为1的相关数据，跳过执行（驳回的状态为0）
                                continue;
                            }
                            //如果没有则向映射表插入一条数据，表示进入签核流程
                            if (entity_fl_MappingForm == null)
                            {
                                entity_fl_MappingForm = new fl_MappingForm();
                                entity_fl_MappingForm.FormNo = id;
                                entity_fl_MappingForm.Table_Name = "bi_" + TableName;
                                entity_fl_MappingForm.Approved = 1;
                                entity_fl_MappingForm.ApprovedDate = DateTime.Now;
                                entity_fl_MappingForm.TemplateUrl = "";//这个暂时不用了
                                entity_fl_MappingForm.ResetCreated();
                                db.fl_MappingForm.Add(entity_fl_MappingForm);
                            }
                            else
                            {
                                //如果已经有了则只要把状态变成1就可以了
                                entity_fl_MappingForm.Approved = 1;
                                entity_fl_MappingForm.ApprovedDate = DateTime.Now;
                                entity_fl_MappingForm.ResetUpdated();
                            }

                            //先查询version版本号
                            int maxVersion = db.Database.SqlQuery<int>("SELECT ISNULL(MAX([Version]),0) FROM bi_" + TableName + " WHERE " + key + " = '" + id + "'").First();
                            version = maxVersion + 1;//最新version

                            //创建实例
                            object entity_fl = assembly.CreateInstance("Panasia.Core.Sys.bi_" + TableName);//OA业务表
                            object entity_Business = assembly.CreateInstance("Panasia.Core.Sys." + TableName);//其他系统业务表
                            //获得2个实例的数据类型
                            Type type_fl = entity_fl.GetType();
                            Type type_Business = entity_Business.GetType();
                            //创建数据访问实例
                            System.Data.Entity.DbSet db_fl = db.Set(type_fl);
                            System.Data.Entity.DbSet db_hr = db.Set(type_Business);
                            //获得送签的业务表数据
                            entity_Business = db_hr.Find(id);
                            //这里的逻辑是，OA业务表拥有和其他系统的对应业务表完全一致的字段，只会多，不会少。如果有多的字段，除了Version，其他肯定是之后在签核过程中使用的，和现在没有关系
                            PropertyInfo[] PropertyList_fl = type_fl.GetProperties();//获得OA业务表的全部字段
                            PropertyInfo[] PropertyList_hr = type_Business.GetProperties();//获得人事业务表的全部字段

                            //获得提交单据时的公司ID
                            object CompanyID = PropertyList_hr.Where(p => p.Name == "CompanyID").First().GetValue(entity_Business, null);

                            //赋值运算
                            foreach (PropertyInfo property_hr in PropertyList_hr)
                            {
                                string column_name = property_hr.Name;
                                if (column_name != "AutoKey")//自增列不需要
                                {
                                    object value = property_hr.GetValue(entity_Business, null);//获得该字段的值
                                    IEnumerable<PropertyInfo> informations = PropertyList_fl.Where(p => p.Name == column_name);
                                    if (informations.Count() == 1)
                                    {
                                        informations.First().SetValue(entity_fl, value, null);
                                    }
                                }
                            }
                            PropertyList_fl.Where(p => p.Name == "Version").First().SetValue(entity_fl, version, null);//OA业务表比较重要的版本号
                            type_fl.GetMethod("ResetCreated").Invoke(entity_fl, null);//执行SysEntity的重置方法

                            db_fl.Add(entity_fl);//添加数据

                            //特例fl_ReimbursementDetail,是fl_Reimbursement的子表，存放报销明细数据，目前看来各单据只有报销有子表，所以特殊处理
                            if (TableName == "Reimbursement")
                            {
                                //由于是特例，直接使用类来操作就可以了，不用像上面那样兼顾所有类型
                                //不能用linq
                                List<fl_ReimbursementDetail> details_fl = new List<fl_ReimbursementDetail>();
                                IQueryable<hr_ReimbursementDetail> details_hr = db.hr_ReimbursementDetail.Where(rd => rd.RID == id);
                                foreach (hr_ReimbursementDetail detail_hr in details_hr)
                                {
                                    details_fl.Add(new fl_ReimbursementDetail
                                    {
                                        RID = detail_hr.RID,
                                        Version = version,
                                        ItemNo = detail_hr.ItemNo,
                                        CostCategories = detail_hr.CostCategories,
                                        SubCostCategories = detail_hr.SubCostCategories,
                                        Amount = detail_hr.Amount,
                                        IsActive = detail_hr.IsActive
                                    });
                                }
                                details_fl.ForEach(d => d.ResetCreated());
                                db.fl_ReimbursementDetail.AddRange(details_fl);
                            }

                            //向签核流表插入下一个需要审核的人的状态
                            InsertFlow_old(id, "bi_" + TableName, entity_fl_MappingForm, TemplateID, db, entity_fl, CompanyID.ToString());
                            ExecuteCount++;

                            Dictionary<string, string> row = new Dictionary<string, string>();
                            row.Add("Key", id);
                            row.Add("ApproveStatus", entity_fl_MappingForm.Approved.ToString());
                            Data.Add(row);
                        }
                    }
                    db.SaveChanges();
                }
                else
                {
                    HasError = true;
                    ErrorMessage = "未提供必要的参数";
                }
            }
            catch (Exception e)
            {
                HasError = true;
                ErrorMessage = e.Message;
            }
            return Json(new { HasError = HasError, ErrorMessage = ErrorMessage, ExecuteCount = ExecuteCount, Data = Data }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 插入送签及待签数据
        /// fgh 2014-11-28 add
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="TableName"></param>
        /// <param name="entity_fl_MappingForm"></param>
        /// <param name="TemplateID"></param>
        /// <param name="db"></param>
        /// <param name="entity_fl"></param>
        /// <param name="CompanyID"></param>
        private bool InsertFlow(string SenderEmployeeID, string SenderDeptID, string SenderJobID, string FormNo, string TableName,
            fl_MappingForm entity_fl_MappingForm, int FormID, SysContext db, object entity_fl, int FromVersion)
        {
            bool HasApprover = false;

            //为了确保下面2个值使用正确，决定不从业务表中取，万一哪个业务表没有这俩字段怎么办，从登录信息中取是绝对正确的

            #region 2. 根据发送者的职位、部门、单据编号获取单据的预定的签核者
            string ApproverCompanyID=string.Empty;
            string ApproverDeptID=string.Empty;
            string ApproverJobID=string.Empty;
            string ApproverEmployeeID=string.Empty;
            int ApproverTemplateID=0;
            int  ApproverEmployee_Sort=0;
            

            // 1111111--本部门   22222--自定义部门   333333--自定义签核者   4444444---事业部

            int currSort = 0;
            int startSort = 0;
            int associateSort = 0;//交接人所在顺位

            var nonfuzzy_forms = db.fl_Approver.Where(f => (f.FormID == FormID) && (f.CompanyID == ApproverCompanyID) && !f.IsFuzzy).OrderBy(f => f.Employee_Sort);
            int firstNonfuzzySort = 0;
            if (nonfuzzy_forms.Count() != 0)
            {
                firstNonfuzzySort = nonfuzzy_forms.Take(1).Single().Employee_Sort;
            }
            //获得第一条非模糊指定签核人之前的模糊指定数据
            var ApproveForm_TopFuzzy = db.fl_Approver.Where(f => (f.FormID == FormID) && (f.CompanyID == ApproverCompanyID) && (f.Employee_Sort < firstNonfuzzySort)).OrderBy(f => f.Employee_Sort);
            if (ApproveForm_TopFuzzy.Count() > 0)
            {
                foreach (var item in ApproveForm_TopFuzzy)
                {
                   
                        associateSort = item.Employee_Sort - 1;
                }
            }
            currSort = Math.Min(associateSort, startSort);


            //var ApproverList = db.fl_Approver.Where(f => (f.FormID == FormID)).OrderBy(f => f.Employee_Sort);
             var ApproverList = GetApproveFormByRules(currSort, TableName, FormID, db, entity_fl, ApproverCompanyID);
            foreach (var item in ApproverList)
            {
                //if (string.IsNullOrEmpty(item.EmployeeID))
                //{
                    if (item.DeptID == "111111")
                    {
                        ApproverDeptID = SenderDeptID;
                        //ApproverEmployeeID = GetNextApprover(SenderDeptID, item.JobID);
                    }
                    else if (item.DeptID == "222222")
                    {
                        Assembly assembly = Assembly.Load("Panasia.Core.Sys");
                        //创建实例
                        object type_Business1 = assembly.CreateInstance("Panasia.Core.Sys." + TableName.Replace("fl_", ""));//OA业务表
                        //获得2个实例的数据类型
                        Type type_Business = type_Business1.GetType();
                        //创建数据访问实例
                        System.Data.Entity.DbSet db_Business = db.Set(type_Business);
                        type_Business1 = db_Business.Find(FormNo);
                        //这里的逻辑是，OA业务表拥有和其他系统的对应业务表完全一致的字段，只会多，不会少。如果有多的字段，除了Version，其他肯定是之后在签核过程中使用的，和现在没有关系
                        PropertyInfo[] PropertyList_fl = type_Business.GetProperties();//获得OA业务表的全部字段

                        //获得提交单据时的公司ID
                        object aDept = PropertyList_fl.Where(p => p.Name == item.DeptIDCondition).First().GetValue(type_Business1, null);
                        ApproverDeptID = aDept.ToString();
                        //ApproverEmployeeID = GetNextApprover(ApproverDeptID, item.JobID);
                    } 
                    else if (item.DeptID == "444444")
                    {
                        DataTable dt = GetNextApproverByBU(SenderDeptID);
                        if (dt.Rows.Count > 0)
                        {

                            ApproverDeptID = dt.Rows[0]["DeptID"].ToString(); //userInfo.Rows[0]["BUManagerDeptID"].ToString();
                            ApproverEmployeeID = dt.Rows[0]["EmployeeID"].ToString();
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        ApproverDeptID = item.DeptID;
                        //ApproverEmployeeID = GetNextApprover(item.DeptID, item.JobID);
                    }
                    
                //}
                //else
                //{
                    if (item.EmployeeID == "333333")
                    {
                        Assembly assembly = Assembly.Load("Panasia.Core.Sys");
                        //创建实例
                        object type_Business1 = assembly.CreateInstance("Panasia.Core.Sys." + TableName.Replace("fl_", ""));//OA业务表

                        //获得2个实例的数据类型

                        Type type_Business = type_Business1.GetType();

                        //创建数据访问实例
                        System.Data.Entity.DbSet db_Business = db.Set(type_Business);

                        type_Business1 = db_Business.Find(FormNo);
                        //这里的逻辑是，OA业务表拥有和其他系统的对应业务表完全一致的字段，只会多，不会少。如果有多的字段，除了Version，其他肯定是之后在签核过程中使用的，和现在没有关系
                        PropertyInfo[] PropertyList_fl = type_Business.GetProperties();//获得OA业务表的全部字段


                        //获得提交单据时的公司ID
                        object Employee1 = PropertyList_fl.Where(p => p.Name == item.EmployeeIDCondition).First().GetValue(type_Business1, null);
                        ApproverEmployeeID = Employee1.ToString();
                    }
                    else if (string.IsNullOrEmpty(item.EmployeeID))
                    {
                        DataTable dt = GetNextApprover(ApproverDeptID, item.JobID);
                        if (dt.Rows.Count > 0)
                        {

                            ApproverEmployeeID = GetNextApprover(ApproverDeptID, item.JobID).Rows[0]["EmployeeID"].ToString();
                            ApproverDeptID = GetNextApprover(ApproverDeptID, item.JobID).Rows[0]["DeptID"].ToString();
                        }
                        else
                        {
                            continue; 
                        }
                    }
                    else
                    {
                        ApproverEmployeeID = item.EmployeeID;
                    }
                //}

                //如果签核者为空，就跳出，直接找下一签核者
                    if (string.IsNullOrEmpty(ApproverEmployeeID))
                        continue; 

                //根据部门、职位查找签核者的员工编号
                fl_ApproveState entity_fl_ApproveFlow = new fl_ApproveState();
                entity_fl_ApproveFlow.FormID = FormID;
                entity_fl_ApproveFlow.FormNo = FormNo;
                entity_fl_ApproveFlow.EmployeeID = ApproverEmployeeID;//下一个签核者的ID，如果是职位的话这个为空
                entity_fl_ApproveFlow.Employee_Sort = item.Employee_Sort;//下一个签核者的顺序
                entity_fl_ApproveFlow.DeptID = ApproverDeptID;//下一个签核者的部门，如果是经理则与自己相同，如果自己是经理则需要去fl_ApproveForm查询
                entity_fl_ApproveFlow.JobID = item.JobID;//下一个签核者的职位，如果没有指定人员这个就必须有，如果指定了人员那就是指定人员的职位
                entity_fl_ApproveFlow.Confirm_Class = "A";
                entity_fl_ApproveFlow.Confirm_Stat = "X";//A签核 X待签 R驳回
                entity_fl_ApproveFlow.Confirm_Time = DateTime.Now;
                entity_fl_ApproveFlow.ApproverID = item.ApproverID;

                entity_fl_ApproveFlow.ResetCreated();
                db.fl_ApproveState.Add(entity_fl_ApproveFlow);
                HasApprover = true;
            }
            #endregion
            return HasApprover;
        }

        private void InsertApproveFlow_Log(int FromVersion, int FormID, string FormNo, string EmployeeID, int SortID, string DeptID,
            string JobID, string Confirm_Class, string Confirm_Stat, DateTime Confirm_Time, SysContext db, string Suggestion)
        {
            fl_ApproveLog entity_fl_ApproveLog = new fl_ApproveLog();
            entity_fl_ApproveLog.Version = FromVersion;
            entity_fl_ApproveLog.FormID = FormID;
            entity_fl_ApproveLog.FormNo = FormNo;
            entity_fl_ApproveLog.EmployeeID = EmployeeID;//发起者的ID
            entity_fl_ApproveLog.Employee_Sort = SortID;//发起者的顺序就是0
            entity_fl_ApproveLog.DeptID = DeptID;//发起者的部门
            entity_fl_ApproveLog.JobID = JobID;//发起者的职位
            entity_fl_ApproveLog.Confirm_Class = Confirm_Class;
            entity_fl_ApproveLog.Confirm_Stat = Confirm_Stat;//送签用S
            entity_fl_ApproveLog.Confirm_Time = Confirm_Time;
            entity_fl_ApproveLog.Suggestion = Suggestion;
            entity_fl_ApproveLog.ResetCreated();
            db.fl_ApproveLog.Add(entity_fl_ApproveLog);
        }

        //根据部门、职位查找签核者ID fgh 2015-01-20 add
        public DataTable GetNextApprover(string DeptID, string JobID)
        {
            //string nextApprover = string.Empty;

            DataTable UserInfo = new DataTable();
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["PASys"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(@"select EmployeeID,DeptID from fun_hr_GetUserDeptJobs(@DeptID,@JobID)", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SqlParameter("@DeptID", DeptID));
                cmd.Parameters.Add(new SqlParameter("@JobID", JobID));
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                conn.Open();
                //nextApprover = (string)cmd.ExecuteScalar();
                sda.Fill(UserInfo);
                conn.Close();
            }

            return UserInfo;
        }
        //根据发起者的部门、职位查找自定义的事业部的负责人签核者ID fgh 2015-01-20 add
        public DataTable GetNextApproverByBU(string DeptID)
        {
            string nextApprover = string.Empty;

            DataTable UserInfo = new DataTable();
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["PASys"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(@"SELECT    dbo.hr_Employee.DeptID, dbo.hr_Employee.EmployeeID
                                            FROM      dbo.hr_Department INNER JOIN
                                                            dbo.hr_BU ON dbo.hr_Department.BUID = dbo.hr_BU.BUID INNER JOIN
                                                            dbo.hr_Employee ON dbo.hr_BU.ManagerID = dbo.hr_Employee.EmployeeID
                                            WHERE   (dbo.hr_Department.DeptID = @DeptID)", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SqlParameter("@DeptID", DeptID));
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                conn.Open();
                //nextApprover = (string)cmd.ExecuteScalar();
                sda.Fill(UserInfo);
                conn.Close();
            }

            return UserInfo;
        }

        /// <summary>
        /// 插入送签及待签数据(杨磊版本)
        /// fgh 2014-11-28 add
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="TableName"></param>
        /// <param name="entity_fl_MappingForm"></param>
        /// <param name="TemplateID"></param>
        /// <param name="db"></param>
        /// <param name="entity_fl"></param>
        /// <param name="CompanyID"></param>
        private void InsertFlow_old(string ID, string TableName, fl_MappingForm entity_fl_MappingForm, int TemplateID, SysContext db, object entity_fl, string CompanyID)
        {
            //为了确保下面2个值使用正确，决定不从业务表中取，万一哪个业务表没有这俩字段怎么办，从登录信息中取是绝对正确的
            Panasia.Core.Sys.SystemParameterFunctions sysParameter = new Core.Sys.SystemParameterFunctions();
            string userID = sysParameter.GetCurrentUserID("").ToString();

            hr_Employee emp = dbSys.hr_Employees.Single(e => e.UserID == userID);
            string employee_id = emp.EmployeeID;
            string department_id = emp.DeptID;
            string job_id = emp.JobID;
            //int fid = db.fl_Form.Single(f => f.CompanyID == CompanyID && f.DataFrom == TableName).FormID;
            //fgh 2015-01-19 modify 帅选公司
            int fid = db.fl_Form.Single(f => f.Companys.Contains(CompanyID) && f.DataFrom == TableName).FormID;

            //插入自己的状态，实际上发起信息也应当作为流程之一被记录
            fl_ApproveState entity_fl_ApproveFlow_mine = new fl_ApproveState();
            entity_fl_ApproveFlow_mine.FormID = fid;
            entity_fl_ApproveFlow_mine.FormNo = ID;
            entity_fl_ApproveFlow_mine.EmployeeID = employee_id;//发起者的ID
            entity_fl_ApproveFlow_mine.Employee_Sort = 0;//发起者的顺序就是0
            entity_fl_ApproveFlow_mine.DeptID = department_id;//发起者的部门
            entity_fl_ApproveFlow_mine.JobID = emp.JobID;//发起者的职位
            entity_fl_ApproveFlow_mine.Confirm_Class = "S";
            entity_fl_ApproveFlow_mine.Confirm_Stat = "S";//送签用S
            entity_fl_ApproveFlow_mine.Confirm_Time = DateTime.Now;
            entity_fl_ApproveFlow_mine.ApproverID = null;
            entity_fl_ApproveFlow_mine.ResetCreated();
            db.fl_ApproveState.Add(entity_fl_ApproveFlow_mine);
            
            
            DataTable userInfo = UserInfo(userID);
            int currSort = 0;
            int startSort = 0;
            int associateSort = 0;//交接人所在顺位

            var nonfuzzy_forms = db.fl_Approver.Where(f => (f.FormID == TemplateID) && (f.CompanyID == CompanyID) && !f.IsFuzzy).OrderBy(f => f.Employee_Sort);
            int firstNonfuzzySort = 0;
            if (nonfuzzy_forms.Count() != 0)
            {
                firstNonfuzzySort = nonfuzzy_forms.Take(1).Single().Employee_Sort;
            }
            //获得第一条非模糊指定签核人之前的模糊指定数据
            var ApproveForm_TopFuzzy = db.fl_Approver.Where(f => (f.FormID == TemplateID) && (f.CompanyID == CompanyID) && f.IsFuzzy && (f.Employee_Sort < firstNonfuzzySort)).OrderBy(f => f.Employee_Sort);
            if (ApproveForm_TopFuzzy.Count() > 0)
            {
                foreach (var item in ApproveForm_TopFuzzy)
                {
                    if (item.FuzzyType.Value != 4) //4是交接人，如果模糊指定了交接人，其实就是自己的部门职位，那就不需要判断了，不跳过
                    {
                        if (GetEmployeeIDByFuzzyType(item.FuzzyType.Value, userInfo) == employee_id ||
                            (GetDeptIDByFuzzyType(item.FuzzyType.Value, userInfo) == department_id && GetJobIDByFuzzyType(item.FuzzyType.Value, userInfo) == job_id))
                        {
                            //模糊指定时，如果员工ID相同或者部门职位ID相同则表示这个签核人就是自己，那么就应该跳过，第一个签核人就变成了下一条
                            startSort = item.Employee_Sort;
                            break;
                        }
                    }
                    else
                    {
                        associateSort = item.Employee_Sort - 1;
                    }
                }
            }
            currSort = Math.Min(associateSort, startSort);

            bool NextSort = false;
            bool HasApprover = false;
            do
            {
                var ApproveForm = GetApproveFormByRules(currSort, TableName, TemplateID, db, entity_fl, CompanyID);
                NextSort = false;//假定下个顺位没有签核者
                if (ApproveForm != null && ApproveForm.Count() > 0)
                {
                    NextSort = true;
                    //考虑到每一步都可能有多个签核者同时进行，每次必须以循环的方式插入相应数量的数据
                    foreach (var item in ApproveForm)
                    {
                        string EmployeeID = item.IsFuzzy ? GetEmployeeIDByFuzzyType(item.FuzzyType.Value, userInfo) : item.EmployeeID;//模糊指定主管或总监因为都是直接关系到负责人所以都有具体的员工ID
                        string DeptID = item.IsFuzzy ? GetDeptIDByFuzzyType(item.FuzzyType.Value, userInfo) : item.DeptID;
                        string JobID = item.IsFuzzy ? GetJobIDByFuzzyType(item.FuzzyType.Value, userInfo) : item.JobID;
                        if (JobID == "" && EmployeeID == "")
                        {
                            //这个情况表示模糊指定到的位置上没有对应人员
                            continue;
                        }
                        if (DeptID == "111111")
                        {
                            DeptID = emp.DeptID;
                            
                        } 
                        else if(DeptID=="222222")
                        {
                            Assembly assembly = Assembly.Load("Panasia.Core.Sys");
                            //创建实例
                            //创建实例
                            object type_Business1 = assembly.CreateInstance("Panasia.Core.Sys." + TableName.Replace("fl_", ""));//OA业务表

                            //获得2个实例的数据类型

                            Type type_Business = type_Business1.GetType();

                            //创建数据访问实例
                            System.Data.Entity.DbSet db_Business = db.Set(type_Business);

                            type_Business1 = db_Business.Find(ID);
                            //这里的逻辑是，OA业务表拥有和其他系统的对应业务表完全一致的字段，只会多，不会少。如果有多的字段，除了Version，其他肯定是之后在签核过程中使用的，和现在没有关系
                            PropertyInfo[] PropertyList_fl = type_Business.GetProperties();//获得OA业务表的全部字段



                            //获得提交单据时的公司ID
                            object Dept1 = PropertyList_fl.Where(p => p.Name == item.DeptIDCondition).First().GetValue(type_Business1, null);
                           DeptID = Dept1.ToString();
                        }
                        else if(DeptID=="444444")
                        {
                            DeptID = userInfo.Rows[0]["BUManagerDeptID"].ToString();
                            EmployeeID = userInfo.Rows[0]["BUManagerID"].ToString();
                        }


                        if (EmployeeID == "333333")
                        {
                            Assembly assembly = Assembly.Load("Panasia.Core.Sys");
                            //创建实例
                            object type_Business1 = assembly.CreateInstance("Panasia.Core.Sys." + TableName.Replace("fl_", ""));//OA业务表

                            //获得2个实例的数据类型

                            Type type_Business = type_Business1.GetType();

                            //创建数据访问实例
                            System.Data.Entity.DbSet db_Business = db.Set(type_Business);

                            type_Business1 = db_Business.Find(ID);
                            //这里的逻辑是，OA业务表拥有和其他系统的对应业务表完全一致的字段，只会多，不会少。如果有多的字段，除了Version，其他肯定是之后在签核过程中使用的，和现在没有关系
                            PropertyInfo[] PropertyList_fl = type_Business.GetProperties();//获得OA业务表的全部字段


                            //获得提交单据时的公司ID
                            object Employee1 = PropertyList_fl.Where(p => p.Name == item.EmployeeIDCondition).First().GetValue(type_Business1, null);
                            EmployeeID = Employee1.ToString();
                        }


                        fl_ApproveState entity_fl_ApproveFlow = new fl_ApproveState();
                        entity_fl_ApproveFlow.FormID = fid;
                        entity_fl_ApproveFlow.FormNo = ID;
                        entity_fl_ApproveFlow.EmployeeID = EmployeeID;//下一个签核者的ID，如果是职位的话这个为空
                        entity_fl_ApproveFlow.Employee_Sort = item.Employee_Sort;//下一个签核者的顺序
                        entity_fl_ApproveFlow.DeptID = DeptID;//下一个签核者的部门，如果是经理则与自己相同，如果自己是经理则需要去fl_ApproveForm查询
                        entity_fl_ApproveFlow.JobID = JobID;//下一个签核者的职位，如果没有指定人员这个就必须有，如果指定了人员那就是指定人员的职位
                        entity_fl_ApproveFlow.Confirm_Class = "A";
                        entity_fl_ApproveFlow.Confirm_Stat = "X";//A签核 X待签 R驳回
                        entity_fl_ApproveFlow.Confirm_Time = DateTime.Now;
                        entity_fl_ApproveFlow.ApproverID = item.ApproverID;
                        int empcount = 0;
                        if (EmployeeID == "")
                        {
                            IList<hr_Employee> hremp = db.hr_Employees.Where(e => e.DeptID == DeptID && e.JobID == JobID).ToList();
                            empcount = hremp.Count;
                            if (empcount == 0)
                            {
                                int jobsort = db.hr_Jobs.Single(j => j.JobID == JobID).SortID;
                                var hremp0 = db.hr_Employees.Join(db.hr_Jobs.AsEnumerable(), e => e.JobID, j => j.JobID, (e, j) => new { e.EmployeeID,j.JobID,e.DeptID,j.SortID }).Where(e => e.DeptID == DeptID &&e.SortID<jobsort).ToList();
                                empcount = hremp0.Count;
                                if(hremp0.Count==0)
                                {
                                    if (DeptID != "")
                                    {
                                        string ParentID = db.hr_Depts.Single(d => d.DeptID == DeptID).ParentID;
                                        if (!string.IsNullOrEmpty(ParentID))
                                        {
                                            IList<hr_Employee> hremp1 = db.hr_Employees.Where(e => e.DeptID == ParentID && e.JobID == JobID).ToList();
                                            empcount = hremp1.Count;
                                            if (empcount > 0)
                                            {
                                                entity_fl_ApproveFlow.DeptID = hremp1[0].DeptID.ToString();
                                            }
                                        }
                                    }
                                    
                                }
                                else
                                {
                                   
                                    string jobid0 = hremp0[0].JobID.ToString();
                                    entity_fl_ApproveFlow.JobID = jobid0;
                                }
                                
                            }
                        }
                        else
                        {
                            empcount = 1;
                        }
                        if(empcount>0)
                        {
                            entity_fl_ApproveFlow.ResetCreated();
                            db.fl_ApproveState.Add(entity_fl_ApproveFlow);
                            
                        }
                        
                        HasApprover = true;//有签核者，不结单
                    }
                    if (associateSort < startSort && currSort == associateSort)
                    {
                        currSort = startSort;
                    }
                    else
                    {
                        currSort = ApproveForm[0].Employee_Sort;
                    }
                }
                
            }
            while (NextSort);
            if (!HasApprover) //没有设置流程或者没有符合条件的签核者，则结单
            {
                //更新映射表的状态，表示已签核完毕，结单
                entity_fl_MappingForm.Approved = 2;
                entity_fl_MappingForm.ApprovedDate = DateTime.Now;
            }
        }
        /// <summary>
        /// 模糊指定员工信息
        /// </summary>
        /// <param name="FuzzyType"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        #region fuzzyType
        private string GetEmployeeIDByFuzzyType(int FuzzyType, DataTable userInfo)
        {
            string employee_id = "";
            switch (FuzzyType)
            {
                case 1: employee_id = userInfo.Rows[0]["ManagerID"].ToString(); break;
                case 2: employee_id = userInfo.Rows[0]["RootDepartmentManagerID"].ToString(); break;
                case 3: employee_id = userInfo.Rows[0]["BUManagerID"].ToString(); break;
                case 4: employee_id = ""; break;
            }
            return employee_id;
        }

        /// <summary>
        /// 模糊指定部门信息
        /// </summary>
        /// <param name="FuzzyType"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        private string GetDeptIDByFuzzyType(int FuzzyType, DataTable userInfo)
        {
            string dept_id = "";
            switch (FuzzyType)
            {
                case 1: dept_id = userInfo.Rows[0]["DeptID"].ToString(); break;
                case 2: dept_id = userInfo.Rows[0]["RootDepartmentID"].ToString(); break;
                case 3: dept_id = userInfo.Rows[0]["BUManagerDeptID"].ToString(); break;
                case 4: dept_id = userInfo.Rows[0]["DeptID"].ToString(); break;
            }
            return dept_id;
        }

        /// <summary>
        /// 模糊指定职位信息
        /// </summary>
        /// <param name="FuzzyType"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        private string GetJobIDByFuzzyType(int FuzzyType, DataTable userInfo)
        {
            string job_id = "";
            //Panasia.Core.Sys.Models.SysContext db = new SysContext();
            switch (FuzzyType)
            {
                //case 1: job_id = db.hr_Jobs.Single(j=>j.Name=="经理").JobID; break;
                //case 2: job_id = db.hr_Jobs.Single(j => j.Name == "总监").JobID; break;
                case 1: job_id = userInfo.Rows[0]["ManagerJobID"].ToString(); break;
                case 2: job_id = userInfo.Rows[0]["RootDepartmentManagerJobID"].ToString(); break;
                case 3: job_id = userInfo.Rows[0]["BUManagerJobID"].ToString(); break;
                case 4: job_id = userInfo.Rows[0]["JobID"].ToString(); break;
            }
            return job_id;
        }
        #endregion

        /// <summary>
        /// 利用设定的规则筛选签核人员，如果规则够多够复杂，可能一次性递归筛选多个顺位。
        /// </summary>
        private List<fl_Approver> GetApproveFormByRules(int Employee_Sort, string TableName, int TemplateID,SysContext db, object entity_fl, string CompanyID)
        {
            int Employee_Sort_MIN = 0;
            //var next_forms = db.fl_Approver.Where(f => (f.FormID == TemplateID) && (f.CompanyID == CompanyID) && (f.Employee_Sort > Employee_Sort));
            var next_forms = db.fl_Approver.Where(f => (f.FormID == TemplateID) && (f.Employee_Sort > Employee_Sort));
            List<fl_Approver> ApproveForm = null;
            if (next_forms.Count() > 0)
            {
                Employee_Sort_MIN = next_forms.Min(f => f.Employee_Sort);
                //ApproveForm = next_forms.Where(f => f.Employee_Sort == Employee_Sort_MIN).ToList();
                ApproveForm = next_forms.ToList();
                string userID = new Panasia.Core.Sys.SystemParameterFunctions().GetCurrentUserID("").ToString();
                if (ApproveForm.Count() > 0)
                {
                    DataTable userInfo = UserInfo(userID);
                    int sortID = int.Parse(userInfo.Rows[0]["JobSortID"].ToString());
                    int new_sort = Employee_Sort_MIN;
                    var ApproveRules = db.fl_ApproveRules.Join(db.fl_Approver.AsEnumerable(), r => r.ApproverID, f => f.ApproverID,
                        (r, f) => new { r.Table_Name, r.Col_Name, r.Condition, r.ApproverID, f.CompanyID }).Where(r => r.Table_Name == TableName && r.CompanyID == CompanyID);
                    if (ApproveRules.Count() > 0)
                    {
                        //把实例转成DataTable，不然不知道用什么方法进行下面的判断，特别是那个Condition
                        DataTable entity_table = EntityToDataTable(entity_fl);
                        foreach (var item in ApproveRules)
                        {
                            //在这里进行规则条件的判断
                            //先判断条件是否成立

                           
                            if (sortID > 4 || TableName != "fl_LeaveApplicationForm")//经理及以上级别签核不受天数影响
                            {
                                //string condition = item.Col_Name + " " + item.Condition;
                                string condition =  item.Condition;
                                if (entity_table.Select(condition).Length == 0)//长度==0表示条件不成立
                                {
                                    //条件不成立的话，该规则所指示的签核人员就不应该出现在流程中
                                    var forms = ApproveForm.Where(f => f.ApproverID == item.ApproverID);
                                    //因为规则并没有规定顺位，所以在每个顺位上都会对所有规则进行判断，所以会出现条件没达到也找不到签核者或职位的情况
                                    if (forms.Count() == 1)
                                    {
                                        //删掉之前先留下Sort，好往后继续判断
                                        new_sort = forms.Single().Employee_Sort;
                                        //从集合中删掉
                                        ApproveForm.Remove(forms.Single());
                                    }
                                }
                            }
                            //else
                            //{
                            //    if(sortID<=4)
                            //    {
                            //        var forms = ApproveForm.Where(f => f.ApproverID == item.ApproverID);
                            //        string userjob=userInfo.Rows[0]["JobID"].ToString();
                            //        string userdept = userInfo.Rows[0]["DeptID"].ToString();
                            //        var forms1=forms.Where(f=>f.DeptID==userjob&&f.JobID==userdept);
                            //        if (forms1.Count()==1)
                            //        {
                            //            //删掉之前先留下Sort，好往后继续判断
                            //            new_sort = forms.Single().Employee_Sort;
                            //            //从集合中删掉
                            //            ApproveForm.Remove(forms.Single());
                            //        }
                            //    }
                            //}
                        }
                        //考虑到可能某顺位就一个签核人员，结果被规则刷掉了，于是如果在规则判断过后签核人员为0的话就要往下一个顺位进发继续判断，或者结单
                        if (ApproveForm.Count() == 0)
                        {
                            //真想用goto回到上面
                            ApproveForm = GetApproveFormByRules(new_sort, TableName, TemplateID, db, entity_fl, CompanyID);
                        }
                    }
                    for (int i = 0; i < ApproveForm.Count;i++ )
                    {
                        fl_Approver item1 = ApproveForm[i];
                        if (item1.EmployeeID!= "333333")
                        {
                            
                            string userjob = userInfo.Rows[0]["JobID"].ToString();
                            string userdept = "111111";
                            if (item1.DeptID == userdept && item1.JobID == userjob)
                            {
                                var forms = ApproveForm.Where(f => f.ApproverID == item1.ApproverID);
                                //删掉之前先留下Sort，好往后继续判断
                                new_sort = forms.Single().Employee_Sort;
                                //从集合中删掉
                                ApproveForm.Remove(forms.Single());
                            }
                        }
                        if (ApproveForm.Count() == 0)
                        {
                            //真想用goto回到上面
                            ApproveForm = GetApproveFormByRules(new_sort, TableName, TemplateID, db, entity_fl, CompanyID);
                        }
                    }
                }
            }
            return ApproveForm;
        }

        /// <summary>
        /// 把一个实例装进DataTable
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private DataTable EntityToDataTable(object entity)
        {
            Type entityType = entity.GetType();
            PropertyInfo[] entityProperties = entityType.GetProperties();

            //生成DataTable的structure
            DataTable dt = new DataTable();
            for (int i = 0; i < entityProperties.Length; i++)
            {
                Type t = entityProperties[i].PropertyType;
                if(entityProperties[i].PropertyType.IsGenericType && entityProperties[i].PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    t = entityProperties[i].PropertyType.GetGenericArguments()[0];
                }

                dt.Columns.Add(entityProperties[i].Name, t);
            }
            object[] entityValues = new object[entityProperties.Length];
            for (int i = 0; i < entityProperties.Length; i++)
            {
                entityValues[i] = entityProperties[i].GetValue(entity, null);
            }
            dt.Rows.Add(entityValues);
            return dt;
        }

        /// <summary>
        /// 获得用户相关信息
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        private DataTable UserInfo(string userID)
        {
            DataTable UserInfo = new DataTable();
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["PASys"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(@"SELECT   dbo.sys_User.UserID, dbo.sys_User.UserName, dbo.sys_User.Email, dbo.sys_User.Password, dbo.sys_User.IsValid, 
                dbo.sys_User.CreatedUser, dbo.sys_User.CreatedTime, dbo.sys_User.ModifiedUser, dbo.sys_User.ModifiedTime, 
                dbo.sys_User.AutoKey, dbo.hr_Employee.EmployeeID, dbo.hr_Employee.Name AS EmployeeName, 
                dbo.hr_Employee.DeptID, hr_Department_1.Name AS DepartmentName, dbo.hr_Employee.JobID, 
                dbo.hr_Job.Name AS JobName,dbo.hr_Job.SortID AS JobSortID, hr_Department_1.BUID, dbo.hr_BU.Name AS BUName, hr_Department_1.CompanyID, 
                dbo.hr_Company.Name AS CompanyName, hr_Department_1.ManagerID, hr_Employee_1.Name AS ManagerName, 
                dbo.hr_BU.ManagerID AS BUManagerID, hr_Employee_2.Name AS BUManagerName, 
                hr_Employee_2.DeptID AS BUManagerDeptID, hr_Employee_2.JobID AS BUManagerJobID, 
                hr_Employee_1.JobID AS ManagerJobID
                FROM      dbo.hr_Company RIGHT OUTER JOIN
                dbo.hr_Job RIGHT OUTER JOIN
                dbo.hr_Employee LEFT OUTER JOIN
                dbo.hr_Department AS hr_Department_1 LEFT OUTER JOIN
                dbo.hr_Employee AS hr_Employee_1 ON hr_Department_1.ManagerID = hr_Employee_1.EmployeeID ON 
                dbo.hr_Employee.DeptID = hr_Department_1.DeptID RIGHT OUTER JOIN
                dbo.sys_User ON dbo.hr_Employee.UserID = dbo.sys_User.UserID ON 
                dbo.hr_Job.JobID = dbo.hr_Employee.JobID LEFT OUTER JOIN
                dbo.hr_Employee AS hr_Employee_2 RIGHT OUTER JOIN
                dbo.hr_BU ON hr_Employee_2.EmployeeID = dbo.hr_BU.ManagerID ON hr_Department_1.BUID = dbo.hr_BU.BUID ON 
                dbo.hr_Company.CompanyID = hr_Department_1.CompanyID
                WHERE dbo.sys_User.UserID=@UserID", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SqlParameter("@UserID", userID));
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                conn.Open();
                sda.Fill(UserInfo);
                conn.Close();

                //RootDepartmentID  RootDepartmentManagerID  RootDepartmentManagerName  RootDepartmentManagerJobID
            }
            if (UserInfo.Rows.Count == 1)
            {
                DataRow dr = UserInfo.Rows[0];
                hr_Department rootDepartment = GetDepartmentRootByDepartmentID(dr["DeptID"].ToString());
                UserInfo.Columns.Add("RootDepartmentID");
                UserInfo.Columns.Add("RootDepartmentManagerID");
                UserInfo.Columns.Add("RootDepartmentManagerName");
                UserInfo.Columns.Add("RootDepartmentManagerJobID");

                dr["RootDepartmentID"] = rootDepartment.DeptID;
                dr["RootDepartmentManagerID"] = rootDepartment.ManagerID;
                Panasia.Core.Sys.hr_Employee DepartmentManager = dbSys.hr_Employees.SingleOrDefault(e => e.EmployeeID == rootDepartment.ManagerID);
                string name = "";
                string jobid = "";
                if (DepartmentManager != null)
                {
                    name = DepartmentManager.Name;
                    jobid = DepartmentManager.JobID;
                }
                dr["RootDepartmentManagerName"] = name;
                dr["RootDepartmentManagerJobID"] = jobid;
            }

            return UserInfo;
        }

        /// <summary>
        /// 部门信息
        /// </summary>
        /// <param name="DeptID"></param>
        /// <returns></returns>
        private hr_Department GetDepartmentRootByDepartmentID(string DeptID)
        {
            hr_Department rootDepartment = new hr_Department();
            GetDepartmentRootByDepartmentID(new SysContext(), DeptID, ref rootDepartment);
            return rootDepartment;
        }

        private void GetDepartmentRootByDepartmentID(SysContext db, string DeptID, ref hr_Department rootDepartment)
        {
            hr_Department dept = db.hr_Depts.Where(d => d.DeptID == DeptID).SingleOrDefault();
            if (dept != null)
            {
                if (dept.ParentID != "")
                {
                    GetDepartmentRootByDepartmentID(db, dept.ParentID, ref rootDepartment);
                }
                else
                {
                    rootDepartment = dept;
                }
            }
        }
        /// <summary>
        /// 送签签核列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ApproveFlow()
        {
            string ID = Request.QueryString["ID"];
            string TableName = Request.QueryString["TableName"];

            DataTable ApproveFlows = new DataTable();
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["PASys"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(@"DECLARE @LastConfirm_Stat VARCHAR(1),@LastConfirm_Time DATETIME

          SELECT TOP 1 @LastConfirm_Stat = Confirm_Stat,@LastConfirm_Time = DATEADD(MS, 100, Confirm_Time) FROM dbo.fl_ApproveState WHERE FormNo=@ID ORDER BY FormID

          DECLARE @SQL NVARCHAR(2000)
          SET @SQL='
			SELECT   dbo.fl_ApproveState.AutoKey, dbo.fl_ApproveState.FormID, dbo.fl_ApproveState.FormNo,
			dbo.fl_ApproveState.EmployeeID, hr_Employee_1.Name AS EmployeeName, dbo.fl_ApproveState.Employee_Sort,
			dbo.fl_ApproveState.DeptID, dbo.hr_Department.Name AS DepartmentName, dbo.fl_ApproveState.JobID,
          dbo.hr_Job.Name AS JobName, dbo.fl_ApproveState.Confirm_Class,
          CASE dbo.fl_ApproveState.Confirm_Stat
          WHEN ''S'' THEN ''送签''
          WHEN ''A'' THEN ''签核''
          WHEN ''X'' THEN ''待签''
          WHEN ''R'' THEN ''驳回''
          END Confirm_Stat,
			dbo.fl_ApproveState.Confirm_Time, dbo.hr_Company.Name AS CompanyName, dbo.fl_ApproveState.Suggestion
          FROM dbo.fl_ApproveState LEFT OUTER JOIN
			dbo.hr_Company RIGHT OUTER JOIN
			dbo.hr_Department ON dbo.hr_Company.CompanyID = dbo.hr_Department.CompanyID ON
			dbo.fl_ApproveState.DeptID = dbo.hr_Department.DeptID LEFT OUTER JOIN
			dbo.hr_Job ON dbo.fl_ApproveState.JobID = dbo.hr_Job.JobID LEFT OUTER JOIN
			dbo.hr_Employee AS hr_Employee_1 ON dbo.fl_ApproveState.EmployeeID = hr_Employee_1.EmployeeID
          WHERE FormNo='''+@ID+'''';

          DECLARE @Remark NVARCHAR(200)
          SET @Remark= ''
          IF NOT EXISTS (SELECT * FROM dbo.fl_ApproveState WHERE FormNo=@ID AND Confirm_Stat='X')
          BEGIN
              SELECT TOP 1 @LastConfirm_Stat = Confirm_Stat,@LastConfirm_Time = DATEADD(MS, 100, Confirm_Time) FROM dbo.fl_ApproveState WHERE FormNo=@ID ORDER BY Confirm_Time DESC
              IF @LastConfirm_Stat <> 'R'
          BEGIN
          IF @LastConfirm_Stat = 'S'
          BEGIN
          SET @Remark = '送签时系统内没有设定该单据的签核流程或该单据不满足签核条件，视为直接核准结单。'
          END
                  SET @SQL = @SQL + ' UNION SELECT 0,'''','''+@ID+''','''','''',99,'''','''','''','''','''',''结单'','''+CONVERT(NVARCHAR(30),@LastConfirm_Time,121)+''','''','''+@Remark+''''
              END
          END

          SET @SQL = @SQL + ' ORDER BY Employee_Sort,Confirm_Stat,Confirm_Time DESC'

          EXEC (@SQL)", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SqlParameter("@ID", ID));
                cmd.Parameters.Add(new SqlParameter("@TableName", TableName));
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                conn.Open();
                sda.Fill(ApproveFlows);
                conn.Close();
            }

            foreach (DataRow row in ApproveFlows.Rows)
            {
                if (row["Confirm_Stat"].ToString() == "结单" || row["Confirm_Stat"].ToString() == "待签")
                {
                    row["Confirm_Time"] = DBNull.Value;
                }
            }

            //ViewBag.Data = ApproveFlows;
            ViewBag.ApproveFlows = ApproveFlows;

            DataTable ApproverLogs = new DataTable();
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["PASys"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(@"DECLARE @LastConfirm_Stat VARCHAR(1),@LastConfirm_Time DATETIME

          SELECT TOP 1 @LastConfirm_Stat = Confirm_Stat,@LastConfirm_Time = DATEADD(MS, 100, Confirm_Time) FROM dbo.fl_ApproveLog WHERE FormNo=@ID ORDER BY FormID

          DECLARE @SQL NVARCHAR(2000)
          SET @SQL='
			SELECT   dbo.fl_ApproveLog.AutoKey, dbo.fl_ApproveLog.FormID, dbo.fl_ApproveLog.FormNo,
			dbo.fl_ApproveLog.EmployeeID, hr_Employee_1.Name AS EmployeeName, dbo.fl_ApproveLog.Employee_Sort,
			dbo.fl_ApproveLog.DeptID, dbo.hr_Department.Name AS DepartmentName, dbo.fl_ApproveLog.JobID,
          dbo.hr_Job.Name AS JobName, dbo.fl_ApproveLog.Confirm_Class,
          CASE dbo.fl_ApproveLog.Confirm_Stat
          WHEN ''S'' THEN ''送签''
          WHEN ''A'' THEN ''签核''
          WHEN ''X'' THEN ''待签''
          WHEN ''R'' THEN ''驳回''
          END Confirm_Stat,
			dbo.fl_ApproveLog.Confirm_Time, dbo.hr_Company.Name AS CompanyName, dbo.fl_ApproveLog.Suggestion
          FROM dbo.fl_ApproveLog LEFT OUTER JOIN
			dbo.hr_Company RIGHT OUTER JOIN
			dbo.hr_Department ON dbo.hr_Company.CompanyID = dbo.hr_Department.CompanyID ON
			dbo.fl_ApproveLog.DeptID = dbo.hr_Department.DeptID LEFT OUTER JOIN
			dbo.hr_Job ON dbo.fl_ApproveLog.JobID = dbo.hr_Job.JobID LEFT OUTER JOIN
			dbo.hr_Employee AS hr_Employee_1 ON dbo.fl_ApproveLog.EmployeeID = hr_Employee_1.EmployeeID
          WHERE FormNo='''+@ID+'''';         

          SET @SQL = @SQL + ' ORDER BY Confirm_Time DESC'

          EXEC (@SQL)", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SqlParameter("@ID", ID));
                cmd.Parameters.Add(new SqlParameter("@TableName", TableName));
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                conn.Open();
                sda.Fill(ApproverLogs);
                conn.Close();
            }

            //foreach (DataRow row in ApproverLogs.Rows)
            //{
            //    if (row["Confirm_Stat"].ToString() == "结单" || row["Confirm_Stat"].ToString() == "待签")
            //    {
            //        row["Confirm_Time"] = DBNull.Value;
            //    }
            //}

            ViewBag.ApproverLog = ApproverLogs;

            return View();
        }

        /// <summary>
        /// 送签追踪
        /// </summary>
        /// <returns></returns>
        public ActionResult ApproveFlowLog()
        {
            string ID = Request.QueryString["ID"];
            string TableName = Request.QueryString["TableName"];

            DataTable ApproveFlows = new DataTable();
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["PASys"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(@"DECLARE @LastConfirm_Stat VARCHAR(1),@LastConfirm_Time DATETIME

          SELECT TOP 1 @LastConfirm_Stat = Confirm_Stat,@LastConfirm_Time = DATEADD(MS, 100, Confirm_Time) FROM dbo.fl_ApproveLog WHERE FormNo=@ID ORDER BY FormID

          DECLARE @SQL NVARCHAR(2000)
          SET @SQL='
			SELECT   dbo.fl_ApproveLog.AutoKey, dbo.fl_ApproveLog.FormID, dbo.fl_ApproveLog.FormNo,
			dbo.fl_ApproveLog.EmployeeID, hr_Employee_1.Name AS EmployeeName, dbo.fl_ApproveLog.Employee_Sort,
			dbo.fl_ApproveLog.DeptID, dbo.hr_Department.Name AS DepartmentName, dbo.fl_ApproveLog.JobID,
          dbo.hr_Job.Name AS JobName, dbo.fl_ApproveLog.Confirm_Class,
          CASE dbo.fl_ApproveLog.Confirm_Stat
          WHEN ''S'' THEN ''送签''
          WHEN ''A'' THEN ''签核''
          WHEN ''X'' THEN ''待签''
          WHEN ''R'' THEN ''驳回''
          END Confirm_Stat,
			dbo.fl_ApproveLog.Confirm_Time, dbo.hr_Company.Name AS CompanyName, dbo.fl_ApproveLog.Suggestion
          FROM dbo.fl_ApproveLog LEFT OUTER JOIN
			dbo.hr_Company RIGHT OUTER JOIN
			dbo.hr_Department ON dbo.hr_Company.CompanyID = dbo.hr_Department.CompanyID ON
			dbo.fl_ApproveLog.DeptID = dbo.hr_Department.DeptID LEFT OUTER JOIN
			dbo.hr_Job ON dbo.fl_ApproveLog.JobID = dbo.hr_Job.JobID LEFT OUTER JOIN
			dbo.hr_Employee AS hr_Employee_1 ON dbo.fl_ApproveLog.EmployeeID = hr_Employee_1.EmployeeID
          WHERE FormNo='''+@ID+'''';

          DECLARE @Remark NVARCHAR(200)
          SET @Remark= ''
          IF NOT EXISTS (SELECT * FROM dbo.fl_ApproveLog WHERE FormNo=@ID AND Confirm_Stat='X')
          BEGIN
              SELECT TOP 1 @LastConfirm_Stat = Confirm_Stat,@LastConfirm_Time = DATEADD(MS, 100, Confirm_Time) FROM dbo.fl_ApproveLog WHERE FormNo=@ID ORDER BY Confirm_Time DESC
              IF @LastConfirm_Stat <> 'R'
          BEGIN
          IF @LastConfirm_Stat = 'S'
          BEGIN
          SET @Remark = '送签时系统内没有设定该单据的签核流程或该单据不满足签核条件，视为直接核准结单。'
          END
                  SET @SQL = @SQL + ' UNION SELECT 0,'''','''+@ID+''','''','''',99,'''','''','''','''','''',''结单'','''+CONVERT(NVARCHAR(30),@LastConfirm_Time,121)+''','''','''+@Remark+''''
              END
          END

          SET @SQL = @SQL + ' ORDER BY Employee_Sort,Confirm_Stat,Confirm_Time DESC'

          EXEC (@SQL)", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SqlParameter("@ID", ID));
                cmd.Parameters.Add(new SqlParameter("@TableName", TableName));
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                conn.Open();
                sda.Fill(ApproveFlows);
                conn.Close();
            }

            foreach (DataRow row in ApproveFlows.Rows)
            {
                if (row["Confirm_Stat"].ToString() == "结单" || row["Confirm_Stat"].ToString() == "待签")
            {
                    row["Confirm_Time"] = DBNull.Value;
                }
            }

            ViewBag.ApproverLog = ApproveFlows;

            return View();
        }
    }
}