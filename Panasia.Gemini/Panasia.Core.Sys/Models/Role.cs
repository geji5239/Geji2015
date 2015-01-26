using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Panasia.Core.Sys
{
    [Table("sys_Role")]
    public class Role:SysEntity
    {
        [Key, Required, MaxLength(6)]
        public string RoleID { get; set; }

        [Required(AllowEmptyStrings = true), MaxLength(50)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = true), MaxLength(200)]
        public string Description { get; set; } 
    }
}
