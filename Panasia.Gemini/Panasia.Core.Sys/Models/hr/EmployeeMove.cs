using Panasia.Core.Sys;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Panasia.Core.Sys
{
    [Table("hr_EmployeeMove")]
    public partial class EmployeeMove : SysEntity
    {
        [Key]
        public string Movid { get; set; }
        public string EmployeeID { get; set; }
        public string CompanyID { get; set; }
        public Nullable<System.DateTime> ApplicationDate { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> RealMoveDate { get; set; }
        public string OldDeptID { get; set; }
        public string OldJobID { get; set; }
        public string NewDeptID { get; set; }
        public string NewJobID { get; set; }
        public string MoveType { get; set; }
        public Nullable<decimal> OldSalaryBase { get; set; }
        public Nullable<decimal> NewSalaryBase { get; set; }
        public string Remark { get; set; }
        public string JJRemark { get; set; }
        public string JJPerson { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}