using System;
using System.ComponentModel.DataAnnotations;
namespace Panasia.Gemini.FL.Data.Models
{
    public partial class fl_Approver : Panasia.Core.Sys.SysEntity
    {
        [Key]
        public int ApproverID { get; set; }
        public int FormID { get; set; }
        public int TemplateID { get; set; }
        public string CompanyID { get; set; }
		public string EmployeeID { get; set; }
		public int Employee_Sort { get; set; }
		public string DeptID { get; set; }
		public string JobID { get; set; }
        public string DeptIDCondition { get; set; }
        public string EmployeeIDCondition { get; set; }
        public bool IsGroupApprover { get; set; }
        public bool IsFuzzy { get; set; }
        public int? FuzzyType { get; set; }
        private new int AutoKey { get; set; }
	}
}

