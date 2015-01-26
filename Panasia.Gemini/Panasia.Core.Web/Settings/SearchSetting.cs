using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Panasia.Core;

namespace Panasia.Core.Web
{
    public static class SearchFields
    {
        [ValueConverter("CreateSearchField", "创建搜索列参数")]
        public static string CreateSearchField(object value, object parameter)
        {
            if (parameter == null)
            {
                throw new Exception("搜索列转换参数不能为空");
            }
            var paras = (parameter == null ? "" : parameter.ToString())
                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (paras == null || paras.Length == 0)
            {
                throw new Exception("搜索列转换参数不能为空");
            }

            var field = new SearchField { Name = paras[0] };
            if (paras.Length > 1)
            {
                field.IsNumber = paras[1].Equals("1") || paras[1].ToLower().Equals("true");
            }
            var tableName = paras.Length > 2 ? paras[2] : "";

            var strValue = value == null ? "" : value.ToString().Trim();

            if (string.IsNullOrEmpty(strValue))
            {
                return "";
            }

            if (strValue.IndexOf(',') < 0)
            {
                if (field.IsNumber)
                {
                    field.Compare = "=";
                    field.Value = strValue;
                }
                else
                {
                    field.Compare = "%";
                    field.Value = strValue;
                }
            }
            else
            {
                var values = strValue.Split(',');
                if (values.Length > 0)
                {
                    field.Compare = values[0];
                }
                if (values.Length > 1)
                {
                    field.Value = values[1];
                }
                if (values.Length > 2)
                {
                    field.Value2 = values[2];
                }
            }
            return field.CreateWhere(tableName);
        }
    }

    #region 搜索字段
    public class SearchField : EntityBase
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

        #region 属性 Compare
        private string _Compare = "=";
        [XmlAttribute("Compare"), DefaultValue("=")]
        public string Compare
        {
            get
            {
                return _Compare;
            }
            set
            {
                _Compare = value;
                RaisePropertyChanged(() => Compare);
            }
        }
        #endregion

        #region 属性 Value
        private string _Value = string.Empty;
        [XmlAttribute("Value"), DefaultValue("")]
        public string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
                RaisePropertyChanged(() => Value);
            }
        }
        #endregion

        #region 属性 Value2
        private string _Value2 = string.Empty;
        [XmlAttribute("Value2"), DefaultValue("")]
        public string Value2
        {
            get
            {
                return _Value2;
            }
            set
            {
                _Value2 = value;
                RaisePropertyChanged(() => Value2);
            }
        }
        #endregion

        #region 属性 IsNumber
        private bool _IsNumber = false;
        [XmlAttribute("IsNumber"), DefaultValue(false)]
        public bool IsNumber
        {
            get
            {
                return _IsNumber;
            }
            set
            {
                _IsNumber = value;
                RaisePropertyChanged(() => IsNumber);
            }
        }
        #endregion

        public string CreateWhere(string tableName = "")
        {
            var columnName = string.IsNullOrEmpty(tableName) ? string.Format("[{0}]", Name) : string.Format("{0}.[{1}]", tableName, Name);
            switch (Compare.ToLower())
            {
                case ">":
                    return string.Format("{0} > {1}", columnName, GetValue());
                case "<":
                    return string.Format("{0} < {1}", columnName, GetValue());
                case ">=":
                    return string.Format("{0} >= {1}", columnName, GetValue());
                case "<=":
                    return string.Format("{0} <= {1}", columnName, GetValue());
                case "%":
                    return string.Format("{0} LIKE '%{1}%'", columnName, Value);
                case "<>":
                case "><":
                    return string.Format("{0} >= {1} AND {0} <= {2} ", columnName, Value, Value2);
                case "=":
                default:
                    return string.Format("{0} = {1}", columnName, GetValue());
            }
        }

        private string GetValue()
        {
            return IsNumber ? Value : string.Format("'{0}'", Value);
        }

        private string GetValue2()
        {
            return IsNumber ? Value2 : string.Format("'{0}'", Value2);
        }

        public override string ToString()
        {
            return CreateWhere();
        }
    }
    #endregion
}
