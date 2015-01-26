using System;
using System.ComponentModel.DataAnnotations;
using Panasia.Core.Sys;
namespace Panasia.Core.Sys
{
    public partial class bi_Payment : SysEntity
    {
        [Key]
        public string PaymentID
        {
            get;
            set;
        }
        public string PaymentName
        {
            get;
            set;
        }
        public string ParentID
        {
            get;
            set;
        }
        public int Type
        {
            get;
            set;
        }
        public string Remark
        {
            get;
            set;
        }
        public bool IsActive
        {
            get;
            set;
        }
    }
}

