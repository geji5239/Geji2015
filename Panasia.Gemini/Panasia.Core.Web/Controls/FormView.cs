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
    [XmlRoot("Form")]
    public class Form:GridBox
    {
    }

    [WebControl("DataView", "数据窗体", false)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DataView : GridBox
    {
    }

    [WebControl("StackPanel", "容器", false)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class StackPanel:Panel
    {
    }

    [WebControl("ToolBar", "工具条", false)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ToolBar:Panel
    {
    }

    [WebControl("GridBox", "表格", false)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class GridBox:Panel
    {
        #region 属性 Title
        private string _Title = string.Empty;
        [Description("Title 标题")]
        [Display(Name = "标题", GroupName = "基本", Order = 2)]	
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

        #region 属性 UseTableLayout
        private bool _UseTableLayout = false;
        [Description("UseTableLayout 是否使用表格布局")]
        [Display(Name = "表格布局", GroupName = "布局")]
        [XmlAttribute("UseTableLayout"), DefaultValue(false)]
        public bool UseTableLayout
        {
            get
            {
                return _UseTableLayout;
            }
            set
            {
                _UseTableLayout = value;
                RaisePropertyChanged(() => UseTableLayout);
            }
        }
        #endregion

        #region 属性 RowCount
        private int _RowCount = 1;
        [Description("RowCount 网格行数")]
        [Display(Name = "网格行数", GroupName = "布局")]	
        [XmlAttribute("RowCount"), DefaultValue(1)]
        public int RowCount
        {
            get
            {
                return _RowCount;
            }
            set
            {
                _RowCount = value;
                RaisePropertyChanged(() => RowCount);
            }
        }
        #endregion

        #region 属性 ColumnCount
        private int _ColumnCount = 1;
        [Description("ColumnCount 网格列数")]
        [Display(Name = "网格列数", GroupName = "布局")]	
        [XmlAttribute("ColumnCount"), DefaultValue(1)]
        public int ColumnCount
        {
            get
            {
                return _ColumnCount;
            }
            set
            {
                _ColumnCount = value;
                RaisePropertyChanged(() => ColumnCount);
            }
        }
        #endregion
    }

    public abstract class Panel : ControlBase
    {
        #region 属性 IsContainer
        private bool _IsContainer = true;
        [Description("IsContainer 是否容器")]
        [Display(Name = "是否容器", GroupName = "布局")]
        [XmlAttribute("IsContainer"), DefaultValue(true)]
        public bool IsContainer
        {
            get
            {
                return _IsContainer;
            }
            set
            {
                _IsContainer = value;
                RaisePropertyChanged(() => IsContainer);
            }
        }
        #endregion

        #region 属性 Binding
        private string _Binding = string.Empty;
        [Description("Binding 绑定")]
        [Display(Name = "绑定", GroupName = "数据")]
        [XmlAttribute("Binding"), DefaultValue("")]
        public string Binding
        {
            get
            {
                return _Binding;
            }
            set
            {
                _Binding = value;
                RaisePropertyChanged(() => Binding);
            }
        }
        #endregion

        #region 属性 Controls
        private ControlCollecttion _Controls = null;
        [Display(AutoGenerateField=false)]
        [XmlArray("Controls")]
        [XmlArrayItem("Label",typeof(Label))]
        [XmlArrayItem("Button",typeof(Button))]
        [XmlArrayItem("TextBox", typeof(TextBox))]
        [XmlArrayItem("CheckBox", typeof(CheckBox))]
        [XmlArrayItem("ComboBox",typeof(ComboBox))]
        [XmlArrayItem("PickBox",typeof(PickBox))]
        [XmlArrayItem("DateBox",typeof(DateBox))]
        [XmlArrayItem("EditBox",typeof(EditBox))]
        [XmlArrayItem("ImageBox",typeof(ImageBox))]
        [XmlArrayItem("DataView",typeof(DataView))]
        [XmlArrayItem("GridView",typeof(GridView))]
        [XmlArrayItem("StackPanel", typeof(StackPanel))]
        [XmlArrayItem("ToolBar", typeof(ToolBar))]
        [XmlArrayItem("GridBox", typeof(GridBox))]
        public ControlCollecttion Controls
        {
            get
            {
                if (_Controls == null)
                {
                    _Controls = new ControlCollecttion();
                    _Controls.CollectionChanged += _Controls_CollectionChanged;
                }
                return _Controls;
            }
            set
            {
                if(_Controls!=null)
                {
                    _Controls.CollectionChanged -= _Controls_CollectionChanged;
                }
                _Controls = value;
                if(_Controls!=null)
                {
                    foreach(var item in _Controls)
                    {
                        item.Parent = this;
                    }
                    _Controls.CollectionChanged += _Controls_CollectionChanged;
                }
                RaisePropertyChanged(() => Controls);
            }
        }

        void _Controls_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach(ControlBase item in e.NewItems)
                    {
                        item.Parent = this;
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach(ControlBase item in e.OldItems)
                    {
                        item.Parent = null;
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }
        #endregion

        public IEnumerable<ControlBase> GetAllChilds()
        {
            //广度优先
            foreach(var item in Controls)
            {
                yield return item;
            }

            foreach(Panel item in Controls.Where(c=>c is Panel))
            {
                foreach(var sitem in item.GetAllChilds())
                {
                    yield return sitem;
                }
            }
        }
    }

    [WebControl("Label", "标签", false)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class Label:ControlBase
    {
        #region 属性 Title
        private string _Title = string.Empty;
        [Description("Title 标题")]
        [Display(Name = "标题", GroupName = "基本", Order=2)]
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

        #region 属性 For
        private string _For = string.Empty;
        [Description("For 关联名称")]
        [Display(Name = "关联名称", GroupName = "基本", Order=3)]
        [XmlAttribute("For"), DefaultValue("")]
        public string For
        {
            get
            {
                return _For;
            }
            set
            {
                _For = value;
                RaisePropertyChanged(() => For);
            }
        }
        #endregion
    }

    [WebControl("Button", "按钮", false)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class Button:ControlBase
    {
        #region 属性 Title
        private string _Title = string.Empty;
        [Description("Title 标题")]
        [Display(Name = "标题", GroupName = "基本", Order = 2)]
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

        #region 属性 Action
        private string _Action = string.Empty;
        [Description("Action 功能")]
        [Display(Name = "功能", GroupName = "基本", Order = 3)]
        [XmlAttribute("Action"), DefaultValue("")]
        public string Action
        {
            get
            {
                return _Action;
            }
            set
            {
                _Action = value;
                RaisePropertyChanged(() => Action);
            }
        }
        #endregion

        #region 属性 ActionValue
        private int _ActionValue = 0;
        [Description("ActionValue 功能值")]
        [Display(Name = "功能值", GroupName = "基本", Order = 4)]
        [XmlAttribute("ActionValue"), DefaultValue(0)]
        public int ActionValue
        {
            get
            {
                return _ActionValue;
            }
            set
            {
                _ActionValue = value;
                RaisePropertyChanged(() => ActionValue);
            }
        }
        #endregion
    }

    public class InputBox : ControlBase
    {
        #region 属性 Binding
        private string _Binding = string.Empty;
        [Description("Binding 绑定")]
        [Display(Name = "绑定", GroupName = "数据", Order = 0)]
        [XmlAttribute("Binding"), DefaultValue("")]
        public string Binding
        {
            get
            {
                return _Binding;
            }
            set
            {
                _Binding = value;
                RaisePropertyChanged(() => Binding);
            }
        }
        #endregion

        #region 属性 IsRequired
        private bool _IsRequired = true;
        [Description("IsRequired 是否必须输入")]
        [Display(Name = "是否必须输入", GroupName = "数据", Order = 1)]
        [XmlAttribute("IsRequired"), DefaultValue(true)]
        public bool IsRequired
        {
            get
            {
                return _IsRequired;
            }
            set
            {
                _IsRequired = value;
                RaisePropertyChanged(() => IsRequired);
            }
        }
        #endregion

        #region 属性 Length
        private int _Length = 0;
        [Description("Length 字符串长度")]
        [Display(Name = "字符串长度", GroupName = "数据", Order = 2)]
        [XmlAttribute("Length"), DefaultValue(0)]
        public int Length
        {
            get
            {
                return _Length;
            }
            set
            {
                _Length = value;
                RaisePropertyChanged(() => Length);
            }
        }
        #endregion

        #region 属性 Validation
        private string _Validation = string.Empty;
        [Description("Validation 验证")]
        [Display(Name = "验证", GroupName = "数据", Order = 3)]
        [XmlAttribute("Validation"), DefaultValue("")]
        public string Validation
        {
            get
            {
                return _Validation;
            }
            set
            {
                _Validation = value;
                RaisePropertyChanged(() => Validation);
            }
        }
        #endregion
        
        #region 属性 IsEnable
        private bool _IsEnable = true;
        [Description("IsEnable 是否有效")]
        [Display(Name = "是否有效", GroupName = "基本", Order = 2)]
        [XmlAttribute("IsEnable"), DefaultValue(true)]
        public bool IsEnable
        {
            get
            {
                return _IsEnable;
            }
            set
            {
                _IsEnable = value;
                RaisePropertyChanged(() => IsEnable);
            }
        }
        #endregion

        #region 属性 IsReadOnly
        private bool _IsReadOnly = false;
        [Description("IsReadOnly 是否只读")]
        [Display(Name = "是否只读", GroupName = "基本", Order = 2)]
        [XmlAttribute("IsReadOnly"), DefaultValue(false)]
        public bool IsReadOnly
        {
            get
            {
                return _IsReadOnly;
            }
            set
            {
                _IsReadOnly = value;
                RaisePropertyChanged(() => IsReadOnly);
            }
        }
        #endregion

        #region 属性 IsHidden
        private bool _IsHidden = false;
        [Description("IsHidden 是否隐藏")]
        [Display(Name = "是否隐藏", GroupName = "基本", Order = 3)]
        [XmlAttribute("IsHidden"), DefaultValue(false)]
        public bool IsHidden
        {
            get
            {
                return _IsHidden;
            }
            set
            {
                _IsHidden = value;
                RaisePropertyChanged(() => IsHidden);
            }
        }
        #endregion
    }

    [WebControl("EditBox", "编辑框", true)]
    [PartCreationPolicy(CreationPolicy.NonShared)] 
    public class EditBox:ControlBase
    {
        #region 属性 Title
        private string _Title = string.Empty;
        [Description("Title 标题")]
        [Display(Name = "标题", GroupName = "基本", Order = 2)]
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

        #region 属性 InputType
        private string _InputType = string.Empty;
        [Description("InputType 输入控件")]
        [Display(Name = "输入控件", GroupName = "基本", Order = 3)]
        [XmlAttribute("InputType"), DefaultValue("")]
        public string InputType
        {
            get
            {
                return _InputType;
            }
            set
            {
                _InputType = value;
                RaisePropertyChanged(() => InputType);
            }
        }
        #endregion
        
        #region 属性 Binding
        private string _Binding = string.Empty;
        [Description("Binding 绑定")]
        [Display(Name = "绑定", GroupName = "数据", Order = 0)]
        [XmlAttribute("Binding"), DefaultValue("")]
        public string Binding
        {
            get
            {
                return _Binding;
            }
            set
            {
                _Binding = value;
                RaisePropertyChanged(() => Binding);
            }
        }
        #endregion

        #region 属性 IsRequired
        private bool _IsRequired = true;
        [Description("IsRequired 是否必须输入")]
        [Display(Name = "是否必须输入", GroupName = "数据", Order = 1)]
        [XmlAttribute("IsRequired"), DefaultValue(true)]
        public bool IsRequired
        {
            get
            {
                return _IsRequired;
            }
            set
            {
                _IsRequired = value;
                RaisePropertyChanged(() => IsRequired);
            }
        }
        #endregion

        #region 属性 Length
        private int _Length = 0;
        [Description("Length 字符串长度")]
        [Display(Name = "字符串长度", GroupName = "数据", Order = 2)]
        [XmlAttribute("Length"), DefaultValue(0)]
        public int Length
        {
            get
            {
                return _Length;
            }
            set
            {
                _Length = value;
                RaisePropertyChanged(() => Length);
            }
        }
        #endregion

        #region 属性 IsReadOnly
        private bool _IsReadOnly = false;
        [Description("IsReadOnly 是否只读")]
        [Display(Name = "是否只读", GroupName = "基本", Order = 2)]
        [XmlAttribute("IsReadOnly"), DefaultValue(false)]
        public bool IsReadOnly
        {
            get
            {
                return _IsReadOnly;
            }
            set
            {
                _IsReadOnly = value;
                RaisePropertyChanged(() => IsReadOnly);
            }
        }
        #endregion

        #region 属性 Validation
        private string _Validation = string.Empty;
        [Description("Validation 验证")]
        [Display(Name = "验证", GroupName = "数据", Order = 3)]
        [XmlAttribute("Validation"), DefaultValue("")]
        public string Validation
        {
            get
            {
                return _Validation;
            }
            set
            {
                _Validation = value;
                RaisePropertyChanged(() => Validation);
            }
        }
        #endregion
        
        #region 属性 IsEnable
        private bool _IsEnable = true;
        [Description("IsEnable 是否有效")]
        [Display(Name = "是否有效", GroupName = "基本", Order = 2)]
        [XmlAttribute("IsEnable"), DefaultValue(true)]
        public bool IsEnable
        {
            get
            {
                return _IsEnable;
            }
            set
            {
                _IsEnable = value;
                RaisePropertyChanged(() => IsEnable);
            }
        }
        #endregion
         
    }
    
    [WebControl("TextBox", "文本框", true)]
    [PartCreationPolicy(CreationPolicy.NonShared)] 
    public class TextBox:InputBox
    {
        #region 属性 IsMutiLine
        private bool _IsMutiLine = false;
        [Description("IsMutiLine 多行文本框")]
        [Display(Name = "是否多行文本框", GroupName = "基本", Order = 4)]
        [XmlAttribute("IsMutiLine"), DefaultValue(false)]
        public bool IsMutiLine
        {
            get
            {
                return _IsMutiLine;
            }
            set
            {
                _IsMutiLine = value;
                RaisePropertyChanged(() => IsMutiLine);
            }
        }
        #endregion
    }

    [WebControl("CheckBox", "复选框", true)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CheckBox:InputBox
    {
    }

    [WebControl("ComboBox", "下拉框", true)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ComboBox:InputBox
    {
        #region 属性 ItemsSource
        private string _ItemsSource = string.Empty;
        [Description("ItemsSource 数据源")]
        [Display(Name = "数据源", GroupName = "数据", Order = 4)]
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

        #region 属性 DisplayMemeber
        private string _DisplayMemeber = string.Empty;
        [Description("DisplayMemeber 显示字段")]
        [Display(Name = "显示字段", GroupName = "数据", Order = 5)]
        [XmlAttribute("DisplayMemeber"), DefaultValue("")]
        public string DisplayMemeber
        {
            get
            {
                return _DisplayMemeber;
            }
            set
            {
                _DisplayMemeber = value;
                RaisePropertyChanged(() => DisplayMemeber);
            }
        }
        #endregion

        #region 属性 ValueMember
        private string _ValueMember = string.Empty;
        [Description("ValueMember 值字段")]
        [Display(Name = "值字段", GroupName = "数据", Order = 6)]
        [XmlAttribute("ValueMember"), DefaultValue("")]
        public string ValueMember
        {
            get
            {
                return _ValueMember;
            }
            set
            {
                _ValueMember = value;
                RaisePropertyChanged(() => ValueMember);
            }
        }
        #endregion

        #region 属性 AllowEmptyValue
        private bool _AllowEmptyValue = true;
        [Description("AllowEmptyValue 是否")]
        [Display(Name = "自动添加空值", GroupName = "数据", Order = 7)]
        [XmlAttribute("AllowEmptyValue"), DefaultValue(true)]
        public bool AllowEmptyValue
        {
            get
            {
                return _AllowEmptyValue;
            }
            set
            {
                _AllowEmptyValue = value;
                RaisePropertyChanged(() => AllowEmptyValue);
            }
        }
        #endregion

        #region 属性 EmptyItemValue
        private string _EmptyItemValue = "";
        [Description("EmptyItemValue 空值")]
        [Display(Name = "空值", GroupName = "数据", Order = 8)]
        [XmlAttribute("EmptyItemValue"), DefaultValue("")]
        public string EmptyItemValue
        {
            get
            {
                return _EmptyItemValue;
            }
            set
            {
                _EmptyItemValue = value;
                RaisePropertyChanged(() => EmptyItemValue);
            }
        }
        #endregion

        #region 属性 EmptyItemDescription
        private string _EmptyItemDescription = "全部";
        [Description("EmptyItemDescription 空值显示")]
        [Display(Name = "空值显示", GroupName = "数据", Order = 9)]
        [XmlAttribute("EmptyItemDescription"), DefaultValue("全部")]
        public string EmptyItemDescription
        {
            get
            {
                return _EmptyItemDescription;
            }
            set
            {
                _EmptyItemDescription = value;
                RaisePropertyChanged(() => EmptyItemDescription);
            }
        }
        #endregion
    }

    [WebControl("PickBox", "搜索", true)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class PickBox:InputBox
    {
        #region 属性 Url
        private string _Url = string.Empty;
        [Description("Url 链接地址")]
        [Display(Name = "链接地址", GroupName = "数据", Order = 4)]
        [XmlAttribute("Url"), DefaultValue("")]
        public string Url
        {
            get
            {
                return _Url;
            }
            set
            {
                _Url = value;
                RaisePropertyChanged(() => Url);
            }
        }
        #endregion
    }


    [WebControl("DateBox", "日期", true)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DateBox:InputBox
    {

    }
    
    
    [WebControl("ImageBox","图片框",true)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ImageBox:InputBox
    {

    }

}
