using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Panasia.Gemini.FL.Data.Models
{
    [Table("hr_ConversionForm")]
    public class ConversionForm : Panasia.Core.Sys.SysEntity
    {
        [Key]
        public string CFID { get; set; }
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
        public decimal SXMonths { get; set; }
        public string Summary { get; set; }
        public bool IsActive { get; set; }
    }
}