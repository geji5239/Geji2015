using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web; 

namespace Panasia.Core.Auth
{
    public class UserCreateModel
    {
        [Required]
        [StringLength(20)]
        [Display(Name="用户名")]
        public string UserName { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name="姓名")]
        public string FullName { get; set; }

        [Required]
        [StringLength(50)]
        [DataType(DataType.EmailAddress, ErrorMessage = "邮件地址不正确")]
        [Display(Name="邮件地址")]
        public string Email { get; set; }
    }
    
    public class UserChangePassword
    {
        [Required]
        public string UserID { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Passwrod { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [Display(Name = "确认密码")]
        public string ConfirmPassword { get; set; }
    }
}