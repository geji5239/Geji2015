using System;
using System.ComponentModel.DataAnnotations;
namespace Panasia.Gemini.FL.Data.Models
{
    public partial class fl_TemplateForm : Panasia.Core.Sys.SysEntity
	{
		/// <summary>
		/// 表单ID
		/// </summary>
        [Key]
		public int TemplateID{ get; set; }
		/// <summary>
		/// 表单名称
		/// </summary>
		public string Template_Name{ get; set; }
		/// <summary>
		/// Table名称
		/// </summary>
		public string Table_Name{ get; set; }
        private new int AutoKey { get; set; }
	}
}

