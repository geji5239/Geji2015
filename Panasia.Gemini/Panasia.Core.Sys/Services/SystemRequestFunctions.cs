using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Panasia.Core;

namespace Panasia.Core.Sys
{
    public class SystemRequestFunctions
    {
        /// <summary>
        /// 获取常用描述
        /// </summary>
        /// <param name="request">name参数为第一个，参数paras为字符串格式化的参数，按顺序以|分割</param>
        /// <returns></returns>
        [RequestFunction("GetLangText", "获取常用描述")]
        public string GetLangText(Dictionary<string, string> request)
        {
            string code = request.GetDictionaryValue("name", "");
            string paras = request.GetDictionaryValue("paras", "");

            if (string.IsNullOrEmpty(code))
            {
                throw new Exception("传入参数name不存在或为空");
            }

            return string.IsNullOrEmpty(paras) ? LangTexts.Current.GetLangText(code, "")
                : LangTexts.Current.GetFormatLangText(code, "", paras.Split('|'));
        }

        [RequestFunction("GetSystemCodes", "获取常用编码")]
        public string GetSystemCodes(Dictionary<string, string> request)
        {
            string parentCode = request.GetDictionaryValue("code", "");
            if (string.IsNullOrEmpty(parentCode))
            {
                return "{}";
            }
            return SystemCodes.Current.GetCodes(parentCode).ToJson();
        }

        // /Share/Query?name={sqlname}&para1={para1}...
        // 转换为树： /Share/Query?name={sqlname}&resultformat=jsontree&root=&pid=ParentID&id=DeptID&text=Name&checked=&para1={para1}&para2={para2}...
        [RequestFunction("Query", "获取SQL数据")]
        public string Query(Dictionary<string, string> request)
        {
            string name = request.GetDictionaryValue("name", "");
            var sqlItem = SqlData.Current.GetShare(name);
            if (sqlItem == null)
            {
                throw LangTexts.Current.GetFormatLangText("9002", "请求查询[{0}] 未定义！", name)
                    .CreateException();
            }
            //权限判断
            if ((!string.IsNullOrEmpty(sqlItem.AuthPage)) && sqlItem.AuthValue > 0)
            {
                var actionValue = SysService.GetCurrentUserPageActionValue(sqlItem.AuthPage);
                if ((sqlItem.AuthValue & actionValue) != actionValue)
                {
                    throw LangTexts.Current.GetFormatLangText("1002", "对不起，您没有使用功能[{0}]的权限，谢谢理解！", sqlItem.SharedName)
                        .CreateException();
                }
            }

            List<string> configParas = new List<string>();
            string resultFormat = request.GetDictionaryValue("resultformat", "");

            Func<IEnumerable<Entity>, string> converter = null;

            configParas.Add("name");
            configParas.Add("resultformat");
            if (!string.IsNullOrEmpty(resultFormat))
            {
                configParas.Add("root");
                configParas.Add("pid");
                configParas.Add("id");
                configParas.Add("text");
                configParas.Add("checked");
                switch (resultFormat)
                {
                    case "jsontree":
                        string root = request.GetDictionaryValue("root", "");
                        string pid = request.GetDictionaryValue("pid", "");
                        string id = request.GetDictionaryValue("id", "");
                        string text = request.GetDictionaryValue("text", "");
                        string checkedid = request.GetDictionaryValue("checked", "");
                        converter = (items) =>
                        {
                            return items.CreateJsonTree(root, pid, id, text, checkedid);
                        };
                        break;
                }
            }

            var sqlparas = sqlItem.CommandText.GetParameters().Where(p => char.IsUpper(p[0])).ToList();
            List<object> paras = new List<object>();
            foreach (var item in request.Where(k => !configParas.Contains(k.Key)))
            {
                if (item.Key.Equals("UserID", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                paras.Add(item.Key);
                paras.Add(item.Value);

                sqlparas.Remove(sqlparas.FirstOrDefault(p => p.Equals(item.Key, StringComparison.OrdinalIgnoreCase)));
            }

            if (sqlparas.Count > 0 && sqlparas.FirstOrDefault(p => p.Equals("UserID", StringComparison.OrdinalIgnoreCase)) != null)
            {
                var user = SysService.GetCurrentUser();
                if (user != null)
                {
                    paras.Add("UserID");
                    paras.Add(user.UserID);
                }
            }
            return converter == null ? sqlItem.ExecuteJson(paras.ToArray()) :
                converter(sqlItem.ExecuteQuery<Entity>(paras.ToArray()));
        }
    }
}
