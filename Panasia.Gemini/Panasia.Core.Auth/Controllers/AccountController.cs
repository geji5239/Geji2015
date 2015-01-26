using System.Web.Mvc;
using System.Web.Security;
using Panasia.Core.Sys;
using Panasia.Core.Auth.Models;

namespace Panasia.Core.Auth
{
    public class AccountController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            //return View();
            return View(new LoginViewModel());
        }
        public ActionResult ForgetPsw() 
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model, string ReturnUrl = "")
        {
            if (ModelState.IsValid)
            {
                if (!model.WebLogin())
                {
                    ModelState.AddModelError("", Messages.System_InvalidUserNameOrPassword);
                    this.AddSystemLog("登录失败", "Account/Login", Messages.System_InvalidUserNameOrPassword,model.UserName);
                }
                else
                {
                    UserSession session = new UserSession();
                    session.UserModel = SysService.GetUserModel(model.UserName);
                    session.Roles = SysService.GetRoles();
                    Session["userSession"] = session;
                    this.AddSystemLog("登录成功", "Account/Login","UserName:"+model.UserName, session.UserModel.FullName);
                    return null;
                }
            }
            return View(model);
        }

        public ActionResult LogOut()
        {
            //清理cookie
            Request.Cookies.Clear();
            string name = FormsAuthentication.FormsCookieName;
            FormsAuthentication.SignOut();
            this.AddSystemLog("登出成功", "Account/LoginOut");
            return View("Login");
        }
    }
}
