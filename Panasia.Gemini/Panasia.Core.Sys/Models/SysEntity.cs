using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Panasia.Core.Sys
{
    public class SysEntity
    {
        public SysEntity()
        {
            CreatedUser = "";
            CreatedTime = DateTime.Now;
            ModifiedUser = "";
            ModifiedTime = DateTime.Now;
            AutoKey = 0;
        }

        [Required(AllowEmptyStrings = true)] 
        public string CreatedUser { get; set; }

        [Required] 
        public DateTime CreatedTime { get; set; }

        [Required(AllowEmptyStrings = true)] 
        public string ModifiedUser { get; set; }

        [Required] 
        public DateTime ModifiedTime { get; set; }

        [Required]       
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AutoKey { get; set; }

        public virtual void ResetCreated()
        {
            var user = SysService.GetCurrentUser();
            if(user!=null)
            {
                CreatedUser = user.UserID;
                CreatedTime = DateTime.Now;
                ModifiedUser = user.UserID;
                ModifiedTime = DateTime.Now;
            }
        }

        public virtual void ResetUpdated()
        {
            var user = SysService.GetCurrentUser();
            if(user!=null)
            {
                ModifiedUser = user.UserID;
                ModifiedTime = DateTime.Now;
            }
        }
    }
}
