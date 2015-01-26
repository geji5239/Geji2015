/*----------------------------------------------------------------
// Copyright (C) 2004 清风软件
// 版权所有。
//
// 文件名：Fl003Model.cs    
// 文件功能描述：表单映射Model
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
    public class MappingModel
      {
        public int MappingFormID { get; set; }
        public string Table_Name { get; set; }
        public string FormID { get; set; }
        public int Approved { get; set; }
   
    }
}