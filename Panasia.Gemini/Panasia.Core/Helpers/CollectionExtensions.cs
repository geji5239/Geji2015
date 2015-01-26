using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Panasia.Core
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// 递归取数据，深度优先
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="fnRecurse"></param>
        /// <returns></returns>
        public static IEnumerable<T> Traverse<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> fnRecurse)
        {
            foreach (T item in source)
            {
                yield return item;
                IEnumerable<T> seqRecurse = fnRecurse(item);

                if (seqRecurse != null)
                {
                    foreach (T itemRecurse in Traverse(seqRecurse, fnRecurse))
                    {
                        yield return itemRecurse;
                    }
                }
            }
        }
        /// <summary>
        /// 转换成ObservableCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            ObservableCollection<T> list = new ObservableCollection<T>();
            foreach (T item in source)
                list.Add(item);
            return list;
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source != null)
            {
                foreach (var item in source)
                {
                    action(item);
                }
            }
        }

        public static void ForEachWithFirst<T>(this IEnumerable<T> source, Action<T> firstAction, Action<T> action)
        {
            if (source != null)
            {
                bool first = true;
                foreach (var item in source)
                {
                    if (first)
                    {
                        firstAction(item);
                        first = false;
                    }
                    else
                    {
                        action(item);
                    }
                }
            }
        }

        public static void AddToCollection<T>(this IEnumerable<T> source, ICollection<T> target)
        {
            foreach (var item in source)
            {
                target.Add(item);
            }
        }

        /// <summary>
        /// 更新列表，新集合中有原集合中没有则增加，新集合中没有原集合中有则删除，新集合和原集合中都有则更新（用update更新函数）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">原列表</param>
        /// <param name="newList">新列表</param>
        /// <param name="compare">列表元素比较</param>
        /// <param name="update">更新列表元素的方法</param>
        public static void UpdateWithMerge<T>(this ICollection<T> list, IEnumerable<T> newList
            , Func<T, T, bool> compare, Action<T, T> update)
        {
            var adds = newList.Where(w => list.FirstOrDefault(t => compare(t, w)) == null).ToList();
            var removes = list.Where(w => newList.FirstOrDefault(t => compare(t, w)) == null).ToList();

            foreach (var t in removes)
            {
                list.Remove(t);
            }

            if (update != null)
            {
                foreach (var t in list)
                {
                    var newItem = newList.FirstOrDefault(w => compare(t, w));
                    if (newItem == null)
                    {
                        continue;//当前逻辑不可能，因为先执行了删除
                    }
                    update(t, newItem);
                }
            }

            foreach (var t in adds)
            {
                list.Add(t);
            }
        }

        public static Dictionary<string, object> ToDictionary(this object[] nameValues, bool keyToLower = false)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (nameValues == null || nameValues.Length == 0)
            {
                return dic;
            }
            for (int i = 0; i < nameValues.Length - 1; i += 2)
            {
                dic.Add(keyToLower ? nameValues[i].ToString().ToLower() : nameValues[i].ToString(), nameValues[i + 1]);
            }
            return dic;
        }

        public static object[] ToNameValues(this IDictionary<string, object> dic)
        {
            object[] nameValues = new object[dic.Count * 2];
            int i = 0;
            foreach (var item in dic)
            {
                nameValues[i] = item.Key;
                nameValues[i + 1] = item.Value;
                i = i + 2;
            }

            return nameValues;
        }

        public static Dictionary<string, string> ToDictionary(this string keyValues, char itemSplit = '|', char split = ':')
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            if (keyValues.IsNullOrEmpty())
            {
                return dic;
            }
            foreach (var item in keyValues.Split(itemSplit))
            {
                string[] kv = item.Split(split);
                if (kv.Length > 1)
                {
                    dic.Add(kv[0], kv[1]);
                }
            }
            return dic;
        }

        public static TValue GetStringDictionaryValue<TValue>(this Dictionary<string, TValue> dic, string key, TValue defaultValue)
        {
            foreach (var item in dic)
            {
                if (item.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    return item.Value;
                }
            }
            return defaultValue;
        }

        public static TValue GetDictionaryValue<Tkey, TValue>(this Dictionary<Tkey, TValue> dic, Tkey key, TValue defaultValue)
        {
            if (dic.ContainsKey(key))
            {
                return dic[key];
            }
            return defaultValue;
        }
    }
}
