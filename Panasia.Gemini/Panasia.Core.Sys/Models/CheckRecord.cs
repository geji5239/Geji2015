using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panasia.Core.Sys
{
    [Table("hr_CheckRecord")]
    public class CheckRecord : SysEntity
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new int AutoKey { get; set; }
        [Required, MaxLength(6)]
        public string DeptID { get; set; }
        [Required, MaxLength(20)]
        public string CardNo { get; set; }
        public DateTime CardTime { get; set; }
        [MaxLength(10)]
        public string Source { get; set; }
        [MaxLength(200)]
        public string Remark { get; set; }
        public bool IsActive { get; set; }
    }
}
