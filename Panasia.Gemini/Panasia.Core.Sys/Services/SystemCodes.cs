using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Panasia.Core;
using Newtonsoft.Json; 

namespace Panasia.Core.Sys
{
    [XmlRoot("SystemCodes")]
    public class SystemCodes : ConfigFile
    {
        private static Dictionary<string, SystemCodeCollection> _Codes = new Dictionary<string, SystemCodeCollection>();

        #region 属性 Items
        private SystemCodeCollection _Items = null;
        [XmlElement("Item", typeof(SystemCodeItem))]
        public SystemCodeCollection Items
        {
            get
            {
                if (_Items == null)
                {
                    _Items = new SystemCodeCollection();
                }
                return _Items;
            }
            set
            {
                _Items = value;
                RaisePropertyChanged(() => Items);
            }
        }
        #endregion

        #region 属性 Current
        private static object _LoadLock = new object();
        private static SystemCodes _Current = null;
        public static SystemCodes Current
        {
            get
            {
                if (_Current == null)
                {
                    Load();
                }
                return _Current;
            }
        }

        private static void Load()
        {
            lock (_LoadLock)
            {
                if (_Current == null)
                {
                    _Codes = new Dictionary<string, SystemCodeCollection>();
                    Reload();
                }
            }
        }

        #endregion

        #region 回收
        [DisposeAction(1)]
        public static void Clear()
        {
            lock (_LoadLock)
            {
                if (_Current != null)
                {
                    _Current.Items.Clear();
                    _Codes = null;
                    _Current = null;
                }
            }
        }
        #endregion

        public static void Reload()
        {
            _Current = Load<SystemCodes>("SystemCodes.xml");
            if (_Current == null)
            {
                _Current = new SystemCodes();
            }
            else
            {
                ResetCodes();
            }
        }

        public static void ResetCodes()
        {
            lock (_Codes)
            {
                _Codes.Clear();
                _Current.Items.ForEach(d =>
                {
                    _Codes[d.Code] = d.Items;
                });
            }
        }

        public SystemCodeCollection GetCodes(string code)
        {
            return _Codes.GetDictionaryValue(code, null);
        }
    }

    public class SystemCodeCollection : CollectionBase<SystemCodeItem>
    {
        public SystemCodeItem this[string codeOrName]
        {
            get
            {
                return Items.FirstOrDefault(c => 
                    c.Code.Equals(codeOrName, StringComparison.OrdinalIgnoreCase)
                    || c.Name.Equals(codeOrName, StringComparison.OrdinalIgnoreCase));
            }
        }
    }

    public class SystemCodeItem : Entity
    {
        #region property Code
        [XmlAttribute("Code"), DefaultValue("")]
        public string Code
        {
            get
            {
                return GetFieldValue<string>("Code", "");
            }
            set
            {
                SetFieldValue("Code", value);
            }
        }
        #endregion

        #region property Name
        [XmlAttribute("Name"), DefaultValue("")]
        public string Name
        {
            get
            {
                return GetFieldValue<string>("Name", "");
            }
            set
            {
                SetFieldValue("Name", value);
            }
        }
        #endregion

        #region property Remark
        [XmlAttribute("Description"), DefaultValue("")]
        [JsonIgnore]
        public string Remark
        {
            get
            {
                return GetFieldValue<string>("Remark", "");
            }
            set
            {
                SetFieldValue("Remark", value);
            }
        }
        #endregion
        
        #region 属性 IsGroup
        [XmlIgnore()]
        [JsonIgnore()]
        public bool IsGroup
        {
            get
            {
                return Items != null && Items.Count > 0;
            }
        }
        #endregion

        #region 属性 Items
        private SystemCodeCollection _Items = null;
        [XmlElement("Item", typeof(SystemCodeItem))]
        public SystemCodeCollection Items
        {
            get
            {
                if (_Items == null)
                {
                    _Items = new SystemCodeCollection();
                    _Items.CollectionChanged += _Items_CollectionChanged;
                }
                return _Items;
            }
            set
            {
                if (_Items != null)
                {
                    _Items.CollectionChanged -= _Items_CollectionChanged;
                }
                _Items = value;
                if (_Items != null)
                {
                    _Items.CollectionChanged += _Items_CollectionChanged;
                }
                RaisePropertyChanged("Items");
                RaisePropertyChanged("IsGroup");
            }
        }

        void _Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("IsGroup");
        }
        #endregion
    }


}
