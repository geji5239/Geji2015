using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Panasia.Core.App;
using Panasia.Core;
using Newtonsoft.Json.Linq;
using Panasia.Core.Web;
using Panasia.Core.Sys;
using System.Text;
using System.Threading;

namespace Panasia.Core.Auth
{
    public class PageController : AuthController
    {
        [ValidateInput(false)]
        public ActionResult Index(string pageID, string actionName)
        {
            var page = Panasia.Core.App.AppConfig.Current.GetPage(pageID);
            if (page == null || page.Config == null)
            {
                var error = Messages.System_PageNotFound;
                this.Log(string.Format("Receive Request\tPage:{0},Action:{1}\tError:{2}", pageID, actionName, error));
                throw new Exception(error);
            }
            if(!page.IsNetAllow())
            {                
                throw new Exception(string.Format(
                    LangTexts.Current.GetLangText("1003","对不起，因网络受限，您不能使用功能[{0}]，谢谢理解！"),
                    page.Title));
            }
#if DEBUG
            var reloadPage = Request["reloadPage"];
            if (!string.IsNullOrEmpty(reloadPage))
            {
                page.ReloadConfig();
            }

            var reloadSql = Request["reloadSql"];
            if (!string.IsNullOrEmpty(reloadSql))
            {
                SqlData.ReloadCurrent();
            }
#endif
            dynamic mPage = page.Config;
            try
            {
                if (mPage.IsRedirect)
                {
                    return RedirectToAction(string.IsNullOrEmpty(actionName) ? mPage.DefaultAction : actionName, mPage.Controller);
                }
            }
            catch
            { 
                //如果出错，那么就不是MvcPage
            } 

            var action = page.Config.Actions[actionName];
            if (action == null)
            {
                var error = string.Format(Messages.System_ActionNotFound, actionName);
                this.Log(string.Format("Receive Request\tPage:{0},Action:{1}\tError:{2}", pageID, actionName, error));
                throw new Exception(error);
            }

            var pageActionValue = SysService.GetCurrentUserPageActionValue(page.PageID);
            if (pageActionValue == 0)
            {
                return RedirectToAction("TimeOut", "Home");
            }
            if (pageActionValue == 0 || ((pageActionValue & action.ActionValue) != action.ActionValue))
            {
                var error = Messages.System_ActionNotAllowed; 
                this.Log(string.Format("Receive Request\tPage:{0},Action:{1}\tError:{2}", pageID, actionName, error));
                throw new Exception(error);
            }

            var context = new WebContext
            {
                Controller = this,
                Request = Request.Params,
                User = User,
                Page = page,
                Files = Request.Files,
                ActionValue = SysService.GetCurrentUserPageActionValue(page.PageID)
            };

            string actionTitle = string.Format("{0}-{1}", page.Title, action.Title);
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("{");
                foreach (var key in Request.Form.AllKeys)
                {
                    sb.AppendFormat("{0}:{1};", key, Request.Form[key]);
                }
                foreach (var key in Request.QueryString.AllKeys)
                {
                    sb.AppendFormat("{0}:{1};", key, Request.QueryString[key]);
                }
                sb.Append("}");

                this.Log(string.Format("Receive Request\tPage:{0},Action:{1},Parameters:{2}", pageID, actionName, sb.ToString()));

                UserLogService.AddLog(actionTitle, Request.RawUrl, sb.ToString());
                var result = context.Execute(action);

                string eventName = string.Format("{0}/{1}", page.PageID, action.Name);
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    AppConfig.Current.EventActions.HandleEvents(eventName, context);
                });

                this.Log(string.Format("End Request\tPage:{0},Action:{1}", pageID, actionName));
                return result;
            }
            catch (Exception ex)
            {
                UserLogService.AddLog(actionTitle, Request.RawUrl, ex.Message);
                this.Log(string.Format("End Request\tPage:{0},Action:{1} \tError:{2}", pageID, actionName, ex.Message));
                throw;
            }
        }
    }
}
