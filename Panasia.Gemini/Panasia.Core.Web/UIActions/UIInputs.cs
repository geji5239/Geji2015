using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Panasia.Core;

namespace Panasia.Core.Web
{
    #region Input
    public class UIInput : UIElement
    {
        #region 属性 InputType
        private string _InputType = string.Empty;
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
        private string _Binding = "";
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

        #region 属性 IsEnable
        private bool _IsEnable = true;
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

        #region 属性 IsHidden
        private bool _IsHidden = false;
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
                if (_IsHidden)
                {
                    InputType = "hidden";
                }
            }
        }
        #endregion

        #region 属性 ValidType
        private string _ValidType = "";
        [XmlAttribute("ValidType"), DefaultValue("")]
        public string ValidType
        {
            get
            {
                return _ValidType;
            }
            set
            {
                _ValidType = value;
                RaisePropertyChanged(() => ValidType);
            }
        }
        #endregion

        #region 属性 Prompt
        private string _Prompt = "";
        [XmlAttribute("Prompt"), DefaultValue("")]
        public string Prompt
        {
            get
            {
                return _Prompt;
            }
            set
            {
                _Prompt = value;
                RaisePropertyChanged(() => Prompt);
            }
        }
        #endregion

        public override void InitSettings()
        {
            base.InitSettings();
            Tag = "input";
            Class = "edit-input"; 
        }

        public override void Render(StringBuilder sb, object viewModel, object dataContext = null)
        {
            if (IsHidden)
            {
                RenderHidden(sb, viewModel, dataContext);
            }
            else
            {
                RenderDefault(sb, viewModel, dataContext);
            }
        }

        protected virtual string GetOptions()
        {
            StringBuilder sb=new StringBuilder(); 

            Action<bool, string> addOption = (condition, content) =>
                {
                    if (condition)
                    {
                        if(sb.Length>0)
                        {
                            sb.Append(",");
                        }
                        sb.Append(content);
                    }
                };

            addOption(IsRequired, "required:true");
            addOption(!string.IsNullOrEmpty(ValidType), string.Format("validType:'{0}'", ValidType));
            addOption(!string.IsNullOrEmpty(Prompt), string.Format("prompt:'{0}'", Prompt));
            addOption(!string.IsNullOrEmpty(Options), Options);

            return sb.ToString();
        }

        #region textbox
        private void RenderDefault(StringBuilder sb, object viewModel, object dataContext)
        {
            sb.AppendTagStart(Tag);

            sb.AppendPropertyIfNotEmpty("id", ID);
            sb.AppendPropertyIfNotEmpty("name", Name);
            sb.AppendPropertyIfNotEmpty("class", Class);

            if (IsReadOnly) { sb.Append(" readonly=\"readonly\""); }
            if (!IsEnable) { sb.Append(" disabled=\"disabled\""); }

            sb.AppendPropertyIfNotEmpty("data-options", GetOptions().ReplaceContextParameters(viewModel,dataContext));
            sb.AppendPropertyIfNotEmpty("style", Style);

            sb.AppendPropertyIfNotEmpty("type", InputType);

            var value = viewModel.GetParameterValue(dataContext, Binding);
            sb.AppendPropertyIfNotEmpty("value", value);
            sb.AppendTagEnd(Tag);
        }
        #endregion

        #region hidden
        private void RenderHidden(StringBuilder sb, object viewModel, object dataContext)
        {
            sb.AppendTagStart(Tag);
            AppendBaseProperties(sb);
            sb.Append("type=\"hidden\"");
            var value = viewModel.GetParameterValue(dataContext, Binding);
            sb.AppendPropertyIfNotEmpty("value", value);

            sb.AppendTagEnd(Tag);
        }
        #endregion

        #region 拷贝基本属性
        public virtual void CopyTo(UIInput input)
        {
            input.ID = ID;
            input.Name = Name;
            input.Class = Class;
            input.Style = Style;
            input.Options = Options;
            input.InputType = InputType;
            input.Binding = Binding;
            input.IsEnable = IsEnable;
            input.IsReadOnly = IsReadOnly;
            input.IsRequired = IsRequired;
            input.Length = Length;
            input.IsHidden = IsHidden;
            input.ValidType = ValidType;
            input.Prompt = Prompt;
        }
        #endregion
    }
    #endregion

    #region 下拉框
    public class UIComboBox : UIInput
    {
        public override void InitSettings()
        {
            base.InitSettings();
            Tag = "select";
            Class = "easyui-combobox edit-input";
        }

        #region ComboBox
        #region 属性 ItemsSource
        private string _ItemsSource = "";
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
        private string _DisplayMember = "";
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
        private string _ValueMember = "";
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

        #region Url数据
        #region 属性 DataUrl
        private string _DataUrl = "";
        [XmlAttribute("DataUrl"), DefaultValue("")]
        public string DataUrl
        {
            get
            {
                return _DataUrl;
            }
            set
            {
                _DataUrl = value;
                RaisePropertyChanged(() => DataUrl);
                Tag = string.IsNullOrEmpty(_DataUrl) ? "select" : "input";
            }
        }
        #endregion

        #region 属性 IsAutoHeight
        private bool _IsAutoHeight = true;
        [XmlAttribute("IsAutoHeight"), DefaultValue(true)]
        public bool IsAutoHeight
        {
            get
            {
                return _IsAutoHeight;
            }
            set
            {
                _IsAutoHeight = value;
                RaisePropertyChanged(() => IsAutoHeight);
            }
        }
        #endregion

        #endregion

        #region 选择事件

        #region 属性 OnSelect
        private string _OnSelect = "";
        [XmlAttribute("OnSelect"), DefaultValue("")]
        public string OnSelect
        {
            get
            {
                return _OnSelect;
            }
            set
            {
                _OnSelect = value;
                RaisePropertyChanged(() => OnSelect);
            }
        }
        #endregion

        #endregion

        #region 生成HTML
        public override void Render(StringBuilder sb, object viewModel, object dataContext = null)
        {
            sb.AppendTagStart(Tag);

            sb.AppendPropertyIfNotEmpty("id", ID);
            sb.AppendPropertyIfNotEmpty("name", Name);
            sb.AppendPropertyIfNotEmpty("class", Class);
            if (IsReadOnly) { sb.Append(" readonly=\"readonly\""); }
            if (!IsEnable) { sb.Append(" disabled=\"disabled\""); }
            sb.AppendPropertyIfNotEmpty("data-options", GetOptions().ReplaceContextParameters(viewModel, dataContext));
            sb.AppendPropertyIfNotEmpty("style", Style);

            sb.AppendPropertyIfNotEmpty("type", InputType);

            var value = viewModel.GetParameterValue(dataContext, Binding);


            if(!string.IsNullOrEmpty(DataUrl))
            {
                sb.AppendPropertyIfNotEmpty("value", value);
                sb.Append(">");
            }
            else if (!string.IsNullOrEmpty(ItemsSource))
            {
                sb.Append(">");
                #region 处理绑定数据源
                var itemsSource = (string.IsNullOrEmpty(DisplayMember) && string.IsNullOrEmpty(ValueMember)) ? ItemsSource :
                    viewModel.GetParameterValue(dataContext, ItemsSource);
                if (itemsSource != null)
                {
                    if (itemsSource is string)
                    {
                        foreach (var item in (itemsSource as string).ToDictionary())
                        {
                            sb.AppendFormat("<option value=\"{0}\"{2}>{1}</option>", item.Key, item.Value
                                , (value != null && value.Equals(item.Key)) ? " selected" : "");
                        }
                    }
                    else if (itemsSource is System.Collections.IEnumerable)
                    {
                        foreach (var item in (itemsSource as System.Collections.IEnumerable))
                        {
                            var pValue = item.GetPathValue(ValueMember);
                            sb.AppendFormat("<option value=\"{0}\"{2}>{1}</option>",
                                item.GetPathValue(ValueMember), item.GetPathValue(DisplayMember)
                                , (value != null && value.Equals(pValue)) ? " selected" : "");
                        }
                    }
                    else
                    {
                    }
                }
                #endregion
            }
            else
            {
                sb.Append(">");
            }

            sb.AppendTagEnd(Tag);
        }
        #endregion

        protected override string GetOptions()
        {

            string options = base.GetOptions();

            StringBuilder sb = new StringBuilder();
            sb.Append(options);

            Action<string, string> addOption = (value, format) =>
            {
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }
                if (sb.Length > 0)
                {
                    sb.Append(",");
                }
                sb.AppendFormat(format, value);
            };
            addOption(DataUrl, "url:'{0}',method:'get'");
            addOption(ValueMember, "valueField:'{0}'");
            addOption(DisplayMember, "textField:'{0}'");
            if (IsAutoHeight)
            {
                sb.Append(sb.Length == 0 ? "panelHeight:'auto'" : ",panelHeight:'auto'");
            }
            addOption(OnSelect, "onSelect:{0}");
            return sb.ToString(); 
        }

        public override void CopyTo(UIInput input)
        {
            base.CopyTo(input);

            if (input is UIComboBox)
            {
                var comboBox = input as UIComboBox;
                ItemsSource = comboBox.ItemsSource;
                DisplayMember = comboBox.DisplayMember;
                ValueMember = comboBox.ValueMember;
                DataUrl = comboBox.DataUrl;
                OnSelect = comboBox.OnSelect;
            }
        }
    }
    #endregion

    public class UIComboTree:UIInput
    {
        public override void InitSettings()
        {
            base.InitSettings();
            Tag = "input";
            Class = "easyui-combotree";
        }

        #region Url数据
        #region 属性 DataUrl
        private string _DataUrl = "";
        [XmlAttribute("DataUrl"), DefaultValue("")]
        public string DataUrl
        {
            get
            {
                return _DataUrl;
            }
            set
            {
                _DataUrl = value;
                RaisePropertyChanged(() => DataUrl);
            }
        }
        #endregion

        #region 属性 DisplayMember
        private string _DisplayMember = "";
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
        private string _ValueMember = "";
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

        #region 属性 ChildrenMember
        private string _ChildrenMember = "";
        [XmlAttribute("ChildrenMember"), DefaultValue("")]
        public string ChildrenMember
        {
            get
            {
                return _ChildrenMember;
            }
            set
            {
                _ChildrenMember = value;
                RaisePropertyChanged(() => ChildrenMember);
            }
        }
        #endregion

        #region 属性 IconMember
        private string _IconMember = "";
        [XmlAttribute("IconMember"), DefaultValue("")]
        public string IconMember
        {
            get
            {
                return _IconMember;
            }
            set
            {
                _IconMember = value;
                RaisePropertyChanged(() => IconMember);
            }
        }
        #endregion
        
        #region 属性 IsAutoHeight
        private bool _IsAutoHeight = true;
        [XmlAttribute("IsAutoHeight"), DefaultValue(true)]
        public bool IsAutoHeight
        {
            get
            {
                return _IsAutoHeight;
            }
            set
            {
                _IsAutoHeight = value;
                RaisePropertyChanged(() => IsAutoHeight);
            }
        }
        #endregion
        #endregion

        #region 选择事件

        #region 属性 OnSelect
        private string _OnSelect = "";
        [XmlAttribute("OnSelect"), DefaultValue("")]
        public string OnSelect
        {
            get
            {
                return _OnSelect;
            }
            set
            {
                _OnSelect = value;
                RaisePropertyChanged(() => OnSelect);
            }
        }
        #endregion

        #endregion
        protected override string GetOptions()
        {
            string options = base.GetOptions();

            StringBuilder sb = new StringBuilder();
            sb.Append(options);

            Action<string, string> addOption = (value, format) =>
            {
                if(string.IsNullOrEmpty(value))
                {
                    return;
                }
                if(sb.Length>0)
                {
                    sb.Append(",");
                }
                sb.AppendFormat(format, value);
            };
            addOption(DataUrl, "url:'{0}',method:'get'");
            addOption(ValueMember, "idField:'{0}'");
            addOption(DisplayMember, "textField:'{0}'");
            addOption(ChildrenMember, "childrenField:'{0}'");
            addOption(IconMember, "iconField:'{0}'");
            if(IsAutoHeight)
            {
                sb.Append(sb.Length == 0 ? "panelHeight:'auto'" : ",panelHeight:'auto'");
            }
            addOption(OnSelect, "onSelect:{0}");
            return sb.ToString();
        }
    }

    #region 搜选框
    public class UIPickBox : UIInput
    {
        #region 属性 KeyID
        private string _KeyID = "";
        [XmlAttribute("KeyID"), DefaultValue("")]
        public string KeyID
        {
            get
            {
                return _KeyID;
            }
            set
            {
                _KeyID = value;
                RaisePropertyChanged(() => KeyID);
            }
        }
        #endregion

        #region 属性 KeyBinding
        private string _KeyBinding = "";
        [XmlAttribute("KeyBinding"), DefaultValue("")]
        public string KeyBinding
        {
            get
            {
                return _KeyBinding;
            }
            set
            {
                _KeyBinding = value;
                RaisePropertyChanged(() => KeyBinding);
            }
        }
        #endregion

        #region 属性 SearchName
        private string _SearchName = "doSearch";
        [XmlAttribute("SearchName"), DefaultValue("doSearch")]
        public string SearchName
        {
            get
            {
                return _SearchName;
            }
            set
            {
                _SearchName = value;
                RaisePropertyChanged(() => SearchName);
            }
        }
        #endregion

        #region 属性 SearchAction
        private string _SearchAction = "";
        [XmlAttribute("SearchAction"), DefaultValue("")]
        public string SearchAction
        {
            get
            {
                return _SearchAction;
            }
            set
            {
                _SearchAction = value;
                RaisePropertyChanged(() => SearchAction);
            }
        }
        #endregion

        public override void InitSettings()
        {
            base.InitSettings();
            Tag = "input";
            Class = "easyui-searchbox";
        }
        public override void Render(StringBuilder sb, object viewModel, object dataContext = null)
        {
            sb.AppendTagStart(Tag);

            sb.AppendPropertyIfNotEmpty("id", ID);
            sb.AppendPropertyIfNotEmpty("name", Name);
            sb.AppendPropertyIfNotEmpty("class", Class);
            if (IsReadOnly) { sb.Append(" readonly=\"readonly\""); }
            if (!IsEnable) { sb.Append(" disabled=\"disabled\""); }
            sb.AppendPropertyIfNotEmpty("data-options", GetOptions().ReplaceContextParameters(viewModel, dataContext));
            sb.AppendPropertyIfNotEmpty("style", Style);

            sb.AppendPropertyIfNotEmpty("type", InputType);

            var value = viewModel.GetParameterValue(dataContext, Binding);
            sb.AppendPropertyIfNotEmpty("value", value);
            sb.AppendTagEnd(Tag);

            //添加隐藏ID 
            sb.Append("<input ");
            sb.AppendPropertyIfNotEmpty("id", KeyID);            
            sb.AppendFormat(" type=\"hidden\" name=\"{0}\"", KeyBinding);

            var keyvalue = viewModel.GetParameterValue(dataContext, Binding);
            sb.AppendPropertyIfNotEmpty("value", keyvalue);
            sb.Append("/>");

            sb.AppendFormat(@"
 <script>
        function {0}(value){{
            {1};
        }}
    </script>
",SearchName,SearchAction);
        }

        protected override string GetOptions()
        {
            string options = base.GetOptions();
            return string.IsNullOrEmpty(options) ? "searcher:"+SearchName 
                :options + ",searcher:"+SearchName;
        }
    }
    #endregion

    public class UITextArea:UIInput
    {
        public override void InitSettings()
        {
            base.InitSettings();
            Tag = "input";
        }

        protected override string GetOptions()
        {
            string options = base.GetOptions(); 
            return string.IsNullOrEmpty(options)?"multiline:true":"multiline:true,"+options;
        }
    }

    public class UICheckBox:UIInput
    {
        public override void InitSettings()
        {
            base.InitSettings();
            InputType = "checkbox";
        }

        public override void Render(StringBuilder sb, object viewModel, object dataContext = null)
        {
            sb.AppendTagStart(Tag);

            sb.AppendPropertyIfNotEmpty("id", ID);
            sb.AppendPropertyIfNotEmpty("name", Name);
            sb.AppendPropertyIfNotEmpty("class", Class);

            if (IsReadOnly) { sb.Append(" readonly=\"readonly\""); }
            if (!IsEnable) { sb.Append(" disabled=\"disabled\""); }

            sb.AppendPropertyIfNotEmpty("data-options", GetOptions().ReplaceContextParameters(viewModel, dataContext));
            sb.AppendPropertyIfNotEmpty("style", Style);

            sb.AppendPropertyIfNotEmpty("type", InputType);

            var value = viewModel.GetParameterValue(dataContext, Binding);

            if(value!=null && 
                (value.ToString().Equals("true", StringComparison.OrdinalIgnoreCase) ||
                value.ToString().Equals("1", StringComparison.OrdinalIgnoreCase)
                ))
            {
                sb.Append(" checked=\"checked\"");
            }

            sb.AppendTagEnd(Tag);
        }  
    }

    public class UIDateBox:UIInput
    {
        public override void InitSettings()
        {
            base.InitSettings();
            Class = "easyui-datebox";
        }
    }

    public class UIDateTimeBox:UIInput
    {
        public override void InitSettings()
        {
            base.InitSettings();
            Class = "easyui-datetimebox";
        }
    }

    public class UINumericBox:UIInput
    {
        public override void InitSettings()
        {
            base.InitSettings();
            Class = "easyui-numberbox";
        }
    }

    public class UIFileBox:UIInput
    {
        public override void InitSettings()
        {
            base.InitSettings();
            Tag = "input";
            Class = "easyui-filebox";
        }
    }

    public class UIImageBox:UIInput
    {
        public override void InitSettings()
        {
            base.InitSettings();
            Tag = "image";
        }

        public override void Render(StringBuilder sb, object viewModel, object dataContext = null)
        {
            sb.AppendTagStart(Tag);

            sb.AppendPropertyIfNotEmpty("id", ID);
            sb.AppendPropertyIfNotEmpty("name", Name);
            sb.AppendPropertyIfNotEmpty("class", Class);
            if (IsReadOnly) { sb.Append(" readonly=\"readonly\""); }
            if (!IsEnable) { sb.Append(" disabled=\"disabled\""); }
            sb.AppendPropertyIfNotEmpty("data-options", GetOptions().ReplaceContextParameters(viewModel, dataContext));
            sb.AppendPropertyIfNotEmpty("style", Style); 

            var value = viewModel.GetParameterValue(dataContext, Binding);
            sb.AppendPropertyIfNotEmpty("src", value);
            sb.AppendTagEnd(Tag);
        }
    }
}
