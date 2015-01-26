using Panasia.Core.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Panasia.Core.Sys
{
    public class fl_EmployeeMove : SysEntity
    {
        [Key, Column(Order = 0)]
        public string Movid { get; set; }
        [Key, Column(Order = 1)]
        public int Version { get; set; }
        public string CompanyID { get; set; }
        public DateTime RealMoveDate { get; set; }
        public DateTime ApplicationDate { get; set; }
        public DateTime StartDate { get; set; }
        public string EmployeeID { get; set; }
        public string OldDeptID { get; set; }
        public string OldJobID { get; set; }
        public string NewDeptID { get; set; }
        public string NewJobID { get; set; }
        public string MoveType { get; set; }
        public Nullable<int> TryMonth { get; set; }
        public string JJRemark { get; set; }
        public string JJPerson { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public string Suggestion { get; set; }
        public string Approver { get; set; }
        public Nullable<System.DateTime> ApproveDate { get; set; }
        public string OSuggestion { get; set; }
        public string OSuggestion1 { get; set; }
        public string OSuggestion2 { get; set; }
        public string OApprover { get; set; }
        public string OApprover1 { get; set; }
        public string OApprover2 { get; set; }
        public Nullable<System.DateTime> OApproveDate { get; set; }
        public Nullable<System.DateTime> OApproveDate1 { get; set; }
        public Nullable<System.DateTime> OApproveDate2 { get; set; }
        public string ISuggestion { get; set; }
        public string ISuggestion1 { get; set; }
        public string ISuggestion2 { get; set; }
        public string IApprover { get; set; }
        public string IApprover1 { get; set; }
        public string IApprover2 { get; set; }
        public Nullable<System.DateTime> IApproveDate { get; set; }
        public Nullable<System.DateTime> IApproveDate1 { get; set; }
        public Nullable<System.DateTime> IApproveDate2 { get; set; }
        public string Suggestion2 { get; set; }
        public string Suggestion3 { get; set; }
        public string Approver2 { get; set; }
        public string Approver3 { get; set; }
        public Nullable<System.DateTime> ApproveDate2 { get; set; }
        public Nullable<System.DateTime> ApproveDate3 { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}