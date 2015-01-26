using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Panasia.Core;
using Panasia.Core.Sys;

namespace Panasia.Core.Web
{
    public class SystemConverters
    {
        [ValueConverter("CreateSystemText", "系统信息")]
        public object CreateSystemText(object value,object parameter)
        {
            if(value==null)
            {
                return "";
            }

            var msg = value.ToString();
            var index = msg.IndexOf('|');
            if(index>0)
            {
                var name = msg.Substring(0, index);
                var format = Sys.LangTexts.Current.GetLangText(name, parameter == null ? "" : parameter.ToString());
                return string.Format(format, msg.Substring(index + 1).Split('|'));
            }
            else
            {
                return Sys.LangTexts.Current.GetLangText(msg, parameter == null ? "" : parameter.ToString());
            }
        }

        [ValueConverter("CreateTreeJson", "Json数据树")]
        public object CreateTreeJson(object value,object parameter)
        {
            if (value == null || parameter == null || (!(value is IEnumerable<IEntityBase>)))
            {
                return "[]";
            }


            var paras = parameter.ToString().ToDictionary('|', ':'); 
            var items = value as IEnumerable<IEntityBase>;
            string rootID = paras.GetDictionaryValue("root", "root");
            string pidName = paras.GetDictionaryValue("pid", "pid");
            string idName = paras.GetDictionaryValue("id", "id");
            string textName = paras.GetDictionaryValue("text", "text");

            return items.CreateJsonTree(rootID, pidName, idName, textName);
        }

        [ValueConverter("DESEncrypt", "DES加密")]
        public object DESEncrypt(object value, object parameter)
        { 
            if(value==null)
            {
                return value;
            }
            return value.ToString().EncryptDES(parameter == null ? "" : parameter.ToString());
        }

        [ValueConverter("DESDecrypt", "DES解密")]
        public object DESDecrypt(object value, object parameter)
        { 
            if(value==null)
            {
                return value;
            }
            return value.ToString().DecryptDES(parameter == null ? "" : parameter.ToString());
        }
    } 
}
