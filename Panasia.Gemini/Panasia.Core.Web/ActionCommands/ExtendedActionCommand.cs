using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml.Serialization;
using Panasia.Core.App;

namespace Panasia.Core.Web
{
    #region MEF导出给配置工具使用
    class ExtendedActionxports
    {


        [ContentAction("SampleAddress","传入一组参数还你一个内容")]
        public string CreateContent(Dictionary<string,object> paras)
        {
            return "";
        }
    }
    #endregion

    public class ExtendedActionResult : CommandResult, IActionCommandResult
    {
        #region 属性 ActionResult
        public ActionResult ActionResult
        {
            get
            {
                return GetFieldValue<ActionResult>("ActionResult", null);
            }
            set
            {
                SetFieldValue("ActionResult", value);
            }
        }
        #endregion 

        public ActionResult GetResult()
        {
            return ActionResult;
        }
    }

    [XmlRoot("ExtendedAction")]
    public class ExtendedActionCommand : CommandBase
    {
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

        #region 属性 ContentAddress
        private string _ContentAddress = "";
        [XmlAttribute("ContentAddress"), DefaultValue("")]
        public string ContentAddress
        {
            get
            {
                return _ContentAddress;
            }
            set
            {
                _ContentAddress = value;
                RaisePropertyChanged(() => ContentAddress);
            }
        }
        #endregion

        public override CommandResult CreateResult()
        {
            return new ExtendedActionResult();
        }

        public override void OnExecute(ICommandContext context, CommandConfig config, CommandResult result)
        {
            var actionResult = result as ExtendedActionResult;
            var paras = Parameters.CreateDictionary(context); 
            var actionItem = ContentActions.GetItem(ContentAddress);
            if(actionItem!=null)
            {
                var content = actionItem.Func(paras);
                if(content is ActionResult)
                {
                    actionResult.ActionResult = content as ActionResult;
                }
                else
                {
                    var contentResult = new ContentResult
                    {
                        Content = content.ToString(),
                        ContentEncoding = actionItem.Encoding,
                        ContentType = actionItem.ContentType
                    };
                    actionResult.ActionResult = contentResult;
                }
            }

            result.End(NextCommand);
            context.Execute(NextCommand);
        }
    }
}
