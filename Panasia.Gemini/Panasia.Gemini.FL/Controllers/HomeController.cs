/*----------------------------------------------------------------
// Copyright (C) 2004 清风软件
// 版权所有。
//
// 文件名：HomeController.cs
// 文件功能描述：Fl001控制器,首页呈现功能。
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Panasia.Gemini.FL.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Fl001()
        {
            HttpCookie login = Request.Cookies["login"];
            if (login == null)
            {
                Response.Redirect("~/User/Logout");
            }
            
            return View();
        }
        private string NodeInfo(int id, string text, string url)
        {
            string s = string.Format("\"id\":{0},\"text\":\"{1}\",\"attributes\":{{\"url\":\"{2}\"}}", id, text, url);
            return s;
        }

        public ActionResult InitTree(string node)
        {
            string json = string.Empty;
            if (node == "tr1")
            {
                if (LoginInfo.UserID == "U00001")
                {
                    json = @"[{" + NodeInfo(11, "模板表单", "/Template/Fl002") + "},{" +
                                   NodeInfo(12, "表单映射", "/Template/Fl003") + "}]";
                }
            }
            else if (node == "tr2")
            {
                if (LoginInfo.UserID == "U00001")
                {
                    json = @"[{" + NodeInfo(21, "流程设计", "/Process/Fl004") + "},{" +
                                   NodeInfo(22, "流程监控", "/Process/Fl005") + "}]";
                }
                else
                {
                    json = @"[{" +  NodeInfo(22, "流程监控", "/Process/Fl005") + "}]";
                }
            }
            //else if (node == "tr3")
            //{
            //    json = @"[{" + NodeInfo(31, "访问日志", "/Log/Fl006") + "},{" +
            //                   NodeInfo(32, "操作日志", "/Log/Fl007") + "}]";
            //}
            else
            {
                json = @"[{" + NodeInfo(01, "单据签核", "/Approve/Fl011") + "}]";
            }
            return Content(json);
        }
    }
}
