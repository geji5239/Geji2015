using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Panasia.Core.Sys
{
    public class hr_ReimbursementDetail : Panasia.Core.Sys.SysEntity
	{
        [Key, Column(Order = 0)]
		public string RID{ get; set; }
        [Key, Column(Order = 1)]
		public int ItemNo{ get; set; }
		public string CostCategories{ get; set; }
		public string SubCostCategories{ get; set; }
		public decimal Amount{ get; set; }
		public bool IsActive{ get; set; }
	}
}

