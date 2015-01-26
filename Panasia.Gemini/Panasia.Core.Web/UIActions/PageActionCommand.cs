using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Panasia.Core;
using Panasia.Core.App;

namespace Panasia.Core.Web
{
    public class PageModel:EntityBase
    {
        public PageModel()
        {
            Parameters = new Dictionary<string, object>();
        }

        public Dictionary<string, object> Parameters { get; set; }
    }

    public class EditDialogActionCommand : CommandBase
    {
        #region 属性 ContentType
        private string _ContentType = "text/html";
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

        #region 属性 Page
        private UIDialog _Page = null;
        [XmlElement("Page")]
        public UIDialog Page
        {
            get
            {
                if (_Page == null)
                {
                    _Page = new UIDialog { SettingType = "EditDialog" };
                }
                return _Page;
            }
            set
            {
                _Page = value;
                RaisePropertyChanged(() => Page);
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

            PageModel vm = new PageModel();
            foreach (var p in Parameters)
            {
                vm.Parameters.Add(p.Name, p.GetValue(context));
            }

            viewResult.Content = Page.Render(vm);

            viewResult.ContentType = ContentType;
            viewResult.Encoding = Encoding;
            result.End(NextCommand);
            context.Execute(NextCommand);
        }
    }

    public class IndexPageActionCommand : CommandBase
    {
        #region 属性 ContentType
        private string _ContentType = "text/html";
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

        #region 属性 Page
        private UIIndexPage _Page = null;
        [XmlElement("Page")]
        public UIIndexPage Page
        {
            get
            {
                if(_Page==null)
                {
                    _Page = new UIIndexPage { SettingType = "IndexPage" };
                }
                return _Page;
            }
            set
            {
                _Page = value;
                RaisePropertyChanged(() => Page);
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

            PageModel vm = new PageModel(); 
            foreach (var p in Parameters)
            {
                vm.Parameters.Add(p.Name, p.GetValue(context));
            }

            viewResult.Content = Page.Render(vm);

            viewResult.ContentType = ContentType;
            viewResult.Encoding = Encoding;
            result.End(NextCommand);
            context.Execute(NextCommand);
        }
    }
}
