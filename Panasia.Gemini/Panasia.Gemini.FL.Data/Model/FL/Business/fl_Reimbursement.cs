using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Panasia.Gemini.FL.Data.Models
{
    public class fl_Reimbursement : Panasia.Core.Sys.SysEntity
    {
        [Key, Column(Order = 0)]
		public string RID { get; set; }
        [Key, Column(Order = 1)]
        public int Version { get; set; }
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

