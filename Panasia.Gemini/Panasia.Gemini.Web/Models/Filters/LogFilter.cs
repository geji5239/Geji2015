using Panasia.Core.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Panasia.Gemini.Web.Models.Filters
{
    public class LogFilter : FilterAttribute, IActionFilter
    {
        public LogFilter()
        {

        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
           // filterContext.WriteLog();
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if(HttpContext.Current.Request.IsLocal && HttpContext.Current.Request.Url.AbsoluteUri
                .Contains("ReloadConfig"))
            {
                return;
            }
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
            var isonline = HttpContext.Current.User != null && HttpContext.Current.User is UserPrincipal;
            if(!isonline)
            {
                filterContext.HttpContext.Response.Redirect("/Home/TimeOut");
            }
        }
    }
}