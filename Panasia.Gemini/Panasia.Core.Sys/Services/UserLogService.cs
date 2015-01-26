using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Web;

namespace Panasia.Core.Sys
{
    public static class UserLogService
    {
        public static void AddSystemLog(this object sender,string actionName, string url, string description = "",string userName="")
        {
            AddLog(actionName, url, description,userName);
        }
        public static void AddLog(string actionName, string url, string description = "",string userName="")
        {
            using (var db = SysContext.GetCurrent())
            {                
                var user = SysService.GetCurrentUser();
                var newLog = new UserLog
                {
                    ActionName = actionName,
                    Url = url,
                    Description = description,
                    ClientIP = HttpContext.Current.Request.GetClientIPAddress(),
                    UserName = user == null ? userName : user.FullName
                };
                newLog.ResetCreated();
                db.UserLogs.Add(newLog);
                db.SaveChanges();
            }
        }
    }
}
