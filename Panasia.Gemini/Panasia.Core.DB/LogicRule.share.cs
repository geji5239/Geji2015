using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Panasia.Core
{
    [XmlRoot("Rule")]
    public class LogicRule : Entity
    {
        public LogicRule()
            : base()
        {
        }

        public LogicRule(string key, string value)
        {
            LogicType = LogicType.Equals;
            LeftKey = key;
            Value = value;
        }

        public LogicRule(LogicType logicType, string key, string value, string value2 = "")
        {
            LogicType = logicType;
            LeftKey = key;
            Value = value;
            Value2 = value2;
        }

        public LogicRule(LogicType logicType, params LogicRule[] rules)
        {
            LogicType = logicType;
            if (rules != null)
            {
                foreach (var rule in rules)
                {
                    Rules.Add(rule);
                }
            }
        }

        public string ToWhere()
        {
            StringBuilder sb = new StringBuilder();
            switch (LogicType)
            {
                case LogicType.Equals:
                    sb.AppendFormat("{0}={1}", LeftKey, Value);
                    break;
                case LogicType.LessThan:
                    sb.AppendFormat("{0}<{1}", LeftKey, Value);
                    break;
                case LogicType.LessEqualsThan:
                    sb.AppendFormat("{0}<={1}", LeftKey, Value);
                    break;
                case LogicType.GreaterThan:
                    sb.AppendFormat("{0}>{1}", LeftKey, Value);
                    break;
                case LogicType.GreaterEuqalsThan:
                    sb.AppendFormat("{0}>={1}", LeftKey, Value);
                    break;
                case LogicType.Between:
                    sb.AppendFormat("{0}>={1} and {0}<={2}", LeftKey, Value, Value2);
                    break;
                case LogicType.NotEquals:
                    sb.AppendFormat("{0}<>{1}", LeftKey, Value);
                    break;
                case LogicType.NotBetween:
                    sb.AppendFormat("{0}<{1} and {0}>{2}", LeftKey, Value, Value2);
                    break;
                case LogicType.StartWith:
                    sb.AppendFormat("{0} like '{1}%'", LeftKey, Value);
                    break;
                case LogicType.EndWith:
                    sb.AppendFormat("{0} like '%{1}'", LeftKey, Value);
                    break;
                case LogicType.Contains:
                    sb.AppendFormat("{0} like '%{1}%'", LeftKey, Value);
                    break;
                case LogicType.Or:
                    AppendRulesWhere(sb, "OR");
                    break;
                case LogicType.And:
                    AppendRulesWhere(sb, "AND");
                    break;
                case LogicType.In:
                    AppendIn(sb); 
                    break;
            }

            return sb.ToString();
        }

        private void AppendIn(StringBuilder sb)
        {
            var values = Value.Split(',');
            if (values.Length == 0)
                return;

            sb.AppendFormat("{0} in (",LeftKey);
            if (Value2.Length > 0)
            {
                for (int i = 0; i < values.Length-1; i++)
                {
                    sb.AppendFormat("{0},", string.Format(Value2, values[i]));
                }
                sb.AppendFormat("{0}", string.Format(Value2, values[values.Length - 1])); 
            }
            else
            {
                for (int i = 0; i < values.Length-1; i++)
                {
                    sb.AppendFormat("{0},", values[i]);
                }
                sb.AppendFormat("{0}",values[values.Length - 1]);  
            }
            sb.Append(")");
        }


        private void AppendRulesWhere(StringBuilder sb, string conn)
        {
            if (Rules == null || Rules.Count == 0)
            {
                throw new Exception("Rules is null.");
            }
            if (Rules.Count == 1)
            {
                throw new Exception("Rules count error.");
            }
            sb.Append("(");
            sb.AppendFormat("({0})", Rules[0].ToWhere());
            for (int i = 1; i < Rules.Count; i++)
            {
                sb.AppendFormat(" {0} ", conn);
                sb.AppendFormat("({0})", Rules[i].ToWhere());
            }
            sb.Append(")");
        }

        #region 属性 LogicType
        [XmlAttribute("LogicType")]
        public LogicType LogicType
        {
            get
            {
                return GetFieldValue<LogicType>("LogicType", LogicType.And);
            }
            set
            {
                SetFieldValue("LogicType", value);
            }
        }
        #endregion

        #region 属性 LeftKey
        [XmlAttribute("LeftKey")]
        public string LeftKey
        {
            get
            {
                return GetFieldValue<string>("LeftKey", string.Empty);
            }
            set
            {
                SetFieldValue("LeftKey", value);
            }
        }
        #endregion

        #region 属性 Value
        [XmlAttribute("Value")]
        public string Value
        {
            get
            {
                return GetFieldValue<string>("Value", string.Empty);
            }
            set
            {
                SetFieldValue("Value", value);
            }
        }
        #endregion

        #region 属性 Value2
        [XmlAttribute("Value2")]
        public string Value2
        {
            get
            {
                return GetFieldValue<string>("Value2", string.Empty);
            }
            set
            {
                SetFieldValue("Value2", value);
            }
        }
        #endregion

        #region 属性 Rules
        private CollectionBase<LogicRule> _Rules = null;
        [XmlElement("Rule", typeof(LogicRule))]
        public CollectionBase<LogicRule> Rules
        {
            get
            {
                if (_Rules == null)
                {
                    _Rules = new CollectionBase<LogicRule>();
                }
                return _Rules;
            }
            set
            {
                _Rules = value;
                RaisePropertyChanged("Rules");
            }
        }
        #endregion
    }

    public enum LogicType
    {
        And,
        Or,
        Equals,
        NotEquals,
        GreaterThan,
        LessThan,
        GreaterEuqalsThan,
        LessEqualsThan,
        Between,
        NotBetween,
        /// <summary>
        /// 注意：目前暂不处理@Name参数形式
        /// </summary>
        StartWith,
        /// <summary>
        /// 注意：目前暂不处理@Name参数形式
        /// </summary>
        EndWith,
        /// <summary>
        /// 注意：目前暂不处理@Name参数形式
        /// </summary>
        Contains,
        /// <summary>
        /// Value中放入值序列,以“,”分割;Value2中放入值的格式化字符，如果是数字，不需要格式化，如果是字符串或者日期，是需要格式化的;
        /// </summary>
        In
    }
}
