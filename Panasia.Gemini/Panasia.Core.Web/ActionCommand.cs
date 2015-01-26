using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml.Serialization;
using Panasia.Core;
using Panasia.Core.App;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using System.Diagnostics;

namespace Panasia.Core.Web
{
    #region 重定向命令
    public class RedirectActionCommand : CommandBase
    {
        #region 属性 ControllerName
        private string _ControllerName = string.Empty;
        [XmlAttribute("ControllerName"), DefaultValue("")]
        public string ControllerName
        {
            get
            {
                return _ControllerName;
            }
            set
            {
                _ControllerName = value;
                RaisePropertyChanged(() => ControllerName);
            }
        }
        #endregion

        #region 属性 ActionName
        private string _ActionName = string.Empty;
        [XmlAttribute("ActionName"), DefaultValue("")]
        public string ActionName
        {
            get
            {
                return _ActionName;
            }
            set
            {
                _ActionName = value;
                RaisePropertyChanged(() => ActionName);
            }
        }
        #endregion

        #region 属性 NameValues
        private string _NameValues = string.Empty;
        [XmlAttribute("NameValues"), DefaultValue("")]
        public string NameValues
        {
            get
            {
                return _NameValues;
            }
            set
            {
                _NameValues = value;
                RaisePropertyChanged(() => NameValues);
            }
        }
        #endregion

        public override CommandResult CreateResult()
        {
            return new RedirectCommandResult();
        }

        public override void OnExecute(ICommandContext context, CommandConfig config, CommandResult result)
        {
            var viewResult = result as RedirectCommandResult;

            viewResult.ControllerName = ControllerName;
            viewResult.ActionName = ActionName;

            result.End(NextCommand);
            context.Execute(NextCommand);
        }
    }
    #endregion

    #region 动态界面
    public class FormViewActionCommand : ViewActionCommand
    {
        #region 属性 SettingType
        private string _SettingType = "IndexPage";
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

        #region 属性 Form
        private Form _Form = null;
        [XmlElement("Form")]
        public Form Form
        {
            get
            {
                if (_Form == null)
                {
                    _Form = new Form();
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

        public override void OnExecute(ICommandContext context, CommandConfig config, CommandResult result)
        {
            var viewResult = result as ActionCommandResult;
            var viewModel = CreateViewModel(context);
            var render = new HtmlRender(viewModel);

            viewResult.Content = render.Render(Form, SettingType);
            viewResult.ContentType = ContentType;
            viewResult.Encoding = Encoding;

            result.End(NextCommand);
            context.Execute(NextCommand);
        }

        protected override object CreateViewModel(object context)
        {
            var vm = new FormActionViewModel { Form = Form, Parameters = Parameters.CreateDictionary(context) };
            if (context is WebContext)
            {
                vm.ActionValue = (context as WebContext).ActionValue;
            }
            else
            {
                var value = context.GetPathValue("ActionValue");
                vm.ActionValue = value == null ? 0 : Convert.ToInt32(value);
            }
            return vm;
        }

        public void CopyToUIPage()
        {
            var toolbar = Form.GetAllChilds().FirstOrDefault(c => c is ToolBar);
            if (toolbar != null)
            {
                CreateIndexPage();
            }
            else
            {
                CreateDialogPage();
            }
        }

        private UIIndexPage CreateIndexPage()
        {
            UIIndexPage page = new UIIndexPage();
            var toolbar = Form.GetAllChilds().FirstOrDefault(c => c is ToolBar);
            if (toolbar != null)
            {
                foreach (Button button in (toolbar as ToolBar).GetAllChilds().Where(c => c is Button))
                {
                    page.ToolBar.Buttons.Add(new UIButton
                    {
                        ID = button.ID,
                        Class = button.Class,
                        Name = button.Name,
                        Title = button.Title,
                        Action = button.Action,
                        ActionValue = button.ActionValue,
                        Options = button.Options,
                        Style = button.Style
                    });
                }
            }

            var datagrid = Form.GetAllChilds().FirstOrDefault(c => c is GridView);
            if (datagrid != null)
            {
                var grid = datagrid as GridView;
                page.DataGrid.Key = grid.Key;
                page.DataGrid.ID = grid.ID;
                page.DataGrid.Class = grid.Class;
                page.DataGrid.Options = grid.Options;
                page.DataGrid.Name = grid.Name;
                page.DataGrid.Style = grid.Style;

                foreach (var col in grid.Columns)
                {
                    page.DataGrid.Columns.Add(new UIDataColumn
                    {
                        Name = col.Name,
                        Title = col.Title,
                        FieldName = col.FieldName,
                        ColumnWidth = col.Width == 0 ? "" : col.Width.ToString(),
                        Class = col.Class,
                        Style = col.Style,
                        Options = col.Options
                    });
                }
            }


            var action = CommandConfig.ActionConfig as Panasia.Core.App.Action;
            var command = AppConfig.Current.CreateCommand(action);
            command.Name = this.Name + "copy";
            command.CommandType = ActionCommands.IndexPageActionCommand;

            if (command.Config != null && command.Config is IndexPageActionCommand)
            {
                var pageCommand = command.Config as IndexPageActionCommand;
                pageCommand.Page = page;
                pageCommand.ContentType = ContentType;
                pageCommand.Encoding = this.Encoding;
                this.Parameters.AddToCollection(pageCommand.Parameters);
            }
            return page;
        }

        private UIDialog CreateDialogPage()
        {
            UIDialog page = new UIDialog { SettingType = "EditDialog" };
            var fields = Form.GetAllChilds().Where(c => c is StackPanel
                && c.Class.Equals("edit-field", StringComparison.OrdinalIgnoreCase));

            var unKnowControls = 0;

            #region 添加字段
            foreach (StackPanel f in fields)
            {
                UIField field = new UIField();
                bool inputCreate = false;
                foreach (var child in f.GetAllChilds())
                {
                    if (child is ComboBox)
                    {
                        if (inputCreate)
                        {
                            unKnowControls += 1;
                            continue;
                        }
                        CreateComboBox(field, child);
                        inputCreate = true;
                        continue;
                    }
                    if (child is TextBox)
                    {
                        if (inputCreate)
                        {
                            unKnowControls += 1;
                            continue;
                        }
                        CreateTextBox(field, child);
                        inputCreate = true;
                        continue;
                    }
                    if (child is Label)
                    {
                        field.Title = (child as Label).Title;
                        continue;
                    }
                    unKnowControls += 1;
                }

                page.Form.Fields.Add(field);
            }
            #endregion

            var buttonPanel = Form.GetAllChilds()
                .FirstOrDefault(c => c is StackPanel && c.Class.Equals("dialog-button", StringComparison.OrdinalIgnoreCase));
            if (buttonPanel != null)
            {
                var buttons = (buttonPanel as StackPanel).GetAllChilds().Where(c => c is Button);
                foreach (Button button in buttons)
                {
                    page.Form.ToolBar.Buttons.Add(new UIButton
                    {
                        ID = button.ID,
                        Class = button.Class,
                        Name = button.Name,
                        Title = button.Title,
                        Action = button.Action,
                        ActionValue = button.ActionValue,
                        Options = button.Options,
                        Style = button.Style
                    });
                }
            }

            var action = CommandConfig.ActionConfig as Panasia.Core.App.Action;
            var command = AppConfig.Current.CreateCommand(action);
            command.Name = this.Name + "copy";
            command.CommandType = ActionCommands.EditDialogActionCommand;

            if (command.Config != null && command.Config is EditDialogActionCommand)
            {
                var pageCommand = command.Config as EditDialogActionCommand;
                pageCommand.Page = page;
                pageCommand.ContentType = ContentType;
                pageCommand.Encoding = this.Encoding;
                this.Parameters.AddToCollection(pageCommand.Parameters);
            }
            return page;
        }

        private static void CreateTextBox(UIField field, ControlBase child)
        {
            var input = child as TextBox;
            field.Input = new UIInput
            {
                ID = input.ID,
                Name = input.Name,
                Class = input.Class,
                Style = input.Style,
                Options = input.Options,
                IsEnable = input.IsEnable,
                IsReadOnly = input.IsReadOnly,
                IsRequired = input.IsRequired,
                IsHidden = input.IsHidden,
                Binding = input.Binding,
                Length = input.Length
            }; 
        }

        private static void CreateComboBox(UIField field, ControlBase child)
        {
            var input = child as ComboBox;
            field.Input = new UIComboBox
            {
                ID = input.ID,
                Name = input.Name,
                Class = input.Class,
                Style = input.Style,
                Options = input.Options,
                IsEnable = input.IsEnable,
                IsReadOnly = input.IsReadOnly,
                IsRequired = input.IsRequired,
                IsHidden = input.IsHidden,
                Binding = input.Binding,
                DisplayMember = input.DisplayMemeber,
                ItemsSource = input.ItemsSource,
                Length = input.Length,
                ValueMember = input.ValueMember
            };
        }
    }
    #endregion

    #region Razor视图
    public class ViewActionCommand : CommandBase
    {
        #region 属性 ContentType
        private string _ContentType = "text/xml";
        [XmlAttribute("ContentType"), DefaultValue("")]
        public string ContentType
        {
            get
            {
                return _ContentType;
            }
            set
            {
                _ContentType = value;
                RaisePropertyChanged(() => ContentType);
            }
        }
        #endregion

        #region 属性 Encoding
        private string _Encoding = string.Empty;
        [XmlAttribute("Encoding"), DefaultValue("")]
        public string Encoding
        {
            get
            {
                return _Encoding;
            }
            set
            {
                _Encoding = value;
                RaisePropertyChanged(() => Encoding);
            }
        }
        #endregion

        #region 属性 ViewPath
        private string _ViewPath = string.Empty;
        [XmlAttribute("ViewPath")]
        public string ViewPath
        {
            get
            {
                return _ViewPath;
            }
            set
            {
                _ViewPath = value;
                RaisePropertyChanged(() => ViewPath);
            }
        }
        #endregion

        #region 属性 IsPartial
        private bool _IsPartial = false;
        [XmlAttribute("IsPartial"), DefaultValue(false)]
        public bool IsPartial
        {
            get
            {
                return _IsPartial;
            }
            set
            {
                _IsPartial = value;
                RaisePropertyChanged(() => IsPartial);
            }
        }
        #endregion

        #region 属性 Parameters
        private CommandParameterCollection _Parameters = null;
        [XmlElement("Parameter", typeof(CommandParameter))]
        public CommandParameterCollection Parameters
        {
            get
            {
                if (_Parameters == null)
                {
                    _Parameters = new CommandParameterCollection();
                }
                return _Parameters;
            }
            set
            {
                _Parameters = value;
                RaisePropertyChanged(() => Parameters);
            }
        }
        #endregion

        public override CommandResult CreateResult()
        {
            return new ActionCommandResult();
        }

        public override void OnExecute(ICommandContext context, CommandConfig config, CommandResult result)
        {
            var viewResult = result as ActionCommandResult;
             
            var viewModel = CreateViewModel(context);

            try
            {
                WebContext webContext = null;
                if(context is WebContext)
                {
                    webContext = context as WebContext;
                }

                if (webContext!=null)
                {
                    viewResult.Content = (webContext).RenderView(viewModel, ViewPath, IsPartial); 
                }
                else
                {
                    dynamic d = context;
                    viewResult.Content = d.RenderView(viewModel, ViewPath, IsPartial);
                    
                    //var renderView = context.GetType().GetMethod("RenderView");
                    //if (renderView != null)
                    //{
                    //    viewResult.Content = renderView.Invoke(context, new object[] { viewModel, ViewPath, IsPartial }) as string;
                    //}
                    //else
                    //{
                    //    throw new Exception("上下文对象异常，请告知项谢进");
                    //}
                }
            }
            catch (Exception ex)
            {
                this.Log(ex.ToString());
                throw new Exception("执行视图引擎错误或视图不存在：" + ex.Message);
            }
            viewResult.ContentType = ContentType;
            viewResult.Encoding = Encoding;

            result.End(NextCommand);
            context.Execute(NextCommand);
        }

        protected virtual object CreateViewModel(object context)
        {
            return new ActionViewModel { Parameters = Parameters.CreateDictionary(context) };
        }

    }
    #endregion

    #region Json
    public class ReturnActionCommand : CommandBase
    {
        #region 属性 ContentType
        private string _ContentType = "appplication/json";
        [XmlAttribute("ContentType"), DefaultValue("")]
        public string ContentType
        {
            get
            {
                return _ContentType;
            }
            set
            {
                _ContentType = value;
                RaisePropertyChanged(() => ContentType);
            }
        }
        #endregion

        #region 属性 Encoding
        private string _Encoding = string.Empty;
        [XmlAttribute("Encoding"), DefaultValue("")]
        public string Encoding
        {
            get
            {
                return _Encoding;
            }
            set
            {
                _Encoding = value;
                RaisePropertyChanged(() => Encoding);
            }
        }
        #endregion             

        #region Parameter
        private CommandParameter _Parameter = null;
        [XmlElement("Parameter")]
        public CommandParameter Parameter
        {
            get
            {
                if(_Parameter == null)
                {
                    _Parameter = new CommandParameter { Name = "json", Type= CommandParameterType.Parameter, Path="LastResult.Json" };
                }
                return _Parameter;
            }
            set
            {
                _Parameter = value;
                RaisePropertyChanged(() => Parameter);
                RaisePropertyChanged(() => Parameters);
            }
        }
        #endregion

        #region 属性 Parameters 
        [XmlIgnore()]
        public CommandParameterCollection Parameters
        {
            get
            {
                return new CommandParameterCollection { Parameter }; 
            } 
        }
        #endregion

        public override CommandResult CreateResult()
        {
            return new ActionCommandResult();
        }

        public override void OnExecute(ICommandContext context, CommandConfig config, CommandResult result)
        {
            var viewResult = result as ActionCommandResult;

            var content = Parameter.GetValue(context);
            viewResult.Content = content == null ? "" : content.ToString(); 

            viewResult.ContentType = ContentType;
            viewResult.Encoding = Encoding;  
            result.End(NextCommand);
            context.Execute(NextCommand);
        }
    }
    #endregion

    #region Json
    public class JsonActionCommand : CommandBase
    {
        #region 属性 ContentType
        private string _ContentType = "application/json";
        [XmlAttribute("ContentType"), DefaultValue("")]
        public string ContentType
        {
            get
            {
                return _ContentType;
            }
            set
            {
                _ContentType = value;
                RaisePropertyChanged(() => ContentType);
            }
        }
        #endregion

        #region 属性 Encoding
        private string _Encoding = string.Empty;
        [XmlAttribute("Encoding"), DefaultValue("")]
        public string Encoding
        {
            get
            {
                return _Encoding;
            }
            set
            {
                _Encoding = value;
                RaisePropertyChanged(() => Encoding);
            }
        }
        #endregion

        #region 属性 Parameters
        private CommandParameterCollection _Parameters = null;
        [XmlElement("Parameter", typeof(CommandParameter))]
        public CommandParameterCollection Parameters
        {
            get
            {
                if (_Parameters == null)
                {
                    _Parameters = new CommandParameterCollection();
                }
                return _Parameters;
            }
            set
            {
                _Parameters = value;
                RaisePropertyChanged(() => Parameters);
            }
        }
        #endregion

        public override CommandResult CreateResult()
        {
            return new ActionCommandResult();
        }

        public override void OnExecute(ICommandContext context, CommandConfig config, CommandResult result)
        {
            var viewResult = result as ActionCommandResult;

            Dictionary<string, object> viewModel = new Dictionary<string, object>();
            foreach (var p in Parameters)
            {
                viewModel.Add(p.Name, p.GetValue(context));
            }
            viewResult.Content = viewModel.CreateJson();

            viewResult.ContentType = ContentType;
            viewResult.Encoding = Encoding;
            result.End(NextCommand);
            context.Execute(NextCommand);
        }
    }
    #endregion

    #region 返回内容
    public class ContentActionCommand : CommandBase
    {
        #region 属性 ContentType
        private string _ContentType = "text/xml";
        [XmlAttribute("ContentType"), DefaultValue("")]
        public string ContentType
        {
            get
            {
                return _ContentType;
            }
            set
            {
                _ContentType = value;
                RaisePropertyChanged(() => ContentType);
            }
        }
        #endregion

        #region 属性 Encoding
        private string _Encoding = string.Empty;
        [XmlAttribute("Encoding"), DefaultValue("")]
        public string Encoding
        {
            get
            {
                return _Encoding;
            }
            set
            {
                _Encoding = value;
                RaisePropertyChanged(() => Encoding);
            }
        }
        #endregion
        
        #region 属性 Content
        private string _Content = string.Empty;
        [XmlText]
        public string Content
        {
            get
            {
                return _Content;
            }
            set
            {
                _Content = value;
                RaisePropertyChanged(() => Content);
            }
        }
        #endregion

        public override CommandResult CreateResult()
        {
            return new ActionCommandResult();
        }

        public override void OnExecute(ICommandContext context, CommandConfig config, CommandResult result)
        {
            var viewResult = result as ActionCommandResult; 

            viewResult.Content = Content;

            viewResult.ContentType = ContentType;
            viewResult.Encoding = Encoding;

            result.End(NextCommand);
            context.Execute(NextCommand);
        }
    }
    #endregion

    public class RedirectCommandResult : CommandResult, IActionCommandResult
    {
        #region 属性 ControllerName
        public string ControllerName
        {
            get
            {
                return GetFieldValue<string>("ControllerName", string.Empty);
            }
            set
            {
                SetFieldValue("ControllerName", value);
            }
        }
        #endregion

        #region 属性 ActionName
        public string ActionName
        {
            get
            {
                return GetFieldValue<string>("ActionName", string.Empty);
            }
            set
            {
                SetFieldValue("ActionName", value);
            }
        }
        #endregion

        #region 属性 NameValues
        public string NameValues
        {
            get
            {
                return GetFieldValue<string>("NameValues", string.Empty);
            }
            set
            {
                SetFieldValue("NameValues", value);
            }
        }
        #endregion

        public ActionResult GetResult()
        {
            return new RedirectToRouteResult(
                new RouteValueDictionary(new { controller = ControllerName, action = ActionName, nameValues = NameValues }));
        }
    }

    public class ActionCommandResult : CommandResult, IActionCommandResult
    {
        #region 属性 ContentType
        public string ContentType
        {
            get
            {
                return GetFieldValue<string>("ContentType", "text/xml");
            }
            set
            {
                SetFieldValue("ContentType", value);
            }
        }
        #endregion

        #region 属性 Encoding
        public string Encoding
        {
            get
            {
                return GetFieldValue<string>("Encoding", string.Empty);
            }
            set
            {
                SetFieldValue("Encoding", value);
            }
        }
        #endregion

        #region 属性 Content
        public string Content
        {
            get
            {
                return GetFieldValue<string>("Content", string.Empty);
            }
            set
            {
                SetFieldValue("Content", value);
            }
        }
        #endregion

        public ActionResult GetResult()
        {
            var result = new ContentResult { Content = Content };
            if (!string.IsNullOrEmpty(ContentType))
            {
                result.ContentType = ContentType;
            }
            if (!string.IsNullOrEmpty(Encoding))
            {
                result.ContentEncoding = System.Text.Encoding.GetEncoding(Encoding);
            }
            else
            {
                result.ContentEncoding = System.Text.Encoding.UTF8;
            }
            return result;
        }
    }


}
