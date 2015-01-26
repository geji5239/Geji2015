using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Panasia.Core;

namespace Panasia.Core.Web
{
    public static class ControlExtensions
    {
        #region 基本方法
        public static string ToHtmlString(this Form control)
        {
            return "";
        }

        public static string ToFormatIfNotEmpty(this string id, string format)
        {
            if(string.IsNullOrWhiteSpace(id))
            {
                return "";
            }
            return string.Format(format, id);
        }

        /// <summary>
        /// 返回ID,Name,Class,Style
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static string GetBaseAttribtes(this ControlBase control)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(control.ID.ToFormatIfNotEmpty(" id=\"{0}\""));
            sb.Append(control.Name.ToFormatIfNotEmpty(" name=\"{0}\""));
            sb.Append(control.Class.ToFormatIfNotEmpty(" class=\"{0}\""));
            sb.Append(control.Style.ToFormatIfNotEmpty(" style=\"{0}\""));
            return sb.ToString();
        }
        #endregion

        #region 创建控件

        public static void AppendDataView(this System.Text.StringBuilder sbHtml, DataView control, FormActionViewModel vm)
        {
            AppendString(sbHtml, "<form ");
            AppendString(sbHtml, control.GetBaseAttribtes());
            AppendString(sbHtml, " method=\"post\">");
            AppendGridBox(sbHtml, control, vm);
            AppendString(sbHtml, "</form>");
        }

        public static void AppendGridBox(this System.Text.StringBuilder sbHtml, GridBox control, FormActionViewModel vm)
        {
            if (control.UseTableLayout)
            {
                AppendTable(sbHtml, control, vm);
            }
            else
            {
                AppendPanel(sbHtml, control, vm);
            }
        }

        public static void AppendPanel(this System.Text.StringBuilder sbHtml, Panel control, FormActionViewModel vm)
        {
            AppendString(sbHtml, "<div ");
            AppendString(sbHtml, control.GetBaseAttribtes());
            AppendString(sbHtml, ">");
            foreach (var item in control.Controls)
            {
                AppendControl(sbHtml, item, vm);
            }
            AppendString(sbHtml, "</div>");
        }

        public static void AppendTable(this System.Text.StringBuilder sbHtml, GridBox control, FormActionViewModel vm)
        {
            AppendString(sbHtml, "<table");
            AppendString(sbHtml, control.GetBaseAttribtes());
            AppendString(sbHtml, ">");
            int rowIndex = 0;
            int colIndex = 0;

            foreach (var row in control.Controls.GroupBy(c => c.Row).OrderBy(d => d.Key))
            {
                if (row.Key > rowIndex)
                {
                    for (int i = 0; i < row.Key - rowIndex; i++)
                    {
                        AppendString(sbHtml, "<tr></tr>");
                    }
                    rowIndex = row.Key;
                }
                AppendString(sbHtml, "<tr>");
                colIndex = 0;
                foreach (var item in row.OrderBy(d => d.Column))
                {
                    if (item.Column > colIndex)
                    {
                        for (int i = 0; i < item.Column - colIndex; i++)
                        {
                            AppendString(sbHtml, "<td></td>");
                        }
                        colIndex = item.Column;
                    }
                    AppendString(sbHtml, item.ColumnSpan > 1 ? string.Format("<td colspan=\"{0}\">", item.ColumnSpan) : "<td >");
                    AppendControl(sbHtml, item, vm);
                    AppendString(sbHtml, "</td>");
                    colIndex = item.ColumnSpan > 1 ? colIndex + item.ColumnSpan : colIndex + 1;
                }
                AppendString(sbHtml, "</tr>");
                rowIndex++;
            }
            AppendString(sbHtml, "</table>");
        }

        public static void AppendHtmlStart(System.Text.StringBuilder sbHtml, ControlBase control, string tag)
        {
            AppendString(sbHtml, string.Format("<{0}{1}>", tag, control.GetBaseAttribtes()));
        }
        public static void AppendHtmlEnd(System.Text.StringBuilder sbHtml, string tag)
        {
            AppendString(sbHtml, string.Format("</{0}>", tag));
        }

        public static void AppendLabel(System.Text.StringBuilder sbHtml, Label control, FormActionViewModel vm)
        {
            AppendHtmlStart(sbHtml, control, "label");
            AppendString(sbHtml, control.Title);
            AppendHtmlEnd(sbHtml, "label");
        }

        public static void AppendButton(System.Text.StringBuilder sbHtml, Button control, FormActionViewModel vm)
        {
            AppendString(sbHtml, string.Format("<a{1} href=\"#\" {2}{3}>{0}</a>",
                control.Title, control.GetBaseAttribtes(),
                control.Action.ToFormatIfNotEmpty(" onclick=\"{0}\""),
                control.Options.ToFormatIfNotEmpty(" data-options=\"{0}\"")));
        }

        public static void AppendTextBox(System.Text.StringBuilder sbHtml, TextBox control, FormActionViewModel vm)
        {
            var value = GetParameterValue(sbHtml, control.Binding, vm);
            AppendString(sbHtml, string.Format("<input{0} {1}{2}/>", control.GetBaseAttribtes(),
                control.Options.ToFormatIfNotEmpty(" data-options=\"{0}\""),
                value == null ? "" : string.Format(" value=\"{0}\"", value)));
        }

        public static void AppendComboBox(System.Text.StringBuilder sbHtml, ComboBox control, FormActionViewModel vm)
        {
            var value = GetParameterValue(sbHtml, control.Binding, vm);
            AppendString(sbHtml, string.Format("<input{0} {1} >", control.GetBaseAttribtes(),
                control.Options.ToFormatIfNotEmpty(" data-options=\"{0}\"")));

            var itemsSource = GetParameterValue(sbHtml, control.ItemsSource, vm);
            if (itemsSource != null)
            {
                if (itemsSource is string)
                {
                    foreach (var item in (itemsSource as string).ToDictionary())
                    {
                        AppendString(sbHtml, string.Format("<option value=\"{0}\"{2}>{1}</option>", item.Key, item.Value
                            , (value != null && value.Equals(item.Key)) ? " selected" : ""));
                    }
                }
                else if (itemsSource is System.Collections.IEnumerable)
                {
                    foreach (var item in (itemsSource as System.Collections.IEnumerable))
                    {
                        var pValue = item.GetPathValue(control.ValueMember);
                        AppendString(sbHtml, string.Format("<option value=\"{0}\"{2}>{1}</option>",
                            item.GetPathValue(control.ValueMember), item.GetPathValue(control.DisplayMemeber)
                            , (value != null && value.Equals(pValue)) ? " selected" : ""));
                    }
                }
                else
                {
                    throw new Exception("不支持的数据源类型");
                }
            }
            AppendString(sbHtml, "</select>");
        }

        public static void AppendGridView(System.Text.StringBuilder sbHtml, GridView control, FormActionViewModel vm)
        {
            AppendString(sbHtml, string.Format("<table{0} {1} >", control.GetBaseAttribtes(),
                control.Options.ToFormatIfNotEmpty(" data-options=\"{0}\"")));

            AppendString(sbHtml, "<thead>");
            AppendString(sbHtml, "<tr>");
            if (!string.IsNullOrEmpty(control.Key))
            {
                AppendString(sbHtml, string.Format("<th data-options=\"field:'{0}',checkbox:true\"></th>", control.Key));
            }
            foreach (var column in control.Columns)
            {
                AppendString(sbHtml, string.Format("<th data-options=\"field:'{0}',fixed:true,\">{1}</th>", column.FieldName, column.Title));
            }
            AppendString(sbHtml, "</tr>");
            AppendString(sbHtml, "</thead>");
            AppendHtmlEnd(sbHtml, "table");
        }

        public static void AppendControl(System.Text.StringBuilder sbHtml, ControlBase control, FormActionViewModel vm)
        {
            switch (control.GetType().Name)
            {
                case "Label": AppendLabel(sbHtml, (Label)control, vm); break;
                case "Button": AppendButton(sbHtml, (Button)control, vm); break;
                case "TextBox": AppendTextBox(sbHtml, (TextBox)control, vm); break;
                case "CheckBox": AppendControl(sbHtml, (CheckBox)control, vm); break;
                case "ComboBox": AppendComboBox(sbHtml, (ComboBox)control, vm); break;
                case "PickBox": AppendControl(sbHtml, (PickBox)control, vm); break;
                case "DateBox": AppendControl(sbHtml, (DateBox)control, vm); break;
                case "ImageBox": AppendControl(sbHtml, (ImageBox)control, vm); break;
                case "GridView": AppendGridView(sbHtml, (GridView)control, vm); break;
                case "DataView": AppendDataView(sbHtml, (DataView)control, vm); break;
                case "StackPanel": AppendPanel(sbHtml, (StackPanel)control, vm); break;
                case "ToolBar": AppendPanel(sbHtml, (ToolBar)control, vm); break;
                case "GridBox": AppendGridBox(sbHtml, (GridBox)control, vm); break;
                default:
                    break;
            }
        }

        public static object GetParameterValue(System.Text.StringBuilder sbHtml, string path, FormActionViewModel vm)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            return vm.Parameters.GetDictionaryValue(path, null);
        }

        public static void AppendString(System.Text.StringBuilder sb, string str)
        {
            //if (str.StartsWith("<"))
            //{
            //    sb.Append("\r\n");
            //}
            sb.Append(str);
        }

        public static HtmlString CreateForm(Form form, FormActionViewModel vm)
        {
            var sbHtml = new System.Text.StringBuilder();
            AppendGridBox(sbHtml, form,vm);
            return new HtmlString(sbHtml.ToString());
        }

        #endregion
    }
}
