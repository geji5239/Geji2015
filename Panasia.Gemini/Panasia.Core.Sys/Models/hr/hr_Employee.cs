using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Panasia.Core.Sys
{
    [Table("hr_Employee")]
    public class hr_Employee : SysEntity
	{
        [Key]
        public string EmployeeID { get; set; }
        public string CompanyID { get; set; }//后来新增的字段2014-10-23 ysh
        public string DeptID { get; set; }
        public string JobID { get; set; }
        public string UserID { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        public string CardNo { get; set; }
        public DateTime? Birthday { get; set; }
        public string Nation { get; set; }
        public string Marriage { get; set; }
        public string Politics { get; set; }
        public string Country { get; set; }
        public string NativeProvince { get; set; }
        public string NativeCounty { get; set; }
        public string RegisterAddress { get; set; }
        public string Health { get; set; }
        public string LiveAddress { get; set; }
        public string MobiPhone { get; set; }
        public string Telephone { get; set; }
        public string EmergencyContact { get; set; }
        public string EmergencyContactTel { get; set; }
        public string Education { get; set; }
        public string InsuranceNo { get; set; }
        public DateTime? HireDate { get; set; }
        public DateTime? RegularDate { get; set; }
        public DateTime? Terminate { get; set; }
        public string Email { get; set; }
        public string Pemail { get; set; }
        public string ZpFrom { get; set; }
        public string CardID { get; set; }
        public string ImoID { get; set; }
        public string ExtNum { get; set; }//2014-10-24 字段类型修改 ysh
        public string EduName { get; set; }
        public string Major { get; set; }
        public DateTime? EduEndDate { get; set; }
        public string Convention { get; set; }
        public string SocialSecurity { get; set; }
        public string ContractOfSale { get; set; }
        public DateTime? ConventionStartDate { get; set; }
        public DateTime? ConventionEndDate { get; set; }
        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public string EnName { get; set; }
        public string State { get; set; }
        public string JobState { get; set; }
        public bool IsActive { get; set; }
        public string PhotoPath { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public DateTime? FirstWorkDate { get; set; }
	}
}

