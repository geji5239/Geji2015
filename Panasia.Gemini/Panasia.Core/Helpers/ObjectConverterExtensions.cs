using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Panasia.Core
{
    public static class ObjectConverterExtensions
    {
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static bool IsNull(this object s)
        {
            return s == null;
        }

        public static bool IsNumber(this string s)
        {
            return (s.Where(c => c.IsNumber() || c.Equals('.')).Count()) == s.Length &&(!(s.Where(c=>c.Equals('.')).Count()>1)) ;
        }

        public static bool IsNumber(this char c)
        {
            return char.IsNumber(c);
        }

        public static bool EqualsOrdinalIgnoreCase(this string s, string t)
        {
            return s.Equals(t, StringComparison.OrdinalIgnoreCase);
        }

        public static decimal ToDecimal(this string s)
        {
            return Convert.ToDecimal(s);
        }
        public static decimal ToDecimalWithDefault(this string s,decimal defaultvalue)
        {
            decimal d = 0;
            if (decimal.TryParse(s, out d))
            {
                return d;
            }
            else
            {
                return defaultvalue;
            } 
        }

        public static double ToDouble(this string s)
        {
            return Convert.ToDouble(s);
        }
        public static double ToDoubleWithDefault(this string s, double defaultvalue)
        {
            double d = 0;
            if (double.TryParse(s, out d))
            {
                return d;
            }
            else
            {
                return defaultvalue;
            }
        }

        public static DateTime ToDateTime(this string s)
        {
            return Convert.ToDateTime(s);
        }
         

        public static DateTime ToDateTimeWithDefault(this string s, DateTime defaultvalue)
        {
            DateTime d = DateTime.Now;
            if (DateTime.TryParse(s, out d))
            {
                return d;
            }
            else
            {
                return defaultvalue;
            }
        }

        public static Int32 ToInt32(this string s)
        {
            return Convert.ToInt32(s);
        }
        public static Int32 ToInt32WithDefault(this string s, Int32 defaultvalue)
        {
            Int32 d = Int32.MinValue;
            if (Int32.TryParse(s, out d))
            {
                return d;
            }
            else
            {
                return defaultvalue;
            }
        }

        public static Int64 ToInt64(this string s)
        {
            return Convert.ToInt64(s);
        }
        
        /// <summary>
        /// 将字符串转换为Int64类型
        /// </summary>
        /// <param name="s">需要转换的数字字符串</param>
        /// <param name="defaultvalue">默认值</param>
        /// <returns>转换失败（如果输入字符串不是数字或者其它的原因）则返回默认值</returns>
        public static Int64 ToInt64WithDefault(this string s, Int64 defaultvalue)
        {
            Int64 d = Int64.MinValue;
            if (Int64.TryParse(s, out d))
            {
                return d;
            }
            else
            {
                return defaultvalue;
            }
        }

        /// <summary>
        /// 判断一个整数是否为奇数
        /// </summary>
        /// <param name="n">整数值</param>
        /// <returns>奇数返回true，偶数返回false</returns>
        public static bool IsOdd(this int n)
        {
            return (n % 2 == 1) ? true : false;
        }

        /// <summary>
        /// 判断一个整数是否为偶数
        /// </summary>
        /// <param name="n">整数值</param>
        /// <returns>偶数返回true,奇数返回false</returns>
        public static bool IsEven(this int n)
        {
            return (n % 2 == 0) ? true : false;
        }

        // 以下为Object类型转换
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this object s)
        {
            return Convert.ToDecimal(s);
        }
        public static decimal ToDecimalWithDefault(this object s, decimal defaultvalue)
        {
            decimal d = 0;
            if (s!=null&&decimal.TryParse(s.ToString(), out d))
            {
                return d;
            }
            else
            {
                return defaultvalue;
            }
        }

        public static double ToDouble(this object s)
        {
            return Convert.ToDouble(s);
        }
        public static double ToDoubleWithDefault(this object s, double defaultvalue)
        {
            double d = 0;
            if (s!=null&&double.TryParse(s.ToString(), out d))
            {
                return d;
            }
            else
            {
                return defaultvalue;
            }
        }

        public static DateTime ToDateTime(this object s)
        {
            return Convert.ToDateTime(s);
        }


        public static DateTime ToDateTimeWithDefault(this object s, DateTime defaultvalue)
        {
            DateTime d = DateTime.Now;
            if (s!=null&&DateTime.TryParse(s.ToString(), out d))
            {
                return d;
            }
            else
            {
                return defaultvalue;
            }
        }

        public static Int32 ToInt32(this object s)
        {
            return Convert.ToInt32(s);
        }

        public static Int32 ToInt32WithDefault(this object s, Int32 defaultvalue)
        {
            Int32 d = Int32.MinValue;
            if (s!=null&&Int32.TryParse(s.ToString(), out d))
            {
                return d;
            }
            else
            {
                return defaultvalue;
            }
        }

        public static Int64 ToInt64(this object s)
        {
            if (s == null || s is DBNull)
            {
                return 0;
            }
            if (s is Int64)
            {
                return (Int64)s;
            }
            return Convert.ToInt64(s);
        }

        public static Int64 ToInt64WithDefault(this object s, Int64 defaultvalue)
        {
            Int64 d = Int64.MinValue;
            if (s!=null&&Int64.TryParse(s.ToString(), out d))
            {
                return d;
            }
            else
            {
                return defaultvalue;
            }
        }

        // 转换货币类型
        public static string AsCurrency(this object s)
        {
            return "$" + string.Format("{0:N2}", s);// Math.Round(s.ToDecimal(), 2).ToString("0.00"); 
        }

        public static bool AsBool(this object s)
        {
            return Convert.ToBoolean(s);
        }

        public static string AsCurrency(this decimal s)
        {
            return "$" + string.Format("{0:N2}", s);//string.Format("{0:N2}", value)
        }

        public static string ToMergeString(this IEnumerable<object> items, string split = ",", string formatItem = "")
        {
            StringBuilder sb = new StringBuilder();
            if (items != null)
            {
                foreach (var item in items)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(split);
                    }
                    sb.Append(string.IsNullOrEmpty(formatItem) ? item.ToString() : string.Format(formatItem, item));
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 将字符串转换为参数列表，比如name1:value1|name2:value2，返回值为Key:name1 Value:value1 Key:name2 Value:value2
        /// </summary>
        /// <param name="source"></param>
        /// <param name="parameterSplit"></param>
        /// <param name="valueSplit"></param>
        /// <returns></returns>
        public static IDictionary<string, string> ToParameters(this string source, char parameterSplit = '|', char valueSplit = ':')
        {
            Dictionary<string, string> paras = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(source))
            {
                var ps = source.Split(parameterSplit);
                foreach (var p in ps)
                {
                    var vs = p.Trim().Split(valueSplit);
                    if (vs.Length >= 2)
                    {
                        paras.Add(vs[0], vs[1]);
                    }
                } 
            }
            return paras;
        }
            

        public static bool IsValidEmail(this string email)
        {
            bool flag = false;
            Regex reg = new Regex("\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*");
            if (reg.IsMatch(email))
            {
                flag = true;
            }
            return flag;
        }

        public static bool IsInLength(this string str, int min, int max)
        {
            bool flag = false;
            if (min == 0 && string.IsNullOrEmpty(str))
            {
                flag = true;
            }
            else if (str.Length >= min && str.Length <= max)
            {
                flag = true;
            }
            return flag;
        }
    }
}
