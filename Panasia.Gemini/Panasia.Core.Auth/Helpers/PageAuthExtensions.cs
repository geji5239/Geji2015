using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Panasia.Core.App;
using Panasia.Core.Sys;

namespace Panasia.Core.Auth
{
    public static class PageAuthExtensions
    {
        public static bool IsNetAllow(this Page page)
        {
            if(HttpContext.Current == null || HttpContext.Current.Request==null)
            {
                return false;
            }
            var request = HttpContext.Current.Request;
            #region 网络权限
            switch (page.NetAuth)
            { 
                case NetAuth.None:
                    return true; 
                case NetAuth.Local:
                    return request.IsLocal; 
                case NetAuth.LocalArea:
                    return request.IsLocal || request.GetRealIP().IsInnerNetWork(); 
                case NetAuth.Mac:
                    if(string.IsNullOrEmpty(page.NetAuthAddress))
                    {
                        return false;
                    }
                    var mac = request.GetRealIP().GetMac();
                    return page.NetAuthAddress.Split(',')
                        .FirstOrDefault(d => d.Equals(mac, StringComparison.OrdinalIgnoreCase)) != null;
                default:
                    break;
            }
            #endregion
            return false;
        }
    }
}
