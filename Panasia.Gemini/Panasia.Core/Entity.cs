using System;
using System.Collections.Generic;

namespace Panasia.Core
{
    /// <summary>
    ///     数据实体对象定义
    /// </summary>
    public class Entity : StaticExpando
    {
        public virtual T GetFieldValue<T>(string name, T defaultValue = default(T))
        {
            object value = GetValue(name);

            if (value != null && value is T)
            {
                return (T) value;
            }
            if (defaultValue == null)
            {
                return defaultValue;
            }

            if (value == null)
            {
                return defaultValue;
            }

            var newValue = value.GetConverterValue<T>();

            SetValue(name, newValue);

            return newValue;
        }

        public virtual void SetFieldValue(string name, object value)
        {
            SetValue(name, value);
        }

        /// <summary>
        ///     更新整个函数
        /// </summary>
        /// <param name="newVlaue"></param>
        public virtual void SetValue(Entity newVlaue)
        {
            foreach (string name in newVlaue.GetFieldNames())
            {
                object value = newVlaue.GetValue(name);
                if (!GetValue(name).Equals(value))
                {
                    SetValue(name, value);
                }
            }
        }

    }
    
    public static class EntityExtends
    {
        private static readonly Dictionary<Type, Func<object, object>> ValueConverters =
            new Dictionary<Type, Func<object, object>>();

        static EntityExtends()
        {
            AddConverter(typeof (bool),
                         value => value != null && ((value is bool) ? (bool) value : Convert.ToBoolean(value)));
            AddConverter(typeof (decimal),
                         value => value == null
                                        ? 0
                                        : ((value is decimal) ? (decimal) value : Convert.ToDecimal(value)));
            AddConverter(typeof (double),
                         value => value == null
                                      ? 0
                                      : ((value is double) ? (double) value : Convert.ToDouble(value)));
            AddConverter(typeof (float),
                         value => value == null ? 0 : ((value is float) ? (float) value : Convert.ToSingle(value)));
            AddConverter(typeof (int),
                         value => value == null ? 0 : ((value is int) ? (int) value : Convert.ToInt32(value)));
            AddConverter(typeof (long),
                         value => value == null ? 0 : ((value is long) ? (long) value : Convert.ToInt64(value)));
            AddConverter(typeof (uint),
                         value => value == null ? 0 : ((value is uint) ? (uint) value : Convert.ToUInt32(value)));
            AddConverter(typeof (ulong),
                         value => value == null ? 0 : ((value is ulong) ? (ulong) value : Convert.ToUInt64(value)));
            AddConverter(typeof (DateTime),
                         value => value == null
                                      ? DateHelper.MinDate
                                      : ((value is DateTime) ? (DateTime) value : Convert.ToDateTime(value)));
            AddConverter(typeof (string),
                         value => value == null
                                      ? string.Empty
                                      : ((value is string) ? (string) value : value.ToString()));
        }

        private static byte[] GetBytes(object value)
        {
            if (value == null)
            {
                return new byte[0];
            }
            if (value is byte[])
            {
                return (byte[]) value;
            }
            if (value is string)
            {
                return Convert.FromBase64String(value.ToString());
            }
            if (value is Guid)
            {
                return ((Guid) value).ToByteArray();
            }
            return null;
        }

        public static void AddConverter(this Type type, Func<object, object> converter)
        {
            ValueConverters[type] = converter;
        }

        public static T GetConverterValue<T>(this object value)
        {
            Func<object, object> converter = null;
            if (value is T)
            {
                return (T) value;
            }
            if (ValueConverters.TryGetValue(typeof (T), out converter))
            {
                return (T) converter(value);
            } 

            if (typeof (T).IsEnum)
            {
                return (value is int) ? (T) value : (T) Enum.Parse(typeof (T), value.ToString(), true);
            }

            if (value == null && typeof (T).IsValueType == false)
            {
                return default(T);
            }
            else
            {
                throw new ArgumentOutOfRangeException("value",
                   string.Format("值{0}与类型{1}不匹配", value, typeof (T).FullName));
            }
        }
    }
}