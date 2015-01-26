using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panasia.Core
{
    public static class ApplicationConfig
    {
        public static string GetAppSettingValue(this string key, string defaultValue)
        {
            if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains(key))
            {
                return System.Configuration.ConfigurationManager.AppSettings[key];
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
