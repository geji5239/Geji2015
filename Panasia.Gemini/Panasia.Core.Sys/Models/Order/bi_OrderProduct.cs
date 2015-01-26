using System;
using System.ComponentModel.DataAnnotations;
namespace Panasia.Core.Sys
{
    public partial class bi_OrderProduct
    {
        [Key]
        public string OrdPctID
        {
            get;
            set;
        }
        public string OrderID
        {
            get;
            set;
        }
        public string TCProductID
        {
            get;
            set;
        }
        public string ProductTypeID
        {
            get;
            set;
        }
        public string ProductID
        {
            get;
            set;
        }

        public string OrderTypeID
        {
            get;
            set;
        }
        public int Qty
        {
            get;
            set;
        }
        public decimal Amount
        {
            get;
            set;
        }
        public decimal ContractAmount
        {
            get;
            set;
        }
        public decimal MinMonetary
        {
            get;
            set;
        }

        public int Number
        {
            get;
            set;
        }
        public string SiteSpace
        {
            get;
            set;
        }
        public string EmailFunID
        {
            get;
            set;
        }
        public string BeiAnTypeID
        {
            get;
            set;
        }
        public string RegeditYearID
        {
            get;
            set;
        }
        public DateTime StartDate
        {
            get;
            set;
        }
        public DateTime EndDate
        {
            get;
            set;
        }
        public string RecordCaiLing
        {
            get;
            set;
        }
        public string BusinessTelephone
        {
            get;
            set;
        }
        public string DomainName1
        {
            get;
            set;
        }
        public string DomainName2
        {
            get;
            set;
        }
        public string DomainName3
        {
            get;
            set;
        }
        public decimal RechargeAmount
        {
            get;
            set;
        }

        public string RechargeAccount
        {
            get;
            set;
        }

        public string SeviceYearID
        {
            get;
            set;
        }
        public string SiteOrderTypeID
        {
            get;
            set;
        }
        public string DomainOrderTypeID
        {
            get;
            set;
        }
        public string EmailOrderTypeID
        {
            get;
            set;
        }
        public string OtherOrderTypeID
        {
            get;
            set;
        }
        public string TemplateNo
        {
            get;
            set;
        }
        public decimal TemplateAmount
        {
            get;
            set;
        }
        public string SiteVersion
        {
            get;
            set;
        }
        public string Remark1
        {
            get;
            set;
        }
        public string Remark2
        {
            get;
            set;
        }
        public string Remark3
        {
            get;
            set;
        }
        public bool IsActive
        {
            get;
            set;
        }
        public string CreatedUser
        {
            get;
            set;
        }
        public DateTime CreatedTime
        {
            get;
            set;
        }
        public string ModifiedUser
        {
            get;
            set;
        }
        public DateTime ModifiedTime
        {
            get;
            set;
        }
        public int AutoKey
        {
            get;
            set;
        }
    }
}

