using System;
using System.ComponentModel.DataAnnotations;
namespace Panasia.Core.Sys
{
    public class hr_Company : Panasia.Core.Sys.SysEntity
	{
        [Key]
        public string CompanyID { get; set; }
        public string Name { get; set; }
        public string JC { get; set; }
        public string JM { get; set; }
        public string BUID { get; set; }
        public string ManagerID { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int? SortID { get; set; }
        public string Url1 { get; set; }
        public string Url2 { get; set; }
        public string Tel1 { get; set; }
        public string Tel2 { get; set; }
        public string Address { get; set; }
        public string Remark { get; set; }
	}
}

