
using Panasia.Gemini.Web.Models.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;

namespace Panasia.Gemini.Web.Controllers
{
    [LogFilter]
    [Panasia.Gemini.Web.Models.Filters.ExceptionFilter]
    [SysAuthorize]
    public class BaseController : Controller
    {
        public virtual ActionResult List()
        {
            return null;
        }
        public virtual ActionResult AddCancel()
        {
            return null;
        }
        public virtual ActionResult AddSave()
        {
            return null;
        }
        public virtual ActionResult Delete(int autoKey,string rowID)
        {
            return null;
        }
        public virtual ActionResult Edit(int autoKey)
        {
            return null;
        }
        public virtual ActionResult EditCanel(int autoKey)
        {
            return null;
        }
        public virtual ActionResult EditSave()
        {
            return null;
        }
        public virtual ActionResult Search()
        {
            return null;
        }
        public virtual ActionResult Find(string searchKey)
        {
            return null;
        }
        public virtual ActionResult ViewDetail(int autoKey, string errorMsg)
        {
            return null;
        }

        public virtual ActionResult First()
        {
            return null;
        }

        public virtual ActionResult Last()
        {
            return null;
        }

        public virtual ActionResult ReturnList()
        {
            return null;
        }
    }
}
