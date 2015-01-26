using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Panasia.Core;

namespace Panasia.Core.Web
{
    public class UIDialog : UIPageBase
    {
        #region 属性 Form
        private UIForm _Form = null;
        [XmlElement("Form")]
        public UIForm Form
        {
            get
            {
                if (_Form == null)
                {
                    _Form = new UIForm();
                    _Form.InitSettings();
                }
                return _Form;
            }
            set
            {
                _Form = value;
                RaisePropertyChanged(() => Form);
            }
        }
        #endregion

        public override void Render(StringBuilder sb, object viewModel, object dataContext = null)
        {
            Form.Render(sb, viewModel, dataContext);
        }
    }

    public class UIForm : UIGrid
    {
        #region 属性 DataBinding
        private string _DataBinding = "";
        [XmlAttribute("DataBinding"), DefaultValue("")]
        public string DataBinding
        {
            get
            {
                return _DataBinding;
            }
            set
            {
                _DataBinding = value;
                RaisePropertyChanged(() => DataBinding);
            }
        }
        #endregion
        
        #region 属性 ErrorMessage
        private UIErrorMessage _ErrorMessage = null;
        [XmlElement]
        public UIErrorMessage ErrorMessage
        {
            get
            {
                return _ErrorMessage;
            }
            set
            {
                _ErrorMessage = value;
                RaisePropertyChanged(() => ErrorMessage);
            }
        }
        #endregion

        #region 属性 ToolBar
        private UIDialogTool _ToolBar = null;
        [XmlElement]
        public UIDialogTool ToolBar
        {
            get
            {
                if (_ToolBar == null)
                {
                    _ToolBar = new UIDialogTool();
                    _ToolBar.InitSettings();
                }
                return _ToolBar;
            }
            set
            {
                _ToolBar = value;
                RaisePropertyChanged(() => ToolBar);
            }
        }
        #endregion

        #region 属性 InnerPanelClass
        private string _InnerPanelClass = "edit-dialog";
        [XmlAttribute("InnerBorderClass"), DefaultValue("edit-dialog")]
        public string InnerPanelClass
        {
            get
            {
                return _InnerPanelClass;
            }
            set
            {
                _InnerPanelClass = value;
                RaisePropertyChanged(() => InnerPanelClass);
            }
        }
        #endregion

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

        public override void InitSettings()
        {
            base.InitSettings();
            ID = "dataform";
            Class = "form-dialog";
        }

        public override void Render(StringBuilder sb, object viewModel, object dataContext = null)
        {
            sb.AppendFormat("<form id=\"{0}\" class=\"{1}\" method=\"post\">", ID, Class);
            if (!string.IsNullOrEmpty(InnerPanelClass))
            {
                sb.AppendFormat(" <div class=\"{0}\">", InnerPanelClass);
            }
            if (!string.IsNullOrEmpty(DataBinding))
            {
                var dataBinding = viewModel.GetParameterValue(dataContext, DataBinding);
                if (dataBinding != null && (dataBinding is IList))
                {
                    var list = dataBinding as IList;
                    dataContext = list.Count > 0 ? list[0] : null;
                }
                else
                {
                    dataContext = dataBinding;
                }
            }

            if (!string.IsNullOrEmpty(EditPanelClass))
            {
                sb.AppendFormat(" <div class=\"{0}\">",EditPanelClass);
            }
            base.Render(sb, viewModel, dataContext);

            var msgField = Fields.LastOrDefault();
            if(msgField.FieldType== UIFieldType.None && msgField.Input.Class.Equals("showmsg"))
            {
            }
            else
            {
                sb.Append("<div class=\"edit-field\">"); 
                sb.AppendFormat("<div class=\"{0}\"><label id=\"{1}\"></label></div>", "showmsg", "showmsg");
                sb.Append("</div>");
                ErrorMessage = null;
            }

            if (!string.IsNullOrEmpty(EditPanelClass))
            {
                sb.Append("</div>"); //div edit-panel
            }
            if (!string.IsNullOrEmpty(InnerPanelClass))
            {
                sb.Append("</div>"); //div edit-dialog
            }
            sb.Append("</form>");
            ToolBar.Render(sb, viewModel, dataContext);
        }
    }

    public class UIIndexPage : UIPageBase
    {
        #region 属性 ToolBar
        private UIToolBar _ToolBar = null;
        [XmlElement("ToolBar")]
        public UIToolBar ToolBar
        {
            get
            {
                if (_ToolBar == null)
                {
                    _ToolBar = new UIToolBar();
                    _ToolBar.InitSettings();
                }
                return _ToolBar;
            }
            set
            {
                _ToolBar = value;
                RaisePropertyChanged(() => ToolBar);
            }
        }
        #endregion

        #region 属性 DataGrid
        private UIDataGrid _DataGrid = null;
        [XmlElement("DataGrid")]
        public UIDataGrid DataGrid
        {
            get
            {
                if (_DataGrid == null)
                {
                    _DataGrid = new UIDataGrid();
                    _DataGrid.InitSettings();
                }
                return _DataGrid;
            }
            set
            {
                _DataGrid = value;
                RaisePropertyChanged(() => DataGrid);
            }
        }
        #endregion

        public override void Render(StringBuilder sb, object viewModel, object dataContext = null)
        {
            sb.AppendFormat("<div data-options=\"region:'north'\" class=\"content-padding\">");
            ToolBar.Render(sb, viewModel, dataContext);
            sb.AppendFormat("</div>");

            sb.Append("<div data-options=\"region:'center'\" class=\"content-padding\">");
            DataGrid.Render(sb, viewModel, dataContext);
            sb.Append("</div>");
        }
    }

    public class UIPageBase : UIElement
    {
        #region 属性 SettingType
        private string _SettingType = string.Empty;
        [XmlAttribute("SettingType"), DefaultValue("")]
        public string SettingType
        {
            get
            {
                return _SettingType;
            }
            set
            {
                _SettingType = value;
                RaisePropertyChanged(() => SettingType);
            }
        }
        #endregion
        
        #region 属性 ContentHeader
        private string _ContentHeader = "";
        [XmlElement("ContentHeader"), DefaultValue("")]
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
        [XmlElement("ContentFooter"), DefaultValue("")]
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

        public string Render(object viewModel, string settingType)
        {
            var setting = UISettings.Current.Items[settingType] as UISetting;
            StringBuilder sb = new StringBuilder();
            if (setting != null)
            {
                sb.Append(setting.ContentHeader);
            }
            if(!string.IsNullOrEmpty(ContentHeader))
            {
                sb.Append(ContentHeader.ReplaceContextParameters(viewModel, viewModel));
            }
            Render(sb, viewModel);
            if(!string.IsNullOrEmpty(ContentFooter))
            {
                sb.Append(ContentFooter.ReplaceContextParameters(viewModel, viewModel));
            }
            if (setting != null)
            {
                sb.Append(setting.ContentFooter);
            }

            return sb.ToString();
        }

        public string Render(object viewModel)
        {
            return Render(viewModel, SettingType);
        }

        public override void Render(StringBuilder sb, object viewModel, object dataContext = null)
        {
            base.Render(sb, viewModel, dataContext);
        }
    }


    public class UIDataGrid : UIElement
    {
        #region 属性 DataBinding
        private string _DataBinding = "";
        [XmlAttribute("DataBinding"), DefaultValue("")]
        public string DataBinding
        {
            get
            {
                return _DataBinding;
            }
            set
            {
                _DataBinding = value;
                RaisePropertyChanged(() => DataBinding);
            }
        }
        #endregion

        #region 属性 Key
        private string _Key = "";
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
        private UIDataColumnCollection _Columns = null;
        [XmlElement("Column", typeof(UIDataColumn))]
        public UIDataColumnCollection Columns
        {
            get
            {
                if (_Columns == null)
                {
                    _Columns = new UIDataColumnCollection();
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

        public override void InitSettings()
        {
            base.InitSettings();
            Tag = "table";
            ID = "datagrid";
            Class = "easyui-datagrid";
            Options = "fitColumns:true,fit:true,pagination: true,rownumbers: true,nowrap: true,striped: true,collapsible: false,resizable:true";
        }

        public override void Render(StringBuilder sb, object viewModel, object dataContext = null)
        {
            AppendStart(sb);
            sb.Append("<thead>");
            sb.Append("<tr>");
            foreach (var col in Columns)
            {
                col.Render(sb, viewModel, dataContext);
            }
            sb.Append("</tr>");
            sb.Append("</thead>");

            if (!string.IsNullOrEmpty(DataBinding))
            {
                var dataBinding = viewModel.GetParameterValue(dataContext, DataBinding);
                if (dataBinding != null && (dataBinding is IList))
                {
                    sb.Append("<tbody>");
                    var list = dataBinding as IList;
                    foreach (var item in list)
                    {
                        sb.Append("<tr>");
                        foreach (var col in Columns)
                        {
                            var value = item.GetPathValue(col.FieldName);
                            sb.AppendFormat("<td>{0}</td>", value == null ? "" : value);
                        }
                        sb.Append("</tr>");
                    }
                    sb.Append("</tbody>");
                }
            }

            sb.AppendTagEnd(Tag);
        }
    }

    public class UIDataColumn : UIElement
    {
        #region 属性 Title
        private string _Title = "";
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
        private string _FieldName = "";
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

        #region 属性 ColumnWidth
        private string _ColumnWidth = "";
        [XmlAttribute("ColumnWidth"), DefaultValue("")]
        public string ColumnWidth
        {
            get
            {
                return _ColumnWidth;
            }
            set
            {
                _ColumnWidth = value;
                RaisePropertyChanged(() => ColumnWidth);
            }
        }
        #endregion
 
        #region 属性 IsCheckBox
        private bool _IsCheckBox = false;
        [XmlAttribute("IsCheckBox"), DefaultValue(false)]
        public bool IsCheckBox
        {
            get
            {
                return _IsCheckBox;
            }
            set
            {
                _IsCheckBox = value;
                RaisePropertyChanged(() => IsCheckBox);
            }
        }
        #endregion

        #region 属性 IsFixed
        private bool _IsFixed = false;
        [XmlAttribute("IsFixed"), DefaultValue(false)]
        public bool IsFixed
        {
            get
            {
                return _IsFixed;
            }
            set
            {
                _IsFixed = value;
                RaisePropertyChanged(() => IsFixed);
            }
        }
        #endregion

        #region 属性 HeaderAlign
        private HAlignment _HeaderAlign = HAlignment.Center;
        [XmlAttribute("HeaderAlign"), DefaultValue(HAlignment.Center)]
        public HAlignment HeaderAlign
        {
            get
            {
                return _HeaderAlign;
            }
            set
            {
                _HeaderAlign = value;
                RaisePropertyChanged(() => HeaderAlign);
            }
        }
        #endregion

        private string GetAlign()
        {
            switch (HAlign)
            {
                case HAlignment.Left:
                    return ",align:'left'";    
                case HAlignment.Right:
                    return ",align:'right'";  
                case HAlignment.Center: 
                default:
                    return ",align:'center'"; 
            }
        }
        private string GetHeaderAlign()
        {
            switch (HeaderAlign)
            {
                case HAlignment.Left:
                    return ",halign:'left'";    
                case HAlignment.Right:
                    return ",halign:'right'";  
                case HAlignment.Center: 
                default:
                    return ",halign:'center'"; 
            }
        }

        public override void Render(StringBuilder sb, object viewModel, object dataContext = null)
        {
            sb.AppendFormat("<th data-options=\"field:'{0}'{2}{3}{4}{5}{6}\">{1}</th>",
                FieldName,
                Title,
                IsCheckBox ? ",checkbox:'true'" : "",
                IsFixed ? ",fixed:true" : ",fixed:false",
                GetAlign()+GetHeaderAlign(),
                string.IsNullOrEmpty(ColumnWidth) ? "" : string.Format(",width:{0}", ColumnWidth),
                string.IsNullOrEmpty(Options) ? "" : Options.StartsWith(",")?Options:(","+Options)
                );
        }
    }

    public class UIDataColumnCollection : CollectionBase<UIDataColumn>
    {

    }

    public class UIErrorMessage : UIElement
    {
        public override void InitSettings()
        {
            base.InitSettings();
            Class = "showMsg";
            ID = "showMsg";
        }

        public override void Render(StringBuilder sb, object viewModel, object dataContext = null)
        {
            sb.AppendFormat("<div class=\"{0}\"><label id=\"{1}\"></label></div>", Class, ID);
        }
    }

    public class TableLayout : EntityBase
    {
        #region 属性 NoSize
        private bool _NoSize = false;
        [XmlAttribute("NoSize"), DefaultValue(false)]
        public bool NoSize
        {
            get
            {
                return _NoSize;
            }
            set
            {
                _NoSize = value;
                RaisePropertyChanged(() => NoSize);
            }
        }
        #endregion

        #region 属性 TableClass
        private string _TableClass = "";
        [XmlAttribute("TableClass"), DefaultValue("")]
        public string TableClass
        {
            get
            {
                return _TableClass;
            }
            set
            {
                _TableClass = value;
                RaisePropertyChanged(() => TableClass);
            }
        }
        #endregion

        #region 属性 Columns
        private string _Columns = "";
        [XmlAttribute("Columns"), DefaultValue("")]
        public string Columns
        {
            get
            {
                return _Columns;
            }
            set
            {
                _Columns = value;
                RaisePropertyChanged(() => Columns);
            }
        }
        #endregion

        #region 属性 RowHeight
        private int _RowHeight = 30;
        [XmlAttribute("RowHeight"), DefaultValue(30)]
        public int RowHeight
        {
            get
            {
                return _RowHeight;
            }
            set
            {
                _RowHeight = value;
                RaisePropertyChanged(() => RowHeight);
            }
        }
        #endregion

        #region 属性 CellPadding
        private int _CellPadding = 0;
        [XmlAttribute("CellPadding"), DefaultValue(0)]
        public int CellPadding
        {
            get
            {
                return _CellPadding;
            }
            set
            {
                _CellPadding = value;
                RaisePropertyChanged(() => CellPadding);
            }
        }
        #endregion

        #region 属性 CellSpacing
        private int _CellSpacing = 0;
        [XmlAttribute("CellSpacing"), DefaultValue(0)]
        public int CellSpacing
        {
            get
            {
                return _CellSpacing;
            }
            set
            {
                _CellSpacing = value;
                RaisePropertyChanged(() => CellSpacing);
            }
        }
        #endregion

        #region 属性 TitleClass
        private string _TitleClass = "";
        [XmlAttribute("TitleClass"), DefaultValue("")]
        public string TitleClass
        {
            get
            {
                return _TitleClass;
            }
            set
            {
                _TitleClass = value;
                RaisePropertyChanged(() => TitleClass);
            }
        }
        #endregion

        #region 属性 ContentClass
        private string _ContentClass = "";
        [XmlAttribute("ContentClass"), DefaultValue("")]
        public string ContentClass
        {
            get
            {
                return _ContentClass;
            }
            set
            {
                _ContentClass = value;
                RaisePropertyChanged(() => ContentClass);
            }
        }
        #endregion
    }

    public class UIGrid : UIElement
    {
        #region TableLayout
        #region 属性 UseTableLayout
        private bool _UseTableLayout = false;
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

        #region 属性 TableLayout
        private TableLayout _TableLayout = null;
        [XmlElement("TableLayout"), DefaultValue(null)]
        public TableLayout TableLayout
        {
            get
            {
                if(_TableLayout==null)
                {
                    _TableLayout = new TableLayout();
                }
                return _TableLayout;
            }
            set
            {
                _TableLayout = value;
                RaisePropertyChanged(() => TableLayout);
            }
        }
        #endregion
        #endregion

        #region 属性 Fields
        private UIFieldCollection _Fields = null;
        [XmlArray("Fields"), XmlArrayItem("Field", typeof(UIField))]
        public UIFieldCollection Fields
        {
            get
            {
                if (_Fields == null)
                {
                    _Fields = new UIFieldCollection();
                }
                return _Fields;
            }
            set
            {
                _Fields = value;
                RaisePropertyChanged(() => Fields);
            }
        }
        #endregion

        public override void InitSettings()
        {
            base.InitSettings();
        }

        public override void Render(StringBuilder sb, object viewModel, object dataContext = null)
        {
            if (UseTableLayout)
            {
                RenderTable(sb, viewModel, dataContext);
            }
            else
            { 
                foreach (var field in Fields)
                {
                    field.Render(sb, viewModel, dataContext);
                }
            }
        }

        #region 生成table排版
        private void RenderTable(StringBuilder sb, object viewModel, object dataContext)
        {
            sb.AppendFormat("<table{0} cellpadding=\"{1}\" cellspacing=\"{2}\" >",
                string.IsNullOrEmpty(TableLayout.TableClass) ? "" : string.Format(" class=\"{0}\"", TableLayout.TableClass),
                TableLayout.CellPadding,TableLayout.CellSpacing);

            var fields = Fields.Where(f => f.FieldType != UIFieldType.Hidden).ToList();
            if (fields.Count > 0)
            {
                var columnWidths = (string.IsNullOrEmpty(TableLayout.Columns) ? (new int[] { 60 }) :
                    (TableLayout.Columns.Split(',').Select(c => Convert.ToInt32(c.Trim())))).ToList();

                var rowCount = fields.Max(f => f.RowIndex + f.RowIndex);
                var columnCount = fields.Max(f => f.ColumnIndex + f.ColumnSpan);
                if (columnCount > columnWidths.Count)
                {
                    for (int i = columnWidths.Count; i < columnCount; i++)
                    {
                        columnWidths.Add(columnWidths[0]);
                    }
                }

                Func<int, int, int> getWidth = (col, span) =>
                {
                    int sum = 0;
                    for (int c = 0; c < span; c++)
                    {
                        sum += columnWidths[col + c];
                    }
                    return sum;
                };

                for (int row = 0; row < rowCount; row++)
                {
                    sb.Append("<tr>");
                    var cells = fields.Where(f => f.RowIndex == row).OrderBy(c => c.ColumnIndex);
                    foreach (var cell in cells)
                    {
                        var titleAdded = false;
                        if (!string.IsNullOrEmpty(cell.Title))
                        {
                            //添加Label
                            sb.AppendFormat("<td{0}{1}>{2}</td>",
                                string.IsNullOrEmpty(TableLayout.TitleClass) ? "" : string.Format(" class=\"{0}\"", TableLayout.TitleClass),
                                TableLayout.NoSize?"":string.Format(" style=\"width:{0}px;height:{1}px\"",
                                    getWidth(cell.ColumnIndex, 1), 
                                    TableLayout.RowHeight),
                                cell.Title);
                            titleAdded = true;
                        }

                        if (cell.FieldType != UIFieldType.None)
                        {
                            //添加输入
                            sb.AppendFormat("<td{0}{1}{2}{3}>",
                                string.IsNullOrEmpty(TableLayout.ContentClass)?"":string.Format(" class=\"{0}\"", TableLayout.ContentClass),
                                cell.ColumnSpan > 2 ? string.Format(" colspan=\"{0}\"", cell.ColumnSpan - 1) : "",
                                cell.RowSpan > 1 ? string.Format(" rowspan=\"{0}\"", cell.RowSpan) : "",
                                TableLayout.NoSize?"":string.Format(" style=\"width:{0}px;height:{1}px\"",
                                    titleAdded ? getWidth(cell.ColumnIndex + 1, cell.ColumnSpan - 1) : getWidth(cell.ColumnIndex, cell.ColumnSpan),
                                    TableLayout.RowHeight * cell.RowSpan));
                            cell.Input.Render(sb, viewModel, dataContext);
                            sb.Append("</td>");
                        }
                    }
                    sb.Append("</tr>");
                }
            }

            sb.Append("</table>");
        }
        #endregion
    }

    public class UIField : UIElement
    {
        public static event EventHandler FieldTypeChanged;

        #region 属性 Title
        private string _Title = "";
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

        #region 属性 TitleClass
        private string _TitleClass = "edit-title";
        [XmlAttribute("TitleClass"), DefaultValue("edit-title")]
        public string TitleClass
        {
            get
            {
                return _TitleClass;
            }
            set
            {
                _TitleClass = value;
                RaisePropertyChanged(() => TitleClass);
            }
        }
        #endregion

        #region 属性 FieldClass
        private string _FieldClass = "edit-field";
        [XmlAttribute("FieldClass"), DefaultValue("edit-field")]
        public string FieldClass
        {
            get
            {
                return _FieldClass;
            }
            set
            {
                _FieldClass = value;
                RaisePropertyChanged(() => FieldClass);
            }
        }
        #endregion

        #region 属性 ContentClass
        private string _ContentClass = "edit-content";
        [XmlAttribute("ContentClass"), DefaultValue("edit-content")]
        public string ContentClass
        {
            get
            {
                return _ContentClass;
            }
            set
            {
                _ContentClass = value;
                RaisePropertyChanged(() => ContentClass);
            }
        }
        #endregion

        #region 属性 Input
        private UIInput _Input = null;
        [XmlElement("TextArea", typeof(UITextArea))]
        [XmlElement("NumericBox", typeof(UINumericBox))]
        [XmlElement("DateTimeBox", typeof(UIDateTimeBox))]
        [XmlElement("DateBox", typeof(UIDateBox))]
        [XmlElement("FileBox", typeof(UIFileBox))]
        [XmlElement("ImageBox", typeof(UIImageBox))]
        [XmlElement("CheckBox", typeof(UICheckBox))]
        [XmlElement("ComboTree", typeof(UIComboTree))]
        [XmlElement("ComboBox", typeof(UIComboBox))]
        [XmlElement("PickBox", typeof(UIPickBox))]
        [XmlElement("Input", typeof(UIInput))]
        public UIInput Input
        {
            get
            {
                if (_Input == null)
                {
                    _Input = InputTypes.Create(FieldType);
                }
                return _Input;
            }
            set
            {
                _Input = value;
                RaisePropertyChanged(() => Input);
            }
        }
        #endregion

        #region 属性 IsHidden
        [XmlIgnore]
        public bool IsHidden
        {
            get
            {
                return Input.IsHidden;
            }
        }
        #endregion

        #region 属性 FieldType
        private UIFieldType _FieldType = UIFieldType.None;
        [XmlAttribute("FieldType"), DefaultValue(typeof(UIFieldType), "None")]
        public UIFieldType FieldType
        {
            get
            {
                return _FieldType;
            }
            set
            {
                _FieldType = value;
                RaisePropertyChanged(() => FieldType);
                if (FieldTypeChanged != null)
                {
                    FieldTypeChanged(this, new EventArgs());
                }

            }
        }
        #endregion

        public override void InitSettings()
        {
            base.InitSettings();
        }

        public void ResetInput(bool copy = true)
        {
            var input = InputTypes.Create(FieldType);
            if (input != null && Input != null && copy)
            {
                Input.CopyTo(input);
            }
            input.InitSettings();
            Input = input;

            if (FieldType == UIFieldType.None)
            {
                input.Class = "showMsg";
                input.ID = "showMsg";
            }
        }
        
        public override void Render(StringBuilder sb, object viewModel, object dataContext = null)
        {
            if (Input.IsHidden)
            {
                Input.Render(sb, viewModel, dataContext);
                return;
            }
            sb.AppendFormat("<div class=\"{0}\">", FieldClass);
            //label
            sb.AppendFormat("<div class=\"{0}\">", TitleClass);
            sb.AppendFormat("<label class=\"edit-label\">{0}：</label>", Title);
            sb.Append("</div>");
            //content

            if (FieldType != UIFieldType.None)
            {
                sb.AppendFormat("<div class=\"{0}\">", ContentClass);
                Input.Render(sb, viewModel, dataContext);
                sb.Append("</div>");
            }

            sb.Append("</div>");
        }
    }

    public class UIFieldCollection : CollectionBase<UIField>
    {

    }

    public class UIToolBar : UIElement
    {
        #region 属性 PageID
        private string _PageID = "";
        [XmlAttribute("PageID"), DefaultValue("")]
        public string PageID
        {
            get
            {
                return _PageID;
            }
            set
            {
                _PageID = value;
                RaisePropertyChanged(() => PageID);
            }
        }
        #endregion

        #region 属性 Buttons
        private UIButtonCollection _Buttons = null;
        public UIButtonCollection Buttons
        {
            get
            {
                if (_Buttons == null)
                {
                    _Buttons = new UIButtonCollection();
                }
                return _Buttons;
            }
            set
            {
                _Buttons = value;
                RaisePropertyChanged(() => Buttons);
            }
        }
        #endregion

        public override void InitSettings()
        {
            base.InitSettings();
            ID = "toolbar";
            Tag = "div";
            Class = "toolbar-panel";
        }

        public override void Render(StringBuilder sb, object viewModel, object dataContext = null)
        {
            AppendStart(sb);

#if Config
            foreach (var button in Buttons)
            {
                RenderButton(sb, dataContext, button);
            }
#else
            int pageValue = UIHelper.GetCurrentUserPageActionValue(PageID);
            foreach (var button in Buttons
                .Where(b => (b.ActionValue == 0
                    || (pageValue & b.ActionValue) == b.ActionValue)))
            {
                RenderButton(sb, dataContext, button);
            }
#endif
            sb.AppendTagEnd(Tag);
        }

        protected virtual void RenderButton(StringBuilder sb, object dataContext, UIButton button)
        {
            sb.Append("<div  class=\"toolbutton\">");
            button.Render(sb, dataContext);
            sb.Append("<div class=\"datagrid-btn-separator\">");
            sb.Append("</div>");
            sb.Append("</div>");
        }
    }

    public class UIDialogTool : UIToolBar
    {
        public override void InitSettings()
        {
            base.InitSettings();
            Tag = "div";
            Class = "dialog-button";
        }

        protected override void RenderButton(StringBuilder sb, object dataContext, UIButton button)
        {
            button.Render(sb, dataContext);
        }
    }

    public class UIButtonCollection : CollectionBase<UIButton>
    {

    }

    public class UIButton : UIElement
    {
        #region 属性 Href
        private string _Href = "";
        [XmlAttribute("Href"), DefaultValue("")]
        public string Href
        {
            get
            {
                return _Href;
            }
            set
            {
                _Href = value;
                RaisePropertyChanged(() => Href);
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

        public override void Render(StringBuilder sb, object viewModel, object dataContext = null)
        {
            if (string.IsNullOrEmpty(Tag))
            {
                Tag = "a";
            }
            sb.AppendTagStart(Tag);
            AppendBaseProperties(sb);
            sb.AppendPropertyIfNotEmpty("href", Href);
            sb.AppendPropertyIfNotEmpty("onclick", Action);
            sb.Append(">");
            sb.Append(Title);
            sb.AppendTagEnd(Tag);
        }
    }

    public abstract class UIElement : EntityBase, IUIRender, IEditableObject
    {
        #region 属性 Tag
        private string _Tag = string.Empty;
        [XmlAttribute("Tag"), DefaultValue("")]
        public string Tag
        {
            get
            {
                return _Tag;
            }
            set
            {
                _Tag = value;
                RaisePropertyChanged(() => Tag);
            }
        }
        #endregion

        #region 属性 ID
        private string _ID = "";
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

        #region 属性 Class
        private string _Class = "";
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
        private string _Style = "";
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
        private string _Options = "";
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
        private UIElement _Parent = null;
        [XmlIgnore()]
        public UIElement Parent
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

        #region 排版
        #region 属性 RowIndex
        private int _RowIndex = 0;
        [XmlAttribute("RowIndex"), DefaultValue(0)]
        public int RowIndex
        {
            get
            {
                return _RowIndex;
            }
            set
            {
                _RowIndex = value;
                RaisePropertyChanged(() => RowIndex);
            }
        }
        #endregion

        #region 属性 ColumnIndex
        private int _ColumnIndex = 0;
        [XmlAttribute("ColumnIndex"), DefaultValue(0)]
        public int ColumnIndex
        {
            get
            {
                return _ColumnIndex;
            }
            set
            {
                _ColumnIndex = value;
                RaisePropertyChanged(() => ColumnIndex);
            }
        }
        #endregion

        #region 属性 RowSpan
        private int _RowSpan = 1;
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
        [XmlAttribute("VAlign"), DefaultValue(typeof(VAlignment), "Center")]
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
        [XmlAttribute("HAlign"), DefaultValue(typeof(HAlignment), "Left")]
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
        #endregion

        #region Ignore
        #region 属性 _IsEditing
        private bool __IsEditing = false;
        [XmlIgnore()]
        public bool _IsEditing
        {
            get
            {
                return __IsEditing;
            }
            private set
            {
                __IsEditing = value;
                RaisePropertyChanged(() => _IsEditing);
            }
        }
        #endregion
        #endregion

        public virtual void Render(StringBuilder sb, object viewModel, object dataContext = null)
        {
            sb.AppendTagStart(Tag);
            AppendBaseProperties(sb);
        }

        protected void AppendStart(StringBuilder sb)
        {
            sb.AppendTagStart(Tag);
            sb.AppendPropertyIfNotEmpty("id", ID);
            sb.AppendPropertyIfNotEmpty("name", Name);
            sb.AppendPropertyIfNotEmpty("class", Class);
            sb.AppendPropertyIfNotEmpty("data-options", Options);
            sb.AppendPropertyIfNotEmpty("style", Style);
            sb.Append(">");
        }

        protected void AppendEnd(StringBuilder sb)
        {
            sb.AppendFormat("</{0}>", Tag);
        }

        protected void AppendBaseProperties(StringBuilder sb)
        {
            sb.AppendPropertyIfNotEmpty("id", ID);
            sb.AppendPropertyIfNotEmpty("name", Name);
            sb.AppendPropertyIfNotEmpty("class", Class);
            sb.AppendPropertyIfNotEmpty("data-options", Options);
            sb.AppendPropertyIfNotEmpty("style", Style);
        }

        public void BeginEdit()
        {
            _IsEditing = true;
        }

        public void CancelEdit()
        {
            _IsEditing = false;
        }

        public void EndEdit()
        {
            _IsEditing = false;
        }

        public virtual void InitSettings()
        {

        }
    }

    public interface IUIRender
    {
        void Render(StringBuilder sb, object viewModel, object dataContext = null);
    }


}
