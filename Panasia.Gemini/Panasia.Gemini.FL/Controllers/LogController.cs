/*----------------------------------------------------------------
// Copyright (C) 2004 清风软件
// 版权所有。
//
// 文件名：LogController.cs
// 文件功能描述：Fl006,Fl007控制器,视图显示功能。
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Panasia.Gemini.FL.Controllers
{
    public class LogController : Controller
    {
        // GET: Log
        public ActionResult Fl006()
        {
            return View();
        }
        public ActionResult Fl007()
        {
            return View();
        }
    }
}