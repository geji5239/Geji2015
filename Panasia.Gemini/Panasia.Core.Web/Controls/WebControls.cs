using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Panasia.Core;

namespace Panasia.Core.Web
{
    public static class WebControls
    {
        private static object _LoadLock = new object();
        private static bool _InitDone = false;

        private static Dictionary<string, WebControlItem> _Items = new Dictionary<string, WebControlItem>();

        static WebControls()
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
                        var exports = ServiceLocator.Current.GetContainer().GetExports<ControlBase, IWebControlMetadata>(WebControlAttribute.WebControlContract);
                        exports.Select(a => new WebControlItem
                        {
                            TypeName = a.Metadata.TypeName,
                            Title = a.Metadata.Title 
                        }).ForEach((a) =>
                        {
                            _Items[a.TypeName] = a;
                        });
                        _InitDone = true;
                    }
                }
            }
        }
        #endregion
        
        public static ControlBase GetWebControl(string name)
        {
            var item =ServiceLocator.Current.GetContainer().GetExports<ControlBase, IWebControlMetadata>(WebControlAttribute.WebControlContract).FirstOrDefault(f=>f.Metadata.TypeName.Equals(name));
            return item == null ? null : item.Value; 
        }

        public static IEnumerable<WebControlItem> GetItems()
        {
            return _Items.Values;
        }
    }

    public class WebControlItem
    {
        public string TypeName { get; internal set; }

        public string Title { get; internal set; }

        public bool IsEdit { get; internal set; } 
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false), MetadataAttribute]
    public class WebControlAttribute : ExportAttribute, IWebControlMetadata
    {
        internal const string WebControlContract = "WebControlContract";

        public string TypeName { get; private set; }

        public string Title { get; private set; }

        public bool IsEdit { get; private set; }

        public WebControlAttribute(string typeName, string title,bool isEdit=false) :
            base(WebControlContract, typeof(ControlBase))
        {
            TypeName = typeName;
            Title = title;
            IsEdit = IsEdit;
        }
    }

    public interface IWebControlMetadata
    {
        string TypeName { get; }

        string Title { get; }

        bool IsEdit { get; }
    }
}
