using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Panasia.Core;

namespace Panasia.Core.Contents
{
    public static class ContentModels
    {
        public const string ContentModelComponents = "ExtensionComponents";
        private static object _LoadLock = new object();
        private static bool _InitDone = false;

        private static Dictionary<string, ContentModelItem> _Items = new Dictionary<string, ContentModelItem>();

        private static Dictionary<string, Type> _ExtensionTypes = new Dictionary<string, Type>();

        static ContentModels()
        {
            Init();
        }
        
        #region 回收
        [DisposeAction(1)]
        public static void Clear()
        {
            lock (_LoadLock)
            {
                _Items.Clear();
                _Items = null;
                _InitDone = false;
            }
        }
        #endregion

        #region 初始化加载
        [InitAction(1)]
        private static void Init()
        {
            if (!_InitDone)
            {
                lock (_LoadLock)
                {
                    if (!_InitDone)
                    {
                        LoadModelTypes();
                        LoadExtensionTypes();
                        _InitDone = true;
                    }
                }
            }
        }

        private static void LoadModelTypes()
        {
            var exports = ServiceLocator.Current.GetContainer().GetExports<Func<string, object>, IContentModelMetadata>(ContentModelAttribute.ContentModelContract);
            exports.Select(a => new ContentModelItem
            {
                TypeName = a.Metadata.TypeName,
                Title = a.Metadata.Title,
                ModelType = a.Metadata.ModelType,
                CreateModel = a.Value
            }).ForEach((a) =>
            {
                _Items[a.TypeName] = a;
            });
        }

        private static void LoadExtensionTypes()
        {
            var extensions = ApplicationConfig.GetAppSettingValue(ContentModelComponents, "")
                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            Func<string, bool> canLoad = (fileName) =>
            {
                if (extensions.Length == 0)
                {
                    return true;
                }
                foreach (var ext in extensions)
                {
                    if (ext.StartsWith("*"))
                    {
                        if (fileName.EndsWith(ext.Substring(1), StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                        continue;
                    }
                    else if (ext.EndsWith("*"))
                    {
                        if (fileName.StartsWith(ext.Substring(0, ext.Length - 1), StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                        continue;
                    }
                    else
                    {
                        if (fileName.Equals(ext, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                        continue;
                    }
                }
                return false;
            };

            AppDomain.CurrentDomain.GetAssemblies()
                .Where(asm => canLoad(asm.FullName.Substring(0, asm.FullName.IndexOf(','))))
                .ForEach(a =>
            {
                a.GetTypes().Where(t => t.FullName.StartsWith("Panasia."))
                    .ForEach(t =>
                    {
                        _ExtensionTypes[t.FullName]= t;
                    });
            });
        }
        #endregion

        public static Type GetModelType(this string typeName)
        {
            if(string.IsNullOrEmpty(typeName))
            {
                return null;
            }

            var item = GetItem(typeName);
            if (item != null)
            {
                return item.ModelType;
            }

            Type itemType = _ExtensionTypes.GetDictionaryValue(typeName, null);
            if (itemType != null)
            {
                return itemType;
            }

            var extItem = _ExtensionTypes.Values.FirstOrDefault(t => t.FullName.EndsWith("." + typeName));
            return extItem;
        }

        public static ContentModelItem GetItem(string name)
        {
            var item = _Items.GetDictionaryValue(name, null);
            return item;
        }

        public static IEnumerable<ContentModelItem> GetItems()
        {
            return _Items.Values;
        }
    }

    public class ContentModelItem
    {
        public string TypeName { get; internal set; }

        public string Title { get; internal set; }

        public Type ModelType { get; internal set; }

        public Func<string, object> CreateModel { get; internal set; }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false), MetadataAttribute]
    public class ContentModelAttribute : ExportAttribute, IContentModelMetadata
    {
        internal const string ContentModelContract = "ContentModelContract";

        public string TypeName { get; private set; }

        public string Title { get; private set; }

        public Type ModelType { get; internal set; }

        public ContentModelAttribute(string typeName, string title, Type type) :
            base(ContentModelContract, typeof(Func<string, object>))
        {
            TypeName = typeName;
            Title = title;
            ModelType = type;
        }
    }

    public interface IContentModelMetadata
    {
        string TypeName { get; }

        string Title { get; }

        Type ModelType { get; }
    }
}
