using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Panasia.Core;

namespace Panasia.Core.Web
{
    public static class ContentActions
    {
        private static object _LoadLock = new object();
        private static bool _InitDone = false;

        private static Dictionary<string, ContentActionItem> _Items =
            new Dictionary<string, ContentActionItem>();

        static ContentActions()
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
                        ServiceLocator.Current.GetContainer()
                            .GetExports<Func<Dictionary<string, object>, object>, IContentActionMetadata>
                            (ContentActionAttribute.ContentActionContract)
                            .Select(a => new ContentActionItem
                        {
                            Address = a.Metadata.Address,
                            Title = a.Metadata.Title,
                            ContentType = a.Metadata.ContentType,
                            Encoding =string.IsNullOrEmpty( a.Metadata.Encoding)?Encoding.UTF8:
                                Encoding.GetEncoding(a.Metadata.Encoding),
                            Func = a.Value
                        }).ForEach((a) =>
                        {
                            _Items[a.Address] = a;
                        });
                        _InitDone = true;
                    }
                }
            }
        }
        #endregion

        public static ContentActionItem GetItem(string address)
        {
            return _Items.GetDictionaryValue(address, null);
        }
        public static IEnumerable<ContentActionItem> GetItems()
        {
            return _Items.Values;
        }
    }

    public class ContentActionItem
    {
        public string Address { get; internal set; }

        public string Title { get; internal set; }

        public string ContentType { get; internal set; }

        public Encoding Encoding { get; internal set; }

        public Func<Dictionary<string, object>, object> Func { get; internal set; }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false), MetadataAttribute]
    public class ContentActionAttribute : ExportAttribute, IContentActionMetadata
    {
        internal const string ContentActionContract = "ContentActionContract";

        public string Address { get; private set; }  

        public string Title { get; private set; }

        public string ContentType { get;  private set; }

        public string Encoding { get;  private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="title"></param>
        public ContentActionAttribute(string address, string title) :
            this(address,title,"","")
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address">调用的实际地址，这个名称必须全局唯一</param>
        /// <param name="title">配置工具中的显示名称</param>
        /// <param name="contentType">返回html内容类型，默认text/html</param>
        /// <param name="encoding">返回内容编码，默认是utf-8</param>
        public ContentActionAttribute(string address, string title,string contentType,string encoding) :
            base(ContentActionContract, typeof(Func<Dictionary<string,object>,object>))
        {
            Address = address; 
            Title = title;
            ContentType = string.IsNullOrEmpty(contentType) ? "text/html" : contentType;
            Encoding = string.IsNullOrEmpty(encoding) ? "" :encoding;
        }
    }

    public interface IContentActionMetadata
    {
        string Address { get; }

        string Title { get; }

        string ContentType { get; }

        string Encoding { get; }
    } 
}