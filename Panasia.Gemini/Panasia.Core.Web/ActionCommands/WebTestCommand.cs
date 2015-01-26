using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using Panasia.Core.App;
using Panasia.Core;

namespace Panasia.Core.Web
{
    
    class WebTestCommandExports
    {
        [CommandConfig("WebTestCommand", "Web测试")]
        public static CommandBase CreateRedirectCommand(string configDoc)
        {
            if (string.IsNullOrEmpty(configDoc))
            {
                return new WebTestCommand();
            }
            return configDoc.ToXObject<WebTestCommand>();
        }
    }

    public class WebTestCommand : CommandBase
    {
        #region 属性 Url
        private string _Url = "";
        [XmlAttribute("Url"), DefaultValue("")]
        public string Url
        {
            get
            {
                //if(string.IsNullOrEmpty(_Url))
                //{
                //    _Url = string.Format("http://localhost:14003/Page/{0}/{1}",
                //        CommandConfig==null?"":CommandConfig.ActionConfig==null?"":CommandConfig.ActionConfig.Page==null?"":CommandConfig.ActionConfig.Page.PageID,
                //        CommandConfig==null?"":CommandConfig.ActionConfig==null?"":CommandConfig.ActionConfig.Name
                //        );
                //}
                return _Url;
            }
            set
            {
                _Url = value;
                RaisePropertyChanged(() => Url);
            }
        }
        #endregion

        #region 属性 Result
        private string _Result = "";
        [XmlAttribute("Result"), DefaultValue("")]
        public string Result
        {
            get
            {
                return _Result;
            }
            set
            {
                _Result = value;
                RaisePropertyChanged(() => Result);
            }
        }
        #endregion

        #region 属性 ResultEncoding
        private string _ResultEncoding = "";
        [XmlAttribute("ResultEncoding"), DefaultValue("")]
        public string ResultEncoding
        {
            get
            {
                return _ResultEncoding;
            }
            set
            {
                _ResultEncoding = value;
                RaisePropertyChanged(() => ResultEncoding);
            }
        }
        #endregion

        public void Request()
        {
            try
            {
                var request = HttpWebRequest.Create(Url);
                request.Headers.Add("ConfigTools", "true");

                var response = request.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream(),
                    string.IsNullOrEmpty(ResultEncoding)?Encoding.UTF8:Encoding.GetEncoding(ResultEncoding)))
                {
                    Result= reader.ReadToEnd().ToFormatHtml();
                }
            }
            catch (Exception ex)
            {
                Result = ex.ToString();
            }
        }

        public override void OnExecute(ICommandContext context, CommandConfig config, CommandResult result)
        {
            throw new Exception("测试命令，不能执行");
        }
    }
}
