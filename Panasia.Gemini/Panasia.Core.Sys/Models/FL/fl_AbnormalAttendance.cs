using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Panasia.Core.Sys
{
    public class fl_AbnormalAttendance : Panasia.Core.Sys.SysEntity
    {
        [Key, Column(Order = 0)]
        public string AAID { get; set; }
        [Key, Column(Order = 1)]
        public int Version { get; set; }
        public string EmployeeID { get; set; }
        public string CompanyID { get; set; }
        public string DeptID { get; set; }
        public string JobID { get; set; }
        public DateTime? ApplicationDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal ApplicationHours { get; set; }
        public string ThrowableType { get; set; }
        public string Reason { get; set; }
        public string Suggestion1 { get; set; }
        public string EmployeeID1 { get; set; }
        public DateTime? ApprovedDate1 { get; set; }
        public string Suggestion2 { get; set; }
        public string EmployeeID2 { get; set; }
        public DateTime? ApprovedDate2 { get; set; }
        public string Suggestion3 { get; set; }
        public string EmployeeID3 { get; set; }
        public DateTime? ApprovedDate3 { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }
    }
}

