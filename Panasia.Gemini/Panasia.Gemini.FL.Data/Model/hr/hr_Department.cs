using System;
using System.ComponentModel.DataAnnotations;
namespace Panasia.Gemini.FL.Data.Models
{
    public class hr_Department : Panasia.Core.Sys.SysEntity
	{
        [Key]
        public string DeptID { get; set; }
        public string Name { get; set; }
        public string ManagerID { get; set; }
        public string ParentID { get; set; }
        public string CompanyID { get; set; }
        public string BUID { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int? SortID { get; set; }
	}
}

