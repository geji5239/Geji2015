
using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using Maticsoft.DBUtility;//Please add references
namespace Panasia.Gemini.FL.Data.Common
{
	/// <summary>
	/// 数据访问类:sys_User
	/// </summary>
	public partial class sys_User
	{
		public sys_User()
		{}
		
		#region  ExtensionMethod
        public DataSet Login(string UserName, string Password)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT   dbo.sys_User.UserID, dbo.sys_User.UserName, dbo.sys_User.Email, dbo.sys_User.Password, dbo.sys_User.IsValid, 
                dbo.sys_User.CreatedUser, dbo.sys_User.CreatedTime, dbo.sys_User.ModifiedUser, dbo.sys_User.ModifiedTime, 
                dbo.sys_User.AutoKey, dbo.hr_Employee.EmployeeID, dbo.hr_Employee.Name AS EmployeeName, 
                dbo.hr_Employee.DeptID, hr_Department_1.Name AS DepartmentName, dbo.hr_Employee.JobID, 
                dbo.hr_Job.Name AS JobName, hr_Department_1.BUID, dbo.hr_BU.Name AS BUName, hr_Department_1.CompanyID, 
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
                dbo.hr_Company.CompanyID = hr_Department_1.CompanyID");
            strSql.Append(" WHERE UserName=@UserName AND Password=@Password AND IsValid='True' ");
            SqlParameter[] parameters = {
					new SqlParameter("@UserName", SqlDbType.NVarChar,30),
                    new SqlParameter("@Password", SqlDbType.NVarChar,128)
			};
            parameters[0].Value = UserName;
            parameters[1].Value = Password;
            return DbHelperSQL.Query(strSql.ToString(), parameters);
        }
		#endregion  ExtensionMethod
	}
}

