using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Panasia.Core;

namespace Panasia.Core.Sys 
{
    public static class RequestFunctions
    {
        private static object _LoadLock = new object();
        private static bool _InitDone = false;

        private static Dictionary<string, RequestFunctionItem> _Items = new Dictionary<string, RequestFunctionItem>();

        static RequestFunctions()
        {
            Init();
        }

        #region 初始化加载
        private static void Init()
        {
            if (!_InitDone)
            {
                lock (_LoadLock)
                {
                    if (!_InitDone)
                    {
                        var items = ServiceLocator.Current.GetContainer()
                            .GetExports<Func<Dictionary<string,string>,string>, IRequestFunctionMetadata>(RequestFunctionAttribute.RequestFunctionContract);
                        foreach(var item in items)
                        {
                            _Items.Add(item.Metadata.FuncName, new RequestFunctionItem
                            {
                                FuncName = item.Metadata.FuncName,
                                Title =item.Metadata.Title,
                                Encoding= item.Metadata.Encoding,
                                ContentType =item.Metadata.ContentType,
                                Func= item.Value 
                            });
                        }
                        _InitDone = true;
                    }
                }
            }
        }
        #endregion

        public static RequestFunctionItem GetItem(string name)
        {
            var item = _Items.GetDictionaryValue(name, null);
            return item; 
        }
         
        public static IEnumerable<RequestFunctionItem> GetItems()
        {
            return _Items.Values;
        }
    }

    public class RequestFunctionItem:IRequestFunctionMetadata
    {
        public string FuncName { get; internal set; }

        public string Title { get; internal set; }

        public string ContentType { get; set; }

        public string Encoding { get; set; }

        public Func<Dictionary<string,string>, string> Func { get; internal set; } 
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false), MetadataAttribute]
    public class RequestFunctionAttribute : ExportAttribute, IRequestFunctionMetadata
    {
        internal const string RequestFunctionContract = "RequestFunctionContract";

        public string FuncName { get; private set; }

        public string Title { get; private set; }

        public string ContentType { get; private set; }

        public string Encoding { get; private set; }

        public RequestFunctionAttribute(string funcName, string title,string contentType="application/json",string encoding="") :
            base(RequestFunctionContract, typeof(Func<Dictionary<string,string>,string>))
        {
            FuncName = funcName;
            Title = title;
            ContentType = contentType;
            Encoding = encoding;
        }
    }

    public interface IRequestFunctionMetadata
    {
        string FuncName { get; }

        string Title { get; }

        string ContentType { get; }

        string Encoding { get; }
    }
}
