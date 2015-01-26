using System;
using System.ComponentModel.DataAnnotations;
namespace Panasia.Gemini.FL.Data.Models
{
    public class hr_Job : Panasia.Core.Sys.SysEntity
	{
        [Key]
        public string JobID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int SortID { get; set; }
	}
}

