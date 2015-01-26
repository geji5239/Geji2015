/*----------------------------------------------------------------
// Copyright (C) 2004 清风软件
// 版权所有。
//
// 文件名：ApproveController.cs
// 文件功能描述：Fl008-Fl013,Fl016控制器，通过反射执行对表单的各种操作。
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using System.Data;
using Panasia.Gemini.FL.Data.Models;
using Panasia.Gemini.FL.Data.Common;
using System.Text;
using System.Data.SqlClient;

namespace Panasia.Gemini.FL.Controllers
{
    public class ApproveController : Controller
    {
        public ActionResult Fl009()
        {
            return View();
        }

        public ActionResult Fl010()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["map"]))
            {
                System.Text.StringBuilder json = new System.Text.StringBuilder(GetDocumentJsonAndSet());
                json = json.Remove(json.ToString().LastIndexOf(","), 1);
                json.Append("}\" name=\"values\" id=\"values\" />");
                ViewData["values"] = json.ToString();
            }

            return View();
        }

        public ActionResult Fl011(string DataForm)
        {
            ViewData["DataForm"] = DataForm;
            return View();
        }

        public ActionResult Fl012()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["map"]))
            {
                System.Text.StringBuilder json = new System.Text.StringBuilder(GetDocumentJsonAndSet());
                json = json.Remove(json.ToString().LastIndexOf(","), 1);
                json.Append("}\" name=\"values\" id=\"values\" />");
                ViewData["values"] = json.ToString();
                SysContext db = new SysContext();
                ViewData["Company"] = new SelectList(db.hr_Company.Where(c => c.IsActive == true).OrderBy(c => c.SortID).AsEnumerable(), "CompanyID", "Name");
            }

            return View();
        }


        /// <summary>
        /// 获得不完整的json串，顺便进行相应的页面设置，因为这几张页面的初始状态差不多，所以可以使用同一段代码
        /// </summary>
        /// <returns></returns>
        private string GetDocumentJsonAndSet()
        {
            int mapID = int.Parse(Request.QueryString["map"]);
            Common c = new Common();
            SysContext db = new SysContext();
            fl_MappingForm map = db.fl_MappingForm.Single(m => m.MappingFormID == mapID);
            string key = map.FormNo;
            string DataTableName = map.Table_Name;
            //反射
            Assembly assembly = Assembly.Load("Panasia.Gemini.Fl.Data");
            //创建空实例
            object entity = assembly.CreateInstance("Panasia.Gemini.FL.Data.Models." + DataTableName);
            Type type = entity.GetType();
            //查询version版本号
            string pkeyName = c.GetPKColumn(DataTableName);
            int maxVersion = db.Database.SqlQuery<int>("SELECT ISNULL(MAX([Version]),0) FROM " + DataTableName + " WHERE " + pkeyName + " = '" + key + "'").First();

            //获得数据实例
            System.Data.Entity.DbSet dbSet = db.Set(type);
            entity = dbSet.Find(key, maxVersion);

            //获得提交单据时的公司ID
            PropertyInfo[] PropertyList = type.GetProperties();
            object CompanyID = PropertyList.Where(p => p.Name == "CompanyID").First().GetValue(entity, null);

            int fid = db.fl_Form.Single(f => f.Companys.Contains(CompanyID.ToString()) && f.DataFrom == DataTableName).FormID;
            ViewData["Key"] = key;

            ViewData["DataTableName"] = DataTableName;

            string attach = "";
            if (!string.IsNullOrEmpty(map.Attachment))
            {
                attach = "/Upload/Attachment/" + DataTableName + "/" + key.ToString() + "/" + map.Attachment;
            }
            ViewData["Attachment"] = attach;
            StringBuilder url = new StringBuilder();

            bool hasTemplateUrl = false;
            if (!string.IsNullOrEmpty(Request.QueryString["approve"]))
            {
                string employee_id = LoginInfo.EmployeeID;
                string job_id = LoginInfo.JobID;
                //获得属于自己的当前单据的最新签核流水
                IList<fl_ApproveState> il_fl_ApproveState_Approve = db.fl_ApproveState.Where(f =>
                        f.FormID == fid &&
                        f.FormNo == key &&
                        (f.EmployeeID == employee_id || f.EmployeeID == "" && f.JobID == job_id) &&
                        f.Confirm_Stat == "X").OrderBy(m => m.Employee_Sort).ToList(); //Single报错，序列包含一个以上元素

                fl_ApproveState entity_fl_ApproveState_Approve = il_fl_ApproveState_Approve[0];
                //获得当前流水对应的签核人设置
                fl_Approver entity_fl_Approve = db.fl_Approver.FirstOrDefault(f => f.ApproverID == entity_fl_ApproveState_Approve.ApproverID);

                if (entity_fl_Approve != null)
                {
                    fl_FormTemplate tfd = db.fl_FormTemplate.FirstOrDefault(d => d.TemplateID == entity_fl_Approve.TemplateID);
                    if (tfd != null)
                    {
                        url.Append(tfd.Template_Url);
                        hasTemplateUrl = true;
                    }
                }
            }
            if (!hasTemplateUrl)
            {
                url.AppendFormat("/Templates/{0}/{0}_Default.html", DataTableName);
            }

            if (!System.IO.File.Exists(Server.MapPath(url.ToString())))
            {
                url = new StringBuilder("/Templates/FileNotExist.html");
            }

            ViewBag.TemplateUrl = url.ToString();

            //System.Reflection.PropertyInfo[] PropertyList = type.GetProperties();

            System.Text.StringBuilder json = new System.Text.StringBuilder("<input type=\"hidden\" value=\"{");

            string employeeid = c.GetValueFromEntity("EmployeeID", entity);
            if (!string.IsNullOrEmpty(employeeid))
            {
                json.AppendFormat("'EmployeeName':'{0}',", db.hr_Employee.Single(e => e.EmployeeID == employeeid).Name);
            }
            string deptid = c.GetValueFromEntity("DeptID", entity);
            if (!string.IsNullOrEmpty(deptid))
            {
                json.AppendFormat("'DeptName':'{0}',", db.hr_Department.Single(d => d.DeptID == deptid).Name);
            }
            string jobid = c.GetValueFromEntity("JobID", entity);
            if (!string.IsNullOrEmpty(jobid))
            {
                json.AppendFormat("'JobName':'{0}',", db.hr_Job.Single(j => j.JobID == jobid).Name);
            }
            foreach (System.Reflection.PropertyInfo property in PropertyList)
            {
                //这是个自增列，不需要
                if (property.Name != "AutoKey")
                {
                    object value = property.GetValue(entity);
                    //如果是日期型值就格式化
                    if (value != null)
                    {
                        if ((value.GetType() == Type.GetType("System.DateTime")))
                        {
                            value = Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        json.AppendFormat("'{0}':'{1}',", property.Name, value.ToString());
                    }
                }
            }
            return json.ToString();
        }

        #region fuzzyType
        private string GetEmployeeIDByFuzzyType(int FuzzyType, string FormID)
        {
            string employee_id = "";
            switch (FuzzyType)
            {
                case 1: employee_id = LoginInfo.ManagerID; break;
                case 2: employee_id = LoginInfo.RootDepartmentManagerID; break;
                case 3: employee_id = LoginInfo.BUManagerID; break;
                case 5: employee_id = IApprover(FormID); break;
            }
            return employee_id;
        }
        private string IApprover(string FormID)
        {
            SysContext db = new SysContext();
            string ia = db.fl_EmployeeMove.Single(e => e.Movid == FormID).IApprover;
            return ia;
        }
        private string GetDeptIDByFuzzyType(int FuzzyType, string FormID)
        {
            string dept_id = "";
            switch (FuzzyType)
            {
                case 1: dept_id = LoginInfo.DeptID; break;
                case 2: dept_id = LoginInfo.RootDepartmentID; break;
                case 3: dept_id = LoginInfo.BUManagerDeptID; break;
                case 4: dept_id = LoginInfo.DeptID; break;
                case 5: dept_id = NewDeptID(FormID); break;
            }
            return dept_id;
        }
        private string NewDeptID(string FormID)
        {
            SysContext db = new SysContext();
            string ia = db.fl_EmployeeMove.Single(e => e.Movid == FormID).NewDeptID;
            return ia;
        }
        private string GetJobIDByFuzzyType(int FuzzyType, string FormID)
        {
            string job_id = "";
            //SysContext db = new SysContext();
            switch (FuzzyType)
            {
                //case 1: job_id = db.hr_Job.Single(j=>j.Name=="经理").JobID; break;//如果这个职位不叫经理怎么办
                case 1: job_id = LoginInfo.ManagerJobID; break;
                //case 2: job_id = db.hr_Job.Single(j => j.Name == "总监").JobID; break;
                case 2: job_id = LoginInfo.RootDepartmentManagerJobID; break;
                case 3: job_id = LoginInfo.BUManagerJobID; break;
                case 4: job_id = LoginInfo.JobID; break;
                case 5: job_id = NewJobID(FormID); break;
            }
            return job_id;
        }
        private string NewJobID(string FormID)
        {
            SysContext db = new SysContext();
            string ia = db.fl_EmployeeMove.Single(e => e.Movid == FormID).NewJobID;
            return ia;
        }
        #endregion

        public string ApproveDocument()
        {
            return ApproveDocument(int.Parse(Request.QueryString["map"]), Request.QueryString["status"], Request.QueryString["suggestion"], Request.QueryString["CompanyID"]);
        }
        public string ApproveSelected()
        {
            string Message = "OK";
            try
            {
                string SelectedData = Request.Form["SelectedData"];
                string[] DataItemCollection = SelectedData.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string DataItem in DataItemCollection)
                {
                    //ApproveDocument(int.Parse(DataItem), "A", "");
                }
            }
            catch
            {
                Message = "Error";
            }
            return Message;
        }
        private string ApproveDocument(int MappingFormID, string Confirm_Stat, string Suggestion, string CID)
        {
            string Message = "OK";
            try
            {
                Common c = new Common();
                SysContext db = new SysContext();
                string employee_id = LoginInfo.EmployeeID;
                string job_id = LoginInfo.JobID;
                //映射信息
                fl_MappingForm map = db.fl_MappingForm.Single(m => m.MappingFormID == MappingFormID);
                string key = map.FormNo;
                string DataTableName = map.Table_Name;

                //签核通过后对原数据（包含fl和hr）进行更新（如果可以编辑的话）
                Assembly assembly = Assembly.Load("Panasia.Gemini.Fl.Data");
                //创建空实例
                object entity = assembly.CreateInstance("Panasia.Gemini.FL.Data.Models." + DataTableName);
                Type type = entity.GetType();
                //查询version版本号
                string pkeyName = c.GetPKColumn(DataTableName);
                int maxVersion = db.Database.SqlQuery<int>("SELECT ISNULL(MAX([Version]),0) FROM " + DataTableName + " WHERE " + pkeyName + " = '" + key + "'").First();
                //获得数据实例
                System.Data.Entity.DbSet dbSet = db.Set(type);
                entity = dbSet.Find(key, maxVersion);

                //获得提交单据时的公司ID
                PropertyInfo[] PropertyList = type.GetProperties();
                object CompanyID = PropertyList.Where(p => p.Name == "CompanyID").First().GetValue(entity, null);

                //1、先找到属于自己的那条待签流水记录，如果没有属于自己的则一定是有指定职位的待签流水，更新状态
                int fid = db.fl_Form.Single(f => f.Companys.Contains(CompanyID.ToString()) && f.DataFrom == DataTableName).FormID;
                fl_ApproveState entity_fl_ApproveState_Approve = db.fl_ApproveState.Where(f =>
                    f.FormID == fid &&
                    f.FormNo == key &&
                    (f.EmployeeID == employee_id || f.EmployeeID == "" && f.JobID == job_id) &&
                    f.Confirm_Stat == "X").OrderBy(f => f.Employee_Sort).Take(1).SingleOrDefault();
                if (entity_fl_ApproveState_Approve != null)
                {
                    entity_fl_ApproveState_Approve.Confirm_Stat = Confirm_Stat;
                    entity_fl_ApproveState_Approve.Confirm_Time = DateTime.Now;
                    entity_fl_ApproveState_Approve.EmployeeID = LoginInfo.EmployeeID;//如果是指定到职位的话，在这里更新的时候要把签核者的ID写上，这样就能从特定职位转到特定的签核人了
                    entity_fl_ApproveState_Approve.Suggestion = Suggestion;//意见和建议

                    //fgh 2015-01-20 add 
                    fl_ApproveLog entity_fl_ApproveLog = new fl_ApproveLog();
                    entity_fl_ApproveLog.Version = maxVersion;
                    entity_fl_ApproveLog.FormID = fid;
                    entity_fl_ApproveLog.FormNo = key;
                    entity_fl_ApproveLog.EmployeeID = LoginInfo.EmployeeID;//发起者的ID
                    //entity_fl_ApproveLog.Employee_Sort = SortID;//发起者的顺序就是0
                    entity_fl_ApproveLog.DeptID = LoginInfo.DeptID;//发起者的部门
                    entity_fl_ApproveLog.JobID = job_id;//发起者的职位
                    entity_fl_ApproveLog.Confirm_Class = "A";
                    entity_fl_ApproveLog.Confirm_Stat = Confirm_Stat;//送签用S
                    entity_fl_ApproveLog.Confirm_Time = DateTime.Now;
                    entity_fl_ApproveLog.Suggestion = Suggestion;
                    entity_fl_ApproveLog.ResetCreated();
                    db.fl_ApproveLog.Add(entity_fl_ApproveLog);

                    //只有当签核通过时才有下面的判断
                    if (Confirm_Stat == "A")
                    {


                        //获得页面提交过来的json字符串
                        string documentValues = Request.QueryString["documentValues"];
                        documentValues = documentValues.Replace("{", "").Replace("}", "").Replace("\"", "");
                        if (!string.IsNullOrEmpty(documentValues))
                        {
                            //将json转成键值对存放
                            Dictionary<string, string> dictionary = new Dictionary<string, string>();
                            string[] array = documentValues.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string item in array)
                            {
                                string key1 = item.Substring(0, item.IndexOf(":")).Trim();
                                string value = item.Substring(item.IndexOf(":") + 1).Trim();
                                dictionary.Add(key1, value);
                            }
                            //以下是对单据进行修改的部分
                            if (dictionary.Count > 0)
                            {
                                foreach (PropertyInfo property in PropertyList)
                                {
                                    //因为是修改所以字段不全，赋值前必须先检查是否有此项
                                    if (dictionary.ContainsKey(property.Name) && property.Name != "AutoKey")
                                    {
                                        //数据类型转换
                                        object value = Convert.ChangeType(dictionary[property.Name], property.PropertyType);
                                        property.SetValue(entity, value);
                                    }
                                }
                                type.GetMethod("ResetUpdated").Invoke(entity, null);//执行SysEntity的重置方法更新本次修改时间

                                //更新hr数据
                                object entity_Business = assembly.CreateInstance("Panasia.Gemini.FL.Data.Models." + DataTableName.Remove(0, 3));
                                Type type_Business = entity_Business.GetType();

                                //获得数据实例
                                System.Data.Entity.DbSet dbSet_hr = db.Set(type_Business);
                                entity_Business = dbSet_hr.Find(key);

                                PropertyInfo[] PropertyList_hr = type_Business.GetProperties();
                                foreach (PropertyInfo property in PropertyList_hr)
                                {
                                    //因为是修改所以字段不全，赋值前必须先检查是否有此项
                                    if (dictionary.ContainsKey(property.Name) && property.Name != "AutoKey")
                                    {
                                        //数据类型转换
                                        object value = Convert.ChangeType(dictionary[property.Name], property.PropertyType);
                                        property.SetValue(entity_Business, value);
                                    }
                                }
                                type_Business.GetMethod("ResetUpdated").Invoke(entity_Business, null);//执行SysEntity的重置方法更新本次修改时间
                            }
                        }
                        int sort = entity_fl_ApproveState_Approve.Employee_Sort;
                        //同组签核
                        int groupcount = db.fl_ApproveState.Join(db.fl_Approver, s => s.ApproverID, a => a.ApproverID,
                        (s, a) => new
                        {
                            a.IsGroupApprover,
                            s.Employee_Sort,
                            s.FormNo
                        }).Where(a => a.IsGroupApprover == true && a.Employee_Sort == sort && a.FormNo == entity_fl_ApproveState_Approve.FormNo).Count();

                        if (groupcount > 1)
                        {
                            List<fl_ApproveState> li = db.fl_ApproveState.Where(a => a.Employee_Sort == sort && a.FormNo == entity_fl_ApproveState_Approve.FormNo).ToList();
                            foreach (fl_ApproveState item in li)
                            {
                                item.Confirm_Stat = Confirm_Stat;
                                item.Confirm_Time = DateTime.Now;
                                item.Suggestion = Suggestion;//意见和建议
                            }
                        }
                        //选中临时增加签核人员时
                        if (Request.Form["temp"] == "1")
                        {
                            fl_ApproveState entity_fl_ApproveState = new fl_ApproveState();
                            entity_fl_ApproveState.FormID = fid;
                            entity_fl_ApproveState.FormNo = key;
                            string emp_id_emp = Request.Form["emp_id_emp"];
                            entity_fl_ApproveState.EmployeeID = emp_id_emp == null ? "" : emp_id_emp;
                            entity_fl_ApproveState.Employee_Sort = entity_fl_ApproveState_Approve.Employee_Sort;//临时增加的顺位和自己是一样的
                            entity_fl_ApproveState.DeptID = Request.Form["dept_id"];
                            entity_fl_ApproveState.JobID = Request.Form["job_id"];
                            entity_fl_ApproveState.Confirm_Class = "A";
                            entity_fl_ApproveState.Confirm_Stat = "X";//A签核 X待签 R驳回
                            entity_fl_ApproveState.Confirm_Time = DateTime.Now;
                            entity_fl_ApproveState.ApproverID = null;
                            entity_fl_ApproveState.CreatedUser = LoginInfo.UserID;
                            entity_fl_ApproveState.ModifiedUser = LoginInfo.UserID;
                            db.fl_ApproveState.Add(entity_fl_ApproveState);

                            //既然临时增加了签核人员，那么下面那些判断也不用做了


                            //fgh 2015-01-20 add 
                            entity_fl_ApproveLog = new fl_ApproveLog();
                            entity_fl_ApproveLog.Version = maxVersion;
                            entity_fl_ApproveLog.FormID = fid;
                            entity_fl_ApproveLog.FormNo = key;
                            entity_fl_ApproveLog.EmployeeID = emp_id_emp == null ? "" : emp_id_emp;//发起者的ID
                            entity_fl_ApproveLog.Employee_Sort = entity_fl_ApproveState_Approve.Employee_Sort; //发起者的顺序就是0
                            entity_fl_ApproveLog.DeptID = Request.Form["dept_id"];//发起者的部门
                            entity_fl_ApproveLog.JobID = Request.Form["job_id"];//发起者的职位
                            entity_fl_ApproveLog.Confirm_Class = "A";
                            entity_fl_ApproveLog.Confirm_Stat = "X";//送签用S
                            entity_fl_ApproveLog.Confirm_Time = DateTime.Now;
                            entity_fl_ApproveLog.Suggestion = Suggestion;
                            entity_fl_ApproveLog.ResetCreated();
                            db.fl_ApproveLog.Add(entity_fl_ApproveLog);
                        }
                        else
                        {
                            #region 2014-12-10 签核记录由逐条插入变更为一次性全部插入数据
                            ////2、再查找数据库，如果本顺位的所有记录都已签核通过，则根据顺序找到下一个签核者，插入记录。如果有一条是待签或驳回的则不做任何操作
                            //var _UnApprovedListBySort = db.fl_ApproveFlow.Where(f =>
                            //    f.FormID == key &&
                            //    f.Form_NO == DataTableName &&
                            //    f.Employee_Sort == entity_fl_ApproveFlow_Approve.Employee_Sort &&
                            //    f.Confirm_Stat == "X" &&
                            //    ((f.EmployeeID != "" && f.EmployeeID != employee_id) ||
                            //    (f.EmployeeID == "" && f.JobID != job_id)));
                            //bool allApproved = true;
                            //if (_UnApprovedListBySort.Count() > 0)
                            //{
                            //    allApproved = false;
                            //}
                            ////如果全部通过，找到下一个顺位的签核者
                            //if (allApproved)
                            //{
                            //    int template_id = db.fl_TemplateForm.Single(t => t.Table_Name == DataTableName).TemplateID;

                            //    var ApproveForm = GetApproveFormByRules(entity_fl_ApproveFlow_Approve.Employee_Sort, DataTableName, key, template_id, db, entity, CompanyID.ToString());
                            //    bool HasApprover = false;//假定没有签核者
                            //    if (ApproveForm != null && ApproveForm.Count() > 0)
                            //    {
                            //        //考虑到每一步都可能有多个签核者同时进行，每次必须以循环的方式插入相应数量的数据
                            //        foreach (var item in ApproveForm)
                            //        {
                            //            bool IsFuzzy = Convert.ToBoolean(item.IsFuzzy);
                            //            int FuzzyType = Convert.ToInt32(item.FuzzyType);
                            //            string EmployeeID = IsFuzzy ? "" : item.EmployeeID;//模糊指定一般只会到职位不会到具体人
                            //            string DeptID = IsFuzzy ? GetDeptIDByFuzzyType(FuzzyType) : item.DeptID;
                            //            string JobID = IsFuzzy ? GetJobIDByFuzzyType(FuzzyType) : item.JobID;
                            //            if (JobID == "" && EmployeeID == "")
                            //            {
                            //                continue;
                            //            }
                            //            fl_ApproveFlow entity_fl_ApproveFlow = new fl_ApproveFlow();
                            //            entity_fl_ApproveFlow.FormID = key;
                            //            entity_fl_ApproveFlow.Form_NO = DataTableName;
                            //            entity_fl_ApproveFlow.EmployeeID = EmployeeID;//下一个签核者的ID，如果是职位的话这个为空
                            //            entity_fl_ApproveFlow.Employee_Sort = item.Employee_Sort;//下一个签核者的顺序
                            //            entity_fl_ApproveFlow.DeptID = DeptID;//下一个签核者的部门，如果是经理则与自己相同，如果自己是经理则需要去fl_ApproveForm查询
                            //            entity_fl_ApproveFlow.JobID = JobID;//下一个签核者的职位，如果没有指定人员这个就必须有，如果指定了人员那就是指定人员的职位
                            //            entity_fl_ApproveFlow.Confirm_Class = "";
                            //            entity_fl_ApproveFlow.Confirm_Stat = "X";//A签核 X待签 R驳回
                            //            entity_fl_ApproveFlow.Confirm_Time = DateTime.Now;
                            //            entity_fl_ApproveFlow.ApproveFormID = item.ApproveFormID;

                            //            db.fl_ApproveFlow.Add(entity_fl_ApproveFlow);
                            //            HasApprover = true;//有签核者，不结单
                            //        }
                            //    }
                            //    if (!HasApprover) //下一个顺位没有签核者，则结单
                            //    {
                            //        //更新映射表的状态，表示已签核完毕，结单
                            //        map.Approved = 2;
                            //        map.ApprovedDate = DateTime.Now;
                            //    }
                            //}
                            #endregion

                            //查找是否还有除本条数据以外的其他待签数据
                            bool HasApprover = db.fl_ApproveState.Any(f =>
                                f.FormID == fid &&
                                f.FormNo == key &&
                                f.Confirm_Stat == "X" &&
                                f.AutoKey != entity_fl_ApproveState_Approve.AutoKey);
                            if (!HasApprover)
                            {
                                //如果没有待签数据了就更新映射表的状态，表示已签核完毕，结单
                                map.Approved = 2;
                                map.ApprovedDate = DateTime.Now;
                            }
                        }
                    }
                    else
                    {
                        //被驳回，更新映射表状态
                        map.Approved = 0;
                        map.ApprovedDate = DateTime.Now;

                        //fgh 2015-01-20 add 
                        entity_fl_ApproveLog.Version = maxVersion;
                        entity_fl_ApproveLog.FormID = fid;
                        entity_fl_ApproveLog.FormNo = key;
                        entity_fl_ApproveLog.EmployeeID = LoginInfo.EmployeeID;//发起者的ID
                        //entity_fl_ApproveLog.Employee_Sort = SortID;//发起者的顺序就是0
                        entity_fl_ApproveLog.DeptID = LoginInfo.DeptID;//发起者的部门
                        entity_fl_ApproveLog.JobID = job_id;//发起者的职位
                        entity_fl_ApproveLog.Confirm_Class = "A";
                        entity_fl_ApproveLog.Confirm_Stat = Confirm_Stat;//送签用S
                        entity_fl_ApproveLog.Confirm_Time = DateTime.Now;
                        entity_fl_ApproveLog.Suggestion = Suggestion;
                        entity_fl_ApproveLog.ResetCreated();
                        db.fl_ApproveLog.Add(entity_fl_ApproveLog);

                        //理论上来说，流程中只要有一个驳回了，其他还没审的就不用审了，全部删掉，审过的记录要保留
                        //所以理论上当一个人审核的时候，表要琐
                        var _UnApprovedListBySort = db.fl_ApproveState.Where(f =>
                                f.FormID == fid &&
                                f.FormNo == key );
                        db.fl_ApproveState.RemoveRange(_UnApprovedListBySort);
                        //被驳回的单据如果重新提交，那么要重新走全部的流程
                    }
                    db.SaveChanges();
                    db.Database.Connection.Open();
                    var cmd = db.Database.Connection.CreateCommand();
                    cmd.CommandText = "exec sp_fl_ApproveMessageRemind @FormID,@FormNo,@UserID";
                    cmd.Parameters.Add(new SqlParameter("@FormID", fid.ToString()));
                    cmd.Parameters.Add(new SqlParameter("@FormNo", key));
                    cmd.Parameters.Add(new SqlParameter("@UserID", employee_id));
                    var companyid = cmd.ExecuteScalar();
                    db.Database.Connection.Close();
                }
                else
                {
                    Message = "Error";
                }
            }
            catch
            {

                Message = "Error";
            }
            return Message;
        }

        private string ApproveDocument_old(int MappingFormID, string Confirm_Stat, string Suggestion, string CID)
        {
            string Message = "OK";
            try
            {
                Common c = new Common();
                SysContext db = new SysContext();
                string employee_id = LoginInfo.EmployeeID;
                string job_id = LoginInfo.JobID;
                //映射信息
                fl_MappingForm map = db.fl_MappingForm.Single(m => m.MappingFormID == MappingFormID);
                string key = map.FormNo;
                string DataTableName = map.Table_Name;

                //签核通过后对原数据（包含fl和hr）进行更新（如果可以编辑的话）
                Assembly assembly = Assembly.Load("Panasia.Gemini.Fl.Data");
                //创建空实例
                object entity = assembly.CreateInstance("Panasia.Gemini.FL.Data.Models." + DataTableName);
                Type type = entity.GetType();
                //查询version版本号
                string pkeyName = c.GetPKColumn(DataTableName);
                int maxVersion = db.Database.SqlQuery<int>("SELECT ISNULL(MAX([Version]),0) FROM " + DataTableName + " WHERE " + pkeyName + " = '" + key + "'").First();
                //获得数据实例
                System.Data.Entity.DbSet dbSet = db.Set(type);
                entity = dbSet.Find(key, maxVersion);

                //获得提交单据时的公司ID
                PropertyInfo[] PropertyList = type.GetProperties();
                object CompanyID = PropertyList.Where(p => p.Name == "CompanyID").First().GetValue(entity, null);

                //1、先找到属于自己的那条待签流水记录，如果没有属于自己的则一定是有指定职位的待签流水，更新状态
                int fid = db.fl_Form.Single(f => f.Companys.Contains(CompanyID.ToString()) && f.DataFrom == DataTableName).FormID;
                fl_ApproveState entity_fl_ApproveState_Approve = db.fl_ApproveState.Where(f =>
                    f.FormID == fid &&
                    f.FormNo == key &&
                    (f.EmployeeID == employee_id || f.EmployeeID == "" && f.JobID == job_id) &&
                    f.Confirm_Stat == "X").OrderBy(f => f.Employee_Sort).Take(1).SingleOrDefault();
                if (entity_fl_ApproveState_Approve != null)
                {
                    entity_fl_ApproveState_Approve.Confirm_Stat = Confirm_Stat;
                    entity_fl_ApproveState_Approve.Confirm_Time = DateTime.Now;
                    entity_fl_ApproveState_Approve.EmployeeID = LoginInfo.EmployeeID;//如果是指定到职位的话，在这里更新的时候要把签核者的ID写上，这样就能从特定职位转到特定的签核人了
                    entity_fl_ApproveState_Approve.Suggestion = Suggestion;//意见和建议

                    //只有当签核通过时才有下面的判断
                    if (Confirm_Stat == "A")
                    {


                        //获得页面提交过来的json字符串
                        string documentValues = Request.QueryString["documentValues"];
                        documentValues = documentValues.Replace("{", "").Replace("}", "").Replace("\"", "");
                        if (!string.IsNullOrEmpty(documentValues))
                        {
                            //将json转成键值对存放
                            Dictionary<string, string> dictionary = new Dictionary<string, string>();
                            string[] array = documentValues.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string item in array)
                            {
                                string key1 = item.Substring(0, item.IndexOf(":")).Trim();
                                string value = item.Substring(item.IndexOf(":") + 1).Trim();
                                dictionary.Add(key1, value);
                            }
                            //以下是对单据进行修改的部分
                            if (dictionary.Count > 0)
                            {
                                foreach (PropertyInfo property in PropertyList)
                                {
                                    //因为是修改所以字段不全，赋值前必须先检查是否有此项
                                    if (dictionary.ContainsKey(property.Name) && property.Name != "AutoKey")
                                    {
                                        //数据类型转换
                                        object value = Convert.ChangeType(dictionary[property.Name], property.PropertyType);
                                        property.SetValue(entity, value);
                                    }
                                }
                                type.GetMethod("ResetUpdated").Invoke(entity, null);//执行SysEntity的重置方法更新本次修改时间

                                //更新hr数据
                                object entity_Business = assembly.CreateInstance("Panasia.Gemini.FL.Data.Models." + DataTableName.Remove(0, 3));
                                Type type_Business = entity_Business.GetType();

                                //获得数据实例
                                System.Data.Entity.DbSet dbSet_hr = db.Set(type_Business);
                                entity_Business = dbSet_hr.Find(key);

                                PropertyInfo[] PropertyList_hr = type_Business.GetProperties();
                                foreach (PropertyInfo property in PropertyList_hr)
                                {
                                    //因为是修改所以字段不全，赋值前必须先检查是否有此项
                                    if (dictionary.ContainsKey(property.Name) && property.Name != "AutoKey")
                                    {
                                        //数据类型转换
                                        object value = Convert.ChangeType(dictionary[property.Name], property.PropertyType);
                                        property.SetValue(entity_Business, value);
                                    }
                                }
                                type_Business.GetMethod("ResetUpdated").Invoke(entity_Business, null);//执行SysEntity的重置方法更新本次修改时间
                            }
                        }
                        int sort = entity_fl_ApproveState_Approve.Employee_Sort;
                        //同组签核
                        int groupcount = db.fl_ApproveState.Join(db.fl_Approver, s => s.ApproverID, a => a.ApproverID,
                        (s, a) => new
                        {
                            a.IsGroupApprover,
                            s.Employee_Sort,
                            s.FormNo
                        }).Where(a => a.IsGroupApprover == true && a.Employee_Sort == sort && a.FormNo == entity_fl_ApproveState_Approve.FormNo).Count();

                        if (groupcount > 1)
                        {
                            List<fl_ApproveState> li = db.fl_ApproveState.Where(a => a.Employee_Sort == sort && a.FormNo == entity_fl_ApproveState_Approve.FormNo).ToList();
                            foreach (fl_ApproveState item in li)
                            {
                                item.Confirm_Stat = Confirm_Stat;
                                item.Confirm_Time = DateTime.Now;
                                item.Suggestion = Suggestion;//意见和建议
                            }
                        }
                        //选中临时增加签核人员时
                        if (Request.Form["temp"] == "1")
                        {
                            fl_ApproveState entity_fl_ApproveState = new fl_ApproveState();
                            entity_fl_ApproveState.FormID = fid;
                            entity_fl_ApproveState.FormNo = key;
                            string emp_id_emp = Request.Form["emp_id_emp"];
                            entity_fl_ApproveState.EmployeeID = emp_id_emp == null ? "" : emp_id_emp;
                            entity_fl_ApproveState.Employee_Sort = entity_fl_ApproveState_Approve.Employee_Sort;//临时增加的顺位和自己是一样的
                            entity_fl_ApproveState.DeptID = Request.Form["dept_id"];
                            entity_fl_ApproveState.JobID = Request.Form["job_id"];
                            entity_fl_ApproveState.Confirm_Class = "";
                            entity_fl_ApproveState.Confirm_Stat = "X";//A签核 X待签 R驳回
                            entity_fl_ApproveState.Confirm_Time = DateTime.Now;
                            entity_fl_ApproveState.ApproverID = null;
                            entity_fl_ApproveState.CreatedUser = LoginInfo.UserID;
                            entity_fl_ApproveState.ModifiedUser = LoginInfo.UserID;
                            db.fl_ApproveState.Add(entity_fl_ApproveState);

                            //既然临时增加了签核人员，那么下面那些判断也不用做了
                        }
                        else
                        {
                            #region 2014-12-10 签核记录由逐条插入变更为一次性全部插入数据
                            ////2、再查找数据库，如果本顺位的所有记录都已签核通过，则根据顺序找到下一个签核者，插入记录。如果有一条是待签或驳回的则不做任何操作
                            //var _UnApprovedListBySort = db.fl_ApproveFlow.Where(f =>
                            //    f.FormID == key &&
                            //    f.Form_NO == DataTableName &&
                            //    f.Employee_Sort == entity_fl_ApproveFlow_Approve.Employee_Sort &&
                            //    f.Confirm_Stat == "X" &&
                            //    ((f.EmployeeID != "" && f.EmployeeID != employee_id) ||
                            //    (f.EmployeeID == "" && f.JobID != job_id)));
                            //bool allApproved = true;
                            //if (_UnApprovedListBySort.Count() > 0)
                            //{
                            //    allApproved = false;
                            //}
                            ////如果全部通过，找到下一个顺位的签核者
                            //if (allApproved)
                            //{
                            //    int template_id = db.fl_TemplateForm.Single(t => t.Table_Name == DataTableName).TemplateID;

                            //    var ApproveForm = GetApproveFormByRules(entity_fl_ApproveFlow_Approve.Employee_Sort, DataTableName, key, template_id, db, entity, CompanyID.ToString());
                            //    bool HasApprover = false;//假定没有签核者
                            //    if (ApproveForm != null && ApproveForm.Count() > 0)
                            //    {
                            //        //考虑到每一步都可能有多个签核者同时进行，每次必须以循环的方式插入相应数量的数据
                            //        foreach (var item in ApproveForm)
                            //        {
                            //            bool IsFuzzy = Convert.ToBoolean(item.IsFuzzy);
                            //            int FuzzyType = Convert.ToInt32(item.FuzzyType);
                            //            string EmployeeID = IsFuzzy ? "" : item.EmployeeID;//模糊指定一般只会到职位不会到具体人
                            //            string DeptID = IsFuzzy ? GetDeptIDByFuzzyType(FuzzyType) : item.DeptID;
                            //            string JobID = IsFuzzy ? GetJobIDByFuzzyType(FuzzyType) : item.JobID;
                            //            if (JobID == "" && EmployeeID == "")
                            //            {
                            //                continue;
                            //            }
                            //            fl_ApproveFlow entity_fl_ApproveFlow = new fl_ApproveFlow();
                            //            entity_fl_ApproveFlow.FormID = key;
                            //            entity_fl_ApproveFlow.Form_NO = DataTableName;
                            //            entity_fl_ApproveFlow.EmployeeID = EmployeeID;//下一个签核者的ID，如果是职位的话这个为空
                            //            entity_fl_ApproveFlow.Employee_Sort = item.Employee_Sort;//下一个签核者的顺序
                            //            entity_fl_ApproveFlow.DeptID = DeptID;//下一个签核者的部门，如果是经理则与自己相同，如果自己是经理则需要去fl_ApproveForm查询
                            //            entity_fl_ApproveFlow.JobID = JobID;//下一个签核者的职位，如果没有指定人员这个就必须有，如果指定了人员那就是指定人员的职位
                            //            entity_fl_ApproveFlow.Confirm_Class = "";
                            //            entity_fl_ApproveFlow.Confirm_Stat = "X";//A签核 X待签 R驳回
                            //            entity_fl_ApproveFlow.Confirm_Time = DateTime.Now;
                            //            entity_fl_ApproveFlow.ApproveFormID = item.ApproveFormID;

                            //            db.fl_ApproveFlow.Add(entity_fl_ApproveFlow);
                            //            HasApprover = true;//有签核者，不结单
                            //        }
                            //    }
                            //    if (!HasApprover) //下一个顺位没有签核者，则结单
                            //    {
                            //        //更新映射表的状态，表示已签核完毕，结单
                            //        map.Approved = 2;
                            //        map.ApprovedDate = DateTime.Now;
                            //    }
                            //}
                            #endregion

                            //查找是否还有除本条数据以外的其他待签数据
                            bool HasApprover = db.fl_ApproveState.Any(f =>
                                f.FormID == fid &&
                                f.FormNo == key &&
                                f.Confirm_Stat == "X" &&
                                f.AutoKey != entity_fl_ApproveState_Approve.AutoKey);
                            if (!HasApprover)
                            {
                                //如果没有待签数据了就更新映射表的状态，表示已签核完毕，结单
                                map.Approved = 2;
                                map.ApprovedDate = DateTime.Now;
                            }
                        }
                    }
                    else
                    {
                        //被驳回，更新映射表状态
                        map.Approved = 0;
                        map.ApprovedDate = DateTime.Now;

                        //理论上来说，流程中只要有一个驳回了，其他还没审的就不用审了，全部删掉，审过的记录要保留
                        //所以理论上当一个人审核的时候，表要琐
                        var _UnApprovedListBySort = db.fl_ApproveState.Where(f =>
                                f.FormID == fid &&
                                f.FormNo == key &&
                                    //f.Employee_Sort == entity_fl_ApproveFlow_Approve.Employee_Sort &&
                                f.Confirm_Stat == "X" &&
                                f.AutoKey != entity_fl_ApproveState_Approve.AutoKey);
                        db.fl_ApproveState.RemoveRange(_UnApprovedListBySort);
                        //被驳回的单据如果重新提交，那么要重新走全部的流程
                    }
                    db.SaveChanges();
                    db.Database.Connection.Open();
                    var cmd = db.Database.Connection.CreateCommand();
                    cmd.CommandText = "exec sp_fl_ApproveMessageRemind @FormID,@FormNo,@UserID";
                    cmd.Parameters.Add(new SqlParameter("@FormID", fid.ToString()));
                    cmd.Parameters.Add(new SqlParameter("@FormNo", key));
                    cmd.Parameters.Add(new SqlParameter("@UserID", employee_id));
                    var companyid = cmd.ExecuteScalar();
                    db.Database.Connection.Close();
                }
                else
                {
                    Message = "Error";
                }
            }
            catch
            {

                Message = "Error";
            }
            return Message;
        }
        //fgh add  Insert ApproveFlow_Log 2015-01-20
        //private void InsertApproveFlow_Log(int FromVersion, int FormID, string FormNo, string EmployeeID, int SortID, string DeptID,
        //   string JobID, string Confirm_Class, string Confirm_Stat, DateTime Confirm_Time, SysContext db)
        //{
        //    fl_ApproveLog entity_fl_ApproveLog = new fl_ApproveLog();
        //    entity_fl_ApproveLog.Version = FromVersion;
        //    entity_fl_ApproveLog.FormID = FormID;
        //    entity_fl_ApproveLog.FormNo = FormNo;
        //    entity_fl_ApproveLog.EmployeeID = EmployeeID;//发起者的ID
        //    entity_fl_ApproveLog.Employee_Sort = SortID;//发起者的顺序就是0
        //    entity_fl_ApproveLog.DeptID = DeptID;//发起者的部门
        //    entity_fl_ApproveLog.JobID = JobID;//发起者的职位
        //    entity_fl_ApproveLog.Confirm_Class = Confirm_Class;
        //    entity_fl_ApproveLog.Confirm_Stat = Confirm_Stat;//送签用S
        //    entity_fl_ApproveLog.Confirm_Time = Confirm_Time;
        //    entity_fl_ApproveLog.ResetCreated();
        //    db.fl_ApproveLog.Add(entity_fl_ApproveLog);
        //}
        public string AddFlow()
        {
            string Message = "OK";
            int MappingFormID = int.Parse(Request.Form["MappingFormID"]);
            string EmployeeID = Request.Form["emp_id_emp"];
            string CompanyID = Request.Form["CompanyID"];
            string DeptID = Request.Form["dept_id"];
            string JobID = Request.Form["job_id"];
            if (!string.IsNullOrEmpty(EmployeeID))
            {
                try
                {
                    SysContext db = new SysContext();
                    fl_MappingForm map = db.fl_MappingForm.Single(m => m.MappingFormID == MappingFormID);
                    int fid = db.fl_Form.Single(f => f.Companys.Contains(CompanyID) && f.DataFrom == map.FormNo).FormID;
                    fl_ApproveState entity_fl_ApproveState = new fl_ApproveState();
                    entity_fl_ApproveState.FormID = fid;
                    entity_fl_ApproveState.FormNo = map.FormNo;
                    entity_fl_ApproveState.EmployeeID = EmployeeID;//下一个签核者的ID，如果是职位的话这个为空
                    entity_fl_ApproveState.Employee_Sort = 100;//下一个签核者的顺序
                    entity_fl_ApproveState.DeptID = DeptID;//下一个签核者的部门，如果是经理则与自己相同，如果自己是经理则需要去fl_ApproveForm查询
                    entity_fl_ApproveState.JobID = JobID;//下一个签核者的职位，如果没有指定人员这个就必须有，如果指定了人员那就是指定人员的职位
                    entity_fl_ApproveState.Confirm_Class = "N";//A已查阅 N未查阅
                    entity_fl_ApproveState.Confirm_Stat = "L";//A签核 X待签 R驳回 L通知
                    entity_fl_ApproveState.Confirm_Time = DateTime.Now;
                    //entity_fl_ApproveFlow.ApproveFormID = map.ApproveFormID;
                    entity_fl_ApproveState.ModifiedUser = LoginInfo.UserID;
                    entity_fl_ApproveState.CreatedUser = LoginInfo.UserID;
                    db.fl_ApproveState.Add(entity_fl_ApproveState);
                    db.SaveChanges();
                }
                catch
                {
                    Message = "Error";
                }
            }
            else
            {
                Message = "请选择通知者！";
            }
            return Message;
        }

        private List<fl_Approver> GetApproveFormByRules1(int Employee_Sort, string DataTableName, string key, int TemplateID, SysContext db, object entity, string CompanyID)
        {
            int Employee_Sort_MIN = 0;
            var next_forms = db.fl_Approver.Where(f => (f.TemplateID == TemplateID) && (f.CompanyID == CompanyID) && (f.Employee_Sort > Employee_Sort));
            List<fl_Approver> ApproveForm = null;
            if (next_forms.Count() > 0)
            {
                Employee_Sort_MIN = next_forms.Min(f => f.Employee_Sort);
                ApproveForm = next_forms.Where(f => f.Employee_Sort >= Employee_Sort_MIN).ToList();

                if (ApproveForm.Count() > 0)
                {
                    int new_sort = Employee_Sort_MIN;
                    var ApproveRules = db.fl_ApproveRules.Where(r => r.Table_Name == DataTableName);
                    if (ApproveRules.Count() > 0)
                    {
                        //把实例转成DataTable，不然不知道用什么方法进行下面的判断，特别是那个Condition
                        Common c = new Common();
                        DataTable entity_table = c.EntityToDataTable(entity);
                        foreach (var item in ApproveRules)
                        {
                            //在这里进行规则条件的判断
                            //先判断条件是否成立

                            string condition = item.Col_Name + " " + item.Condition;
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
                        //考虑到可能某顺位就一个签核人员，结果被规则刷掉了，于是如果在规则判断过后签核人员为0的话就要往下一个顺位进发继续判断，或者结单
                        if (ApproveForm.Count() == 0)
                        {
                            //真想用goto回到上面
                            ApproveForm = GetApproveFormByRules1(new_sort, DataTableName, key, TemplateID, db, entity, CompanyID);
                        }
                    }
                }
            }
            return ApproveForm;
        }

        public ActionResult ApproveFlow()
        {
            string FormNo = Request.Params["Key"];
            //int FormID = int.Parse(Request.Params["fid"]);
            string DataTableName = Request.Params["DataTableName"];

            SysContext db = new SysContext();
            var ApproveFlows = db.fl_ApproveState.Where(f => f.FormNo == FormNo).ToList();

            var emps = db.hr_Employee.AsEnumerable();
            var jobs = db.hr_Job.AsEnumerable();
            var depts = db.hr_Department.AsEnumerable();
            var companies = db.hr_Company.AsEnumerable();

            var query = (from f in ApproveFlows
                         join e in emps on f.EmployeeID equals e.EmployeeID into joinTemp
                         from tmp in joinTemp.DefaultIfEmpty()
                         join d in depts on f.DeptID equals d.DeptID
                         join j in jobs on f.JobID equals j.JobID
                         join c in companies on d.CompanyID equals c.CompanyID
                         orderby f.Employee_Sort, f.Confirm_Stat, f.Confirm_Time
                         select new
                         {
                             CompanyName = c.Name,
                             DepartmentName = d.Name,
                             JobName = j.Name,
                             EmployeeName = tmp == null ? "" : tmp.Name,
                             f.Confirm_Stat,
                             Confirm_Time = f.Confirm_Stat == "X" ? "" : f.Confirm_Time.Value.ToString(),
                             f.Suggestion
                         }).ToList();

            //一定要放在linq下面，不然会因为各种没有值被刷掉，因为上面的查询不是外联查询
            if (ApproveFlows.Count > 0)
            {
                //如果最后一条记录的状态是A或S 签核就表示整个流程走完了，加一条虚拟的结单记录
                //这里不能拿最后一条判断，当同顺位有多人的时候，有可能正好处于最后一条的人优先签核了，于是在其他人都没有发现的情况下结了单
                int maxSort = ApproveFlows.Max(f => f.Employee_Sort);

                var maxSortFlows = ApproveFlows.Where(f => f.Employee_Sort == maxSort);
                bool AllApproved = true;
                foreach (var flow in maxSortFlows)
                {
                    if (flow.Confirm_Stat.ToLower() == "x" || flow.Confirm_Stat.ToLower() == "r")
                    {
                        AllApproved = false;
                    }
                }
                if (AllApproved)
                {
                    fl_ApproveState last = new fl_ApproveState();
                    last.Confirm_Stat = "O";//Over
                    last.Confirm_Time = maxSortFlows.Max(f => f.Confirm_Time);//时间等于最晚签核记录的时间

                    ApproveFlows.Add(last);

                    query.Add(new
                    {
                        CompanyName = "",
                        DepartmentName = "",
                        JobName = "",
                        EmployeeName = "",
                        Confirm_Stat = "O",
                        //Confirm_Time = ApproveFlows.Last().Confirm_Time.Value.ToString(),
                        Confirm_Time = "",
                        Suggestion = ""
                    });
                }
            }


            JsonResult json = Json(new { rows = query }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public string ApproveMappingFlow()
        {
            int MappingFormID = int.Parse(Request.Form["id"]);
            string ret = "";

            SysContext db = new SysContext();
            Common co = new Common();
            string FormNo = db.fl_MappingForm.Single(m => m.MappingFormID == MappingFormID).FormNo;
            string DataTableName = db.fl_MappingForm.Single(m => m.MappingFormID == MappingFormID).Table_Name;

            //签核通过后对原数据（包含fl和hr）进行更新（如果可以编辑的话）
            Assembly assembly = Assembly.Load("Panasia.Gemini.Fl.Data");
            //创建空实例
            object entity = assembly.CreateInstance("Panasia.Gemini.FL.Data.Models." + DataTableName);
            Type type = entity.GetType();
            //查询version版本号
            string pkeyName = co.GetPKColumn(DataTableName);
            int maxVersion = db.Database.SqlQuery<int>("SELECT ISNULL(MAX([Version]),0) FROM " + DataTableName + " WHERE " + pkeyName + " = '" + FormNo + "'").First();
            //获得数据实例
            System.Data.Entity.DbSet dbSet = db.Set(type);
            entity = dbSet.Find(FormNo, maxVersion);

            //获得提交单据时的公司ID
            PropertyInfo[] PropertyList = type.GetProperties();
            object CompanyID = PropertyList.Where(p => p.Name == "CompanyID").First().GetValue(entity, null);

            int fid = db.fl_Form.Single(f => f.Companys.Contains(CompanyID.ToString()) && f.DataFrom == DataTableName).FormID;
            var ApproveFlows = db.fl_ApproveState.Where(f => f.FormID == fid && f.FormNo == FormNo).ToList();
            var emps = db.hr_Employee.AsEnumerable();
            var jobs = db.hr_Job.AsEnumerable();
            var depts = db.hr_Department.AsEnumerable();
            var companies = db.hr_Company.AsEnumerable();

            var query = (from f in ApproveFlows
                         join e in emps on f.EmployeeID equals e.EmployeeID into joinTemp
                         from tmp in joinTemp.DefaultIfEmpty()
                         join d in depts on f.DeptID equals d.DeptID
                         join j in jobs on f.JobID equals j.JobID
                         join c in companies on d.CompanyID equals c.CompanyID
                         orderby f.Employee_Sort, f.Confirm_Stat, f.Confirm_Time
                         select new
                         {
                             CompanyName = c.Name,
                             DepartmentName = d.Name,
                             JobName = j.Name,
                             EmployeeName = tmp == null ? j.Name : tmp.Name + "(" + j.Name + ")",
                             f.Confirm_Stat,
                             Confirm_Time = f.Confirm_Time.Value.ToString(),
                             f.Suggestion

                         }).ToList();

            foreach (var flows in query)
            {

                if (flows.Confirm_Stat.ToLower() == "r")
                {
                    ret += "," + flows.EmployeeName + ":3";
                }
                else if (flows.Confirm_Stat.ToLower() == "a")
                {
                    ret += "," + flows.EmployeeName + ":1";
                }
                else
                {
                    ret += "," + flows.EmployeeName + ":0";
                }
            }

            //一定要放在linq下面，不然会因为各种没有值被刷掉，因为上面的查询不是外联查询
            if (ApproveFlows.Count > 0)
            {
                //如果最后一条记录的状态是A或S 签核就表示整个流程走完了，加一条虚拟的结单记录
                //这里不能拿最后一条判断，当同顺位有多人的时候，有可能正好处于最后一条的人优先签核了，于是在其他人都没有发现的情况下结了单
                int maxSort = ApproveFlows.Max(f => f.Employee_Sort);

                var maxSortFlows = ApproveFlows.Where(f => f.Employee_Sort == maxSort);
                bool AllApproved = true;
                foreach (var flow in maxSortFlows)
                {
                    if (flow.Confirm_Stat.ToLower() == "x" || flow.Confirm_Stat.ToLower() == "r")
                    {
                        AllApproved = false;

                    }
                }
                if (AllApproved)
                {
                    fl_ApproveState last = new fl_ApproveState();
                    last.Confirm_Stat = "O";//Over
                    last.Confirm_Time = maxSortFlows.Max(f => f.Confirm_Time);//时间等于最晚签核记录的时间

                    ApproveFlows.Add(last);

                    ret += ",结单:2";
                }
                else
                {
                    int template_id = db.fl_Form.Single(t => t.DataFrom == DataTableName).FormID;
                    int maxSort1 = ApproveFlows.Max(f => f.Employee_Sort);

                    var ApproveForm = GetApproveFormByRules1(maxSort1, DataTableName, FormNo, template_id, db, entity, CompanyID.ToString());
                    if (ApproveForm != null && ApproveForm.Count() > 0)
                    {
                        //考虑到每一步都可能有多个签核者同时进行，每次必须以循环的方式插入相应数量的数据
                        foreach (var item in ApproveForm)
                        {
                            bool IsFuzzy = Convert.ToBoolean(item.IsFuzzy);
                            int FuzzyType = Convert.ToInt32(item.FuzzyType);
                            string EmployeeID = IsFuzzy ? "" : item.EmployeeID;//模糊指定一般只会到职位不会到具体人
                            string DeptID = IsFuzzy ? GetDeptIDByFuzzyType(FuzzyType, FormNo) : item.DeptID;
                            string JobID = IsFuzzy ? GetJobIDByFuzzyType(FuzzyType, FormNo) : item.JobID;
                            if (!string.IsNullOrEmpty(EmployeeID))
                            {
                                string name = db.hr_Employee.Single(e => e.EmployeeID == EmployeeID).Name;
                                string job = db.hr_Job.Single(e => e.JobID == JobID).Name;
                                ret += "," + name + "(" + job + ")" + ":0";
                            }
                            else
                            {
                                if (FuzzyType == 3)
                                {
                                    ret += ",事业部总监:0";
                                }
                                else
                                {
                                    string name = db.hr_Job.Single(e => e.JobID == JobID).Name;
                                    ret += "," + name + ":0";
                                }

                            }
                        }
                    }
                }
            }


            //JsonResult json = Json(new { rows = query }, JsonRequestBehavior.AllowGet);
            return ret;
        }

        public ActionResult ApproveDocumentList()
        {
            string DataTableName = Request.Params["table"];
            int pageIndex = string.IsNullOrEmpty(Request.Params["page"]) ? 1 : int.Parse(Request.Params["page"]);
            int pageSize = string.IsNullOrEmpty(Request.Params["rows"]) ? 10 : int.Parse(Request.Params["rows"]);
            string status = Request.Params["status"];
            List<string> where = new List<string>();
            where.Add("(EmployeeID='" + LoginInfo.EmployeeID + "' OR (EmployeeID='' AND JobID='" + LoginInfo.JobID + "' AND DeptID='" + LoginInfo.DeptID + "'))");
            if (status == "")
            {
                status = "X";
            }
            Pager dal_Pager = new Pager();
            PagerSet entity_PagerSet = new PagerSet();
            entity_PagerSet.SortExpression = "CreatedTime";
            entity_PagerSet.SortDirection = "Desc";
            entity_PagerSet.PageCurr = pageIndex;
            entity_PagerSet.PageSize = pageSize;

            Common c = new Common();
            string pkey = c.GetPKColumn(DataTableName);
            string extraCondition = "";
            if (status == "X")
            {
                extraCondition = @" AND b.Employee_Sort=(SELECT MIN(Employee_Sort) FROM fl_ApproveState 
 WHERE fl_ApproveState.FormID=b.FormID AND fl_ApproveState.FormNo=b.FormNo AND fl_ApproveState.Confirm_Stat='X')";
            }
            else
            {
                extraCondition = @" AND b.Confirm_Time = (SELECT ISNULL(MAX(Confirm_Time),0) FROM fl_ApproveState 
 WHERE fl_ApproveState.FormID=b.FormID AND fl_ApproveState.FormNo=b.FormNo AND fl_ApproveState.EmployeeID='" + LoginInfo.EmployeeID + "')";
            }
            StringBuilder table = new StringBuilder();
            table.AppendFormat(@"(SELECT fl_MappingForm.CreatedTime,hr_Department.Name + ' - ' + hr_Employee.Name Name,b.Confirm_Stat,fl_MappingForm.MappingFormID ,fl_MappingForm.FormNo,b.EmployeeID,b.JobID,b.DeptID
FROM fl_MappingForm INNER JOIN fl_ApproveState b ON fl_MappingForm.FormNo=b.FormNo 
 INNER JOIN {0} ON fl_MappingForm.FormNo={0}.{1}
 INNER JOIN hr_Employee ON {0}.EmployeeID=hr_Employee.EmployeeID
 INNER JOIN hr_Department ON hr_Department.DeptID=hr_Employee.DeptID
 LEFT JOIN hr_Employee e1 ON b.EmployeeID=e1.EmployeeID
 WHERE {0}.Version=1 AND b.Confirm_Stat='{2}'
 {3}) c", DataTableName, pkey, status, extraCondition);
            entity_PagerSet.TableOrViewName = table.ToString();
            entity_PagerSet.ConditionString = "WHERE " + string.Join(" AND ", where.ToArray());

            DataTable dt_DocumentList = dal_Pager.ExecutePager(entity_PagerSet);

            int total = entity_PagerSet.RecordCount;
            var totalpage = total / Convert.ToInt32(pageSize);
            if (total % Convert.ToInt32(pageSize) > 0) { totalpage = totalpage + 1; }
            JsonResult json = null;
            if (dt_DocumentList != null)
            {
                var query = from p in dt_DocumentList.AsEnumerable()
                            select new
                            {
                                CreatedTime = p["CreatedTime"].ToString(),
                                Name = p["Name"].ToString(),
                                Confirm_Stat = p["Confirm_Stat"].ToString(),
                                MappingFormID = p["MappingFormID"].ToString(),
                                FormNo = p["FormNo"].ToString()
                            };
                json = Json(new { total = totalpage, rows = query, records = total }, JsonRequestBehavior.AllowGet);
            }

            return json;
        }

        public string DocumentList()
        {
            int pageIndex = string.IsNullOrEmpty(Request.Params["page"]) ? 1 : int.Parse(Request.Params["page"]);
            int pageSize = string.IsNullOrEmpty(Request.Params["rows"]) ? 10 : int.Parse(Request.Params["rows"]);

            string status = Request.Params["status"];
            string DataTableName = Request.Params["tablename"];
            string keyword = Request.Params["keyword"];

            Common c = new Common();
            string pkey = c.GetPKColumn(DataTableName);
            List<string> where = new List<string>();
            DataTable dt_DocumentList = new DataTable();
            //where.Add("EmployeeID='" + LoginInfo.EmployeeID + "'");

            string json = string.Empty;
            if (!string.IsNullOrEmpty(DataTableName) && !string.IsNullOrEmpty(status))
            {
                where.Add("Approved=" + status);
                string extraCondition = "";
                string IsEmployeeID = "";
                if (LoginInfo.UserID != "U00001")
                {
                    IsEmployeeID = "and EmployeeID='" + LoginInfo.EmployeeID + "'";
                }
                if (status == "1")
                {
                    extraCondition = " WHERE (Confirm_Stat='S' or Confirm_Stat='X' or Confirm_Stat='A') " + IsEmployeeID;
                }
                else
                {
                    extraCondition = " WHERE 1=1 " + IsEmployeeID;
                }
                if (!string.IsNullOrEmpty(keyword))
                {
                    where.Add("FormID LIKE '%" + keyword + "%'");
                }
                Pager dal_Pager = new Pager();
                PagerSet entity_PagerSet = new PagerSet();
                entity_PagerSet.SortExpression = "CreatedTime";
                entity_PagerSet.SortDirection = "Desc";
                entity_PagerSet.PageCurr = pageIndex;
                entity_PagerSet.PageSize = pageSize;

                StringBuilder table = new StringBuilder();
                table.AppendFormat(@"(SELECT  dbo.fl_MappingForm.FormNo,dbo.fl_MappingForm.MappingFormID, dbo.fl_MappingForm.CreatedTime,
                dbo.fl_MappingForm.Approved,fl_ApproveState.DeptID AS ApproveDepartmentID, dbo.hr_Department.Name AS ApproveDepartmentName, 
                fl_ApproveState.JobID AS ApproveJobID, dbo.hr_Job.Name AS ApproveJobName, 
                fl_ApproveState.EmployeeID AS ApproveEmployeeID, dbo.hr_Employee.Name AS ApproveEmployeeName, 
                hr_Employee_1.Name AS CreatedEmployeeName, hr_Department_1.Name AS CreatedDeptName, 
                fl_ApproveState.Confirm_Time
FROM      dbo.fl_MappingForm INNER JOIN
                dbo.{0} ON dbo.fl_MappingForm.FormNo = dbo.{0}.{1} INNER JOIN
                    (SELECT   *
                     FROM      (SELECT   t .*, row_number() OVER (partition BY FormID, FormNo
                                      ORDER BY AutoKey Desc) rn
                     FROM      dbo.fl_ApproveState t{2}) a
WHERE   rn = 1) fl_ApproveState ON dbo.fl_MappingForm.FormNo = fl_ApproveState.FormNo INNER JOIN
dbo.hr_Department ON fl_ApproveState.DeptID = dbo.hr_Department.DeptID INNER JOIN
dbo.hr_Job ON fl_ApproveState.JobID = dbo.hr_Job.JobID LEFT OUTER JOIN
dbo.hr_Employee ON fl_ApproveState.EmployeeID = dbo.hr_Employee.EmployeeID INNER JOIN
dbo.hr_Employee hr_Employee_1 ON dbo.{0}.EmployeeID = hr_Employee_1.EmployeeID INNER JOIN
dbo.hr_Department hr_Department_1 ON hr_Employee_1.DeptID = hr_Department_1.DeptID) a", DataTableName, pkey, extraCondition);

                entity_PagerSet.TableOrViewName = table.ToString();
                entity_PagerSet.ConditionString = "WHERE " + string.Join(" AND ", where.ToArray());

                dt_DocumentList = dal_Pager.ExecutePager(entity_PagerSet);

                int total = entity_PagerSet.RecordCount;
                //拼接json语句
                if (dt_DocumentList != null)
                {
                    StringBuilder rowJson = new StringBuilder();
                    int colLen = dt_DocumentList.Columns.Count;

                    System.Data.DataColumnCollection col = dt_DocumentList.Columns;

                    foreach (System.Data.DataRow row in dt_DocumentList.Rows)
                    {
                        System.Text.StringBuilder colJson = new System.Text.StringBuilder();
                        rowJson.Append("{");
                        SysContext db = new SysContext();
                        string FormNo = row[1].ToString();
                        IList<fl_ApproveState> flaEmployeeID = db.fl_ApproveState.Where(m => m.Confirm_Stat == "A" && m.FormNo == FormNo).OrderByDescending(m => m.Employee_Sort).ToList();
                        if (flaEmployeeID != null && flaEmployeeID.Count > 0 && !string.IsNullOrEmpty(flaEmployeeID[0].EmployeeID))
                        {
                            string EmployeeID = flaEmployeeID[0].EmployeeID;
                            string appname = db.hr_Employee.Single(e => e.EmployeeID == EmployeeID).Name;
                            row[10] = appname;
                        }

                        for (int i = 0; i < colLen; i++)
                        {
                            colJson.Append("\"" + col[i].ColumnName.Trim() + "\":\"" + row[i].ToString().Trim() + "\",");
                        }
                        rowJson.Append(colJson.ToString().TrimEnd(','));
                        rowJson.Append("},");
                    }
                    var page = Convert.ToInt32(total) / Convert.ToInt32(pageSize);
                    if (Convert.ToInt32(total) % Convert.ToInt32(pageSize) > 0) { page = page + 1; }
                    json = "{\"total\":" + page + ",\"rows\":[" + rowJson.ToString().TrimEnd(',') + "],\"page\":" + pageIndex + ",\"records\":" + total + "}";
                }
            }
            return json;
        }

        public string GetValueByID()
        {
            string table = Request.Params["table"];
            string nameField = Request.Params["nameField"];
            string valueField = Request.Params["valueField"];
            string value = Request.Params["value"];

            Common c = new Common();
            DataSet ds = c.Query(table, valueField + "='" + value + "'", "", nameField);
            string returnString = "";
            if (ds != null && ds.Tables[0].Rows.Count == 1)
            {
                returnString = ds.Tables[0].Rows[0][0].ToString();
            }
            return returnString;
        }

        public ActionResult GetListByKey()
        {
            string table = Request.Params["table"];
            string keys = Request.Params["keys"];
            string[] keyArray = keys.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            string fields = Request.Params["fields"];
            string[] fieldArray = fields.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            Common c = new Common();
            DataSet ds = c.Query(table, string.Join(" AND ", keyArray), "", fieldArray);
            bool HasError = true;
            List<Dictionary<string, string>> data = new List<Dictionary<string, string>>();
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                HasError = false;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Dictionary<string, string> item = new Dictionary<string, string>();
                    foreach (DataColumn col in ds.Tables[0].Columns)
                    {
                        item.Add(col.ColumnName, row[col.ColumnName].ToString());
                    }
                    data.Add(item);
                }
            }
            return Json(new { HasError = HasError, Data = data }, JsonRequestBehavior.AllowGet);
        }
    }
}
