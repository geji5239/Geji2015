using Panasia.Gemini.FL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Panasia.Gemini.FL.Controllers
{
    public class LoginController : Controller
    {
        [HttpPost, ActionName("Login")]
        public ActionResult Login(LoginModel model)
        {
            System.Data.DataSet ds_UserList = new Data.DAL.sys_User().GetList("UserName='" + model.UserName + "' AND Password='" + model.Password + "'");

            if (ds_UserList != null && ds_UserList.Tables[0].Rows.Count == 1)
            {
                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                    1,
                    model.UserName,
                    DateTime.Now,
                    DateTime.Now.AddMinutes(30),
                    false,
                    "admins"
                    );
                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                System.Web.HttpCookie authCookie = new System.Web.HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);
                Response.Redirect("~/");
            }
            return View("Fl000");

        }

        public ActionResult Fl000()
        {
            return View();
        }
    }
}