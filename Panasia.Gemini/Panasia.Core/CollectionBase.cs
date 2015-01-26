using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text; 
using System.Runtime.Serialization;
using System.Collections.Specialized; 
using System.Collections;
using System.Diagnostics;

#if SILVERLIGHT
using System.Windows;
#endif

namespace Panasia.Core
{
    public class CollectionBase<T> : System.Collections.ObjectModel.ObservableCollection<T>,
        System.ComponentModel.INotifyPropertyChanged where T : INotifyPropertyChanged, new()
    {
    }
}
