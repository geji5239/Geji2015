using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Panasia.Core.App;
using Panasia.Core;
using Panasia.Core.Sys;
using System.IO;

namespace Panasia.Core.Web
{
    public interface IWebContext
    {
        int ActionValue { get; set; }

        HttpFileCollectionBase Files { get; set; }

        Controller Controller { get; set; }

        ActionResult Execute(Panasia.Core.App.Action action);
    }

    public class WebContext : CommandContextBase, IWebContext
    {
        #region 属性 ActionValue
        public int ActionValue
        {
            get
            {
                return GetFieldValue<int>("ActionValue", 0);
            }
            set
            {
                SetFieldValue("ActionValue", value);
            }
        }
        #endregion

        #region 属性 Files
        public HttpFileCollectionBase Files
        {
            get
            {
                return GetFieldValue<HttpFileCollectionBase>("Files", null);
            }
            set
            {
                SetFieldValue("Files", value);
            }
        }
        #endregion

        #region 属性 Controller
        public Controller Controller
        {
            get
            {
                return GetFieldValue<Controller>("Controller", null);
            }
            set
            {
                SetFieldValue("Controller", value);
            }
        }
        #endregion

        #region 属性 User
        public UserPrincipal User
        {
            get
            {
                return GetFieldValue<UserPrincipal>("User", null);
            }
            set
            {
                SetFieldValue("User", value);
            }
        }
        #endregion

        #region 属性 Page
        public Page Page
        {
            get
            {
                return GetFieldValue<Page>("Page", null);
            }
            set
            {
                SetFieldValue("Page", value);
            }
        }
        #endregion

        public ActionResult Execute(Panasia.Core.App.Action action)
        {           
            action.Execute(this);
            var actionResult = this.Results.LastOrDefault();

            if (actionResult.IsError)
            {
                throw new Exception(actionResult.ErrorMessage);
            }
            if (actionResult is IActionCommandResult)
            {
                return (actionResult as IActionCommandResult).GetResult();
            }
            else
            {
                var getResult = actionResult.GetType().GetMethod("GetResult");
                if (getResult != null)
                {
                    var result = getResult.Invoke(actionResult, null);
                    this.Log("GetResult", Category.Debug);
                    return result as ActionResult;
                }

                throw new Exception("Web请求结果必须是IActionCommandResult" + actionResult.GetType().ToString());
            }
        }

        public string RenderView(object viewModel, string viewPath, bool isPartial)
        {
            using (var sw = new StringWriter())
            {
                if (this.Controller.ControllerContext == null)
                {
                    throw new Exception("ControllerContext is null.");
                }
                try
                {
                    var view = isPartial ? ViewEngines.Engines.FindPartialView(this.Controller.ControllerContext, viewPath) :
                        ViewEngines.Engines.FindView(this.Controller.ControllerContext, viewPath, null);

                    if (view == null)
                    {
                        throw new Exception("find view failed." + viewPath);
                    }
                    if (view.View == null)
                    {
                        throw new Exception("find view is null." + viewPath);
                    }
                    this.Controller.ViewData.Model = viewModel;

                    ViewContext viewContext = new ViewContext(this.Controller.ControllerContext, view.View,
                        this.Controller.ViewData,
                        this.Controller.TempData, sw);

                    view.View.Render(viewContext, sw);
                }
                catch (Exception ex)
                {
                    this.Log("view error:" + viewPath + "\r\n" + ex.ToString());
                    throw;
                }

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
