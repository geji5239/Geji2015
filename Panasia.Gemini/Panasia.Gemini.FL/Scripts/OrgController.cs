using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Panasia.Gemini.FL.Models;
using Panasia.Gemini.FL.Data.Common;
using Panasia.Gemini.FL.Data.Models;

namespace Panasia.Gemini.FL.Controllers
{
    public class OrgController : Controller
    {
        public ActionResult DepartmentTree()
        {
            string CompanyID = Request.QueryString["cid"];
            SysContext db = new SysContext();
            List<hr_Department> dept = db.hr_Department.Where(d => d.IsActive && d.CompanyID == CompanyID).OrderBy(d => d.SortID).ToList();
            List<TreeNode> tree = DepartmentChildren("", dept);
            //List<TreeNode> tree = new List<TreeNode>();

            //foreach (hr_Department deptnode in dept)
            //{
            //    TreeNode node = new TreeNode();
            //    node.id = deptnode.DeptID;
            //    node.text = deptnode.Name;
            //    node.parent = deptnode.ParentID;
            //    tree.Add(node);
            //}
            return Json(tree, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 递归取得部门结构JSON
        /// </summary>
        private List<TreeNode> DepartmentChildren(string ParentID, List<hr_Department> all_depts)
        {
            List<hr_Department> depts_by_parentid = all_depts.Where(d => d.ParentID == ParentID).OrderBy(d => d.SortID).ToList();
            List<TreeNode> tree = new List<TreeNode>();
            if (depts_by_parentid.Count > 0)
            {
                TreeNode node1 = new TreeNode();
                node1.id = "111111";
                node1.text = "发起部门";

                tree.Add(node1);
                foreach (hr_Department dept in depts_by_parentid)
                {
                    TreeNode node = new TreeNode();
                    node.id = dept.DeptID;
                    node.text = dept.Name;
                    //node.iconCls = "icon-dept";
                    List<TreeNode> dept_children = DepartmentChildren(dept.DeptID, all_depts);
                    if (dept_children.Count > 0)
                    {
                        node.state = "open";
                        node.children.AddRange(dept_children);
                    }
                    tree.Add(node);
                
                }
                TreeNode node2 = new TreeNode();
                node2.id = "222222";
                node2.text = "自定义部门";

                tree.Add(node2 );
                TreeNode node3 = new TreeNode();
                node3.id = "444444";
                node3.text = "事业部";

                tree.Add(node3);
            }
            return tree;
        }

        public ActionResult GetJobByDepartment()
        {
            string deptID = Request.QueryString["deptID"];
            if (deptID != "" && deptID !=null)
            {
                SysContext db = new SysContext();
                var jobs = db.hr_Job.Join(db.hr_Employee, j => j.JobID, e => e.JobID,
                    (j, e) =>
                        new
                        {
                            j.JobID,
                            j.Name,
                            j.IsActive,
                            j.SortID,
                            e.DeptID
                        }).Where(j => j.IsActive && j.DeptID == deptID).OrderBy(j => j.SortID)
                            .Select(j => new { id = j.JobID, text = j.Name }).Distinct();

                JsonResult json = Json(jobs, JsonRequestBehavior.AllowGet);
                return json;
            }
            else
            {
                SysContext db = new SysContext();
                var jobs = db.hr_Job.Where(j => j.IsActive).OrderBy(j => j.SortID)
                            .Select(j => new { id = j.JobID, text = j.Name }).Distinct();

                JsonResult json = Json(jobs, JsonRequestBehavior.AllowGet);
                return json;
            }
            //return Json("");
        }

        public ActionResult GetEmployeeByDepartmentAndJob()
        {
            string deptID = Request.QueryString["deptID"];
            string jobID = Request.QueryString["jobID"];
            if (deptID != "" && jobID != "")
            {
                SysContext db = new SysContext();
                var employees = db.hr_Employee.Where(e => e.IsActive && e.DeptID == deptID && e.JobID == jobID && e.UserID.Trim() != "" && e.UserID != null).OrderBy(e => e.Name)
                            .Select(e => new { id = e.EmployeeID, text = e.Name });

                JsonResult json = Json(employees, JsonRequestBehavior.AllowGet);
                return json;
            }
            return Json("");
        }
    }
}
