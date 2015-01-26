using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using Panasia.Core.App;

namespace Panasia.Core.Web
{
    #region 文件结果
    public class TextFileResult:CommandResult, IActionCommandResult
    {
        #region 属性 FileName
        private string _FileName = string.Empty;
        public string FileName
        {
            get
            {
                return _FileName;
            }
            set
            {
                _FileName = value; 
            }
        }
        #endregion 

        #region 属性 ContentType
        private string _ContentType = string.Empty;
        public string ContentType
        {
            get
            {
                return _ContentType;
            }
            set
            {
                _ContentType = value; 
            }
        }
        #endregion
        
        #region 属性 FileContent
        private string _FileContent = "";
        public string FileContent
        {
            get
            {
                return _FileContent;
            }
            set
            {
                _FileContent = value; 
            }
        }
        #endregion
        
        public ActionResult GetResult()
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(FileContent);
            return new FileContentResult(bytes, ContentType) { FileDownloadName =  FileName }; 
        }
    }
    #endregion

    #region 配置项

    [XmlRoot("ExportToHtmlExcel")]
    public class ExportToHtmlExcel : CommandBase
    {
        #region 属性 FileName
        private string _FileName = string.Empty;
        [XmlAttribute("FileName"), DefaultValue("")]
        public string FileName
        {
            get
            {
                return _FileName;
            }
            set
            {
                _FileName = value;
                RaisePropertyChanged(() => FileName);
            }
        }
        #endregion

        #region 属性 ContentType
        private string _ContentType = "application/ms-excel";
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

        #region 属性 TableStyle
        private string _TableStyle = "";
        [XmlAttribute("TableStyle"), DefaultValue("")]
        public string TableStyle
        {
            get
            {
                return _TableStyle;
            }
            set
            {
                _TableStyle = value;
                RaisePropertyChanged(() => TableStyle);
            }
        }
        #endregion

        #region 属性 ItemsSource
        private string _ItemsSource = "";
        [XmlAttribute("ItemsSource")]
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
        
        #region 属性 Fields
        private ExportFieldCollection _Fields = null;
        [XmlElement("Field", typeof(ExportField))]
        public ExportFieldCollection Fields
        {
            get
            {
                if (_Fields == null)
                {
                    _Fields = new ExportFieldCollection();
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
        
        public override CommandResult CreateResult()
        {
            return new TextFileResult
            {
                FileName = FileName,
                ContentType = ContentType 
            };
        }

        public override void OnExecute(ICommandContext context, CommandConfig config, CommandResult result)
        {
            var fileResult = result as TextFileResult;
            if(fileResult!=null)
            {
                fileResult.FileName = FileName.ReplaceContextParameters(context, context);
                fileResult.ContentType = ContentType;
                var items = context.GetParameterValue(context, ItemsSource);
                if(items is IEnumerable)
                {
                    fileResult.FileContent = CreateTable(items as IEnumerable); 
                }
                else
                {
                    //导出文件错误
                    throw new Exception("数据源路径不正确");
                }
            }

            result.End(NextCommand);
            context.Execute(NextCommand);
        }

        private string CreateTable(IEnumerable items=null)
        {
            var sbHtml = new StringBuilder();
            sbHtml.AppendFormat("<table cellspacing='0' cellpadding='0'{0}>",
                string.IsNullOrEmpty(TableStyle) ? "" : string.Format(" style=\"{0}\"", TableStyle));

            sbHtml.Append("<tr>");
            foreach(var field in Fields)
            {
                sbHtml.AppendFormat("<th{1}>{0}</th>",field.Title,
                   string.IsNullOrEmpty(field.HeaderStyle) ? "" : string.Format(" style=\"{0}\"", field.HeaderStyle));
            }
            sbHtml.Append("</tr>");

            foreach(var item in items)
            {
                sbHtml.Append("<tr>");
                foreach(var field in Fields)
                {
                    sbHtml.AppendFormat("<td{1}>{0}</td>",item.GetPathValue(field.Name),
                       string.IsNullOrEmpty(field.ItemStyle) ? "" : string.Format(" style=\"{0}\"", field.ItemStyle));
                }    
                sbHtml.Append("</tr>");           
            } 

            sbHtml.Append("</table>");
            return sbHtml.ToString();
        }
    }

    public class ExportFieldCollection : CollectionBase<ExportField>
    {
    }

    #region 输出列配置
    public class ExportField : EntityBase
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

        #region 属性 Title
        private string _Title = string.Empty;
        [XmlAttribute("Title"), DefaultValue("")]
        public string Title
        {
            get
            {
                if (string.IsNullOrEmpty(_Title))
                {
                    return Name;
                }
                return _Title;
            }
            set
            {
                _Title = value;
                RaisePropertyChanged(() => Title);
            }
        }
        #endregion

        #region 属性 HeaderStyle
        private string _HeaderStyle = "";
        [XmlAttribute("HeaderStyle"), DefaultValue("")]
        public string HeaderStyle
        {
            get
            {
                return _HeaderStyle;
            }
            set
            {
                _HeaderStyle = value;
                RaisePropertyChanged(() => HeaderStyle);
            }
        }
        #endregion

        #region 属性 ItemStyle
        private string _ItemStyle = "";
        [XmlAttribute("ItemStyle"), DefaultValue("")]
        public string ItemStyle
        {
            get
            {
                return _ItemStyle;
            }
            set
            {
                _ItemStyle = value;
                RaisePropertyChanged(() => ItemStyle);
            }
        }
        #endregion
    } 
    #endregion
    #endregion
}
