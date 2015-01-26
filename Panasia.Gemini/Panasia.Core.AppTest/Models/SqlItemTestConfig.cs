using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Panasia.Core;
using Panasia.Core.App;

namespace Panasia.Core.AppTest
{
    public class SqlItemTestConfig : EntityBase, ITestJobConfig
    {
        [XmlIgnore]
        public TestJob TestJob { get; set; }

        [TestJobConfig("SqlItemTest","SQL命令测试")]
        public static SqlItemTestConfig CreateConfig(string configDoc)
        {
            try
            {
                return configDoc.ToXObject<SqlItemTestConfig>(); 
            }
            catch 
            {
                return new SqlItemTestConfig();
            }
        }

        #region 属性
        #region 属性 SqlFullName
        private string _SqlFullName = string.Empty;
        [XmlAttribute("SqlFullName"), DefaultValue("")]
        public string SqlFullName
        {
            get
            {
                return _SqlFullName;
            }
            set
            {
                _SqlFullName = value;
                RaisePropertyChanged(() => SqlFullName);
            }
        }
        #endregion

        #region 属性 Parameters
        private TestParameterCollection _Parameters = null;
        [XmlElement("Parameter", typeof(TestParameter))]
        public TestParameterCollection Parameters
        {
            get
            {
                if (_Parameters == null)
                {
                    _Parameters = new TestParameterCollection();
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

        #region 属性 ExecuteType
        private SqlExecuteType _ExecuteType = SqlExecuteType.Query;
        [XmlAttribute("ExecuteType")]
        public SqlExecuteType ExecuteType
        {
            get
            {
                return _ExecuteType;
            }
            set
            {
                _ExecuteType = value;
                RaisePropertyChanged(() => ExecuteType);
            }
        }
        #endregion

        #endregion

        public TestResult Execute()
        {
            var result = new SqlResult();
            try
            {
                var sqlnames = SqlFullName.Split(',');
                if(sqlnames.Length!=3)
                {
                    throw new Exception("命令名称错误");
                }
                var sqlItem = SqlData.Current.GetSqlItem(sqlnames[0], sqlnames[1], sqlnames[2]);
                if(sqlItem==null)
                {
                    throw new Exception("SQL命令不存在");                    
                }
                
                switch (ExecuteType)
                {
                    case SqlExecuteType.Query:
                        result.Items= sqlItem.ExecuteQuery<Entity>(Parameters.GetNameValues().ToArray());
                        break;
                    case SqlExecuteType.NoneQuery:
                        result.Count = sqlItem.ExecuteNoneQuery(Parameters.GetNameValues().ToArray());
                        break;
                    case SqlExecuteType.Scalar:
                        result.Value = sqlItem.ExecuteScalar(Parameters.GetNameValues().ToArray());
                        break;
                    case SqlExecuteType.QueryJson:
                        result.Json = sqlItem.ExecuteJson(Parameters.GetNameValues().ToArray());
                        break;
                    default:
                        break;
                }

                result.Description = "命令执行成功";
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.ErrorMessage = ex.ToString();
            }
            return result;
        }
    }

    public class SqlResult:TestResult
    {
        #region 属性 Items
        private IEnumerable<Entity> _Items = null;
        public IEnumerable<Entity> Items
        {
            get
            {
                return _Items;
            }
            set
            {
                _Items = value;
                RaisePropertyChanged(() => Items);
                Count = Items == null ? 0 : Items.Count();
            }
        }
        #endregion

        #region 属性 Count
        private int _Count = 0;
        public int Count
        {
            get
            {
                return _Count;
            }
            set
            {
                _Count = value;
                RaisePropertyChanged(() => Count);
            }
        }
        #endregion

        #region 属性 Value
        private object _Value = null;
        public object Value
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

        #region 属性 Json
        private string _Json = string.Empty;
        public string Json
        {
            get
            {
                return _Json;
            }
            set
            {
                _Json = value;
                RaisePropertyChanged(() => Json);
            }
        }
        #endregion

    }

    public enum SqlExecuteType
    {
        Query,
        NoneQuery,
        Scalar,
        QueryJson
    }
}
