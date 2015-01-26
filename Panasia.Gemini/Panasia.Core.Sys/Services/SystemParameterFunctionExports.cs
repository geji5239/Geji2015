using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using System.Web.Caching;
using Panasia.Core;
using Panasia.Core.Commands;

namespace Panasia.Core.Sys
{
    public class SystemParameterFunctions
    {
        public const string SqlGetProfiles = "GetUserProfiles";

        protected UserPrincipal GetUser()
        {
            if (HttpContext.Current == null || HttpContext.Current.User == null)
            {
                return null;
            }

            return HttpContext.Current.User as UserPrincipal;
        }

        #region 和服务相关的导出方法
        //增加一些系统参数 
        [SystemParameterFunction("CurrentUserID", "当前系统用户编号")]
        public object GetCurrentUserID(string parameter)
        {
            var user = GetUser();
            if (user != null)
            {
                return user.UserID;
            }
            //参数及返回值由通讯双方约定
            return "";
        }

        //增加一些系统参数 
        [SystemParameterFunction("CurrentUserName", "当前系统用户名")]
        public object GetCurrentUserName(string parameter)
        {
            var user = GetUser();
            if (user != null)
            {
                return user.UserName;
            }
            //参数及返回值由通讯双方约定
            return "";
        }

        [SystemParameterFunction("CurrentUserDataFilter", "当前用户页面数据过滤")]
        public object GetCurrentUserDataFilter(string parameter)
        {
            if(string.IsNullOrWhiteSpace(parameter))
            {
                return "";
            }
            var pageID = parameter.Trim();
            var user = GetUser();
            if (user != null)
            {
                var roleModel = SysService.GetUserPage(user.UserID, pageID);
                return roleModel == null ? "" : roleModel.DataFilter;
            } 
            return "";
        }

        [SystemParameterFunction("TimeStamp", "当前系统Ticks")]
        public object GetCurrentDateTimeTicks(string parameter)
        {
            return DateTime.Now.Ticks;
        }

        [SystemParameterFunction("CurrentDateTime", "当前系统时间")]
        public object GetCurrentDateTime(string parameter)
        {
            if (parameter.IsNullOrEmpty())
            {
                return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                return DateTime.Now.ToString(parameter);
            }
        }
        #endregion 
        
        [SystemParameterFunction("CurrentUserProfiles", "当前用户属性")]
        public object GetCurrentUserProfiles(string parameter)
        {
            var userID = GetCurrentUserID(null);
            if (string.IsNullOrEmpty(userID.ToString()))
            {
                "UserProfiles".Log("当前用户未登录或已超时");
                return "";
            }

            var sessionID = string.Format("UserProfiles_{0}", userID);
            var oldValue = HttpContext.Current.Session[sessionID];
            if (oldValue != null && oldValue is RowEntity)
            {
                var userItem = oldValue as RowEntity;
                return userItem.GetValue(parameter, null);
            }

            SqlItem sqlitem = SqlData.Current.GetShare(SqlGetProfiles);
            if (sqlitem == null)
            {
                throw new Exception(string.Format("还未设置取用户属性的查询[{0}]", SqlGetProfiles));
            }

            var item = sqlitem.ExecuteQuery<RowEntity>("UserID", GetCurrentUserID(null)).FirstOrDefault();
            if (item == null)
            {
                "UserProfiles".Log("查询用户属性为空");
                return "";
            }

            HttpContext.Current.Session[sessionID] = item;
            return item.GetValue(parameter, null);
        }

        #region 数据流水号
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter">Role,R,6——>类别为Role,前缀为R,总长为6</param>
        /// <returns></returns>
        [SystemParameterFunction("SerialCode", "数据流水号")]
        public object GetSerialCode(string parameter)
        {
            if (!parameter.IsNullOrEmpty())
            {
                string[] paras = parameter.Trim().Split(',');
                if (paras.Length >= 3)
                {
                    var category = paras[0];
                    var prefix = GetPrefix(paras[1]);
                    var length = Convert.ToInt32(paras[2]);

                    using (var db = SysContext.GetCurrent())
                    {
                        return db.GetNextSerialCode(category, prefix, length);
                    }
                }
            }

            return "";
        }

        private static string GetPrefix(string prefix)
        {
            if (prefix.StartsWith("{"))
            {
                var values = prefix.TrimStart('{').TrimEnd('}').Split(':');
                if (values.Length == 2)
                {
                    switch (values[0].ToLower())
                    {
                        //处理日期类序号
                        case "date":
                            prefix = DateTime.Now.ToString(values[1]);
                            break;
                    }
                }
            }
            return prefix;
        }
        #endregion

        #region 系统编码
        [SystemParameterFunction("SystemCodes", "系统编码")]
        public object GetSystemCodes(string parameter)
        {
            if (parameter.IsNullOrEmpty())
            {
                return "";
            }
            else
            {
                var item = SystemCodes.Current.Items[parameter];
                return item==null? null: item.Items;
            }
        }
        #endregion 
    }
}