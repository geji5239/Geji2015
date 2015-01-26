//转换功能已经移到基本组件中
/*
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Panasia.Core;

namespace Panasia.Core.Web
{
    public static class ValueConverters
    {
        private static object _LoadLock = new object();
        private static bool _InitDone = false;

        private static Dictionary<string, ValueConverterItem> _Items = new Dictionary<string, ValueConverterItem>();

        static ValueConverters()
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
                        var exports = ServiceLocator.Current.GetContainer().GetExports<Func<object, object,object>, IValueConverterMetadata>(ValueConverterAttribute.ValueConverterContract);
                        exports.Select(a => new ValueConverterItem
                        {
                            TypeName = a.Metadata.TypeName,
                            Title = a.Metadata.Title,
                            Func = a.Value
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
        
        public static object GetConverterValue(string converterName,object value,object parameter=null)
        {
            var func = GetValueConverter(converterName);
            if(func==null)
            {
                return value;
            }
            return func(value,parameter);
        }

        public static Func<object, object, object> GetValueConverter(string name)
        {
            var item = _Items.GetDictionaryValue(name, null);
            return item == null ? null : item.Func;
        }

        public static IEnumerable<ValueConverterItem> GetItems()
        {
            return _Items.Values;
        }
    }

    public class ValueConverterItem
    {
        public string TypeName { get; internal set; }

        public string Title { get; internal set; }

        public Func<object, object, object> Func { get; internal set; }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false), MetadataAttribute]
    public class ValueConverterAttribute : ExportAttribute, IValueConverterMetadata
    {
        internal const string ValueConverterContract = "ValueConverterContract";

        public string TypeName { get; private set; }

        public string Title { get; private set; }

        public ValueConverterAttribute(string typeName, string title) :
            base(ValueConverterContract, typeof(Func<object, object, object>))
        {
            TypeName = typeName;
            Title = title;
        }
    }

    public interface IValueConverterMetadata
    {
        string TypeName { get; }

        string Title { get; }
    }
}
*/