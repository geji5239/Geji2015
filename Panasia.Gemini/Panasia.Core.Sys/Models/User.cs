using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Panasia.Core.Sys
{
    [Table("sys_User")]
    public class User:SysEntity
    {
        [Key, Required, MaxLength(6)]
        public string UserID { get; set; }

        [Required(AllowEmptyStrings = true), MaxLength(30)]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = true), MaxLength(30)]
        public string FullName { get; set; }
        
        [Required(AllowEmptyStrings = true), MaxLength(50)]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = true), MaxLength(128)]
        public string Password { get; set; }

        [Required]
        public bool IsValid { get; set; }
    }
}
