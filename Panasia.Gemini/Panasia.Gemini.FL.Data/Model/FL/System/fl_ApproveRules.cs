using System;
using System.ComponentModel.DataAnnotations;
namespace Panasia.Gemini.FL.Data.Models
{
    public partial class fl_ApproveRules : Panasia.Core.Sys.SysEntity
    {
        [Key]
		public int ApproveRulesid { get; set; }
        public int FormID { get; set; }
		public int ApproverID { get; set; }
		public string Table_Name { get; set; }
		public string Col_Name { get; set; }
		public string Condition { get; set; }
        private new int AutoKey { get; set; }
	}
}

