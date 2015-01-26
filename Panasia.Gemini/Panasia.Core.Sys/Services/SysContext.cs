using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Panasia.Core.Sys
{
    public class SysContext:DbContext
    {
        public SysContext()
            : base("PAConnectionName".GetAppSettingValue("PASys"))
        {
        }
        #region sys
        //用户表
        public DbSet<User> Users { get; set; }

        //角色表
        public DbSet<Role> Roles { get; set; } 

        //用户角色表
        public DbSet<UserRole> UserRoles { get; set; }

        //角色页面表
        public DbSet<RolePage> RolePages { get; set; }

        //系列号表
        public DbSet<SerialCode> SerialCodes { get; set; }

        public DbSet<UserLog> UserLogs { get; set; }

        public DbSet<MailServer> MailServers { get; set; }
        #endregion

        #region Org
        public DbSet<hr_BU> hr_BUs { get; set; }
        public DbSet<hr_Company> hr_Companies { get; set; }
        public DbSet<hr_Department> hr_Depts { get; set; }
        public DbSet<hr_Job> hr_Jobs { get; set; }
        public DbSet<hr_Employee> hr_Employees { get; set; }
        #endregion

        #region FL

        #region 系统
        public DbSet<fl_ApproveLog> fl_ApproveLog { get; set; }
        public DbSet<fl_MappingForm> fl_MappingForm { get; set; }
        public DbSet<fl_Form> fl_Form { get; set; }
        //public DbSet<fl_TemplateForm> fl_TemplateForm { get; set; }
        public DbSet<fl_FormTemplate> fl_FormTemplate { get; set; }
        //public DbSet<fl_TemplateFormDetail> fl_TemplateFormDetail { get; set; }
        public DbSet<fl_ApproveRules> fl_ApproveRules { get; set; }
        public DbSet<fl_Approver> fl_Approver { get; set; }
        //public DbSet<fl_ApproveForm> fl_ApproveForm { get; set; }
        public DbSet<fl_ApproveState> fl_ApproveState { get; set; }
        //public DbSet<fl_ApproveFlow> fl_ApproveFlow { get; set; }
        public DbSet<fl_DimissionForm> fl_DimissionForm { get; set; }
        public DbSet<fl_EmployeeMove> fl_EmployeeMove { get; set; }
        #endregion

        #region 业务
        public DbSet<fl_Reimbursement> fl_Reimbursement { get; set; }
        public DbSet<fl_ReimbursementDetail> fl_ReimbursementDetail { get; set; }
        public DbSet<fl_ConversionForm> fl_ConversionForm { get; set; }
        public DbSet<fl_AbnormalAttendance> fl_AbnormalAttendance { get; set; }
        public DbSet<fl_Addition> fl_Addition { get; set; }
        public DbSet<fl_OvertimeApplicationForm> fl_OvertimeApplicationForm { get; set; }
        public DbSet<fl_LeaveApplicationForm> fl_LeaveApplicationForm { get; set; }
        public DbSet<fl_GoOutForm> fl_GoOutForm { get; set; }
        public DbSet<fl_PaidLeaveForm> fl_PaidLeaveForm { get; set; }
        #endregion

        #endregion

        #region 人事业务（各种单据）
        public DbSet<Reimbursement> hr_Reimbursement { get; set; }
        public DbSet<hr_ReimbursementDetail> hr_ReimbursementDetail { get; set; }
        public DbSet<ConversionForm> hr_ConversionForm { get; set; }
        public DbSet<EmployeeMove> hr_EmployeeMove { get; set; }
        #endregion

        //其他未分类↓
        public DbSet<bi_Bank> bi_Bank { get; set; }
        public DbSet<bi_BankRole> bi_BankRole { get; set; }
        public DbSet<bi_Payment> bi_Payment { get; set; }
        public DbSet<Addition> rc_Addition { get; set; }
        public DbSet<Hr_Rule> Rules { get; set; }
        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<bi_Order> bi_Order { get; set; }
        public DbSet<bi_OrderProduct> bi_OrderProduct { get; set; }
        #region 考勤结果记录
        public DbSet<CheckRecord> CheckRecords { get; set; }
        #endregion
        #region 日出勤结果
        public DbSet<Attendance> Attendances { get; set; }
        #endregion
        #region 月考勤结果
        public DbSet<CheckResult> AttendanceResults { get; set; }
        #endregion
        #region 月考勤相关表单
        public DbSet<PaidLeaveForm> PaidLeaveForms { get; set; }
        public DbSet<GoOutForm> GoOutForms { get; set; }
        public DbSet<AbnormalAttendance> AbnormalAttendances { get; set; }
        public DbSet<LeaveApplicationForm> LeaveApplicationForms { get; set; }
        public DbSet<OvertimeApplicationForm> OvertimeApplicationForms { get; set; }
        public DbSet<DimissionForm> DimissionForms { get; set; }
        #endregion
      

        public static SysContext GetCurrent()
        {
            return new SysContext();
        }
    }
}
