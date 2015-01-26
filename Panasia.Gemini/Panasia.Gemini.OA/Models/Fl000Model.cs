/*----------------------------------------------------------------
// Copyright (C) 2004 清风软件
// 版权所有。
//
// 文件名：Fl000Model.cs
// 文件功能描述：登录页面Model
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Panasia.Gemini.FL.Models
{
    public class LoginModel
    {
        public string UserName
        { get; set; }

        public string Password
        { get; set; }
    }
}