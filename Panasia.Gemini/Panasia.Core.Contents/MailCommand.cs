using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Panasia.Core.App;
using Panasia.Core.Sys;
using Panasia.Core.Web;
using Panasia.Core.Email;

namespace Panasia.Core.Contents
{
    public class MailActionExport
    {
        [CommandConfig("MailCommand", "发送邮件")]
        public static CommandBase CreateMailCommand(string configDoc)
        {
            if (string.IsNullOrEmpty(configDoc))
            {
                return new MailCommand();
            }
            return configDoc.ToXObject<MailCommand>();
        }
    }
    public class MailResult : CommandResult
    {
        #region 属性 ServerID
        public string ServerID
        {
            get
            {
                return GetFieldValue<string>("ServerID", string.Empty);
            }
            set
            {
                SetFieldValue("ServerID", value);
            }
        }
        #endregion

        #region 属性 SendTo
        public string SendTo
        {
            get
            {
                return GetFieldValue<string>("SendTo", string.Empty);
            }
            set
            {
                SetFieldValue("SendTo", value);
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

        #region 属性 Title
        public string Title
        {
            get
            {
                return GetFieldValue<string>("Title", string.Empty);
            }
            set
            {
                SetFieldValue("Title", value);
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
    }

    [XmlRoot("MailCommand")]
    public class MailCommand : CommandBase
    {
        #region 属性 ServerID
        private string _ServerID = string.Empty;
        [XmlAttribute("ServerID"), DefaultValue("")]
        public string ServerID
        {
            get
            {
                return _ServerID;
            }
            set
            {
                _ServerID = value;
                RaisePropertyChanged(() => ServerID);
            }
        }
        #endregion

        #region 属性 UserID
        private string _UserID = string.Empty;
        [XmlAttribute("UserID"), DefaultValue("")]
        public string UserID
        {
            get
            {
                return _UserID;
            }
            set
            {
                _UserID = value;
                RaisePropertyChanged(() => UserID);
            }
        }
        #endregion

        #region 属性 UserPassword
        private string _UserPassword = string.Empty;
        [XmlAttribute("UserPassword"), DefaultValue("")]
        public string UserPassword
        {
            get
            {
                return _UserPassword;
            }
            set
            {
                _UserPassword = value;
                RaisePropertyChanged(() => UserPassword);
            }
        }
        #endregion

        #region 属性 SendTo
        private string _SendTo = string.Empty;
        [XmlAttribute("SendTo"), DefaultValue("")]
        public string SendTo
        {
            get
            {
                return _SendTo;
            }
            set
            {
                _SendTo = value;
                RaisePropertyChanged(() => SendTo);
            }
        }
        #endregion

        #region 属性 SendOneByOne
        private bool _SendOneByOne = true;
        [XmlAttribute("SendOneByOne"), DefaultValue(true)]
        public bool SendOneByOne
        {
            get
            {
                return _SendOneByOne;
            }
            set
            {
                _SendOneByOne = value;
                RaisePropertyChanged(() => SendOneByOne);
            }
        }
        #endregion

        #region 属性 Encoding
        private string _Encoding = "utf-8";
        [XmlAttribute("Encoding"), DefaultValue("utf-8")]
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

        #region 属性 TemplatePath
        private string _TemplatePath = string.Empty;
        [XmlAttribute("TemplatePath"), DefaultValue("")]
        public string TemplatePath
        {
            get
            {
                return _TemplatePath;
            }
            set
            {
                _TemplatePath = value;
                RaisePropertyChanged(() => TemplatePath);
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
            return new MailResult();
        }

        public override void OnExecute(ICommandContext context, CommandConfig config, CommandResult result)
        {
            var viewResult = result as MailResult;
            var viewModel = new RazorContentViewModel { Parameters = Parameters.CreateDictionary(context) };

            try
            {
                if (string.IsNullOrEmpty(TemplatePath))
                {
                    throw LangTexts.Current.GetLangText("9013", "模板没有设置").CreateException();
                }

                if (TemplatePath.Contains("{@"))
                {
                    viewResult.Content = TemplatePath.ReplaceContextParameters(context,viewModel );
                }
                else
                {
                    var template = ContentLib.Current.FolderRoot.GetFile(TemplatePath);
                    if (template == null || (!(template is RazorTemplate)))
                    {
                        throw LangTexts.Current.GetFormatLangText("9011", "模板[{0}]加载失败", TemplatePath).CreateException();
                    }
                    var razorTemplate = template as RazorTemplate;
                    viewResult.Content = razorTemplate.Render(viewModel); 
                }


                var title = Title.ReplaceContextParameters( context,viewModel);
                var sendTo = SendTo.ReplaceContextParameters(context, viewModel);
                var server = GetMailServer();
                if (string.IsNullOrEmpty(sendTo))
                {
                    throw new Exception("邮件接收者不能为空");
                }

                viewResult.Title = title;
                viewResult.SendTo = sendTo;
                viewResult.Encoding = Encoding; 

                //在Action配置中增加可否为订阅事件的标识，如果有，则在该Action中增加一个MailAction，在该成功执行之后再执行
                if (sendTo.IndexOf(',') > 0 && SendOneByOne)
                {
                    foreach (var to in sendTo.Split(','))
                    {
                        server.SendMail(title, viewResult.Content, to);
                    }
                }
                else if (!string.IsNullOrEmpty(sendTo))
                {
                    server.SendMail(title, viewResult.Content, sendTo);
                }
            }
            catch
            {
                throw;
            }

            result.End(NextCommand);
            context.Execute(NextCommand);
        }

        private Email.MailServer GetMailServer()
        {
            var serverItem = MailService.GetItem(ServerID);
            var mailServer = string.IsNullOrEmpty(UserID) ?
                new Email.MailServer(serverItem.ServerAddress, serverItem.ServerPort, serverItem.EnableSSL, serverItem.DefaultUser, serverItem.DefaultPassword) :
                new Email.MailServer(serverItem.ServerAddress, serverItem.ServerPort, serverItem.EnableSSL, UserID, UserPassword);
            return mailServer;
        }
    }
}
