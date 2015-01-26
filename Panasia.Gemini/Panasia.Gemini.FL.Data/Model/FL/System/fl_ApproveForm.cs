using System;
using System.ComponentModel.DataAnnotations;
namespace Panasia.Gemini.FL.Data.Models
{
    public partial class fl_ApproveForm : Panasia.Core.Sys.SysEntity
    {
        [Key]
		public int ApproveFormID { get; set; }
		public int TemplateID { get; set; }
        public int TFDID { get; set; }
        public string CompanyID { get; set; }
		public string EmployeeID { get; set; }
		public int Employee_Sort { get; set; }
		public string DeptID { get; set; }
		public string JobID { get; set; }
		public bool IsFuzzy { get; set; }
		public int? FuzzyType { get; set; }
        private new int AutoKey { get; set; }
	}
}

