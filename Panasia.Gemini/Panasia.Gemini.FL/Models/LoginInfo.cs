/*----------------------------------------------------------------
// Copyright (C) 2004 清风软件
// 版权所有。
//
// 文件名：LoginInfo.cs    
// 文件功能描述：登录登出及用户信息Model
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Panasia.Gemini.FL
{
    public static class LoginInfo
    {
        public static string UserID
        {
            get
            {
                return GetCookieValue("UserID");
            }
        }
        public static string UserName
        {
            get
            {
                return GetCookieValue("UserName");
            }
        }
        public static string Email
        {
            get
            {
                return GetCookieValue("Email");
            }
        }
        public static string Password
        {
            get
            {
                return GetCookieValue("Password");
            }
        }
        public static bool IsValid
        {
            get
            {
                bool value = false;
                bool.TryParse(GetCookieValue("IsValid"), out value);
                return value;
            }
        }
        public static int CreatedUser
        {
            get
            {
                int value = 0;
                int.TryParse(GetCookieValue("CreatedUser"), out value);
                return value;
            }
        }
        public static DateTime CreatedTime
        {
            get
            {
                DateTime value = DateTime.Now;
                DateTime.TryParse(GetCookieValue("CreatedTime"), out value);
                return value;
            }
        }
        public static int ModifiedUser
        {
            get
            {
                int value = 1;
                int.TryParse(GetCookieValue("ModifiedUser"), out value);
                return value;
            }
        }
        public static DateTime ModifiedTime
        {
            get
            {
                DateTime value = DateTime.Now;
                DateTime.TryParse(GetCookieValue("ModifiedTime"), out value);
                return value;
            }
        }
        public static int AutoKey
        {
            get
            {
                int value = 0;
                int.TryParse(GetCookieValue("AutoKey"), out value);
                return value;
            }
        }
        public static string EmployeeID
        {
            get
            {
                return GetCookieValue("EmployeeID");
            }
        }
        public static string EmployeeName
        {
            get
            {
                return GetCookieValue("EmployeeName");
            }
        }
        public static string DeptID
        {
            get
            {
                return GetCookieValue("DeptID");
            }
        }
        public static string DepartmentName
        {
            get
            {
                return GetCookieValue("DepartmentName");
            }
        }
        public static string JobID
        {
            get
            {
                return GetCookieValue("JobID");
            }
        }
        public static string JobName
        {
            get
            {
                return GetCookieValue("JobName");
            }
        }
        public static string BUID
        {
            get
            {
                return GetCookieValue("BUID");
            }
        }
        public static string BUName
        {
            get
            {
                return GetCookieValue("BUName");
            }
        }
        public static string CompanyID
        {
            get
            {
                return GetCookieValue("CompanyID");
            }
        }
        public static string CompanyName
        {
            get
            {
                return GetCookieValue("CompanyName");
            }
        }
        public static string ManagerID
        {
            get
            {
                return GetCookieValue("ManagerID");
            }
        }
        public static string ManagerName
        {
            get
            {
                return GetCookieValue("ManagerName");
            }
        }
        public static string ManagerJobID
        {
            get
            {
                return GetCookieValue("ManagerJobID");
            }
        }
        public static string BUManagerID
        {
            get
            {
                return GetCookieValue("BUManagerID");
            }
        }
        public static string BUManagerName
        {
            get
            {
                return GetCookieValue("BUManagerName");
            }
        }
        public static string BUManagerDeptID
        {
            get
            {
                return GetCookieValue("BUManagerDeptID");
            }
        }
        public static string BUManagerJobID
        {
            get
            {
                return GetCookieValue("BUManagerJobID");
            }
        }
        public static string RootDepartmentID
        {
            get
            {
                return new Data.Common.Common().GetDepartmentRootByDepartmentID(GetCookieValue("DeptID")).DeptID;
            }
        }
        public static string RootDepartmentManagerID
        {
            get
            {
                return new Data.Common.Common().GetDepartmentRootByDepartmentID(GetCookieValue("DeptID")).ManagerID;
            }
        }
        public static string RootDepartmentManagerName
        {
            get
            {
                Data.Common.SysContext db = new Data.Common.SysContext();
                Data.Models.hr_Department rootDepartment = new Data.Common.Common().GetDepartmentRootByDepartmentID(GetCookieValue("DeptID"));

                Panasia.Core.Sys.hr_Employee DepartmentManager = db.hr_Employee.SingleOrDefault(e => e.EmployeeID == rootDepartment.ManagerID);
                string name = "";
                if(DepartmentManager!=null)
                {
                    name = DepartmentManager.Name;
                }

                return name;
            }
        }
        public static string RootDepartmentManagerJobID
        {
            get
            {
                Data.Common.SysContext db = new Data.Common.SysContext();
                Data.Models.hr_Department rootDepartment = new Data.Common.Common().GetDepartmentRootByDepartmentID(GetCookieValue("DeptID"));

                Panasia.Core.Sys.hr_Employee DepartmentManager = db.hr_Employee.SingleOrDefault(e => e.EmployeeID == rootDepartment.ManagerID);
                string jobid = "";
                if (DepartmentManager != null)
                {
                    jobid = DepartmentManager.JobID;
                }

                return jobid;
            }
        }

        private static string GetCookieValue(string Key)
        {
            HttpCookie login = HttpContext.Current.Request.Cookies["login"];
            string value = "";
            if (login != null)
            {
                value = HttpUtility.UrlDecode(login.Values[Key]);
            }
            else
            {
                HttpContext.Current.Response.Write("<script>parent.location.href='/User/Logout'</script>");
            }
            return value;
        }
    }
}