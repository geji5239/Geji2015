using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Panasia.Core;

namespace Panasia.Core.Web
{
    public class Layout:EntityBase
    {
        #region 属性 TableLayout
        private bool _TableLayout = false;
        [XmlAttribute("TableLayout"), DefaultValue(false)]
        public bool TableLayout
        {
            get
            {
                return _TableLayout;
            }
            set
            {
                _TableLayout = value;
                RaisePropertyChanged(() => TableLayout);
            }
        }
        #endregion
        
        #region 控件排版

        #region 属性 LabelWidth
        private int _LabelWidth = 1;
        [XmlAttribute("LabelWidth"), DefaultValue(1)]
        public int LabelWidth
        {
            get
            {
                return _LabelWidth;
            }
            set
            {
                _LabelWidth = value;
                RaisePropertyChanged(() => LabelWidth);
            }
        }
        #endregion

        #region 属性 BaseWidth
        private int _BaseWidth = 1;
        [XmlAttribute("BaseWidth"), DefaultValue(1)]
        public int BaseWidth
        {
            get
            {
                return _BaseWidth;
            }
            set
            {
                _BaseWidth = value;
                RaisePropertyChanged(() => BaseWidth);
            }
        }
        #endregion

        #region 属性 BaseHeight
        private int _BaseHeight = 1;
        [XmlAttribute("BaseHeight"), DefaultValue(1)]
        public int BaseHeight
        {
            get
            {
                return _BaseHeight;
            }
            set
            {
                _BaseHeight = value;
                RaisePropertyChanged(() => BaseHeight);
            }
        }
        #endregion

        #endregion

        #region 样式

        #region 属性 EditPanelClass
        private string _EditPanelClass = "edit-panel";
        [XmlAttribute("EditPanelClass"), DefaultValue("edit-panel")]
        public string EditPanelClass
        {
            get
            {
                return _EditPanelClass;
            }
            set
            {
                _EditPanelClass = value;
                RaisePropertyChanged(() => EditPanelClass);
            }
        }
        #endregion

        #region 属性 EditFieldClass
        private string _EditFieldClass = "edit-field";
        [XmlAttribute("EditFieldClass"), DefaultValue("")]
        public string EditFieldClass
        {
            get
            {
                return _EditFieldClass;
            }
            set
            {
                _EditFieldClass = value;
                RaisePropertyChanged(() => EditFieldClass);
            }
        }
        #endregion

        #region 属性 EditLabelClass
        private string _EditLabelClass = "edit-title";
        [XmlAttribute("EditLabelClass"), DefaultValue("edit-title")]
        public string EditLabelClass
        {
            get
            {
                return _EditLabelClass;
            }
            set
            {
                _EditLabelClass = value;
                RaisePropertyChanged(() => EditLabelClass);
            }
        }
        #endregion

        #region 属性 EditContentClass
        private string _EditContentClass = "edit-content";
        [XmlAttribute("EditContentClass"), DefaultValue("edit-content")]
        public string EditContentClass
        {
            get
            {
                return _EditContentClass;
            }
            set
            {
                _EditContentClass = value;
                RaisePropertyChanged(() => EditContentClass);
            }
        }
        #endregion

        #region 属性 EditInputClass
        private string _EditInputClass = "edit-input";
        [XmlAttribute("EditInputClass"), DefaultValue("edit-input")]
        public string EditInputClass
        {
            get
            {
                return _EditInputClass;
            }
            set
            {
                _EditInputClass = value;
                RaisePropertyChanged(() => EditInputClass);
            }
        }
        #endregion

        #region 属性 EditToolClass
        private string _EditToolClass = "dialog-button";
        [XmlAttribute("EditToolClass"), DefaultValue("dialog-button")]
        public string EditToolClass
        {
            get
            {
                return _EditToolClass;
            }
            set
            {
                _EditToolClass = value;
                RaisePropertyChanged(() => EditToolClass);
            }
        }
        #endregion

        #region 属性 EditButtonClass
        private string _EditButtonClass = "easyui-linkbutton edit-button";
        [XmlAttribute("EditButtonClass"), DefaultValue("easyui-linkbutton edit-button")]
        public string EditButtonClass
        {
            get
            {
                return _EditButtonClass;
            }
            set
            {
                _EditButtonClass = value;
                RaisePropertyChanged(() => EditButtonClass);
            }
        }
        #endregion

        #endregion
    }

    public class ChildTable:EntityBase
    {
        #region 属性 TableName
        private string _TableName = string.Empty;
        [XmlAttribute("TableName"), DefaultValue("")]
        public string TableName
        {
            get
            {
                return _TableName;
            }
            set
            {
                _TableName = value;
                RaisePropertyChanged(() => TableName);
            }
        }
        #endregion

        #region 属性 TableKey
        private string _TableKey = string.Empty;
        [XmlAttribute("TableKey"), DefaultValue("")]
        public string TableKey
        {
            get
            {
                return _TableKey;
            }
            set
            {
                _TableKey = value;
                RaisePropertyChanged(() => TableKey);
            }
        }
        #endregion

        #region 属性 ReferenceKey
        private string _ReferenceKey = string.Empty;
        [XmlAttribute("ReferenceKey"), DefaultValue("")]
        public string ReferenceKey
        {
            get
            {
                return _ReferenceKey;
            }
            set
            {
                _ReferenceKey = value;
                RaisePropertyChanged(() => ReferenceKey);
            }
        }
        #endregion

        #region 属性 DeleteCheckError
        private string _DeleteCheckError = "存在子表信息，删除取消";
        [XmlAttribute("DeleteCheckError"), DefaultValue("")]
        public string DeleteCheckError
        {
            get
            {
                return _DeleteCheckError;
            }
            set
            {
                _DeleteCheckError = value;
                RaisePropertyChanged(() => DeleteCheckError);
            }
        }
        #endregion
    }

    public class ChildTableCollection:CollectionBase<ChildTable>
    {

    }

    #region 设置项
    public class EditButton : EntityBase
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

        #region 属性 ActionValue
        private int _ActionValue = 0;
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

        #region 属性 Action
        private string _Action = string.Empty;
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

        #region 属性 Class
        private string _Class = string.Empty;
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

        #region 属性 Option
        private string _Option = string.Empty;
        [XmlAttribute("Option"), DefaultValue("")]
        public string Option
        {
            get
            {
                return _Option;
            }
            set
            {
                _Option = value;
                RaisePropertyChanged(() => Option);
            }
        }
        #endregion

        #region 属性 ShowInToolBar
        private bool _ShowInToolBar = true;
        [XmlAttribute("ShowInToolBar"), DefaultValue(true)]
        public bool ShowInToolBar
        {
            get
            {
                return _ShowInToolBar;
            }
            set
            {
                _ShowInToolBar = value;
                RaisePropertyChanged(() => ShowInToolBar);
            }
        }
        #endregion

        public EditButton()
        {

        }

        public EditButton(string id,int actionValue,string title,string action,bool showInToolbar
            ,string controlClass,string controlStyle,string controlOption)
        {
            _ID = id;
            _Title = title;
            _ActionValue = actionValue;
            _Action = action;
            _ShowInToolBar = showInToolbar;
            _Class = controlClass;
            _Style = controlStyle;
            _Option = controlOption;
        }
    }

    public class EditButtonCollection : CollectionBase<EditButton>
    {

    }

    public class EditSource : EntityBase
    {
        private string[] _ForeignTables = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };

        #region 属性 GroupName
        private string _GroupName = string.Empty;
        [XmlAttribute("GroupName"), DefaultValue("")]
        public string GroupName
        {
            get
            {
                return _GroupName;
            }
            set
            {
                _GroupName = value;
                RaisePropertyChanged(() => GroupName);
            }
        }
        #endregion

        #region 属性 TableName
        private string _TableName = string.Empty;
        [XmlAttribute("TableName"), DefaultValue("")]
        public string TableName
        {
            get
            {
                return _TableName;
            }
            set
            {
                _TableName = value;
                RaisePropertyChanged(() => TableName);
            }
        }
        #endregion

        #region 属性 Key
        private string _Key = string.Empty;
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

        #region 属性 LogicDeleteFlag
        private string _LogicDeleteFlag = "";
        [XmlAttribute("LogicDeleteFlag"), DefaultValue("")]
        public string LogicDeleteFlag
        {
            get
            {
                return _LogicDeleteFlag;
            }
            set
            {
                _LogicDeleteFlag = value;
                RaisePropertyChanged(() => LogicDeleteFlag);
            }
        }
        #endregion

        #region 属性 Fields
        private EditFieldCollection _Fields = null;
        public EditFieldCollection Fields
        {
            get
            {
                if (_Fields == null)
                {
                    _Fields = new EditFieldCollection();
                    _Fields.CollectionChanged += _Fields_CollectionChanged;
                }
                return _Fields;
            }
            set
            {
                if (_Fields != null)
                { 
                    _Fields.CollectionChanged -= _Fields_CollectionChanged;
                    foreach(var item in _Fields)
                    {
                        item.EditSource = null;
                    }
                }
                _Fields = value;
                RaisePropertyChanged(() => Fields);
                if (_Fields != null)
                { 
                    _Fields.CollectionChanged += _Fields_CollectionChanged;
                    foreach(var item in _Fields)
                    {
                        item.EditSource = this;
                    }
                }
            }
        }

        void _Fields_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (EditField item in e.NewItems)
                    {
                        item.EditSource = this;
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (EditField item in e.OldItems)
                    {
                        item.EditSource = null;
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

        #region 属性 ChildTables
        private ChildTableCollection _ChildTables = null;
        [XmlArray("ChildTables"),XmlArrayItem("ChildTable",typeof(ChildTable))]
        public ChildTableCollection ChildTables
        {
            get
            {
                if (_ChildTables == null)
                {
                    _ChildTables = new ChildTableCollection();
                }
                return _ChildTables;
            }
            set
            {
                _ChildTables = value;
                RaisePropertyChanged(() => ChildTables);
            }
        }
        #endregion

        public string GetFreignTableShortName(EditField c)
        {
            if(!c.IsForeignColumn)
            {
                return "";
            }

            var targetField = Fields.FirstOrDefault(f => f.Name.Equals(c.TargetForeignKey, StringComparison.OrdinalIgnoreCase));
            if (targetField == null)
            {
                throw new Exception(string.Format("列[{0}]的外键列[{1}]没有找到", c.Name, c.TargetForeignKey));
            } 

            var foreignFields = Fields.Where(f => f.IsForeignColumn).GroupBy(f => f.TargetForeignKey).ToList();
            var g = foreignFields.FirstOrDefault(f => f.Key.Equals(c.TargetForeignKey, StringComparison.OrdinalIgnoreCase));
            var index = foreignFields.IndexOf(g);
            if(index>=_ForeignTables.Length)
            {
                throw new Exception(string.Format("列[{0}]的外键表索引超出[{1}]", c.Name, _ForeignTables.Length));
            }
            var shortTableName = _ForeignTables[index]; 
            return shortTableName;
        }

        public string GetFreignTableShortName(string fieldName)
        {
            var foreignFields = Fields.Where(f => f.IsForeignColumn).GroupBy(f => f.TargetForeignKey).ToList();
            var g = foreignFields.FirstOrDefault(f => f.Key.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
            var index = foreignFields.IndexOf(g);
            if(index>=_ForeignTables.Length)
            {
                throw new Exception(string.Format("列[{0}]的外键表索引超出[{1}]", fieldName, _ForeignTables.Length));
            }
            var shortTableName = _ForeignTables[index]; 
            return shortTableName;
        }
    }

    public class EditField : EntityBase
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

        #region 属性 FieldName
        private string _FieldName = string.Empty;
        /// <summary>
        /// 查询字段名，可以写公式等等
        /// </summary>
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

        #region 属性 DataType
        private string _DataType = string.Empty;
        [XmlAttribute("DataType"), DefaultValue("")]
        public string DataType
        {
            get
            {
                return _DataType;
            }
            set
            {
                _DataType = value;
                RaisePropertyChanged(() => DataType);
            }
        }
        #endregion

        #region 属性 Length
        private int _Length = 0;
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

        #region 属性 ShowInGrid
        private bool _ShowInGrid = true;
        [XmlAttribute("ShowInGrid"), DefaultValue(true)]
        public bool ShowInGrid
        {
            get
            {
                return _ShowInGrid;
            }
            set
            {
                _ShowInGrid = value;
                RaisePropertyChanged(() => ShowInGrid);
            }
        }
        #endregion

        #region 属性 ShowInCreate
        private bool _ShowInCreate = true;
        [XmlAttribute("ShowInCreate"), DefaultValue(true)]
        public bool ShowInCreate
        {
            get
            {
                return _ShowInCreate;
            }
            set
            {
                _ShowInCreate = value;
                RaisePropertyChanged(() => ShowInCreate);
            }
        }
        #endregion

        #region 属性 ShowInEdit
        private bool _ShowInEdit = true;
        [XmlAttribute("ShowInEdit"), DefaultValue(true)]
        public bool ShowInEdit
        {
            get
            {
                return _ShowInEdit;
            }
            set
            {
                _ShowInEdit = value;
                RaisePropertyChanged(() => ShowInEdit);
            }
        }
        #endregion

        #region 属性 ShowInDetail
        private bool _ShowInDetail = true;
        [XmlAttribute("ShowInDetail"), DefaultValue(true)]
        public bool ShowInDetail
        {
            get
            {
                return _ShowInDetail;
            }
            set
            {
                _ShowInDetail = value;
                RaisePropertyChanged(() => ShowInDetail);
            }
        }
        #endregion

        #region 属性 ShowInSearch
        private bool _ShowInSearch = true;
        [XmlAttribute("ShowInSearch"), DefaultValue(true)]
        public bool ShowInSearch
        {
            get
            {
                return _ShowInSearch;
            }
            set
            {
                _ShowInSearch = value;
                RaisePropertyChanged(() => ShowInSearch);
            }
        }
        #endregion

        #region 属性 IsRequired
        private bool _IsRequired = true;
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

        #region 属性 EnableEdit
        private bool _EnableEdit = true;
        /// <summary>
        /// 编辑时可否修改
        /// </summary>
        [XmlAttribute("EnableEdit"), DefaultValue(true)]
        public bool EnableEdit
        {
            get
            {
                return _EnableEdit;
            }
            set
            {
                _EnableEdit = value;
                RaisePropertyChanged(() => EnableEdit);
            }
        }
        #endregion

        #region 属性 EditBox
        private UIFieldType _EditBox = UIFieldType.TextBox;
        [XmlAttribute("EditBox"), DefaultValue(typeof(UIFieldType), "TextBox")]
        public UIFieldType EditBox
        {
            get
            {
                return _EditBox;
            }
            set
            {
                _EditBox = value;
                RaisePropertyChanged(() => EditBox);
            }
        }
        #endregion

        #region 引用列
        #region 属性 IsForeignColumn
        private bool _IsForeignColumn = false;
        [XmlAttribute("IsForeignColumn"), DefaultValue(false)]
        public bool IsForeignColumn
        {
            get
            {
                return _IsForeignColumn;
            }
            set
            {
                _IsForeignColumn = value;
                RaisePropertyChanged(() => IsForeignColumn);
            }
        }
        #endregion

        #region 属性 TargetForeignKey
        private string _TargetForeignKey = string.Empty;
        [XmlAttribute("TargetForeignKey"), DefaultValue("")]
        public string TargetForeignKey
        {
            get
            {
                return _TargetForeignKey;
            }
            set
            {
                _TargetForeignKey = value;
                RaisePropertyChanged(() => TargetForeignKey);
            }
        }
        #endregion

        #region 属性 ForeignColumnName
        private string _ForeignColumnName = string.Empty;
        [XmlAttribute("ForeignColumnName"), DefaultValue("")]
        public string ForeignColumnName
        {
            get
            {
                return _ForeignColumnName;
            }
            set
            {
                _ForeignColumnName = value;
                RaisePropertyChanged(() => ForeignColumnName);
            }
        }
        #endregion
        #endregion

        #region 如果该列是外键列
        #region 属性 IsForeignKey
        private bool _IsForeignKey = false;
        [XmlAttribute("IsForeignKey"), DefaultValue(false)]
        public bool IsForeignKey
        {
            get
            {
                return _IsForeignKey;
            }
            set
            {
                _IsForeignKey = value;
                RaisePropertyChanged(() => IsForeignKey);
            }
        }
        #endregion

        #region 属性 ForeignGroup
        private string _ForeignGroup = string.Empty;
        [XmlAttribute("ForeignGroup"), DefaultValue("")]
        public string ForeignGroup
        {
            get
            {
                return _ForeignGroup;
            }
            set
            {
                _ForeignGroup = value;
                RaisePropertyChanged(() => ForeignGroup);
            }
        }
        #endregion

        #region 属性 ForeignTable
        private string _ForeignTable = string.Empty;
        [XmlAttribute("ForeignTable"), DefaultValue("")]
        public string ForeignTable
        {
            get
            {
                return _ForeignTable;
            }
            set
            {
                _ForeignTable = value;
                RaisePropertyChanged(() => ForeignTable);
            }
        }
        #endregion

        #region 属性 ForeignSqlName
        private string _ForeignSqlName = string.Empty;
        [XmlAttribute("ForeignSqlName"), DefaultValue("")]
        public string ForeignSqlName
        {
            get
            {
                return _ForeignSqlName;
            }
            set
            {
                _ForeignSqlName = value;
                RaisePropertyChanged(() => ForeignSqlName);
            }
        }
        #endregion
        #endregion

        #region 如果编辑控件是下拉框，则要设置数据源
        #region 属性 ItemsSource
        private string _ItemsSource = string.Empty;
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

        #region 属性 DisplayMember
        private string _DisplayMember = string.Empty;
        [XmlAttribute("DisplayMember"), DefaultValue("")]
        public string DisplayMember
        {
            get
            {
                return _DisplayMember;
            }
            set
            {
                _DisplayMember = value;
                RaisePropertyChanged(() => DisplayMember);
            }
        }
        #endregion

        #region 属性 ValueMember
        private string _ValueMember = string.Empty;
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
        #endregion

        #region 查询设置
        #region 属性 ValueType
        private QueryValueType _ValueType = QueryValueType.Char;
        [XmlAttribute("ValueType"), DefaultValue(typeof(QueryValueType), "Char")]
        public QueryValueType ValueType
        {
            get
            {
                return _ValueType;
            }
            set
            {
                _ValueType = value;
                RaisePropertyChanged(() => ValueType);
            }
        }
        #endregion
        #endregion

        #region 系列号
        #region 属性 IsSerialCode
        private bool _IsSerialCode = false;
        [XmlAttribute("IsSerialCode"), DefaultValue(false)]
        public bool IsSerialCode
        {
            get
            {
                return _IsSerialCode;
            }
            set
            {
                _IsSerialCode = value;
                RaisePropertyChanged(() => IsSerialCode);
            }
        }
        #endregion

        #region 属性 SerialRule
        private string _SerialRule = string.Empty;
        [XmlAttribute("SerialRule"), DefaultValue("")]
        public string SerialRule
        {
            get
            {
                return _SerialRule;
            }
            set
            {
                _SerialRule = value; 
                RaisePropertyChanged(() => SerialRule);
            }
        }
        #endregion
        #endregion

        #region 控件排版

        #region 属性 BaseWidth
        private int _BaseWidth = 1;
        [XmlAttribute("BaseWidth"), DefaultValue(1)]
        public int BaseWidth
        {
            get
            {
                return _BaseWidth;
            }
            set
            {
                _BaseWidth = value;
                RaisePropertyChanged(() => BaseWidth);
            }
        }
        #endregion

        #region 属性 BaseHeight
        private int _BaseHeight = 1;
        [XmlAttribute("BaseHeight"), DefaultValue(1)]
        public int BaseHeight
        {
            get
            {
                return _BaseHeight;
            }
            set
            {
                _BaseHeight = value;
                RaisePropertyChanged(() => BaseHeight);
            }
        }
        #endregion

        #endregion

        #region 属性 EditSource
        private EditSource _EditSource = null;
        [XmlIgnore()]
        public EditSource EditSource
        {
            get
            {
                return _EditSource;
            }
            internal set
            {
                _EditSource = value;
                RaisePropertyChanged(() => EditSource);
            }
        }
        #endregion

    }

    public enum QueryValueType
    {
        Char,
        Number,
        Date
    }
    
    public class EditFieldCollection : CollectionBase<EditField>
    {

    }
    #endregion
}
