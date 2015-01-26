using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Panasia.Core;

namespace Panasia.Core.Web
{
    #region GridView
    [WebControl("DataView", "数据网格", false)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class GridView : ControlBase
    {
        #region 属性 Title
        private string _Title = string.Empty;
        [Description("Title 标题")]
        [Display(Name = "标题", GroupName = "基本")]	
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

        #region 属性 Key
        private string _Key = string.Empty;
        [Description("Key 主键")]
        [Display(Name = "主键", GroupName = "基本")]
        [XmlAttribute("Key"), DefaultValue("")] 
        public string Key
        {
            get
            {
                return _Key;
            }
            set
            {
                _Key = value;
                RaisePropertyChanged(() => Key);
            }
        }
        #endregion

        #region 属性 Columns
        private GridColumnCollection _Columns = null;
        [Description("Columns 列")]
        [Display(Name = "Columns", GroupName = "基本")]	
        [XmlArray("Columns"), XmlArrayItem("Column", typeof(GridColumn))]
        public GridColumnCollection Columns
        {
            get
            {
                if (_Columns == null)
                {
                    _Columns = new GridColumnCollection();
                }
                return _Columns;
            }
            set
            {
                _Columns = value;
                RaisePropertyChanged(() => Columns);
            }
        }
        #endregion

        #region 属性 ItemsSource
        private string _ItemsSource = string.Empty;
        [Description("ItemsSource 数据源")]
        [Display(Name = "数据源", GroupName = "数据")]
        [XmlAttribute("ItemsSource"), DefaultValue("")]
        public string ItemsSource
        {
            get
            {
                return _ItemsSource;
            }
            set
            {
                _ItemsSource = value;
                RaisePropertyChanged(() => ItemsSource);
            }
        }
        #endregion

    }

    public class GridColumn : EntityBase
    {
        #region 属性 Name
        private string _Name = string.Empty;
        [Description("Name 列名")]
        [Display(Name = "名称", GroupName = "基本")]
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

        #region 属性 Title
        private string _Title = string.Empty;
        [Description("Title 列标题")]
        [Display(Name = "列标题", GroupName = "基本")]
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

        #region 属性 FieldName
        private string _FieldName = string.Empty;
        [Description("FieldName 字段名")]
        [Display(Name = "字段名", GroupName = "基本")]
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

        #region 属性 Width
        private int _Width = 0;
        [Description("Width 宽度")]
        [Display(Name = "宽度", GroupName = "布局")]
        [XmlAttribute("Width"), DefaultValue(0)]
        public int Width
        {
            get
            {
                return _Width;
            }
            set
            {
                _Width = value;
                RaisePropertyChanged(() => Width);
            }
        }
        #endregion

        #region 属性 HAlign
        private HAlignment _HAlign = HAlignment.Left;
        [Description("HAlign 水平对齐")]
        [Display(Name = "水平对齐", GroupName = "布局")]
        [XmlAttribute("HAlignment"), DefaultValue(typeof(HAlignment), "Left")]
        public HAlignment HAlign
        {
            get
            {
                return _HAlign;
            }
            set
            {
                _HAlign = value;
                RaisePropertyChanged(() => HAlign);
            }
        }
        #endregion
        
        #region 属性 Class
        private string _Class = string.Empty;
        [Description("Class 类别")]
        [Display(Name = "类别", GroupName = "样式")]
        [XmlAttribute("Class"), DefaultValue("")]
        public string Class
        {
            get
            {
                return _Class;
            }
            set
            {
                _Class = value;
                RaisePropertyChanged(() => Class);
            }
        }
        #endregion

        #region 属性 Style
        private string _Style = string.Empty;
        [Description("Style 样式")]
        [Display(Name = "样式", GroupName = "样式")]
        [XmlAttribute("Style"), DefaultValue("")]
        public string Style
        {
            get
            {
                return _Style;
            }
            set
            {
                _Style = value;
                RaisePropertyChanged(() => Style);
            }
        }
        #endregion

        #region 属性 Options
        private string _Options = string.Empty;
        [Description("Options 其它设置")]
        [Display(Name = "其它设置", GroupName = "样式")]
        [XmlAttribute("Options"), DefaultValue("")]
        public string Options
        {
            get
            {
                return _Options;
            }
            set
            {
                _Options = value;
                RaisePropertyChanged(() => Options);
            }
        }
        #endregion         

        public override string ToString()
        {
            return string.Format("{0} [{1}]", Title, FieldName); 
        }
    }

    public class GridColumnCollection : CollectionBase<GridColumn>
    {
        public override string ToString()
        {
            return Items.Count == 0 ? "" :
                Items.Count == 1 ? Items[0].Title :
                Items.Select(t=>t.Title).Aggregate((c1, c2) => c1 + "," + c2);
        }
    }
    #endregion

}
