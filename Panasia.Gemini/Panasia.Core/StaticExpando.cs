using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
#if SILVERLIGHT
using System.Windows;
#endif
namespace Panasia.Core
{
    public class StaticExpando : INotifyPropertyChanged, IEntityBase
    {
        private readonly IDictionary<string, object> data = new Dictionary<string, object>();
        private readonly object datalock = new object();

        #region INotifyPropertyChanged Members
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string name)
        {
#if SILVERLIGHT
            if (!Deployment.Current.Dispatcher.CheckAccess())
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    RaisePropertyChanged(name);
                });
                return;
            }
            else if(PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(name));
            }  
#else
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(name));
            } 
#endif
        }
        #endregion

        public virtual IEnumerable<string> GetFieldNames()
        {
            return data.Keys;
        }

        public virtual object[] GetNameValues()
        {
            return data.ToNameValues();
        }

        public void SetValue(string key, object value)
        { 
            if (string.IsNullOrWhiteSpace(key))
                return;

            lock (datalock)
            {
                data[key] = value;
                RaisePropertyChanged(key);
            }
        }

        public object GetValue(string key, object defaultValue = null)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;
            lock (datalock)
            {
                object dataValue;
                if (data.TryGetValue(key, out dataValue))
                {  
                }
                else
                {
                    data[key] = defaultValue;
                    dataValue = defaultValue;
                }
                return dataValue;
            }
        } 
    }
}
