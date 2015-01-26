using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Panasia.Gemini.Web.Models;
using System.Text;
using Panasia.Core.Sys;

namespace Panasia.Gemini.Web.Controllers
{

    public class TreeNodeExtend : Panasia.Gemini.Web.Models.Common.TreeNode
    {
        public string type { get; set; }
    }
    public class CommonController : Controller
    {
        Panasia.Core.Sys.SysContext dbsys = new Core.Sys.SysContext();

        private string state = "";
        private string defaultDataType = "";
        private string defaultData = "";
        public ActionResult InitOrgTree()
        {
            SysContext db = new SysContext();
            defaultDataType = Request.QueryString["defaultDataType"].ToLower();
            defaultData = Request.QueryString["defaultData"];
            if (!string.IsNullOrEmpty(defaultData) && !string.IsNullOrEmpty(defaultDataType))
            {
                if (defaultData.ToLower() == "[user]")
                {
                    if (User == null)
                    {
                        defaultData = "";
                    }
                    else
                    {
                        Panasia.Core.Sys.SystemParameterFunctions sysParameter = new Core.Sys.SystemParameterFunctions();
                        string userID = sysParameter.GetCurrentUserID("").ToString();
                        //登录的时候没有把公司部门等ID信息放到登录者信息中去，这里暂时用数据库查询的方式获取，以后可以改
                        string departmentID = dbsys.hr_Employees.Where(e => e.UserID == userID).Select(e => e.DeptID).ToList()[0];
                        if (defaultDataType == "c")
                        {
                            defaultData = db.hr_Depts.Find(departmentID).CompanyID;
                        }
                        else if (defaultDataType == "d")
                        {
                            defaultData = departmentID;
                        }
                    }
                }
            }
            List<hr_Company> companies = null;
            //这里的逻辑是这样的，不管defaultDataType是c还是d，company肯定都只有特定的一个了
            if (defaultDataType == "c" || defaultDataType == "d")
            {
                if (defaultDataType == "c")
                {
                    companies = db.hr_Companies.Where(c => c.IsActive && c.CompanyID == defaultData).ToList();
                }
                else if (defaultDataType == "d")
                {
                    hr_Department dept = db.hr_Depts.Find(defaultData);
                    companies = db.hr_Companies.Where(c => c.IsActive && c.CompanyID == dept.CompanyID).ToList();
                }
            }
            else
            {
                companies = db.hr_Companies.Where(c => c.IsActive).OrderBy(c => c.SortID).ToList();
            }
            List<hr_Department> all_depts = null;
            List<hr_Job> all_jobs = null;
            List<hr_Employee> all_emps = null;

            //type:1、全部呈现，包括公司，部门，职位和员工 2、公司、部门、职位 3、公司、部门 4、公司
            int configType = 1;
            int.TryParse(Request.QueryString["configType"], out configType);
            if (configType < 4)
            {
                if (defaultDataType == "d")
                {
                    all_depts = new List<hr_Department>();
                    GetListWithAllParent(defaultData, db.hr_Depts.ToList(), all_depts);
                }
                else
                {
                    all_depts = db.hr_Depts.Where(d => d.IsActive).OrderBy(c => c.SortID).ToList();
                }
            }
            if (configType < 3)
            {
                //因为部门职位需要通过员工来查询，所以<3的就全部信息都需要了
                all_jobs = db.hr_Jobs.Where(j => j.IsActive).OrderBy(j => j.SortID).ToList();
                all_emps = dbsys.hr_Employees.Where(e => e.IsActive).ToList();
            }

            state = Request.QueryString["defaultState"];
            if (state == "1")
            {
                state = "open";
            }
            else
            {
                state = "closed";
            }

            List<TreeNodeExtend> tree = new List<TreeNodeExtend>();
            if (companies.Count > 0)
            {
                foreach (hr_Company company in companies)
                {
                    TreeNodeExtend node = new TreeNodeExtend();
                    node.id = company.CompanyID;
                    node.text = company.Name;
                    node.type = "c";
                    node.iconCls = "icon-company";

                    if (configType < 4)
                    {
                        List<TreeNodeExtend> children = DepartmentChildren(company.CompanyID, "", all_depts, all_jobs, all_emps, configType);
                        if (children.Count > 0)
                        {
                            node.state = state;
                            node.children.AddRange(children);
                        }
                    }
                    tree.Add(node);
                }
            }
            JsonResult json = Json(tree, JsonRequestBehavior.AllowGet);
            return json;
        }

        /// <summary>
        /// 递归取得部门结构JSON
        /// </summary>
        private List<TreeNodeExtend> DepartmentChildren(string CompanyID, string ParentID, List<hr_Department> all_depts, List<hr_Job> all_jobs, List<hr_Employee> all_emps, int configType)
        {
            List<hr_Department> depts_by_parentid = all_depts.Where(d => (d.CompanyID == CompanyID && d.ParentID == ParentID)).OrderBy(d => d.SortID).ToList();
            List<TreeNodeExtend> tree = new List<TreeNodeExtend>();
            if (depts_by_parentid.Count > 0)
            {
                foreach (hr_Department dept in depts_by_parentid)
                {
                    TreeNodeExtend node = new TreeNodeExtend();
                    node.id = dept.DeptID;
                    node.text = dept.Name;
                    node.type = "d";
                    node.iconCls = "icon-dept";
                    List<TreeNodeExtend> children = new List<TreeNodeExtend>();
                    if (configType < 3)
                    {
                        if (defaultDataType != "d" || defaultDataType == "d" && dept.DeptID == defaultData)
                        {
                            children = DepartmentJobs(dept.DeptID, all_jobs, all_emps, configType);
                        }
                    }
                    List<TreeNodeExtend> dept_children = DepartmentChildren(CompanyID, dept.DeptID, all_depts, all_jobs, all_emps, configType);
                    children.AddRange(dept_children);
                    if (children.Count > 0)
                    {
                        node.state = state;
                        node.children.AddRange(children);
                    }
                    tree.Add(node);
                }
            }
            return tree;
        }

        /// <summary>
        /// 通过部门ID查询该部门职位（关联员工表）
        /// </summary>
        private List<TreeNodeExtend> DepartmentJobs(string DeptID, List<hr_Job> all_jobs, List<hr_Employee> all_emps, int configType)
        {
            List<string> jobs_id_by_deptid = all_emps.Where(e => (e.DeptID == DeptID)).Select(e => e.JobID).Distinct().ToList();//查询该部门所有员工，取出他们的职位，去重

            List<hr_Job> jobs = all_jobs.Where(j => jobs_id_by_deptid.Contains(j.JobID)).ToList();

            List<TreeNodeExtend> tree = new List<TreeNodeExtend>();
            if (jobs.Count > 0)
            {
                foreach (hr_Job job in jobs)
                {
                    TreeNodeExtend node = new TreeNodeExtend();
                    node.id = job.JobID;
                    node.text = job.Name;
                    node.type = "j";
                    node.iconCls = "icon-job";
                    if (configType < 2)
                    {
                        node.state = state;
                        node.children.AddRange(DepartmentJobEmployees(DeptID, job.JobID, all_emps));
                    }
                    tree.Add(node);
                }
            }
            return tree;
        }
        /// <summary>
        /// 通过部门职位查询员工
        /// </summary>
        private List<TreeNodeExtend> DepartmentJobEmployees(string DeptID, string JobID, List<hr_Employee> all_emps)
        {
            List<hr_Employee> emps_by_deptid = all_emps.Where(e => (e.DeptID == DeptID && e.JobID == JobID)).ToList();//查询该部门职位的所有员工

            List<TreeNodeExtend> tree = new List<TreeNodeExtend>();
            if (emps_by_deptid.Count > 0)
            {
                foreach (hr_Employee employee in emps_by_deptid)
                {
                    TreeNodeExtend node = new TreeNodeExtend();
                    node.id = employee.EmployeeID;
                    node.text = employee.Name;
                    node.type = "e";
                    node.iconCls = "icon-employee";
                    tree.Add(node);
                }
            }
            return tree;
        }

        private void GetListWithAllParent(string DeptID, List<hr_Department> all_depts, List<hr_Department> ListWithAllParent)
        {
            if (DeptID != null)
            {
                hr_Department dept = all_depts.Where(d => d.IsActive && d.DeptID == DeptID).ToList()[0];
                ListWithAllParent.Add(dept);
                GetListWithAllParent(dept.ParentID, all_depts, ListWithAllParent);
            }
        }
    }
}
