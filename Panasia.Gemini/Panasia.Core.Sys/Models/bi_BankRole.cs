using System;
using System.ComponentModel.DataAnnotations;
namespace Panasia.Core.Sys
{
    public partial class bi_BankRole
    {
        [Key]
        public string BankRoleID
        {
            get;
            set;
        }
        public string EmployeeID
        {
            get;
            set;
        }
        public string BankIDList
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

