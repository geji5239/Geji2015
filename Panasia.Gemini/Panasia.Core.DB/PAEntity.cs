using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panasia.Core
{
    public class PAEntity:TableEntity
    {       
        #region 属性 ID
        public int ID
        {
            get
            {
                return GetFieldValue<int>("ID",0);
            }
            set
            {
                SetFieldValue("ID", value);
            }
        }
        #endregion

        #region 属性 CreatedUser
        public int CreatedUser
        {
            get
            {
                return GetFieldValue<int>("CreatedUser", 0);
            }
            set
            {
                SetFieldValue("CreatedUser", value);
            }
        }
        #endregion

        #region 属性 CreatedTime
        public DateTime CreatedTime
        {
            get
            {
                return GetFieldValue<DateTime>("CreatedTime", DateHelper.MinDate);
            }
            set
            {
                SetFieldValue("CreatedTime", value);
            }
        }
        #endregion

        #region 属性 ModifiedUser
        public int ModifiedUser
        {
            get
            {
                return GetFieldValue<int>("ModifiedUser", 0);
            }
            set
            {
                SetFieldValue("ModifiedUser", value);
            }
        }
        #endregion

        #region 属性 ModifiedTime
        public DateTime ModifiedTime
        {
            get
            {
                return GetFieldValue<DateTime>("ModifiedTime", DateHelper.MinDate);
            }
            set
            {
                SetFieldValue("ModifiedTime", value);
            }
        }
        #endregion
    }
}
