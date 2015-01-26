using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Panasia.Core.Sys;
using Panasia.Core;

namespace Panasia.Core.Auth
{
    public abstract class AuthController:Controller
    {
        protected virtual new UserPrincipal User
        {
            get { return HttpContext.User as UserPrincipal; }
        }

        public virtual ActionResult CreateJson(object vm, string contentType = "text/html")
        {
            return new ContentResult { Content = vm.ToJson(), ContentType = contentType };
        }
    }
}
