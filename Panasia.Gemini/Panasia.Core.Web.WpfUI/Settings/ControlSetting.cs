using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Panasia.Core;

namespace Panasia.Core.Web.Settings
{
    [XmlRoot("ControlSettings")]
    public class ControlSettings:ConfigFile
    {
        #region 属性 ClassItems
        private SettingCollection _ClassItems = null;
        [XmlArray("Classes"),XmlArrayItem("Item",typeof(SettingItem))]
        public SettingCollection ClassItems
        {
            get
            {
                if(_ClassItems==null)
                {
                    _ClassItems = new SettingCollection();
                }
                return _ClassItems;
            }
            set
            {
                _ClassItems = value;
                RaisePropertyChanged(() => ClassItems);
            }
        }
        #endregion

        #region 属性 StyleItems
        private SettingCollection _StyleItems = null;
        [XmlArray("Styles"), XmlArrayItem("Item", typeof(SettingItem))]
        public SettingCollection StyleItems
        {
            get
            {
                if (_StyleItems == null)
                {
                    _StyleItems = new SettingCollection();
                }
                return _StyleItems;
            }
            set
            {
                _StyleItems = value;
                RaisePropertyChanged(() => StyleItems);
            }
        }
        #endregion
        
        #region 属性 OptionItems
        private SettingCollection _OptionItems = null;
        [XmlArray("Settings"), XmlArrayItem("Item", typeof(SettingItem))]
        public SettingCollection OptionItems
        {
            get
            {
                if (_OptionItems == null)
                {
                    _OptionItems = new SettingCollection();
                }
                return _OptionItems;
            }
            set
            {
                _OptionItems = value;
                RaisePropertyChanged(() => OptionItems);
            }
        }
        #endregion 

        #region 属性 Current
        private static ControlSettings _Current = null;
        public static ControlSettings Current
        {
            get
            {
                if(_Current==null)
                {
                    _Current = Load<ControlSettings>("webcontrolsettings.xml");
                }
                return _Current;
            }
        }
        #endregion
    }

    public class SettingItem:EntityBase
    { 
        #region 属性 Name
        private string _Name = string.Empty;
        [XmlAttribute("Name"), DefaultValue("")]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                RaisePropertyChanged(() => Name);
            }
        }
        #endregion

        #region 属性 Value
        private string _Value = string.Empty;
        [XmlAttribute("Value"), DefaultValue("")]
        public string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
                RaisePropertyChanged(() => Value);
            }
        }
        #endregion

        #region 属性 Targets
        private string _Targets = string.Empty;
        [XmlAttribute("Targets"), DefaultValue("")]
        public string Targets
        {
            get
            {
                return _Targets;
            }
            set
            {
                _Targets = value;
                RaisePropertyChanged(() => Targets);
            }
        }
        #endregion
    }

    public class SettingCollection:CollectionBase<SettingItem>
    {
    }

    public static class SettingExtensions
    {
        public static string GetValues(this IEnumerable<SettingItem> items,string split=",")
        {
            StringBuilder sb=new StringBuilder();
            items.ForEachWithFirst(
                (item) => { sb.Append(item.Value); },
                (item) => { sb.AppendFormat("{0}{1}", split, item.Value); }
                );
            return sb.ToString();
        }
    }
}
