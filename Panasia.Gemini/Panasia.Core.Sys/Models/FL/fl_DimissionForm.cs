using Panasia.Core.Sys;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Panasia.Core.Sys
{
    public class fl_DimissionForm : SysEntity
    {
        [Key, Column(Order = 0)]
        public string DFID { get; set; }
        [Key, Column(Order = 1)]
        public int Version { get; set; }
        public string CompanyID { get; set; }
        public string EmployeeID { get; set; }
        public string DeptID { get; set; }
        public System.DateTime HireDate { get; set; }
        public string JobID { get; set; }
        public string Tel { get; set; }
        public System.DateTime ApplicationDate { get; set; }
        public System.DateTime DimissionDate { get; set; }
        public string DimissionType { get; set; }
        public string Reason { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsHandover { get; set; }
        public Nullable<bool> IsCustomerSafe { get; set; }
        public Nullable<bool> IsDocOrTelInfo { get; set; }
        public Nullable<bool> IsErpOrMail { get; set; }
        public string AssociateContent { get; set; }
        public string AssociateEmployeeID { get; set; }
        public string Approver { get; set; }
        public Nullable<System.DateTime> ApproveDate { get; set; }
    }
}

