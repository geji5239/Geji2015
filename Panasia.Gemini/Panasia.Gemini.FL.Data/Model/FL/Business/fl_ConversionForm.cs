using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Panasia.Gemini.FL.Data.Models
{
    public class fl_ConversionForm : Panasia.Core.Sys.SysEntity
    {
        [Key, Column(Order = 0)]
        public string CFID { get; set; }
        [Key, Column(Order = 1)]
        public int Version { get; set; }
        public string EmployeeID { get; set; }
        public string CompanyID { get; set; }
        public string DeptID { get; set; }
        public DateTime? HireDate { get; set; }
        public string JobID { get; set; }
        public DateTime? ApplicationDate { get; set; }
        public string EduName { get; set; }
        public string Education { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Summary { get; set; }
        public decimal SXMonths { get; set; }
        public int Assess1 { get; set; }
        public int Assess2 { get; set; }
        public int Assess3 { get; set; }
        public int Assess4 { get; set; }
        public int Assess5 { get; set; }
        public int Assess6 { get; set; }
        public int Assess7 { get; set; }
        public int Assess8 { get; set; }
        public int Assess9 { get; set; }
        public int Assess10 { get; set; }
        public int Assess11 { get; set; }
        public int Assess12 { get; set; }
        public string Suggestion1 { get; set; }
        public decimal Salary1 { get; set; }
        public string EmployeeID1 { get; set; }
        public DateTime? ApprovedDate1 { get; set; }
        public string Suggestion2 { get; set; }
        public string Suggestion3 { get; set; }
        public decimal Salary2 { get; set; }
        public decimal Salary3 { get; set; }
        public string EmployeeID2 { get; set; }
        public string EmployeeID3 { get; set; }
        public DateTime? ApprovedDate2 { get; set; }
        public DateTime? ApprovedDate3 { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }
    }
}

