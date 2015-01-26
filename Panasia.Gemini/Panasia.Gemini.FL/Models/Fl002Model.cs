/*----------------------------------------------------------------
// Copyright (C) 2004 清风软件
// 版权所有。
//
// 文件名：Fl002Model.cs    
// 文件功能描述：模板表单Model
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Panasia.Gemini.FL.Models
{
    public class TemplateModel
    {
        public int TemplateID { get; set; }
        public int FormID { get; set; }
        public string FormName { get; set; }
        public string TemplateDetail_Name { get; set; }

        public string DataFrom { get; set; }

        public string Template_FileName { get; set; }

        public string Template_Url { get; set; }

        public string CompanyID { get; set; }
        public string MappingFormID { get; set; }
    }
}