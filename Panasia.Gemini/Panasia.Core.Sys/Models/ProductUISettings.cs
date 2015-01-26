using Panasia.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Serialization;

namespace Panasia.Core
{
    [XmlRoot("ProductUISettings")]
    public class ProductUISettings:ConfigFile
    {
        private static object _LockLoad = new object();
        private const string SettingFileName = "ProductUISettings.xml";

        #region 属性 Items
        private ProductUIFieldCollection _Items = null;
        [XmlElement("Item",typeof(ProductUIField))]
        public ProductUIFieldCollection Items
        {
            get
            {
                if (_Items == null)
                {
                    _Items = new ProductUIFieldCollection();
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
        private static ProductUISettings _Current = null;
        public static ProductUISettings Current
        {
            get
            {
                if(_Current==null)
                {
                    lock(_LockLoad)
                    {
                        if(_Current ==null)
                        {
                            _Current = Load<ProductUISettings>(SettingFileName);
                        }
                    }
                }
                return _Current;
            } 
        }
        #endregion        

        public string CreateProductHtml(Dictionary<string,string> fields)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var f in fields.Keys)
            {
                var item = Items[f];
                if(item==null)
                {
                    continue;
                }
                sb.Append(item.GetElementHtml(fields[f]));
            }
            return sb.ToString();
        }
    }

    public class ProductUIField:EntityBase
    {
        #region 属性 FieldName
        private string _FieldName = string.Empty;
        [XmlAttribute("FieldName"), DefaultValue("")]
        public string FieldName
        {
            get
            {
                return _FieldName;
            }
            set
            {
                _FieldName = value;
                RaisePropertyChanged(() => FieldName);
            }
        }
        #endregion

        #region 属性 Title
        private string _Title = string.Empty;
        [XmlAttribute("Title"), DefaultValue("")]
        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                _Title = value;
                RaisePropertyChanged(() => Title);
            }
        }
        #endregion

        #region 属性 Html
        private XmlRawItem _Html = "";
        [XmlElement("Html")]
        public XmlRawItem Html
        {
            get
            {
                return _Html;
            }
            set
            {
                _Html = value;
                RaisePropertyChanged(() => Html);
            }
        }
        #endregion

        public string GetElementHtml(string title)
        {
            return "" + Html;
        }
    }

    public class ProductUIFieldCollection:CollectionBase<ProductUIField>
    {
        public ProductUIField this[string name]
        {
            get
            {
                return Items.FirstOrDefault(f => f.FieldName.Equals(name, StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}