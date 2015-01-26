using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Panasia.Core.Sys
{
    public static class RequestExtension
    {
        public static string GetClientIPAddress(this HttpRequest request)
        {
            return request.IsLocal ? request.GetServerName() : request.UserHostAddress;
        }
        public static string GetServerName(this HttpRequest request)
        {
            return System.Net.Dns.GetHostName();
        }
        public static string GetServerIPAddress(this HttpRequest request)
        {
            string strHostName = System.Net.Dns.GetHostName();
            IPHostEntry ipHostInfo = Dns.GetHostEntry(strHostName);
            IPAddress ipAddress = ipHostInfo.AddressList[0]; 
            return ipAddress.ToString();
        }

        public static string GetRealIP(this HttpRequest request)
        {
            string ip;
            try
            {
                if (request.ServerVariables["HTTP_VIA"] != null)
                {
                    ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString().Split(',')[0].Trim();
                }
                else
                {
                    ip = request.UserHostAddress;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return ip;
        }
    }
     
}
