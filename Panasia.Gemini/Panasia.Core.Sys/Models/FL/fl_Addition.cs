using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Panasia.Core.Sys
{
    public class fl_Addition : Panasia.Core.Sys.SysEntity
    {
        [Key, Column(Order = 0)]
        public string AID { get; set; }
        [Key, Column(Order = 1)]
        public int Version { get; set; }
        [MaxLength(6)]
        public string CompanyID { get; set; }
        [Required, MaxLength(6)]
        public string DeptID { get; set; }
        [MaxLength(50)]
        public string Position { get; set; }
        [Required, MaxLength(50)]
        public string Reason { get; set; }
        [Required]
        public int Need { get; set; }
        [Required]
        public DateTime? ApplicationDate { get; set; }
        [Required]
        public DateTime? AvailableDate { get; set; }
        [Required, MaxLength(2)]
        public string Sex { get; set; }
        [Required, MaxLength(50)]
        public string Degree { get; set; }
        [Required, MaxLength(30)]
        public string YearsOfWorking { get; set; }
        [Required, MaxLength(50)]
        public string SalaryRange { get; set; }
        [Required, MaxLength(500)]
        public string Duty { get; set; }
        [MaxLength(500)]
        public string Remark { get; set; }
        public bool? IsActive { get; set; }
    }
}

