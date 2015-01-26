using Panasia.Core.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Panasia.Core.Auth.Models
{
    public class UserSession
    {
        public UserModel UserModel { get; set; }

        public List<RoleModel> Roles { get; set; }
    }
}