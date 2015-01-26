using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Panasia.Gemini.FL.Data.Models
{

    [Table("hr_Reimbursement")]
    public class Reimbursement : Panasia.Core.Sys.SysEntity
    {
        [Key]
        public string RID { get; set; }
        public string CompanyID { get; set; }
        public string EmployeeID { get; set; }
        public string DeptID { get; set; }
        public DateTime ApplicationDate { get; set; }
        public DateTime ReimbursementDate { get; set; }
        public decimal Amount { get; set; }
        public bool Fund { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }
    }
}