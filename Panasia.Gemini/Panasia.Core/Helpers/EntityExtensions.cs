using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using System.IO;

namespace Panasia.Core
{
    public static class EntityExtensions
    {
        /// <summary>
        /// 根据键、值更新原实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="newEntity"></param>
        public static void UpdateWith(this IEntityBase entity, IEntityBase newEntity)
        {
            if (newEntity != null)
            {
                var namevalues = newEntity.GetNameValues();
                entity.UpdateWith(namevalues);
            }
        }

        /// <summary>
        /// 根据键、值更新原实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="namevalues"></param>
        public static void UpdateWith(this IEntityBase entity, object[] namevalues)
        {
            if (namevalues != null)
            {
                for (int i = 0; i < namevalues.Length - 1; i += 2)
                {
                    entity.SetValue(namevalues[i].ToString(), namevalues[i + 1]);
                }
            }
        }

        /// <summary>
        /// 更新对象实体列表，如果collection中有相同的元素则执行更新，如果collection中没有相同元素则执行添加
        /// </summary>
        /// <typeparam name="T">对象实体类型</typeparam>
        /// <param name="collection">对象实体列表</param>
        /// <param name="newItems">新的对象实体列表</param>
        /// <param name="getOldItem">从collection中取相同的元素的函数</param>
        public static void UpdateWith<T>(this ICollection<T> collection, IEnumerable<T> newItems, Func<T,T> getOldItem=null) where T:IEntityBase,new()
        {
            foreach (var item in newItems)
            {
                var oldItem = getOldItem(item);
                if (oldItem != null)
                {
                    oldItem.UpdateWith(item);
                }
                else
                {
                    collection.Add(item);
                }
            }
        } 
    }
}
