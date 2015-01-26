using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Panasia.Core.Auth
{
    public class RoleAuthModel
    {
        public string RoleID { get; set; }
        public string Name { get; set; }
        public List<String> Titles { get; set; }
        public string Description { get; set; }
        public string DataFilter { get; set; }
        public List<AuthPageModel> Pages { get; set; }
    }

    public class AuthPageModel
    {
        public string PageID { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string DataFilter { get; set; }
         
        public List<AuthActionModel> Actions { get; set; }  
    }

    public class AuthActionModel
    {
        public int ActionValue { get; set; }

        public string ActionTitle { get; set; }

        public bool IsSelected { get; set; }
    }
}