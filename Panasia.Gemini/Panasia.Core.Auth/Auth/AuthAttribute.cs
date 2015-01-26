using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Panasia.Core.Sys;

namespace Panasia.Core.Auth
{
    public class AuthAttribute : AuthorizeAttribute
    {
    }

    public class AuthActionAttribute : AuthAttribute
    {
        /// <summary>
        /// 页面编号：来源于页面定义
        /// </summary>
        public string PageID { get; set; }

        /// <summary>
        /// 方法编号：来源于页面定义
        /// </summary>
        public int Action { get; set; }

        public AuthActionAttribute(string pageID, int action)
        {
            PageID = pageID;
            Action = action;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!string.IsNullOrEmpty(PageID))
            {
                var page = Panasia.Core.App.AppConfig.Current.GetPage(PageID);
                if (page != null)
                {
                    if(!page.IsNetAllow())
                    {
                        throw new Exception(string.Format(
                            LangTexts.Current.GetLangText("1003", "对不起，因网络受限，您不能使用功能[{0}]，谢谢理解！"),
                            page.Title));
                    }
                }
            }
            var user = filterContext.HttpContext.User as UserPrincipal;
            if (user == null || (!SysService.IsActionAllowed(user.UserID, PageID, Action)))
            {
                //TODO:这里应该跳转(弹出对话框）到一个无权访问页面
                //filterContext.RequestContext.HttpContext.Response.Write("无权访问");
                //filterContext.RequestContext.HttpContext.Response.End();
                filterContext.HttpContext.Response.Redirect("/Home/TimeOut");
                return;
            }
            base.OnAuthorization(filterContext);
        }
    }
}
