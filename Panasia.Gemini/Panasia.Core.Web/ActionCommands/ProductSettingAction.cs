using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panasia.Core.Web
{
    public class ProductSettingActions
    {
        [ContentAction("GetProductSetting","取产品界面配置")]
        public object GetProductSettingUI(Dictionary<string,object> paras)
        {
            dynamic rows = paras.GetDictionaryValue("Fields",null);
            if(rows==null)
            {
                throw new Exception("必须有个参数传入该产品需要的列集合");
            }
            Dictionary<string, string> fields = new Dictionary<string, string>();
            foreach(dynamic row in rows)
            {
                fields.Add(row.Name, row.Title); //确定一下从数据库中取来的列名是不是对应
            }

            var content = ProductUISettings.Current.CreateProductHtml(fields);
            return content;
        }
    }
}
