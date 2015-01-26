using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Panasia.Core.Sys
{
    [Table("sys_UserRole")]
    public class UserRole:SysEntity
    {
        [Key, Required, MaxLength(6),Column(Order=0)]
        public string UserID { get; set; }

        [Key, Required, MaxLength(6),Column(Order=1)]
        public string RoleID { get; set; } 
    }
}
