using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Panasia.Gemini.FL.Data.Models
{
        [Table("hr_AbnormalAttendance")]
    public class AbnormalAttendance : Panasia.Core.Sys.SysEntity
    {
        [Key]
        public string AAID { get; set; }
        public string EmployeeID { get; set; }
        public string CompanyID { get; set; }
        public string DeptID { get; set; }
        public string JobID { get; set; }
        public DateTime? ApplicationDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal ApplicationHours { get; set; }
        public decimal ThrowableType { get; set; }
        public decimal Reason { get; set; }
        public bool IsActive { get; set; }
    }
}

