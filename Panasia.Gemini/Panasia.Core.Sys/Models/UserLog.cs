using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Panasia.Core.Sys
{
    [Table("sys_UserLog")]
    public class UserLog : SysEntity
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new int AutoKey { get; set; }

        [Required(AllowEmptyStrings = true), MaxLength(20)]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = true), MaxLength(20)]
        public string ActionName { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Description { get; set; }

        [Required(AllowEmptyStrings = true), MaxLength(2000)]
        public string Url { get; set; }

        [Required(AllowEmptyStrings = true), MaxLength(30)]
        public string ClientIP { get; set; } 

    }
}
