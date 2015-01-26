using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using Panasia.Core;
using Panasia.Core.Sys;


namespace Panasia.Core.Auth
{
    public class ApplicationBase : HttpApplication
    {
        protected virtual void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            var request = Request;
            if (request.IsLocal && request.Headers.AllKeys.Contains("ConfigTools"))
            {
                UserPrincipal newUser = new UserPrincipal("admin");
                newUser.UserID ="U00001";
                newUser.UserName = "admin";
                newUser.Email = "";
                newUser.FullName = "";
                HttpContext.Current.User = newUser;
                return;
            }

            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                UserModel userModel = authTicket.UserData.ToJsonObject<UserModel>();
                UserPrincipal newUser = new UserPrincipal(authTicket.Name);
                newUser.UserID = userModel.UserID;
                newUser.UserName = userModel.UserName;
                newUser.Email = userModel.Email;
                newUser.FullName = userModel.FullName;
                newUser.Roles = userModel.Roles;

                HttpContext.Current.User = newUser;
            }
        }
    }
}