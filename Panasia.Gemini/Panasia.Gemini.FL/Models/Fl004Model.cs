/*----------------------------------------------------------------
// Copyright (C) 2004 清风软件
// 版权所有。
//
// 文件名：Fl004Model.cs
// 文件功能描述：流程设计Model
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
    public class ProcessModel
    {
        public int approveform_id { get; set; }
        public string dept_id { get; set; }
        public string job_id { get; set; }
        public string emp_id_rule { get; set; }
        public string emp_id_emp { get; set; }
        public int emp_sort { get; set; }
        public int ar_id { get; set; }
        public string Template_Info { get; set; }
        public string col_name { get; set; }
        public string condition { get; set; }
        public int approvedrule_id { get; set; }
        public string form_id { get; set; }
        public string value { get; set; }
        public int FormID { get; set; }
        public bool IsGroupApprover { get; set; }
        public bool IsFuzzy { get; set; }
        public int FuzzyType { get; set; }
        public int TemplateID { get; set; }
        public string CompanyID { get; set; }
        public string hDeptIDCondition { get; set; }
        public string hEmployeeIDCondition { get; set; }
        public string Approverid { get; set; }
        public string Approver { get; set; }
    }
}