using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Panasia.Core.Sys;
using Panasia.Gemini.Web.Models;
using Panasia.Gemini.Web.Models.Common;

namespace Panasia.Gemini.Web.Controllers
{
    public class bi_PaymentController : Controller
    {
        //
        // GET: /Product/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Search()
        {
            return View();
        }
        public ActionResult GetTree()
        {
            TreeNode T = new TreeNode();
            TreeNode tree = T.createTreePayment();
            tree.id = "000000";
            tree.text = "无";
            T.children.Add(tree);
            var jsonResult = Json(T.children, "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
            return jsonResult;
        }
    }
}
