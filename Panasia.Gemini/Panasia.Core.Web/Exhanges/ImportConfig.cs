using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Panasia.Core;
using Panasia.Core.App;

namespace Panasia.Core.Web
{
    public class ExcelToSql:EntityBase
    {
        #region 属性 Source
        private ExcelSource _Source = null;
        [XmlElement("Source")]
        public ExcelSource Source
        {
            get
            {
                if(_Source ==null)
                {
                    _Source = new ExcelSource();
                }
                return _Source;
            }
            set
            {
                _Source = value;
                RaisePropertyChanged(() => Source);
            }
        }
        #endregion

        #region 属性 Target
        private SqlTarget _Target = null;
        [XmlElement("Target")]
        public SqlTarget Target
        {
            get
            {
                if(_Target == null)
                {
                    _Target = new SqlTarget();
                }
                return _Target;
            }
            set
            {
                _Target = value;
                RaisePropertyChanged(() => Target);
            }
        }
        #endregion
    }

    public class ExcelSource:ImportSource
    { 
        #region 属性 SheetName
        private string _SheetName = string.Empty;
        [XmlAttribute("SheetName"), DefaultValue("")]
        public string SheetName
        {
            get
            {
                return _SheetName;
            }
            set
            {
                _SheetName = value;
                RaisePropertyChanged(() => SheetName);
            }
        }
        #endregion
    }

    /* 导入的SQL语句约定
     * 1、SQL采用参数形式，参数名与导入字段名相同，另外可以使用其它系统运行参数；
     * 2、逐行SQL执行后返回三个值（可扩展）：0、不更新；1、新增成功；2、更新成功；
     */ 
    public class SqlTarget:EntityBase
    {
        #region 属性 SqlRowImport
        private SqlConfig _SqlRowImport = null;
        [XmlElement("SqlRowImport")]
        public SqlConfig SqlRowImport
        {
            get
            {
                if(_SqlRowImport == null)
                {
                    _SqlRowImport = new SqlConfig();
                }
                return _SqlRowImport;
            }
            set
            {
                _SqlRowImport = value;
                RaisePropertyChanged(() => SqlRowImport);
            }
        }
        #endregion
    }

    public class SqlConfig:EntityBase
    {
        #region 属性 GroupName
        private string _GroupName = string.Empty;
        [XmlAttribute("GroupName"), DefaultValue("")]
        public string GroupName
        {
            get
            {
                return _GroupName;
            }
            set
            {
                _GroupName = value;
                RaisePropertyChanged(() => GroupName);
                SqlItem = null;
            }
        }
        #endregion

        #region 属性 TableName
        private string _TableName = string.Empty;
        [XmlAttribute("TableName"), DefaultValue("")]
        public string TableName
        {
            get
            {
                return _TableName;
            }
            set
            {
                _TableName = value;
                RaisePropertyChanged(() => TableName);
                SqlItem = null;
            }
        }
        #endregion

        #region 属性 CommandName
        private string _CommandName = string.Empty;
        [XmlAttribute("CommandName")]
        public string CommandName
        {
            get
            {
                return _CommandName;
            }
            set
            {
                _CommandName = value;
                RaisePropertyChanged(() => CommandName);
                SqlItem = SqlData.Current.GetSqlItem(GroupName, TableName, CommandName);
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

        #region 属性 SqlItem
        private SqlItem _SqlItem = null;
        [XmlIgnore()]
        public SqlItem SqlItem
        {
            get
            {
                if (_SqlItem == null)
                {
                    _SqlItem = SqlData.Current.GetSqlItem(GroupName, TableName, CommandName);
                }
                return _SqlItem;
            }
            private set
            {
                _SqlItem = value;
                RaisePropertyChanged(() => SqlItem);
            }
        }
        #endregion
    }

    public class ImportSource:EntityBase
    {
        #region 属性 RowFrom
        private int _RowFrom = 0;
        /// <summary>
        /// 原表数据行开始
        /// </summary>
        [XmlAttribute("RowFrom"), DefaultValue(0)]
        public int RowFrom
        {
            get
            {
                return _RowFrom;
            }
            set
            {
                _RowFrom = value;
                RaisePropertyChanged(() => RowFrom);
            }
        }
        #endregion

        #region 属性 TitleRow
        private int _TitleRow = 0;
        /// <summary>
        /// 标题行序号
        /// </summary>
        [XmlAttribute("TitleRow"), DefaultValue(0)]
        public int TitleRow
        {
            get
            {
                return _TitleRow;
            }
            set
            {
                _TitleRow = value;
                RaisePropertyChanged(() => TitleRow);
            }
        }
        #endregion

        #region 属性 Fields
        private CollectionBase<ImportField> _Fields = null;
        [XmlElement("Field",typeof(ImportField))]
        public CollectionBase<ImportField> Fields
        {
            get
            {
                if (_Fields == null)
                {
                    _Fields = new CollectionBase<ImportField>();
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
    }

    public class ImportField:EntityBase
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

        #region 属性 Index
        private int _Index = -1;
        /// <summary>
        /// 如果Index=-1说明未定义
        /// </summary>
        [XmlAttribute("Index"), DefaultValue(-1)]
        public int Index
        {
            get
            {
                return _Index;
            }
            set
            {
                _Index = value;
                RaisePropertyChanged(() => Index);
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
                return _Title;
            }
            set
            {
                _Title = value;
                RaisePropertyChanged(() => Title);
            }
        }
        #endregion

        #region 属性 IsRequired
        private bool _IsRequired = true;
        [XmlAttribute("IsRequired"), DefaultValue(true)]
        public bool IsRequired
        {
            get
            {
                return _IsRequired;
            }
            set
            {
                _IsRequired = value;
                RaisePropertyChanged(() => IsRequired);
            }
        }
        #endregion

        #region 属性 AllowEmpty
        private bool _AllowEmpty = true;
        [XmlAttribute("AllowEmpty"), DefaultValue(true)]
        public bool AllowEmpty
        {
            get
            {
                return _AllowEmpty;
            }
            set
            {
                _AllowEmpty = value;
                RaisePropertyChanged(() => AllowEmpty);
            }
        }
        #endregion

        #region 属性 Converter
        private string _Converter = "";
        [XmlAttribute("Converter"), DefaultValue("")]
        public string Converter
        {
            get
            {
                return _Converter;
            }
            set
            {
                _Converter = value;
                RaisePropertyChanged(() => Converter);
            }
        }
        #endregion

        #region 属性 ConverterParameter
        private string _ConverterParameter = "";
        [XmlAttribute("ConverterParameter"), DefaultValue("")]
        public string ConverterParameter
        {
            get
            {
                return _ConverterParameter;
            }
            set
            {
                _ConverterParameter = value;
                RaisePropertyChanged(() => ConverterParameter);
            }
        }
        #endregion
    }
}
