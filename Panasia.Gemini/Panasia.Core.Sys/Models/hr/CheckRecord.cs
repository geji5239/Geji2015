using Panasia.Core.Sys;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Panasia.Core.Sys
{
    [Table("hr_CheckRecord")]
    public class CheckRecord : SysEntity
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new int AutoKey { get; set; }
        [Required, MaxLength(6)]
        public string CompanyID { get; set; }
        [Required, MaxLength(6)]
        public string DeptID { get; set; }
        [Required, MaxLength(30)]
        public string CardID { get; set; }
        public DateTime CardTime { get; set; }
        [MaxLength(10)]
        public string Source { get; set; }
        [MaxLength(200)]
        public string Remark { get; set; }
        public bool IsActive { get; set; }
    }   //原始数据

    [Table("hr_ArrangeWork")]
    public class ArrangeWork : SysEntity     //工作时间
    {
        [Key]
        [Required, MaxLength(6)]
        public string AWID { get; set; }
        [Required, MaxLength(6)]
        public string CompanyID { get; set; }
        [Required, MaxLength(20)]
        public string WorkDate { get; set; }
        [Required]
        public TimeSpan UpTime { get; set; }
        [Required]
        public TimeSpan DownTime { get; set; }
    }   
    [Table("hr_Rule")]
    public class Hr_Rule : SysEntity
    {
        [Key]
        public string RID { get; set; }
        [Required, MaxLength(30)]
        public string RName { get; set; }
        [MaxLength(6)]
        public string CompanyID { get; set; }
        [MaxLength(100)]
        public string Remark { get; set; }
        public Nullable<int> MinutesLate { get; set; }
        public Nullable<int> IgnoreMinutesLate { get; set; }
        public Nullable<int> MinutesEarlyleave { get; set; }
        public Nullable<int> IgnoreMinutesEarlyleave { get; set; }
        public Nullable<int> MinutesAbsent { get; set; }
        public Nullable<int> MinAbsentHours { get; set; }
        public Nullable<int> MaxAbsentHours { get; set; }
        public Nullable<int> MinOverTimeMinutes { get; set; }
        public Nullable<int> RealCalculateMinutes { get; set; }
        public Nullable<int> MaxOverTimeHours { get; set; }
        public Nullable<int> MaxForgetCardTimes { get; set; }
        public Nullable<int> AutoFiredDays { get; set; }
        public Nullable<int> MonthClosingDay { get; set; }
        public Nullable<bool> r14 { get; set; }
        public Nullable<bool> r15 { get; set; }
        public Nullable<bool> r16 { get; set; }
        public Nullable<bool> r17 { get; set; }
        public Nullable<bool> r18 { get; set; }
        public Nullable<bool> r19 { get; set; }
        public Nullable<bool> r20 { get; set; }
    }       //考勤规则

    [Table("hr_Holiday")]
    public class Holiday : SysEntity     //假期设置
    {
        [Key, Required, MaxLength(6)]
        public string HhID { get; set; }
        [Required, MaxLength(8)]
        public string CompanyID { get; set; }
        [MaxLength(20)]
        public string HolidayDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? Coefficient { get; set; }
        public bool? IsWork { get; set; }
        public string BuBanDate { get; set; }
    }

    [Table("hr_LeaveType")]
    public class LeaveType: SysEntity   //假期类别
    {
        [Key,Required,MaxLength(4)]
        public string LvID { get; set; }
        [Required]
        public string LeaveName { get; set; }
        public Nullable<decimal> Coefficient { get; set; }
        public string Remark { get; set; }
    }

    [Table("hr_Attendance")]
    public class Attendance : SysEntity      //日出勤记录
    {
        [Key, MaxLength(4),Required]
        [Column(Order = 0)]
        public string Year { get; set; }
        [Key, MaxLength(2), Required]
        [Column(Order=1)]
        public string Month { get; set; }
        [Key, MaxLength(2), Required]
        [Column(Order = 2)]
        public string Day { get; set; }
        [Key, MaxLength(6), Required]
        [Column(Order = 3)]
        public string CompanyID { get; set; }
        [Key, MaxLength(6), Required]
        [Column(Order = 4)]
        public string DeptID { get; set; }
        [Key, MaxLength(6), Required]
        [Column(Order = 5)]
        public string EmployeeID { get; set; }
        [MaxLength(6)]
        public string JobID { get; set; }
        public Nullable<TimeSpan> StartTime { get; set; }//上班打卡时间
        public Nullable<TimeSpan> EndTime { get; set; }//下班打卡时间
        public Nullable<decimal> Overtime { get; set; }//加班时数
        public Nullable<int> MinutesLate { get; set; }//迟到分钟
        public Nullable<int> LeaveEarlyMinutes { get; set; }//早退分钟
        public Nullable<decimal> AbsenteeismHours { get; set; }//旷工时数
        [MaxLength(200)]
        public string Remark { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }

    [Table("hr_PaidLeaveForm")]
    public class PaidLeaveForm : SysEntity   //调休单
    {
        [Key,Required,MaxLength(6)]
        public string PLID { get; set; }
        [Required,MaxLength(6)]
        public string EmployeeID { get; set; }
        [MaxLength(6)]
        public string CompanyID { get; set; }
        [Required,MaxLength(6)]
        public string DeptID { get; set; }
        [Required, MaxLength(6)]
        public string JobID { get; set; }
        [Required]
        public DateTime ApplicationDate { get; set; }
        [Required]
        public DateTime TStartDate { get; set; }
        [Required]
        public DateTime TEndDate { get; set; }
        [Required]
        public decimal ApplicationHours { get; set; }
        [MaxLength(2)]
        public string TimeUnit { get; set; }
        [Required,MaxLength(20)]
        public string Reason { get; set; }
        [Required,MaxLength(200)]
        public string Job { get; set; }
        [Required, MaxLength(6)]
        public string Agent { get; set; }
        public bool? IsActive { get; set; }//这个字段可以放到SysEntity
    }

    [Table("hr_GoOutForm")]
    public class GoOutForm : SysEntity   //外出单
    {
        [Key,Required,MaxLength(6)]
        public string GOFID { get; set; }
        [Required,MaxLength(6)]
        public string EmployeeID { get; set; }
        [MaxLength(6)]
        public string CompanyID { get; set; }
        [Required,MaxLength(6)]
        public string DeptID { get; set; }
        [Required,MaxLength(6)]
        public string JobID { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public Nullable<DateTime> ApplicationDate { get; set; }
        public Nullable<decimal> ApplicationHours { get; set; }
        [MaxLength(2)]
        public string TimeUnit { get; set; }
        [Required,MaxLength(50)]
        public string Reason { get; set; }
        public bool? IsActive { get; set; }
    }

    [Table("hr_AbnormalAttendance")]
    public class AbnormalAttendance : SysEntity  //考勤异常申请单
    {
        [Key,Required]
        public string AAID { get; set; }
        [Required,MaxLength(6)]
        public string EmployeeID { get; set; }
        [MaxLength(6)]
        public string CompanyID { get; set; }
        [Required,MaxLength(6)]
        public string DeptID { get; set; }
        [Required,MaxLength(6)]
        public string JobID { get; set; }
        [Required]
        public DateTime ApplicationDate { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public decimal ApplicationHours { get; set; }
        [Required,MaxLength(30)]
        public string ThrowableType { get; set; }
        [Required, MaxLength(200)]
        public string Reason { get; set; }
        public bool? IsActive { get; set; }
    }

    [Table("hr_LeaveApplicationForm")]
    public class LeaveApplicationForm : SysEntity    //休假单
    {
        [Key,Required,MaxLength(6)]
        public string LAFID { get; set; }
        [Required, MaxLength(6)]
        public string EmployeeID { get; set; }
        [MaxLength(6)]
        public string CompanyID { get; set; }
        [Required, MaxLength(6)]
        public string DeptID { get; set; }
        [Required]
        public DateTime HireDate { get; set; }
        [Required, MaxLength(6)]
        public string JobID { get; set; }
        [Required]
        public DateTime ApplicationDate { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public decimal ApplicationHours { get; set; }
        [Required, MaxLength(2)]
        public string TimeUnit { get; set; }
        [Required,MaxLength(4)]
        public string LeaveType { get; set; }
        [Required,MaxLength(200)]
        public string Reason { get; set; }
        [Required, MaxLength(200)]
        public string AssociateContent { get; set; }
        [Required, MaxLength(6)]
        public string AssociateEmployeeID { get; set; }
        public bool? IsActive { get; set; }
    }

    [Table("hr_OvertimeApplicationForm")]
    public class OvertimeApplicationForm : SysEntity     //加班申请单
    {
        [Key,Required,MaxLength(6)]
        public string OAFID { get; set; }
        [Required, MaxLength(6)]
        public string EmployeeID { get; set; }
        [MaxLength(6)]
        public string CompanyID { get; set; }
        [Required, MaxLength(6)]
        public string DeptID { get; set; }
        [MaxLength(6)]
        public string JobID { get; set; }
        [Required]
        public DateTime ApplicationDate { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public decimal ApplicationHours { get; set; }
        [MaxLength(2)]
        public string TimeUnit { get; set; }
        [Required, MaxLength(200)]
        public string Reason { get; set; }
        public bool? IsActive { get; set; }
    }

    [Table("hr_DimissionForm")]
    public class DimissionForm : SysEntity       //离职单
    {
        [Key,Required,MaxLength(6)]
        public string DFID { get; set; }
        [Required, MaxLength(6)]
        public string EmployeeID { get; set; }
        [MaxLength(6)]
        public string CompanyID { get; set; }
        [Required, MaxLength(6)]
        public string DeptID { get; set; }
        [Required]
        public DateTime HireDate { get; set; }
        [Required,MaxLength(6)]
        public string JobID { get; set; }
        [Required,MaxLength(20)]
        public string Tel { get; set; }
        [Required]
        public DateTime ApplicationDate { get; set; }
        [Required]
        public DateTime DimissionDate { get; set; }
        [Required,MaxLength(20)]
        public string DimissionType { get; set; }
        public Nullable<DateTime> StopSalaryDate { get; set; }
        [Required, MaxLength(200)]
        public string Reason { get; set; }
        [MaxLength(200)]
        public string AssociateContent { get; set; }
        [Required,MaxLength(6)]
        public string AssociateEmployeeID { get; set; }
        public Nullable<bool> IsActive { get; set; }
    
    }

    [Table("hr_CheckResult")]
    public class CheckResult : SysEntity     //月考勤结果
    {
        [Key,Required,MaxLength(4)]
        [Column(Order = 0)]
        public string Year { get; set; }
        [Key, Required, MaxLength(2)]
        [Column(Order = 1)]
        public string Month { get; set; }
        [Key, Required, MaxLength(6)]
        [Column(Order = 2)]
        public string CompanyID { get; set; }
        [Key, Required, MaxLength(6)]
        [Column(Order = 3)]
        public string DeptID { get; set; }
        [Key, Required, MaxLength(6)]
        [Column(Order = 4)]
        public string EmployeeID { get; set; }
        [MaxLength(6)]
        public string JobID { get; set; }
        public decimal AttendanceDays { get; set; }//出勤天数
        public decimal RealAttendanceDays { get; set; }//实际出勤天数
        public decimal PaidHolidays { get; set; }//带薪假天数
        public int LaterTimes { get; set; }//迟到次数
        public int LeaveEarlyTimes { get; set; }//早退次数
        public decimal SickDays { get; set; }//病假天数
        public decimal LeaveDays { get; set; }//事假天数
        public decimal AbsenteeismDays { get; set; }//旷工天数
        public decimal AbsenceDays { get; set; }//缺勤数
        public decimal FullDays { get; set; }//满勤天数
        public Nullable<decimal> Overtime { get; set; }//加班时数
        [MaxLength(10)]
        public string State { get; set; }//备注
        [MaxLength(200)]
        public string Remark { get; set; }
    }


    [Table("rc_Addition")]
    public class Addition : SysEntity
    {
        [Key, Required, MaxLength(6)]
        public string AID { get; set; }
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
        public bool? IsFill { get; set; }
        public bool? IsActive { get; set; }
    }
}
