using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Panasia.Gemini.Web.Models.Filters
{
    public class ExceptionFilter:FilterAttribute,IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
           // filterContext.Exception.AddLog("Application Exception", 0, "Url", filterContext.HttpContext.Request.Url.AbsoluteUri);
        }
    }
}