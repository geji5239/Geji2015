using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization; 

namespace Panasia.Core
{ 
    public static class LangTextsExtension
    { 
        public static string GetLangText(this string name,string defaultText)
        {
            return  LangTexts.Current.GetLangText(name.ToLower(), defaultText);
        }

    }

    [XmlRoot("LangTexts")]
    public class LangTexts:ConfigFile
    {
        private Dictionary<string, string> _Texts = new Dictionary<string, string>();

        #region 属性 Items
        private CollectionBase<LangTextItem> _Items = null;
        [XmlElement("Item", typeof(LangTextItem))]
        public CollectionBase<LangTextItem> Items
        {
            get
            {
                if (_Items == null)
                {
                    _Items = new CollectionBase<LangTextItem>();
                }
                return _Items;
            }
            set
            {
                _Items = value;
                RaisePropertyChanged("Items");
            }
        }
        #endregion  

        #region 属性 Current
        private static object _LoadLock = new object();
        private static LangTexts _Current = null; 
        public static LangTexts Current
        {
            get
            {
                if(_Current==null)
                {
                    lock(_LoadLock)
                    {
                        if(_Current==null)
                        {
                            _Current = Load<LangTexts>("LangTexts.xml");
                            if(_Current ==null)
                            {
                                _Current = new LangTexts();
                            }
                            else
                            {
                                _Current.Items.ForEach(d =>
                                {
                                    _Current._Texts[d.Name.ToLower()] = d.Text;
                                    _Current._Texts[d.ID.ToLower()] = d.Text;
                                });
                            }
                        }
                    }
                }
                return _Current;
            } 
        }
        #endregion

        public string GetLangText(string name,string defaultText)
        {
            return _Texts.GetDictionaryValue(name.ToLower(), defaultText);
        }
    }

    public class LangTextItem:EntityBase
    {
        #region 属性 ID
        private string _ID = string.Empty;
        [XmlAttribute("ID"), DefaultValue("")]
        public string ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
                RaisePropertyChanged(() => ID);
            }
        }
        #endregion

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

        #region 属性 Text
        private string _Text = string.Empty;
        [XmlAttribute("Text"), DefaultValue("")]
        public string Text
        {
            get
            {
                return _Text;
            }
            set
            {
                _Text = value;
                RaisePropertyChanged(() => Text);
            }
        }
        #endregion

        #region 属性 Description
        private string _Description = string.Empty;
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

        #region 属性 Category
        private string _Category = "";
        [XmlAttribute("Category"), DefaultValue("")]
        public string Category
        {
            get
            {
                return _Category;
            }
            set
            {
                _Category = value;
                RaisePropertyChanged(() => Category);
            }
        }
        #endregion
    }
}
