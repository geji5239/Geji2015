using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Panasia.Core.Sys;

namespace Panasia.Core.Auth
{
    public class ShareController : AuthController
    {
        public ActionResult Data(string funcName)
        {
            var sysfun = RequestFunctions.GetItem(funcName);
            if(sysfun==null)
            {
                throw LangTexts.Current.GetFormatLangText("9001", "请求系统方法[{0}] 未定义！", funcName).CreateException();
            }

            var paras = new Dictionary<string, string>();
            var type = Request.QueryString["type"];
            var encoding = Request.QueryString["encoding"];

            foreach(var q in Request.QueryString.AllKeys
                .Where(k=> !(k.Equals("encoding", StringComparison.OrdinalIgnoreCase) 
                    || k.Equals("type",StringComparison.OrdinalIgnoreCase))))
            {
                paras.Add(q,Request.QueryString[q]);
            }

            var result = new ContentResult
            {
                Content = sysfun.Func(paras),
                ContentEncoding = string.IsNullOrEmpty(encoding)?
                    string.IsNullOrEmpty(sysfun.Encoding) ? Encoding.UTF8 : Encoding.GetEncoding(sysfun.Encoding):
                    Encoding.GetEncoding(encoding),
                ContentType = string.IsNullOrEmpty(type)?
                    string.IsNullOrEmpty(sysfun.ContentType) ? "applicaton/json" : sysfun.ContentType:
                    type.Equals("html", StringComparison.OrdinalIgnoreCase)?"text/html":
                    type.Equals("json" , StringComparison.OrdinalIgnoreCase)?"application/json":
                    "appliation/json"
            };
            return result;
        }
    }
}