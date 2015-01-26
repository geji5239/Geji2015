using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using Microsoft.Reporting.WebForms;
using Panasia.Core.App;

namespace Panasia.Core.Web
{
    public class ReportViewerSetting
    {
        public string ReportPath { get; internal set; }

        public string ContentHeader { get;internal  set; }

        public string ContentFooter { get;internal  set; }

        public List<ReportDataSource> DataSources { get;internal  set; }

        public List<ReportParameter> Parameters { get; internal set; }
    }

    public class ReportViewers
    {
        private static Dictionary<string, ReportViewerSetting> _ReportViewers = new Dictionary<string, ReportViewerSetting>();

        public static ReportViewerSetting GetReportViewer(string id, bool remove = true)
        {
            var item = _ReportViewers.GetDictionaryValue(id, null);
            if(remove)
            {
                Remove(id);
            }
            return item;
        }

        internal static void Add(string id, ReportViewerSetting item)
        {
            lock (_ReportViewers)
            {
                _ReportViewers[id] = item;
            }
        }

        internal static void Remove(string id)
        {
            lock (_ReportViewers)
            {
                _ReportViewers.Remove(id);
            }
        }

        public static ActionResult Redirect(string id,string path="/Reports/ReportViewerWebForm.aspx")
        {
            return new RedirectResult(string.Format("{0}?id={1}",path, id));
        }

        //public static string Render(ReportViewerSetting item)
        //{
        //    Add(item);
        //    var reportViewerPage = "/Reports/Viewer/ReportViewerWebForm.aspx";
        //    using(var sw =new StringWriter())
        //    {
        //        HttpContext.Current.Server.Execute(reportViewerPage, sw, true);
        //        Remove();
        //        return sw.ToString();
        //    }
        //}
    }

    public class ReportActionCommand : CommandBase
    {
        private static readonly string ReportPathConfig = ServiceLocator.Current.BaseDirectory +
            ApplicationConfig.GetAppSettingValue("ReportPath", "..\\Reports\\");

        #region 属性 ReportViewerUrl
        private string _ReportViewerUrl = "/Reports/ReportViewerWebForm.aspx";
        [XmlAttribute("ReportViewerUrl"), DefaultValue("/Reports/ReportViewerWebForm.aspx")]
        public string ReportViewerUrl
        {
            get
            {
                return _ReportViewerUrl;
            }
            set
            {
                _ReportViewerUrl = value;
                RaisePropertyChanged(() => ReportViewerUrl);
            }
        }
        #endregion

        #region 属性 ReportPath
        private string _ReportPath = "";
        [XmlAttribute("ReportPath "), DefaultValue("")]
        public string ReportPath
        {
            get
            {
                return _ReportPath;
            }
            set
            {
                _ReportPath = value;
                RaisePropertyChanged(() => ReportPath);
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

        #region 属性 DataSources
        private CommandParameterCollection _DataSources = null;
        [XmlElement("DataSource", typeof(CommandParameter))]
        public CommandParameterCollection DataSources
        {
            get
            {
                if (_DataSources == null)
                {
                    _DataSources = new CommandParameterCollection();
                }
                return _DataSources;
            }
            set
            {
                _DataSources = value;
                RaisePropertyChanged(() => DataSources);
            }
        }
        #endregion

        public override CommandResult CreateResult()
        {
            return new ExtendedActionResult();
        }
        public override void OnExecute(ICommandContext context, CommandConfig config, CommandResult result)
        {
            var viewResult = result as ExtendedActionResult;

            #region 创建ReportViewer

            ReportViewerSetting reportViewer = new ReportViewerSetting
            {
                DataSources = new List<ReportDataSource>(),
                Parameters = new List<ReportParameter>()
            };
            reportViewer.ReportPath = ReportPath;

            DataSources.ForEach(p =>
            {
                var pValue = p.GetValue(context);
                if (pValue == null)
                {
                    throw new Exception(string.Format("参数[{0}]不能为空", p.Name));
                }
                reportViewer.DataSources.Add(new ReportDataSource(p.Name, pValue));
            });

            Parameters.ForEach(p =>
            {
                var pValue = p.GetValue(context);
                if (pValue == null)
                {
                    throw new Exception(string.Format("参数[{0}]不能为空", p.Name));
                }
                reportViewer.Parameters.Add(new ReportParameter(p.Name, pValue.ToString()));
            });

            reportViewer.ContentHeader = string.IsNullOrEmpty(ContentHeader) ? "" : 
                ContentHeader.ReplaceContextParameters(context, context);

            reportViewer.ContentFooter = string.IsNullOrEmpty(ContentFooter) ? "" :
                ContentFooter.ReplaceContextParameters(context, context);
            #endregion

            string id = Guid.NewGuid().ToString();
            ReportViewers.Add(id, reportViewer);
            viewResult.ActionResult = ReportViewers.Redirect(id,ReportViewerUrl);

            result.End(NextCommand);
            context.Execute(NextCommand);
        }

        /*
        public override void OnExecute(ICommandContext context, CommandConfig config, CommandResult result)
        {
            var viewResult = result as ActionCommandResult;

            #region 创建ReportViewer

            ReportViewerSetting reportViewer = new ReportViewerSetting
            {
                DataSources = new List<ReportDataSource>(),
                Parameters = new List<ReportParameter>()
            };
            reportViewer.ReportPath = ReportPath;

            DataSources.ForEach(p =>
            {
                var pValue = p.GetValue(context);
                if(pValue==null)
                {
                    throw new Exception(string.Format("参数[{0}]不能为空",p.Name));
                }
                reportViewer.DataSources.Add(new ReportDataSource(p.Name, pValue));
            });
             
            Parameters.ForEach(p =>
            {
                var pValue = p.GetValue(context);
                if(pValue==null)
                {
                    throw new Exception(string.Format("参数[{0}]不能为空",p.Name));
                }
                reportViewer.Parameters.Add(new ReportParameter(p.Name, pValue.ToString()));
            }); 
            #endregion

            #region 渲染HTML
            StringBuilder sb = new StringBuilder();  
            var setting = UISettings.Current.Items[LayoutSetting] as UISetting; 
            if (setting != null)
            {
                sb.Append(setting.ContentHeader);
            }
            if(!string.IsNullOrEmpty(ContentHeader))
            {
                sb.Append(ContentHeader);
            }

            var content = ReportViewers.Render(reportViewer);
            sb.Append(content);

            if(!string.IsNullOrEmpty(ContentFooter))
            {
                sb.Append(ContentHeader);
            }

            if (setting != null)
            {
                sb.Append(setting.ContentFooter);
            }
            #endregion

            viewResult.Content = sb.ToString();
            viewResult.ContentType = ContentType;
            viewResult.Encoding = Encoding;

            result.End(NextCommand);
            context.Execute(NextCommand);
        }
         */
    }
}
