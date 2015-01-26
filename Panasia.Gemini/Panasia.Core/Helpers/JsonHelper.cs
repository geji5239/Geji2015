using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;

namespace Panasia.Core
{
    public static class JsonHelper
    {
        static JsonHelper()
        {
            _DefaultValues.Add(typeof(string),string.Empty);
            _DefaultValues.Add(typeof(int),0);
            _DefaultValues.Add(typeof(long),0);
            _DefaultValues.Add(typeof(Int16),0);
            _DefaultValues.Add(typeof(double),0);
            _DefaultValues.Add(typeof(DateTime), DateHelper.MinDate); 
        }

        #region 类型默认值
        private static Dictionary<Type, object> _DefaultValues = new Dictionary<Type, object>(); 
        #endregion
         
        public static string ToJson(this object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj); 
        }
        
        public static T ToJsonObject<T>(this string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);  
        }

    }
}
