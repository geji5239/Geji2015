using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Panasia.Core.App;

namespace Panasia.Core.Web
{
    public class ActionCommands
    {
        /// <summary>
        /// SQL查询
        /// </summary>
        public const string QuerySqlCommand = "QuerySqlCommand";
        /// <summary>
        /// SQL查询Json
        /// </summary>
        public const string QueryJsonSqlCommand = "QueryJsonSqlCommand";
        /// <summary>
        /// SQL执行
        /// </summary>
        public const string NoneQuerySqlCommand = "NoneQuerySqlCommand";
        /// <summary>
        /// SQL取值
        /// </summary>
        public const string ScalarSqlCommand = "ScalarSqlCommand";

        /// <summary>
        /// Json数据
        /// </summary>
        public const string JsonActionCommand = "JsonActionCommand";

        /// <summary>
        /// 返回数据
        /// </summary>
        public const string ReturnActionCommand = "ReturnActionCommand";

        /// <summary>
        /// RazorView
        /// </summary>
        public const string ViewActionCommand = "RazorView";

        /// <summary>
        /// Razor窗体设计
        /// </summary>
        public const string FormViewActionCommand = "FormViewActionCommand";

        /// <summary>
        /// 静态内容
        /// </summary>
        public const string ContentActionCommand = "ContentActionCommand";

        /// <summary>
        /// Action重定向
        /// </summary>
        public const string RedirectActionCommand = "RedirectActionCommand";

        /// <summary>
        /// 列表页面
        /// </summary>
        public const string IndexPageActionCommand = "IndexPageActionCommand";

        /// <summary>
        /// 编辑对话框页面
        /// </summary>
        public const string EditDialogActionCommand = "EditDialogActionCommand";

        /// <summary>
        /// 保存上传文件
        /// </summary>
        public const string SaveUploadFileCommand = "";

        /// <summary>
        /// 导出到HTML格式的Excel文件
        /// </summary>
        public const string ExportToHtmlExcel = "ExportToHtmlExcel";

        /// <summary>
        /// 扩展方法
        /// </summary>
        public const string ExtendedAction = "ExtendedAction";

        /// <summary>
        /// RDLC报表命令
        /// </summary>
        public const string ReportActionCommand = "ReportActionCommand";


        [CommandConfig("IndexPageActionCommand", "列表页面")]
        public static CommandBase CreateIndexPageCommand(string configDoc)
        {
            if (string.IsNullOrEmpty(configDoc))
            {
                return new IndexPageActionCommand();
            }
            return configDoc.ToXObject<IndexPageActionCommand>();
        }

        [CommandConfig("EditDialogActionCommand", "编辑对话框")]
        public static CommandBase CreateEditDialogCommand(string configDoc)
        {
            if (string.IsNullOrEmpty(configDoc))
            {
                return new EditDialogActionCommand();
            }
            return configDoc.ToXObject<EditDialogActionCommand>();
        }

        [CommandConfig("JsonActionCommand", "Json数据")]
        public static CommandBase CreateJsonCommand(string configDoc)
        {
            if (string.IsNullOrEmpty(configDoc))
            {
                return new JsonActionCommand();
            }
            return configDoc.ToXObject<JsonActionCommand>();
        }

        [CommandConfig("ReturnActionCommand", "返回数据")]
        public static CommandBase CreateReturnCommand(string configDoc)
        {
            if (string.IsNullOrEmpty(configDoc))
            {
                return new ReturnActionCommand();
            }
            return configDoc.ToXObject<ReturnActionCommand>();
        }

        [CommandConfig("ViewActionCommand", "RazorView")]
        public static CommandBase CreateViewCommand(string configDoc)
        {
            if (string.IsNullOrEmpty(configDoc))
            {
                return new ViewActionCommand();
            }
            return configDoc.ToXObject<ViewActionCommand>();
        }

        [CommandConfig("FormViewActionCommand", "Razor窗体设计")]
        public static CommandBase CreateFormViewCommand(string configDoc)
        {
            if (string.IsNullOrEmpty(configDoc))
            {
                return new FormViewActionCommand();
            }
            return configDoc.ToXObject<FormViewActionCommand>();
        }

        [CommandConfig("ContentActionCommand", "静态内容")]
        public static CommandBase CreateContentCommand(string configDoc)
        {
            if (string.IsNullOrEmpty(configDoc))
            {
                return new ContentActionCommand();
            }
            return configDoc.ToXObject<ContentActionCommand>();
        }

        [CommandConfig("RedirectActionCommand", "重定向Action")]
        public static CommandBase CreateRedirectCommand(string configDoc)
        {
            if (string.IsNullOrEmpty(configDoc))
            {
                return new RedirectActionCommand();
            }
            return configDoc.ToXObject<RedirectActionCommand>();
        }
        
        [CommandConfig("ExportToHtmlExcel", "导出Excel(html格式)")]
        public static CommandBase CreateExportToHtmlExcel(string configDoc)
        {
            if (string.IsNullOrEmpty(configDoc))
            {
                return new ExportToHtmlExcel();
            }
            return configDoc.ToXObject<ExportToHtmlExcel>();
        }

        [CommandConfig("ExtendedAction", "扩展方法")]
        public static CommandBase CreateExtendedAction(string configDoc)
        {
            if (string.IsNullOrEmpty(configDoc))
            {
                return new ExtendedActionCommand();
            }
            return configDoc.ToXObject<ExtendedActionCommand>();
        }

        [CommandConfig("ReportActionCommand", "RDLC报表")]
        public static CommandBase CreateReportActionCommand(string configDoc)
        {
            if (string.IsNullOrEmpty(configDoc))
            {
                return new ReportActionCommand();
            }
            return configDoc.ToXObject<ReportActionCommand>();
        }
    }

}
