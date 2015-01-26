using System;
using System.ComponentModel.DataAnnotations;
namespace Panasia.Core.Sys
{
	public  class fl_ApproveState : Panasia.Core.Sys.SysEntity
    {
		public int FormID { get; set; }
		public string FormNo { get; set; }
        public int? ApproverID { get; set; }
		public string EmployeeID { get; set; }
		public int Employee_Sort { get; set; }
		public string DeptID { get; set; }
		public string JobID { get; set; }
		public string Confirm_Class { get; set; }
		public string Confirm_Stat { get; set; }
		public DateTime? Confirm_Time { get; set; }
		public string Suggestion { get; set; }
        [Key]
        public new int AutoKey { get; set; }
	}
}

