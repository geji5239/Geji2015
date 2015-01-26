using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Panasia.Core.Sys
{
    public class fl_GoOutForm : Panasia.Core.Sys.SysEntity
    {
        [Key, Column(Order = 0)]
        public string GOFID { get; set; }
        [Key, Column(Order = 1)]
        public int Version { get; set; }
        public string EmployeeID { get; set; }
        public string DeptID { get; set; }
        public string CompanyID { get; set; }
        public string JobID { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string TimeUnit { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal ApplicationHours { get; set; }
        public string Reason { get; set; }
        public bool IsActive { get; set; }
    }
}