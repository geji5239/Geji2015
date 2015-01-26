using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Panasia.Gemini.Web.Models.Filters
{
    public class HttpsRequire:RequireHttpsAttribute
    {
        public HttpsRequire() : base() { }
        public HttpsRequire(bool requireSecure)
            : this()
        {
            RequireSecure = requireSecure;
        }
        #region property RequireSecure
        private bool _RequireSecure = false;
        public bool RequireSecure
        {
            get
            {
                return _RequireSecure;
            }
            set
            {
                _RequireSecure = value;
            }
        }
        #endregion //property RequireSecure

        #region property HttpPort
        private static string _HttpPort = string.Empty;
        public static string HttpPort
        {
            get
            {
                return _HttpPort;
            }
            set
            {
                _HttpPort = value;
            }
        }
        #endregion //property HttpPort

        public static string _HttpsPort = null;
        public static string HttpsPort
        {
            get
            {
                if (_HttpsPort == null)
                {
                    _HttpsPort = System.Configuration.ConfigurationManager.AppSettings["HttpsPort"].Trim();
                }
                return _HttpsPort;
            }
        }

        public static string _HttpsEnable = null;
        public static string HttpsEnable
        {
            get
            {
                if (_HttpsEnable == null)
                {
                    _HttpsEnable = System.Configuration.ConfigurationManager.AppSettings["HttpsEnable"].Trim();
                }
                return _HttpsEnable;
            }
        }

        protected override void HandleNonHttpsRequest(AuthorizationContext filterContext)
        {
            if (String.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                if (filterContext.HttpContext.Request.Url.AbsoluteUri.IndexOf("https://", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    string path = filterContext.HttpContext.Request.Path;
                    string host = filterContext.HttpContext.Request.Url.Host;

                    if (!HttpPort.Equals("80"))
                    {
                        host = string.Format("{0}:{1}", host, HttpPort);
                    }

                    string url = string.Format("http://{0}{1}", host, path);
                    filterContext.Result = new RedirectResult(url);
                }
            }
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (HttpsEnable.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                if (RequireSecure)
                {
                    if (!filterContext.HttpContext.Request.IsSecureConnection)
                    {
                        SetHttps(filterContext);
                    }
                }
                else
                {
                    if (filterContext.HttpContext.Request.IsSecureConnection)
                    {
                        HandleNonHttpsRequest(filterContext);
                    }
                }
            }
        }

        private static void SetHttps(AuthorizationContext filterContext)
        {
            string path = filterContext.HttpContext.Request.Path;

            string host = filterContext.HttpContext.Request.Url.Host;
            string port = HttpsPort;

            if (!HttpPort.Equals(filterContext.HttpContext.Request.Url.Port.ToString()))
            {
                HttpPort = filterContext.HttpContext.Request.Url.Port.ToString();
            }

            if (!string.IsNullOrEmpty(port))
            {
                host = string.Format("{0}:{1}", host, port);
            }

            if (!host.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
            {
                host = "www." + host;
            }

            filterContext.Result = new RedirectResult(string.Format("https://{0}{1}", host, path));
        }
    }
}