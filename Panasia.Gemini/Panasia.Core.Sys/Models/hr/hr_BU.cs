using System;
using System.ComponentModel.DataAnnotations;
namespace Panasia.Core.Sys
{
    public class hr_BU : Panasia.Core.Sys.SysEntity
    {
        [Key]
        public string BUID { get; set; }
        public string Name { get; set; }
        public string ManagerID { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int SortID { get; set; }
    }
}

