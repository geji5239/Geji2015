using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Panasia.Core;
using Panasia.Core.Sys;

namespace Panasia.Core.Auth
{
    public class PAEntity:Entity
    {
        #region 属性 CreatedUser
        public string CreatedUser
        {
            get
            {
                return GetFieldValue<string>("CreatedUser", string.Empty);
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
                return GetFieldValue<DateTime>("CreatedTime", DateTime.Now);
            }
            set
            {
                SetFieldValue("CreatedTime", value);
            }
        }
        #endregion

        #region 属性 ModifiedUser
        public string ModifiedUser
        {
            get
            {
                return GetFieldValue<string>("ModifiedUser", string.Empty);
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
                return GetFieldValue<DateTime>("ModifiedTime", DateTime.Now);
            }
            set
            {
                SetFieldValue("ModifiedTime", value);
            }
        }
        #endregion

        #region 属性 AutoKey
        public int AutoKey
        {
            get
            {
                return GetFieldValue<int>("AutoKey", 0);
            }
            set
            {
                SetFieldValue("AutoKey", value);
            }
        }
        #endregion

        #region 通用函数
        /// <summary>
        /// 新增记录的时候调用，给创建用户、修改用户及时间赋值
        /// </summary>
        public virtual void ResetCreated()
        {
            var user = SysService.GetCurrentUser();
            if (user != null)
            {
                CreatedUser = user.UserID;
                CreatedTime = DateTime.Now;
                ModifiedUser = user.UserID;
                ModifiedTime = DateTime.Now;
            }
        }

        /// <summary>
        /// 更改记录的时候调用，给修改用户、时间赋值
        /// </summary>
        public virtual void ResetUpdated()
        {
            var user = SysService.GetCurrentUser();
            if (user != null)
            {
                ModifiedUser = user.UserID;
                ModifiedTime = DateTime.Now;
            }
        }
        #endregion
    }
}