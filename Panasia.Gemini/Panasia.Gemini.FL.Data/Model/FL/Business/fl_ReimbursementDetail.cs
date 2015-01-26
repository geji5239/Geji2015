using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Panasia.Gemini.FL.Data.Models
{
    public class fl_ReimbursementDetail : Panasia.Core.Sys.SysEntity
    {
        [Key, Column(Order = 0)]
		public string RID { get; set; }
        [Key, Column(Order = 1)]
		public int Version { get; set; }
        [Key, Column(Order = 2)]
		public int ItemNo { get; set; }
		public string CostCategories { get; set; }
		public string SubCostCategories { get; set; }
		public decimal Amount { get; set; }
		public bool IsActive { get; set; }
	}
}

