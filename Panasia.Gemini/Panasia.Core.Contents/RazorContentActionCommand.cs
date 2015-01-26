using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Panasia.Core;
using Panasia.Core.App;
using Panasia.Core.Sys;
using Panasia.Core.Web;

namespace Panasia.Core.Contents
{
    public class RazorContentActionExport
    {
        [CommandConfig("RazorContentActionCommand", "RazorEngine页面模板")]
        public static CommandBase CreateRedirectCommand(string configDoc)
        {
            if (string.IsNullOrEmpty(configDoc))
            {
                return new RazorContentActionCommand();
            }
            return configDoc.ToXObject<RazorContentActionCommand>();
        }

        [ContentModel("RazorContentViewModel","RazorEngine页面模型",typeof(RazorContentViewModel))]
        public static object CreateRazorContentViewModel(string doc)
        {
            return string.IsNullOrEmpty(doc) ? null :
                doc.ToJsonObject<RazorContentViewModel>();
        }
    }

    public class RazorContentViewModel : ActionViewModel
    {
        public ICommandContext Context { get; set; } 
    }

    public class RazorContentActionCommand:CommandBase
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

        #region 属性 TemplateFile
        private string _TemplateFile = "";
        [XmlAttribute("TemplateFile"), DefaultValue("")]
        public string TemplateFile
        {
            get
            {
                return _TemplateFile;
            }
            set
            {
                _TemplateFile = value;
                RaisePropertyChanged(() => TemplateFile);
            }
        }
        #endregion
                
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
            dynamic viewModel = null;
            if (RazorTemplate.IsIgnoreModelType)
            {
                viewModel = new { Context = context, Parameters = Parameters.CreateDictionary(context) };
            }
            else
            {
                viewModel = new RazorContentViewModel { Context = context, Parameters = Parameters.CreateDictionary(context) };
            }

            try
            {
                if (string.IsNullOrEmpty(TemplateFile))
                {
                    throw LangTexts.Current.GetLangText("9013", "模板没有设置").CreateException();
                }

                var template = ContentLib.Current.FolderRoot.GetFile(TemplateFile); 
                if(template==null || (!(template is RazorTemplate)))
                {
                    throw LangTexts.Current.GetFormatLangText("9011", "模板[{0}]加载失败",TemplateFile).CreateException();
                }
                var razorTemplate = template as RazorTemplate;
                 
                var setting = UISettings.Current.Items[SettingType] as UISetting;
                StringBuilder sb = new StringBuilder();
                if (setting != null)
                {
                    sb.Append(setting.ContentHeader);
                }

#if DEBUG
                DateTime beginRender = DateTime.Now;
#endif
                sb.Append(razorTemplate.Render(viewModel));
#if DEBUG
                this.Log(string.Format("Razor Render\t{0}-{1}:{2}", beginRender.ToString("HH:mm:ss.fff"), 
                    DateTime.Now.ToString("HH:mm:ss.fff"), (DateTime.Now - beginRender).TotalSeconds));
#endif
                if (setting != null)
                {
                    sb.Append(setting.ContentFooter);
                }
                viewResult.Content = sb.ToString();
            }
            catch (Exception ex)
            {
                viewResult.Content = ex.Message; 
            } 
            viewResult.ContentType = ContentType;
            viewResult.Encoding = Encoding;

            result.End(NextCommand);
            context.Execute(NextCommand);
        }
    }
}
