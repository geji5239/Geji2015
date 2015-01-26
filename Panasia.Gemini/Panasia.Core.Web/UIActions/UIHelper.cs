using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Panasia.Core;
using Panasia.Core.Sys;

namespace Panasia.Core.Web
{
    public static class UIHelper
    {
        #region 基本内容
        public static void AppendTagStart(this StringBuilder sb, string tag)
        {
            sb.AppendIfNotEmpty(tag, " <{0}");
        }

        public static void AppendTagEnd(this StringBuilder sb, string tag)
        {
            sb.AppendIfNotEmpty(tag, " </{0}>");
        }

        public static void AppendTagShortEnd(this StringBuilder sb)
        {
            sb.Append("/>");
        }


        /// <summary>
        /// 如果值为空，不添加；如果名称为空，添加值 value；如果名称和值都不为空，添加name="value"
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void AppendPropertiesIfNotEmpty(this StringBuilder sb, params string[] nameValues)
        {
            if (nameValues == null || nameValues.Length == 0)
            {
                return;
            }

            for (int i = 0; i < nameValues.Length - 1; i += 2)
            {
                sb.AppendPropertyIfNotEmpty(nameValues[i], nameValues[i + 1]);
            }
        }

        /// <summary>
        /// 如果值为空，不添加；如果名称为空，添加值 value；如果名称和值都不为空，添加name="value"
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void AppendPropertyIfNotEmpty(this StringBuilder sb, string name, object value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return;
            }
            if (string.IsNullOrEmpty(name))
            {
                sb.AppendFormat(" {0}", value);
            }
            else
            {
                sb.AppendFormat(" {0}=\"{1}\"", name, value);
            }
        }

        public static void AppendIfNotEmpty(this StringBuilder sb, object item, string format = "")
        {
            if (item == null || string.IsNullOrWhiteSpace(item.ToString()))
            {
                return;
            }
            if (string.IsNullOrEmpty(format))
            {
                sb.Append(item);
            }
            else
            {
                sb.AppendFormat(format, item);
            }
        }

        public static int GetCurrentUserPageActionValue(string pageID)
        {
            if (!string.IsNullOrEmpty(pageID))
            {
                var func = ServiceLocator.Current.GetContainer().GetExportedValueOrDefault<Func<string, int>>(SystemConsts.Func_GetCurrentUserPageActionValue);
                if (func != null)
                {
                    return func(pageID);
                }
            }
            return 0xFFFFFF;
        }
        #endregion

        public static UIElement GetParent<T>(this UIElement element) where T : UIElement
        {
            if (element.Parent == null)
            {
                return null;
            }
            if (element.Parent is T)
            {
                return (T)element.Parent;
            }
            return element.Parent.GetParent<T>();
        }

        public static object GetParameterValue(this object viewModel, object dataContext, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            if (path.StartsWith("/") || path.StartsWith("\\"))
            {
                return viewModel.GetParameterValue(path.Substring(1));
            }
            if (dataContext == null)
            {
                return viewModel.GetParameterValue(path);
            }
            return dataContext.GetParameterValue(path);
        }

        static object GetParameterValue(this object dataContext, string path)
        {
            object value = null;
            if (dataContext == null)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    "GetParameterValue".Log(string.Format("datacontext is null,path={0}", path));
                }
                return null;
            }
            if (string.IsNullOrEmpty(path))
            {
                "GetParameterValue".Log("path is null");
                return null;
            }
            value = dataContext.GetPathValue(path);
            if (value!=null && (!value.GetType().IsClass))
            {
                "GetParameterValue".Log(string.Format("path={0},value={1}", path,value==null?"":value));
            }
            return value;
        }

        public static string ReplaceContextParameters(this string source, object viewModel,object context)
        {
            return source.ReplaceParameters((path) =>
                {
                    var v = viewModel.GetParameterValue(context, path);
                    return v == null ? "" : v.ToString();
                }); 
        }
    }
}
