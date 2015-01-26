using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Panasia.Gemini.Web.Models;
using Panasia.Core.Sys;

namespace Panasia.Gemini.Web.Controllers
{
    public class BankController : Controller
    {
        //
        // GET: /Bank_Role_Set/
        Panasia.Gemini.Web.Models.SysContext db = new Panasia.Gemini.Web.Models.SysContext();
        Panasia.Core.Sys.SysContext dbSys = new Panasia.Core.Sys.SysContext();

        public ActionResult Bank_Role_Set()
        {
            List<hr_Company> companies = db.hr_Companies.Where(c => c.IsActive).OrderBy(c => c.SortID).ToList();
            string defaultCompanyID = companies[0].CompanyID;
            ViewData["companies"] = new SelectList(companies, "CompanyID", "Name", defaultCompanyID);

            return View();
        }

        public ActionResult test()
        {
            return View();
        }

        public string SetBankRole()
        {
            string returnValue = "";
            try
            {
                string EmployeeID = Request.Form["EmployeeID"];
                string BankIDString = Request.Form["BankIDString"];
                Panasia.Core.Sys.SystemParameterFunctions sysParameter = new Core.Sys.SystemParameterFunctions();
                string userID = sysParameter.GetCurrentUserID("").ToString();
                DateTime now = DateTime.Now;
                bi_Bank_Role entity = new bi_Bank_Role();
                System.Data.Entity.DbContext dbSys = new System.Data.Entity.DbContext("PASys");
                var result = dbSys.Database.SqlQuery(null, "EXEC sp_CreateSerialCode 'Bank_Role', 'BR' , 6, @Bank_Role_ID", new System.Data.SqlClient.SqlParameter("@Bank_Role_ID", null));
                entity.Bank_Role_ID = "";
                entity.EmployeesID = EmployeeID;
                entity.Bank_ID_List = BankIDString;
                entity.CreatedUser = userID;
                entity.CreatedTime = now;
                entity.ModifiedUser = userID;
                entity.ModifiedTime = now;
                entity.IsActive = true;
                db.bi_Bank_Role.Add(entity);
                
                if (db.SaveChanges() == 1)
                {
                    returnValue = "设置成功";
                }
                else
                {
                    returnValue = "设置失败，请重试";
                }
            }
            catch
            {
                returnValue = "系统出现错误，请联系管理员";
            }
            return returnValue;
        }

        #region 页面操作
        public string GetDepartmentListByCompanyID()
        {
            System.Text.StringBuilder optionItems = new System.Text.StringBuilder();
            string id = Request.Form["id"];
            List<hr_Department> depts = db.hr_Depts.Where(d => d.IsActive && d.CompanyID == id).OrderBy(d => d.SortID).ToList();
            foreach (hr_Department dept in depts)
            {
                optionItems.AppendFormat("<option value='{0}'>{1}</option>", dept.DeptID, dept.Name);
            }
            return optionItems.ToString();
        }

        public string GetEmployeeListByDeptID()
        {
            System.Text.StringBuilder optionItems = new System.Text.StringBuilder();
            string id = Request.Form["id"];
            List<hr_Employee> emps = dbSys.hr_Employees.Where(e => e.IsActive && e.DeptID == id).ToList();
            foreach (hr_Employee emp in emps)
            {
                optionItems.AppendFormat("<option value='{0}'>{1}</option>", emp.EmployeeID, emp.Name);
            }
            return optionItems.ToString();
        }

        public string GetBankListByCompanyID()
        {
            System.Text.StringBuilder optionItems = new System.Text.StringBuilder(
                "<div><input type=\"checkbox\" id=\"selAll\" class=\"align_middle\" /><label for=\"selAll\" class=\"align_middle\">全选</label></div>");
            string id = Request.Form["id"];
            List<bi_Bank> banks = db.bi_Bank.Where(b => b.IsActive && b.CompanyID == id).ToList();
            foreach (bi_Bank bank in banks)
            {
                optionItems.AppendFormat("<div><input type=\"checkbox\" id=\"bank_{0}\" name=\"bank\" value=\"{0}\" class=\"align_middle\" /><label for=\"bank_{0}\" class=\"align_middle\">{1}</label></div>", bank.BankID, bank.Name + " " + bank.Account);
            }
            return optionItems.ToString();
        }
        #endregion

    }
}
