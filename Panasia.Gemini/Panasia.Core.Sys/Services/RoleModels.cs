using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panasia.Core.Sys
{
    public class RoleModel
    {
        public string RoleID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }

    public class RolePageModel
    {
        public string RoleID { get; set; }

        public string PageID { get; set; }

        public int ActionValue { get; set; }

        public string DataFilter { get; set; }
    }

    public class UserRoleModel
    {
        public string UserID { get; set; }

        public string[] Roles { get; set; }
    }
    
}
