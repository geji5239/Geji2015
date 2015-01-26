using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Panasia.Gemini.FL.Data.Models;

namespace Panasia.Gemini.FL.Data.Common
{
    public class SysContext : DbContext
    {
        public SysContext()
            : base("conn")
        {
        }

        #region Fl
        #region 系统
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
        
        #endregion

        #region OA业务
        public DbSet<fl_Reimbursement> fl_Reimbursement { get; set; }
        public DbSet<fl_ReimbursementDetail> fl_ReimbursementDetail { get; set; }
        public DbSet<fl_ConversionForm> fl_ConversionForm { get; set; }
        public DbSet<fl_AbnormalAttendance> fl_AbnormalAttendance { get; set; }
        public DbSet<fl_Addition> fl_Addition { get; set; }
        public DbSet<fl_DimissionForm> fl_DimissionForm { get; set; }
        public DbSet<fl_EmployeeMove> fl_EmployeeMove { get; set; }
        public DbSet<fl_OvertimeApplicationForm> fl_OvertimeApplicationForm { get; set; }
        public DbSet<fl_LeaveApplicationForm> fl_LeaveApplicationForm { get; set; }
        public DbSet<fl_GoOutForm> fl_GoOutForm { get; set; }
        public DbSet<fl_PaidLeaveForm> fl_PaidLeaveForm { get; set; }
        public DbSet<fl_ApproveLog> fl_ApproveLog { get; set; }
        #endregion
        #endregion

        #region hr
        public DbSet<hr_BU> hr_BU { get; set; }
        public DbSet<hr_Company> hr_Company { get; set; }
        public DbSet<hr_Department> hr_Department { get; set; }
        public DbSet<Panasia.Core.Sys.hr_Employee> hr_Employee { get; set; }
        public DbSet<hr_Job> hr_Job { get; set; }
        #endregion        

        #region Business（含人事和业务）
        public DbSet<Reimbursement> hr_Reimbursement { get; set; }
        public DbSet<ConversionForm> hr_ConversionForm { get; set; }
        public DbSet<AbnormalAttendance> hr_AbnormalAttendance { get; set; }
        #endregion

        
    }

}