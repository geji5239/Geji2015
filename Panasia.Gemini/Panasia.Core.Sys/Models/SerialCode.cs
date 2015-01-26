using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Panasia.Core.Sys
{
    #region 系统系列号 app_serial_code SerialCode
    [Table("sys_SerialCode")]
    public class SerialCode : SysEntity
    {
        #region 属性 CodeType
        private string _CodeType = string.Empty;
        [Key]
        [Required]
        [MaxLength(20)]
        [Column(Order = 0)]
        public string CodeType
        {
            get
            {
                return _CodeType;
            }
            set
            {
                _CodeType = value; 
            }
        }
        #endregion

        #region 属性 Prefix
        private string _Prefix = string.Empty;
        [Key]
        [Required]
        [MaxLength(10)]
        [Column(Order = 1)]
        public string Prefix
        {
            get
            {
                return _Prefix;
            }
            set
            {
                _Prefix = value; 
            }
        }
        #endregion

        #region 属性 NextIndex
        private int _NextIndex = 0;
        [Required]
        public int NextIndex
        {
            get
            {
                return _NextIndex;
            }
            set
            {
                _NextIndex = value; 
            }
        }
        #endregion

        #region 属性 FixedLength
        private int _FixedLength = 10;
        [Required]
        public int FixedLength
        {
            get
            {
                return _FixedLength;
            }
            set
            {
                _FixedLength = value; 
            }
        }
        #endregion
    }
    #endregion
}
