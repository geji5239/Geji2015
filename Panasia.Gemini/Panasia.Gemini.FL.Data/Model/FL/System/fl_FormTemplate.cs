using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Panasia.Gemini.FL.Data.Models
{
    public partial class fl_FormTemplate : Panasia.Core.Sys.SysEntity
    {
        /// <summary>
        /// 表单ID
        /// </summary>
        [Key, Column(Order = 0), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FormID { get; set; }
        [Key, Column(Order = 1), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TemplateID { get; set; }
        /// <summary>
        /// 单据模板名称
        /// </summary>
        public string Template_Name { get; set; }
        /// <summary>
        /// 模板程序代号
        /// </summary>
        public string Template_FileName { get; set; }
        /// <summary>
        /// 模板程序文件路径
        /// </summary>
        public string Template_Url { get; set; }
        private new int AutoKey { get; set; }
    }
}

