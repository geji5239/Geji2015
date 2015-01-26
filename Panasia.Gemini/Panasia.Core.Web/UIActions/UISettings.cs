using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Panasia.Core;

namespace Panasia.Core.Web
{
    [XmlRoot("UISettings")]
    public class UISettings : ConfigFile
    {
        #region 属性 Items
        private UISettingCollection _Items = null;
        [XmlElement("Item", typeof(UISetting))]
        [XmlElement("Select", typeof(UISelectSetting))]
        public UISettingCollection Items
        {
            get
            {
                if (_Items == null)
                {
                    _Items = new UISettingCollection();
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
        private static UISettings _Current = null;
        public static UISettings Current
        {
            get
            {
                if (_Current == null)
                {
                    _Current = Load<UISettings>("UISettings.xml");
                }
                return _Current;
            }
        }
        #endregion

        public void AddSetting(UISettingBase setting)
        {
            var oldItem = Items[setting.TargetType];
            if (oldItem != null)
            {
                Items.Remove(oldItem);
            }
            Items.Add(setting);
            Save();
        }

        public static void Reload()
        {
            _Current = Load<UISettings>("UISettings.xml");
        }
    }

    public class UISettingCollection : CollectionBase<UISettingBase>
    {
        public UISettingBase this[string typeName]
        {
            get
            {
                return Items.FirstOrDefault(u => u.TargetType.Equals(typeName, StringComparison.OrdinalIgnoreCase));
            }
        }
    }

    public class UISelectSetting : UISettingBase
    {
        #region 属性 CanUserInput
        private bool _CanUserInput = true;
        [XmlAttribute("CanUserInput"), DefaultValue(true)]
        public bool CanUserInput
        {
            get
            {
                return _CanUserInput;
            }
            set
            {
                _CanUserInput = value;
                RaisePropertyChanged(() => CanUserInput);
            }
        }
        #endregion

        #region 属性 CanMutiSelect
        private bool _CanMutiSelect = true;
        [XmlAttribute("CanMutiSelect"), DefaultValue(true)]
        public bool CanMutiSelect
        {
            get
            {
                return _CanMutiSelect;
            }
            set
            {
                _CanMutiSelect = value;
                RaisePropertyChanged(() => CanMutiSelect);
            }
        }
        #endregion

        #region 属性 Items
        private CollectionBase<UISettingOption> _Items = null;
        [XmlElement("Option", typeof(UISettingOption))]
        public CollectionBase<UISettingOption> Items
        {
            get
            {
                if (_Items == null)
                {
                    _Items = new CollectionBase<UISettingOption>();
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
    }

    public class UISettingOption : EntityBase
    {
        #region 属性 Name
        private string _Name = "";
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

        #region 属性 Description
        private string _Description = "";
        [XmlAttribute("Description"), DefaultValue("")]
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = value;
                RaisePropertyChanged(() => Description);
            }
        }
        #endregion
    }

    public class UISetting : UISettingBase
    {
        #region 属性 TargetProperties
        private string _TargetProperties = "";
        [XmlAttribute("TargetProperties"), DefaultValue("")]
        public string TargetProperties
        {
            get
            {
                return _TargetProperties;
            }
            set
            {
                _TargetProperties = value;
                RaisePropertyChanged(() => TargetProperties);
            }
        }
        #endregion

        #region 属性 ContentHeader
        private string _ContentHeader = "";
        [XmlElement("ContentHeader")]
        public string ContentHeader
        {
            get
            {
                return _ContentHeader;
            }
            set
            {
                _ContentHeader = value;
                RaisePropertyChanged(() => ContentHeader);
            }
        }
        #endregion

        #region 属性 ContentFooter
        private string _ContentFooter = "";
        [XmlElement("ContentFooter")]
        public string ContentFooter
        {
            get
            {
                return _ContentFooter;
            }
            set
            {
                _ContentFooter = value;
                RaisePropertyChanged(() => ContentFooter);
            }
        }
        #endregion
    }

    public class UISettingBase : EntityBase
    {
        #region 属性 TargetType
        private string _TargetType = "";
        [XmlAttribute("TargetType"), DefaultValue("")]
        public string TargetType
        {
            get
            {
                return _TargetType;
            }
            set
            {
                _TargetType = value;
                RaisePropertyChanged(() => TargetType);
            }
        }
        #endregion

        #region 属性 Description
        private string _Description = "";
        [XmlAttribute("Description"), DefaultValue("")]
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = value;
                RaisePropertyChanged(() => Description);
            }
        }
        #endregion
    }
}
