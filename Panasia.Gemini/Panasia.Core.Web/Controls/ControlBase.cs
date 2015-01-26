using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Panasia.Core;

namespace Panasia.Core.Web
{
    public class ControlBase:EntityBase
    {
        #region 属性 ID
        private string _ID = string.Empty;
        [Description("ID 控件标识")]
        [Display(Name = "ID(标识)", GroupName = "基本", Order = 0)]	
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
        [Description("Name 控件名称")]
        [Display(Name = "名称", GroupName = "基本", Order=1)]	
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
                
        #region 属性 Row
        private int _Row = 0;
        [Description("Row 行序号")]
        [Display(Name = "行序号", GroupName = "布局")]	
        [XmlAttribute("Row"), DefaultValue(0)]
        public int Row
        {
            get
            {
                return _Row;
            }
            set
            {
                _Row = value;
                RaisePropertyChanged(() => Row);
            }
        }
        #endregion

        #region 属性 Column
        private int _Column = 0;
        [Description("Column 列序号")]
        [Display(Name = "列序号", GroupName = "布局")]	
        [XmlAttribute("Column"), DefaultValue(0)]
        public int Column
        {
            get
            {
                return _Column;
            }
            set
            {
                _Column = value;
                RaisePropertyChanged(() => Column);
            }
        }
        #endregion

        #region 属性 RowSpan
        private int _RowSpan = 1;
        [Description("RowSpan 行数")]
        [Display(Name = "行数", GroupName = "布局")]	
        [XmlAttribute("RowSpan"), DefaultValue(1)]
        public int RowSpan
        {
            get
            {
                return _RowSpan;
            }
            set
            {
                _RowSpan = value;
                RaisePropertyChanged(() => RowSpan);
            }
        }
        #endregion

        #region 属性 ColumnSpan
        private int _ColumnSpan = 1;
        [Description("ColumnSpan 列数")]
        [Display(Name = "列数", GroupName = "布局")]	
        [XmlAttribute("ColumnSpan"), DefaultValue(1)]
        public int ColumnSpan
        {
            get
            {
                return _ColumnSpan;
            }
            set
            {
                _ColumnSpan = value;
                RaisePropertyChanged(() => ColumnSpan);
            }
        }
        #endregion

        #region 属性 VAlign
        private VAlignment _VAlign = VAlignment.Center;
        [Description("VAlign 竖直对齐")]
        [Display(Name = "竖直对齐", GroupName = "布局")]	
        [XmlAttribute("VAlign"), DefaultValue(typeof(VAlignment),"Center")]
        public VAlignment VAlign
        {
            get
            {
                return _VAlign;
            }
            set
            {
                _VAlign = value;
                RaisePropertyChanged(() => VAlign);
            }
        }
        #endregion

        #region 属性 HAlign
        private HAlignment _HAlign = HAlignment.Left;
        [Description("HAlign 水平对齐")]
        [Display(Name = "水平对齐", GroupName = "布局")]	
        [XmlAttribute("HAlign"), DefaultValue(typeof(HAlignment),"Left")]
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

        #region 属性 Parent
        private object _Parent = null;
        [Display(AutoGenerateField=false)]
        [XmlIgnore()]
        public object Parent
        {
            get
            {
                return _Parent;
            }
            internal set
            {
                _Parent = value;
                RaisePropertyChanged(() => Parent);
            }
        }
        #endregion
         
    }

    public class ControlAttribute : EntityBase
    {
        #region 属性 Name
        private string _Name = string.Empty;
        [Description("Name 属性名称")]
        [Display(Name = "名称", GroupName = "基本")]	
        [XmlAttribute("Name")]
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
        [Description("Value 属性值")]
        [Display(Name = "值", GroupName = "基本")]	
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
    }

    public class ControlAttributeCollection : CollectionBase<ControlAttribute>
    {

    }

    public enum VAlignment
    {
        Top,
        Center,
        Bottom
    }

    public enum HAlignment
    {
        Left,
        Center,
        Right
    }
}
