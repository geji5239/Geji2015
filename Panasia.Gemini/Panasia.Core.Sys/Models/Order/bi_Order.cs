using System;
using System.ComponentModel.DataAnnotations;
namespace Panasia.Core.Sys
{
    public partial class bi_Order
    {
        [Key]
        public string OrderID
        {
            get;
            set;
        }
        public string ContractID
        {
            get;
            set;
        }
        public DateTime OrderDate
        {
            get;
            set;
        }
        public string EmployeeID
        {
            get;
            set;
        }

        public string CustomerID
        {
            get;
            set;
        }

        public decimal ContractAmount
        {
            get;
            set;
        }
        public DateTime ContractDate
        {
            get;
            set;
        }
        public string OrderState
        {
            get;
            set;
        }
        public DateTime OrderStateDate
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

