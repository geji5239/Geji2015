using System;
using System.ComponentModel.DataAnnotations;
namespace Panasia.Core.Sys
{
    public class fl_MappingForm : Panasia.Core.Sys.SysEntity
	{
        [Key]
        public int MappingFormID { get; set; }
		/// <summary>
		/// Table名称
		/// </summary>
        public string Table_Name { get; set; }
		/// <summary>
		/// 单据id
		/// </summary>
        public string FormNo { get; set; }
		/// <summary>
        /// 签核状态标志 0签核流程中 -1驳回 1结单
		/// </summary>
        public int Approved { get; set; }
        public DateTime ApprovedDate { get; set; }
        /// <summary>
        /// 保存本次单据所使用的模版，防止以后修改模版造成的字段不匹配等问题
        /// </summary>
        public string TemplateUrl { get; set; }
        public string Attachment { get; set; }
        //数据库里没有这个字段，继承的基类里又有，只好重写隐藏原成员并且限制访问
        private new int AutoKey { get; set; }
	}
}

