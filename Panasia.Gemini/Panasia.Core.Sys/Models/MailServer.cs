using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panasia.Core.Sys
{
    [Table("sys_MailServer")]
    public class MailServer : SysEntity
    {
        [Key, Required, MaxLength(10)]
        public string ServerID { get; set; }

        [Required, MaxLength(50)]
        public string ServerName { get; set; }

        [Required(AllowEmptyStrings = true), MaxLength(50)]
        public string DomainName { get; set; }

        [Required, MaxLength(100)]
        public string ServerAddress { get; set; }

        [Required]
        public int ServerPort { get; set; }

        [Required]
        public bool EnableSSL { get; set; }

        [Required(AllowEmptyStrings = true), MaxLength(50)]
        public string DefaultUser { get; set; }

        [Required(AllowEmptyStrings = true), MaxLength(100)]
        public string DefaultPassword { get; set; }

        [Required(AllowEmptyStrings = true), MaxLength(200)]
        public string Description { get; set; }
        
    }
}
