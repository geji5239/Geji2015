using System;
using System.ComponentModel.DataAnnotations;
namespace Panasia.Core.Sys
{
    public partial class fl_Form : Panasia.Core.Sys.SysEntity
	{
		/// <summary>
		/// 表单ID
		/// </summary>
        [Key]
        public int FormID { get; set; }
		/// <summary>
		/// 表单名称
		/// </summary>
        public string FormName { get; set; }
		/// <summary>
		/// Table名称
		/// </summary>
        public string DataFrom { get; set; }
        /// <summary>
        /// 公司编号
        /// </summary>
        //public string CompanyID { get; set; }
        /// <summary>
        /// 单据类别 1.一般单据 2.产品单据
        /// </summary>
        public string FormType { get; set; }
        private new int AutoKey { get; set; }

        public string Companys { get; set; }
	}
}

