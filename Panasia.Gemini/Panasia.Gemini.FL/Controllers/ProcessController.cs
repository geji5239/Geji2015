/*----------------------------------------------------------------
// Copyright (C) 2004 清风软件
// 版权所有。
//
// 文件名：ProcessController.cs
// 文件功能描述：Fl004,Fl005控制器,流程设计，包括规则设置，签核者设置，其他设置的各项功能。
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Panasia.Gemini.FL.Data.Common;
using Panasia.Gemini.FL.Data.Models;
using System.Collections;

namespace Panasia.Gemini.FL.Controllers
{
    public class ProcessController : Controller
    {
        // GET: Process
        public ActionResult Fl004(string FormID,string DataForm)
        {
            SysContext db = new SysContext();
            var tf = db.fl_Form.AsEnumerable();
            ViewData["TemplateList_Rule"] = new SelectList(tf.Select(t => new { Text = t.FormName, Value = t.DataFrom + "|" + t.FormID.ToString() }), "Value", "Text");
            ViewData["TemplateList_Emp"] = new SelectList(tf, "FormID", "FormName");
            ViewData["Company"] = new SelectList(db.hr_Company.Where(c=>c.IsActive==true).OrderBy(c=>c.SortID).AsEnumerable(), "CompanyID", "Name");
            ViewData["FormID"] = FormID;
            ViewData["DataForm"] = DataForm;
            int id=Convert.ToInt32(FormID);
            var approve = db.fl_Approver.Where(app => app.FormID.Equals(id)).FirstOrDefault();
            if (approve == null)
            {
                ViewData["IsCopy"] = 1;
            }
            else
            {
                ViewData["IsCopy"] = 0;
            }
            ViewData["FormName"] = db.fl_Form.Where(f => f.FormID.Equals(id)).SingleOrDefault().FormName;
            return View();
        }
        public ActionResult Fl007(string FormID, string DataForm)
        {
            SysContext db = new SysContext();
            var tf = db.fl_Form.AsEnumerable();
            ViewData["TemplateList_Rule"] = new SelectList(tf.Select(t => new { Text = t.FormName, Value = t.DataFrom + "|" + t.FormID.ToString() }), "Value", "Text");
            ViewData["TemplateList_Emp"] = new SelectList(tf, "FormID", "FormName");
            ViewData["Company"] = new SelectList(db.hr_Company.Where(c => c.IsActive == true).OrderBy(c => c.SortID).AsEnumerable(), "CompanyID", "Name");
            ViewData["FormID"] = FormID;
            ViewData["DataForm"] = DataForm;
            int id = Convert.ToInt32(FormID);
            var approve = db.fl_Approver.Where(app => app.FormID.Equals(id)).FirstOrDefault();
            if (approve == null)
            {
                ViewData["IsCopy"] = 1;
            }
            else
            {
                ViewData["IsCopy"] = 0;
            }
            ViewData["FormName"] = db.fl_Form.Where(f => f.FormID.Equals(id)).SingleOrDefault().FormName;
            return View();
        }
        public ActionResult Fl006()
        {
            return View();
        }

        public ActionResult GetTemplateWithProcess()
        {
            try {
                using (SysContext db = new SysContext())
                {
                    string userid= Panasia.Gemini.FL.LoginInfo.UserID;
                    db.Database.Connection.Open();
                    var cmd = db.Database.Connection.CreateCommand();
                    cmd.CommandText = "select * from fun_sys_GetUserCompanys('" + userid + "')";
                    ArrayList companyids =new ArrayList();
                    using (var reader = cmd.ExecuteReader(System.Data.CommandBehavior.SequentialAccess))
                    {
                        while (reader.Read())
                        {
                            companyids.Add(reader["CompanyID"]);
                        }
                        reader.Close();
                    }
                    var ids = "";
                    db.Database.Connection.Close();
                    var query = db.fl_Form.ToList();
                    List<object> frms = new List<object>();
                    foreach (var frm in query)
                    {
                        //var comName = db.hr_Company.Where(com => com.CompanyID.Equals(companyid)).SingleOrDefault().JC;
                        var user = string.IsNullOrEmpty(frm.CreatedUser); //? "" :db.hr_Employee.Where(emp=>emp.UserID.Equals(frm.CreatedUser)).SingleOrDefault().Name;
                        var Apper = db.fl_Approver.Where(a => a.FormID == frm.FormID).ToList();

                        string formname = "";
                        string companyName = "";
                        foreach (var item in Apper)
                        {
                            string deptname = "";
                            string jobname = "";
                            if (item.DeptID == "111111")
                            {
                                deptname = "发起部门";
                            }
                            else if (item.DeptID == "222222")
                            {
                                deptname = "自定义部门";
                            }
                            else if (item.DeptID == "444444")
                            {
                                deptname = "事业部";
                            }
                            else
                            {
                                deptname = db.hr_Department.Single(j => j.DeptID == item.DeptID).Name;
                            }
                            jobname = db.hr_Job.Single(j => j.JobID == item.JobID).Name;
                            formname += "-" + deptname + jobname;
                        }
                        var companys = frm.Companys.Split(',');
                        foreach (var arr in companys)
                        {
                            string companyid = arr.ToString();
                            if(companyid!="on"&&!string.IsNullOrEmpty(companyid))
                            {
                                companyName += "," + db.hr_Company.Where(com => com.CompanyID.Equals(companyid)).SingleOrDefault().JC;
                            }
                            
                        }
                        frms.Add(new { ID = frm.FormID, DataFrom=frm.DataFrom, CompanyName =companyName==""?"": companyName.Substring(1), FormName = frm.FormName, ApproverName =formname==""?"": formname.Substring(1), CreateUser = user, CreateTime = frm.CreatedTime.ToString() });
                    }
                    var page = Convert.ToInt32(Request["page"]);
                    var rows = Convert.ToInt32(Request["rows"]);
                    var totalpage = frms.Count / Convert.ToInt32(rows);
                    if (frms.Count % Convert.ToInt32(rows) > 0) { totalpage = totalpage + 1; }
                    return Json(new { rows = frms.Skip((page - 1) * rows).Take(rows), page = page, total = totalpage, records = frms.Count }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e) {
                return null;
            }
        }
        public string AddEmployee(Panasia.Gemini.FL.Models.ProcessModel model)
        {
            bool IsFuzzy = model.IsFuzzy;
            string dept_id = IsFuzzy ? "" : model.dept_id;
            string job_id = IsFuzzy ? "" : model.job_id;
            string emp_id_emp = IsFuzzy ? "" : model.emp_id_emp;
            int emp_sort = model.emp_sort;
            int TemplateID = model.FormID;
            int FuzzyType = IsFuzzy ? model.FuzzyType : 0;
            string CompanyID = model.CompanyID;
            string Message = "OK";
            string EmployeeIDCondition = model.hEmployeeIDCondition;
            string DeptIDCondition=model.hDeptIDCondition;
            bool IsGroupApprover = model.IsGroupApprover;
            string Approverid = model.Approverid;
            string Approver = model.Approver;
            if(emp_id_emp=="333333"&&model.hEmployeeIDCondition==null)
            {
                emp_id_emp = "";
            }
            if(dept_id=="222222"&&model.hDeptIDCondition==null)
            {
                dept_id = "111111";
            }
            using (SysContext db = new SysContext())
            {
               
                try
                {
                    //bool isg = db.fl_Approver.Where(a => a.FormID == TemplateID && a.Employee_Sort == emp_sort).First().IsGroupApprover;
                    //if (isg == IsGroupApprover)
                    //{
                    if (string.IsNullOrEmpty(Approverid))
                    {
                        fl_Approver entity_fl_Approve = new fl_Approver();
                        entity_fl_Approve.FormID = TemplateID;
                        entity_fl_Approve.Employee_Sort = emp_sort;
                        entity_fl_Approve.EmployeeID = emp_id_emp == null ? "" : emp_id_emp;
                        entity_fl_Approve.DeptID = dept_id;
                        entity_fl_Approve.JobID = job_id;
                        entity_fl_Approve.IsFuzzy = IsFuzzy;
                        entity_fl_Approve.FuzzyType = FuzzyType;
                        entity_fl_Approve.TemplateID = model.TemplateID;
                        entity_fl_Approve.CompanyID = CompanyID;
                        entity_fl_Approve.DeptIDCondition = DeptIDCondition;
                        entity_fl_Approve.EmployeeIDCondition = EmployeeIDCondition;
                        entity_fl_Approve.IsGroupApprover = IsGroupApprover;
                        entity_fl_Approve.CreatedUser = LoginInfo.UserID;
                        entity_fl_Approve.ModifiedUser = LoginInfo.UserID;
                        entity_fl_Approve.ResetCreated();


                        db.fl_Approver.Add(entity_fl_Approve);
                        db.SaveChanges();
                    }
                    else
                    {
                        string[] Approverids = Approverid.Split(',');
                        foreach(var arr in Approverids)
                        {
                            int aid=int.Parse(arr.ToString());
                            fl_Approver fl_Approve = db.fl_Approver.Single(a => a.ApproverID == aid);
                            fl_Approver entity_fl_Approve = new fl_Approver();
                            entity_fl_Approve.FormID = int.Parse(model.form_id);
                            entity_fl_Approve.Employee_Sort = fl_Approve.Employee_Sort;
                            entity_fl_Approve.EmployeeID = fl_Approve.EmployeeID;
                            entity_fl_Approve.DeptID = fl_Approve.DeptID;
                            entity_fl_Approve.JobID = fl_Approve.JobID;
                            entity_fl_Approve.IsFuzzy = fl_Approve.IsFuzzy;
                            entity_fl_Approve.FuzzyType = fl_Approve.FuzzyType;
                            entity_fl_Approve.TemplateID = fl_Approve.TemplateID;
                            entity_fl_Approve.CompanyID = fl_Approve.CompanyID;
                            entity_fl_Approve.DeptIDCondition = fl_Approve.DeptIDCondition;
                            entity_fl_Approve.EmployeeIDCondition = fl_Approve.EmployeeIDCondition;
                            entity_fl_Approve.IsGroupApprover = fl_Approve.IsGroupApprover;
                            entity_fl_Approve.CreatedUser = fl_Approve.CreatedUser;
                            entity_fl_Approve.ModifiedUser = fl_Approve.ModifiedUser;
                            entity_fl_Approve.ResetCreated();

                            db.fl_Approver.Add(entity_fl_Approve);
                            db.SaveChanges();
                        }
                        if (!string.IsNullOrEmpty(Approver))
                        {
                            string[] Approvers = Approver.Split('|');
                            foreach (var arr in Approvers)
                            {
                                string[] fl_Approves = arr.Replace("undefined", "").Split(',');
                                fl_Approver entity_fl_Approve = new fl_Approver();
                                entity_fl_Approve.FormID = int.Parse(model.form_id);
                                entity_fl_Approve.Employee_Sort = int.Parse(fl_Approves[5].ToString());
                                entity_fl_Approve.EmployeeID = fl_Approves[4];
                                entity_fl_Approve.DeptID = fl_Approves[2];
                                entity_fl_Approve.JobID = fl_Approves[3];
                                entity_fl_Approve.IsFuzzy = false;
                                entity_fl_Approve.FuzzyType = 1;
                                entity_fl_Approve.TemplateID = int.Parse(fl_Approves[0].ToString());
                                entity_fl_Approve.CompanyID = fl_Approves[1];
                                entity_fl_Approve.DeptIDCondition = fl_Approves[6];
                                entity_fl_Approve.EmployeeIDCondition = fl_Approves[7];
                                entity_fl_Approve.IsGroupApprover = bool.Parse(fl_Approves[8].ToString());
                                entity_fl_Approve.CreatedUser = LoginInfo.UserID;
                                entity_fl_Approve.ModifiedUser = LoginInfo.UserID;
                                entity_fl_Approve.ResetCreated();

                                db.fl_Approver.Add(entity_fl_Approve);
                                db.SaveChanges();
                            }

                        }
                    
                    }
                    //}
                    //else
                    //{
                    //    Message = "同一次序必须选择相同的签核次序";
                    //}
                  

                }
                catch
                {
                   
                    Message = "Error";
                }
            }
            return Message;
        }

        public string DeleteEmployee()
        {
            string Message = "OK";
            try
            {
                using (SysContext db = new SysContext())
                {
                    int id = int.Parse(Request["id"]);
                    db.fl_Approver.Remove(db.fl_Approver.Find(id));
                    fl_ApproveRules rule = db.fl_ApproveRules.FirstOrDefault(r => r.ApproverID == id);
                    if (rule != null)
                    {
                        db.fl_ApproveRules.Remove(rule);
                    }
                    db.SaveChanges();
                }
            }
            catch
            {
                Message = "Error";
            }
            return Message;
        }
        public ActionResult ProcessList()
        {
            int tid = int.Parse(Request.QueryString["tid"]);
            //string companyID = Request.QueryString["cid"];
            //string Message = "OK";
            JsonResult json = null;
            try
            {
                SysContext db = new SysContext();
                var forms = db.fl_Approver.AsEnumerable();
                var emps = db.hr_Employee.AsEnumerable();
                var jobs = db.hr_Job.AsEnumerable();
                var depts = db.hr_Department.AsEnumerable();
                var companys = db.hr_Company.AsEnumerable();

                //string companyID = db.fl_Form.Single(f => f.FormID == tid).CompanyID;
                var tfd = db.fl_FormTemplate.Where(d => d.FormID == tid ).AsEnumerable();
                var query = from form in forms
                            join emp in emps on form.EmployeeID equals emp.EmployeeID into Join_1
                            from j1 in Join_1.DefaultIfEmpty()
                            join job in jobs on form.JobID equals job.JobID into Join_2
                            from j2 in Join_2.DefaultIfEmpty()
                            join dept in depts on form.DeptID equals dept.DeptID into Join_3
                            from j3 in Join_3.DefaultIfEmpty()
                            join template_detail in tfd on form.TemplateID equals template_detail.TemplateID into Join_4
                            from j4 in Join_4.DefaultIfEmpty()
                            join company in companys on form.CompanyID equals company.CompanyID into Join_5
                            from j5 in Join_5.DefaultIfEmpty()
                            where form.FormID == tid 
                            orderby form.Employee_Sort, ((j1 == null ? "" : j1.Name))
                            select new
                            {
                                approveform_id = form.ApproverID,
                                CompanyID = (j5 == null) ? "" : j5.Name,
                                emp_sort = form.Employee_Sort,
                                emp_name = (form.EmployeeID == "333333") ? "自定义签核" : (j1 == null) ? "" : j1.Name,
                                dept_name = (form.DeptID=="111111" || form.DeptID=="222222" || form.DeptID=="444444") ? Getdept_id(form.DeptID) : (j3 == null) ? "" : j3.Name,
                                job_name = form.IsFuzzy ? GetFuzzyName(form.FuzzyType.Value) : (j2 == null) ? "" : j2.Name,
                                template_name = (j4 == null) ? "" : j4.Template_Name,
                                isGroup = form.IsGroupApprover?"同组":"逐一",
                                dept_id = (j3 == null) ? "" : j3.DeptID,
                                job_id = (j2 == null) ? "" : j2.JobID,
                                emp_id_emp = (form.EmployeeID=="333333")?"自定义签核":(j1 == null) ? "" : j1.EmployeeID
                            };
                json = Json(new { rows = query }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                //Message = "Error";
            }
            return json;
        }


        private string Getdept_id(string dept_id)
        {
            string name = "";
            switch (dept_id)
            {
                case "111111": name = "发起部门"; break;
                case "222222": name = "自定义部门"; break;
                case "444444": name = "事业部"; break;
            }
            return name;
        }


        private string GetFuzzyName(int fuzzyType)
        {
            string name = "";
            switch (fuzzyType)
            {
                case 1: name = "部门主管"; break;
                case 2: name = "部门总监"; break;
                case 3: name = "事业部总监"; break;
                case 4: name = "交接人"; break;
                case 5: name = "部门主管（调入）"; break;
            }
            return name;
        }
        public ActionResult RuleList()
        {
            int tid = int.Parse(Request.QueryString["tid"]);
            //string companyID = Request.QueryString["cid"];
            //string Message = "OK";
            JsonResult json = null;
            try
            {
                SysContext db = new SysContext();
                var rules = db.fl_ApproveRules.AsEnumerable();
                var forms = db.fl_Approver.AsEnumerable();
                var emps = db.hr_Employee.AsEnumerable();
                var jobs = db.hr_Job.AsEnumerable();
                var depts = db.hr_Department.AsEnumerable();
                string companyID = db.fl_Form.Single(f => f.FormID == tid).Companys;
                var query = from rule in rules
                            join form in forms on rule.ApproverID equals form.ApproverID
                            join emp in emps on form.EmployeeID equals emp.EmployeeID into Join_1
                            from j1 in Join_1.DefaultIfEmpty()
                            join job in jobs on form.JobID equals job.JobID into Join_2
                            from j2 in Join_2.DefaultIfEmpty()
                            join dept in depts on form.DeptID equals dept.DeptID into Join_3
                            from j3 in Join_3.DefaultIfEmpty()
                            where rule.FormID == tid && companyID.Contains(form.CompanyID)
                            orderby rule.Col_Name, form.Employee_Sort, ((j1 == null ? "" : j1.Name))
                            select new
                            {
                                ApproveRulesid = rule.ApproveRulesid,
                                Col_Name = rule.Col_Name,
                                Condition =  rule.Condition,
                                ApproveFormName = GetNameForRule(form, j1, j2, j3)
                            };
                json = Json(new { rows = query }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                //Message = "Error";
            }
            return json;
        }

        private string GetNameForRule(fl_Approver form, Panasia.Core.Sys.hr_Employee emp, hr_Job job, hr_Department dept)
        {
            string name = "";
            if (form.IsFuzzy)
            {
                name = "本部门-" + GetFuzzyName(form.FuzzyType.Value);
            }
            else
            {
                name = ((dept == null) ? "" : dept.Name) + "-" + (emp == null ? ((job == null) ? "" : job.Name) : emp.Name);
            }
            return name;
        }

        public string GetColumnsListByTemplateID()
        {
            Common c = new Common();
            System.Data.DataSet ds_ColumnList = c.GetNumericColumnNameByTableName(Request["table_name"]);

            System.Text.StringBuilder optionItems = new System.Text.StringBuilder();
            foreach (System.Data.DataRow dr in ds_ColumnList.Tables[0].Rows)
            {
                if (dr["COLUMN_NAME"].ToString().ToLower() == "version")
                {
                    //版本号不算条件
                    continue;
                }
                optionItems.AppendFormat("<option value='{0}'>{0}</option>", dr["COLUMN_NAME"].ToString());
            }

            return optionItems.ToString();
        }

        public string GetEmployeeByTemplateID()
        {
            int template_id = int.Parse(Request.Params["template_id"]);
            //string companyID = Request.Params["company_id"];
            SysContext db = new SysContext();
            string companyID = db.fl_Form.Single(f => f.FormID == template_id).Companys;
            var query = (from form in db.fl_Approver
                         join emp in db.hr_Employee on form.EmployeeID equals emp.EmployeeID into Join_1
                         from j1 in Join_1.DefaultIfEmpty()
                         join job in db.hr_Job on form.JobID equals job.JobID into Join_2
                         from j2 in Join_2.DefaultIfEmpty()
                         join dept in db.hr_Department on form.DeptID equals dept.DeptID into Join_3
                         from j3 in Join_3.DefaultIfEmpty()
                         where form.FormID == template_id && companyID.Contains(form.CompanyID)
                         orderby form.Employee_Sort, ((j1 == null ? "" : j1.Name))
                         select new
                         {
                             Value = form.ApproverID.ToString(),
                             form.IsFuzzy,
                             FuzzyType = form.FuzzyType.Value,
                             Name = ((j3 == null) ? "" : j3.Name) + "-" + (j1 == null ? ((j2 == null) ? "" : j2.Name) : j1.Name),
                             DeptID=form.DeptID,
                            
                         }).ToList();
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var item in query)
            {
                string Text = "";
                if (item.DeptID=="111111")
                {
                    Text = "发起部门" + item.Name;
                }
                else if(item.DeptID=="222222")
                {
                    Text = "自定义部门" + item.Name;
                }
                else if(item.DeptID=="444444")
                {
                    Text = "事业部" + item.Name;
                }
                else
                {
                    Text = item.Name;
                }
                items.Add(new SelectListItem { Text = Text, Value = item.Value });
            }

            System.Text.StringBuilder optionItems = new System.Text.StringBuilder();
            items.ForEach(delegate(SelectListItem item)
            {
                optionItems.AppendFormat("<option value='{0}'>{1}</option>", item.Value, item.Text);
            });

            return optionItems.ToString();
        }

        public string AddRule(Panasia.Gemini.FL.Models.ProcessModel model)
        {
            string Template_Info = model.Template_Info;
            string col_name = model.col_name;
            string condition = model.value;
            //decimal value = model.value;
            int approveform_id = model.approveform_id;
            string Message = "OK";
            try
            {

                fl_ApproveRules entity_fl_ApproveRules = new fl_ApproveRules();
                entity_fl_ApproveRules.FormID = int.Parse(Template_Info.Substring(Template_Info.IndexOf("|") + 1));
                entity_fl_ApproveRules.Table_Name = Template_Info.Substring(0, Template_Info.IndexOf("|"));
                entity_fl_ApproveRules.Col_Name = col_name;
                entity_fl_ApproveRules.Condition = condition;
                entity_fl_ApproveRules.ApproverID = approveform_id;
                entity_fl_ApproveRules.CreatedUser = LoginInfo.UserID;
                entity_fl_ApproveRules.ModifiedUser = LoginInfo.UserID;
                entity_fl_ApproveRules.ResetCreated();
                using (SysContext db = new SysContext())
                {
                    db.fl_ApproveRules.Add(entity_fl_ApproveRules);
                    db.SaveChanges();
                }
            }
            catch
            {
                Message = "Error";
            }
            return Message;
        }

        public string DeleteRule()
        {
            string Message = "OK";
            try
            {
                using (SysContext db = new SysContext())
                {
                    db.fl_ApproveRules.Remove(db.fl_ApproveRules.Find(int.Parse(Request["id"])));
                    db.SaveChanges();
                }
            }
            catch
            {
                Message = "Error";
            }
            return Message;;
        }

        public ActionResult Fl005()
        {
            ViewData["TemplateList"] = new SelectList(new SysContext().fl_Form, "DataFrom", "FormName");
            SysContext db = new SysContext();
            ViewData["Company"] = new SelectList(db.hr_Company.OrderBy(c => c.SortID).AsEnumerable(), "CompanyID", "Name");
            return View();
        }

        public string GetTemplateDetailByID()
        {
            int id = int.Parse(Request.Params["id"]);
            System.Text.StringBuilder options = new System.Text.StringBuilder();
            SysContext db = new SysContext();
            var details = db.fl_FormTemplate.Where(d => d.FormID == id).ToList();
            details.ForEach(delegate(fl_FormTemplate d)
            {
                options.AppendFormat("<option value=\"{0}\">{1}</option>", d.TemplateID, d.Template_Name);
            });

            return options.ToString();
        }
    }
}