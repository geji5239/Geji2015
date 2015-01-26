using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Panasia.Core.Commands;

namespace Panasia.Core.Web
{
    public class ActionViewModel
    {
        public Dictionary<string, object> Parameters { get; set; }

        private Func<string, string> _funcGetValue = null;
        private bool _DataInit = false;

        public string getParameterDataValue(string name)
        {
            if (_DataInit)
            {
                if (_funcGetValue == null)
                {
                    return "";
                }
                return _funcGetValue(name);
            }

            _DataInit = true;
            var value = "";
            try
            {
                var data = Parameters == null ? null : Parameters.GetDictionaryValue("Data", null);
                if (data == null)
                {
                    return value;
                }
                
                var items = data as CollectionBase<RowEntity>; 
                if(items!=null)
                {
                    if(items.Count==0)
                    {
                        return value;
                    }
                    value = items[0].GetFieldValue(name, "");
                    _funcGetValue = (n) => items[0].GetFieldValue(n, "");
                    return value;
                }

                var getCount = data.GetType().GetProperties().FirstOrDefault(p => p.Name.Equals("Count"));
                if (getCount == null)
                {
                    return value;
                }
                var count = (int)getCount.GetValue(data);
                if (count == 0)
                {
                    return value;
                }
                dynamic dyData = data;
                if (dyData == null)
                {
                    return value;
                }
                dynamic item = dyData[0];
                if (item == null)
                {
                    return value;
                }

                value = item.GetFieldValue(name, "");
                _funcGetValue = (n) => item.GetFieldValue(n, "");
                return value;
            }
            catch
            {
            }

            return value;
        }
    }

    public class FormActionViewModel : ActionViewModel
    {
        /// <summary>
        /// 当前页面的用户权限值
        /// </summary>
        public int ActionValue { get; set; }

        public Form Form { get; set; } 
    }
}
