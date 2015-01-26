using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Panasia.Gemini.Web.Models.Filters
{
    public class SysAuthorizeAttribute : AuthorizeAttribute
    {
        public SysAuthorizeAttribute() : base() { }
        public SysAuthorizeAttribute(UserValidType validType)
            : this()
        {
            ValidType = validType;
        }
        private UserValidType _ValidType = UserValidType.Default;
        public UserValidType ValidType
        {
            get { return _ValidType; }
            set { _ValidType = value; }
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //UserInfo currentUser = filterContext.HttpContext.Session.GetUserInfo();
            //if (!currentUser.IsLogIn)
            //{
            //    // 导航到登录页面
            //    filterContext.Result = new HttpUnauthorizedResult();
            //}
            //else
            //{
            //    if (ValidType == UserValidType.Default)
            //    {
            //        string controllername = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            //        string actionname = filterContext.ActionDescriptor.ActionName;

            //        if (currentUser.Functions.IsExist(controllername, actionname))
            //        {
            //            // 通过验证
            //        }
            //        else
            //        {
            //            // 导航到无此权限页面
            //            filterContext.Result = new RedirectResult(ViewExtends.GetUrl("NotPermission", "Error"));
            //        }
            //    }
            //}
        }

    }
    public enum UserValidType
    {
        Default,
        LoginValid
    }
}