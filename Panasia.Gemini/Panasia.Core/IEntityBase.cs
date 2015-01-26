using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace Panasia.Core
{
    public interface IEntityBase : INotifyPropertyChanged
    {
        //取得动态扩展属性的值
        object GetValue(string key, object defaultValue = null); 
        //设置动态扩展属性的值
        void SetValue(string key, object value);
        //取所有（动态）属性名称
        IEnumerable<string> GetFieldNames();
        //取所有（动态）属性的名称、值系列
        object[] GetNameValues();
    }
}
