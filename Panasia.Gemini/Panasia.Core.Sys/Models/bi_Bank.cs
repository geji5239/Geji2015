using System;
using System.ComponentModel.DataAnnotations;
namespace Panasia.Core.Sys
{
    public partial class bi_Bank
    {
        [Key]
        public string BankID
        {
            get;
            set;
        }
        public string BankName
        {
            get;
            set;
        }
        public string CompanyID
        {
            get;
            set;
        }
        public string Account
        {
            get;
            set;
        }
        public decimal Remaining
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

