using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Panasia.Gemini.Web.Models;
using System.Text;
using Panasia.Core;
using Panasia.Gemini.Web.Models.Common;
using Panasia.Core.Sys;

namespace Panasia.Gemini.Web.Controllers
{
    public class ComboboxController : Controller
    {
        //
        // GET: /Combobox/
        public JsonResult JobData()
        {
            SysContext db = new SysContext();

            List<hr_Job> jobData = db.hr_Jobs.ToList();
            List<Combobox> date = new List<Combobox>();
            foreach (hr_Job job in jobData)
            {
                Combobox com = new Combobox();
                com.id = job.JobID;
                com.text = job.Name;
                date.Add(com);
            }
            return Json(date, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CompanyAndDeptData()
        {
            SysContext db = new SysContext();
            List<hr_Department> Dept = db.hr_Depts.ToList();
            List<hr_Company> Company = db.hr_Companies.Where(x=>x.IsActive==true).ToList();
            
            TreeNode Tree = new TreeNode();

            foreach (var company in Company)
            {
                TreeNode CompanyTree = new TreeNode();
                CompanyTree.id = company.CompanyID;
                CompanyTree.text = company.Name;
                var child = db.hr_Depts.Where(x => x.IsActive == true).Where(x => x.CompanyID == company.CompanyID).ToList();
                if (child.Count > 0)
                {
                    CompanyTree.state = "closed";
                    foreach (var chidNode in child)
                    {
                        CompanyTree.children.Add(CreateChildNode(chidNode));
                    }
                }
                else
                {
                    CompanyTree.state = "open";
                    // 叶节点无子项
                    CompanyTree.children = null;
                }
                Tree.children.Add(CompanyTree);
            }
            return Json(Tree, JsonRequestBehavior.AllowGet);
        }

        public TreeNode CreateChildNode(hr_Department dept)
        {
            SysContext db = new SysContext();
            TreeNode tn = new TreeNode();
            tn.id = dept.DeptID;
            tn.text = dept.Name;
            var childs = db.hr_Depts.Where(x => x.IsActive==true).Where(x => x.ParentID == dept.DeptID);//加载可用的区域
            if (childs.Count() > 0)
            {
                tn.state = "closed";
                foreach (var childNode in childs)
                {
                    tn.children.Add(CreateChildNode(childNode));
                }
            }
            else
            {
                tn.state = "open";
                // 叶节点无子项
                tn.children = null;
            }
            return tn;
                
        }
        public ActionResult GetOutPayment()
        {
            string ParentID = Request.Params["ParentID"];
            SysContext db = new SysContext();
            List<Combobox> list = db.bi_Payment.Where(p => p.IsActive && p.Type == 1 && p.ParentID == ParentID).Select(p => new Combobox { id = p.PaymentID, text = p.PaymentName }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}
