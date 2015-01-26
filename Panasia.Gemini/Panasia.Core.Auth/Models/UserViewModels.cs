using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Panasia.Core.Auth
{
    public class UserAuthModel
    {
        public string UserID { get; set; }
        public List<AuthRoleModel> Roles { get; set; }
    }
    public class AuthRoleModel
    {
        public string RoleID { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}