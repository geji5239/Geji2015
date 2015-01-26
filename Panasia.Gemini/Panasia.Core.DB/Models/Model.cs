using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Panasia.Core;

namespace AF.DB.Models
{
    public class Model:EntityBase
    {
        //输入：请求参数

        //处理数据：

        //显示
    }

    public class ModelTable:EntityBase
    {
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
            }
        }
        #endregion

        #region 属性 Columns
        private string _Columns = string.Empty;
        [XmlAttribute("Columns"), DefaultValue("")]
        public string Columns
        {
            get
            {
                return _Columns;
            }
            set
            {
                _Columns = value;
                RaisePropertyChanged(() => Columns);
            }
        }
        #endregion

        #region 属性 IsReferance
        private bool _IsReferance = false;
        [XmlAttribute("IsReferance"), DefaultValue(false)]
        public bool IsReferance
        {
            get
            {
                return _IsReferance;
            }
            set
            {
                _IsReferance = value;
                RaisePropertyChanged(() => IsReferance);
            }
        }
        #endregion
    }

    public class ModelObject:EntityBase
    {
        #region 属性 Header
        private string _Header = string.Empty;
        [XmlAttribute("Header"), DefaultValue("")]
        public string Header
        {
            get
            {
                return _Header;
            }
            set
            {
                _Header = value;
                RaisePropertyChanged(() => Header);
            }
        }
        #endregion

        #region 属性 Fields
        private CollectionBase<ModelField> _Fields = null;
        [XmlArray("Fields"), XmlArrayItem("Field",typeof(ModelField))]
        public CollectionBase<ModelField> Fields
        {
            get
            {
                if(_Fields==null)
                {
                    _Fields = new CollectionBase<ModelField>();
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

        #region 属性 IsList
        private bool _IsList = false;
        [XmlAttribute("IsList"), DefaultValue(false)]
        public bool IsList
        {
            get
            {
                return _IsList;
            }
            set
            {
                _IsList = value;
                RaisePropertyChanged(() => IsList);
            }
        }
        #endregion

        #region 属性 Editor
        private string _Editor = string.Empty;
        [XmlAttribute("Editor"), DefaultValue("")]
        public string Editor
        {
            get
            {
                return _Editor;
            }
            set
            {
                _Editor = value;
                RaisePropertyChanged(() => Editor);
            }
        }
        #endregion
    }

    public class ModelField:EntityBase
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

        #region 属性 DisplayName
        private string _DisplayName = string.Empty;
        [XmlAttribute("DisplayName"), DefaultValue("")]
        public string DisplayName
        {
            get
            {
                return _DisplayName;
            }
            set
            {
                _DisplayName = value;
                RaisePropertyChanged(() => DisplayName);
            }
        }
        #endregion
        
        #region 属性 Editor
        private string _Editor = string.Empty;
        [XmlAttribute("Editor"), DefaultValue("")]
        public string Editor
        {
            get
            {
                return _Editor;
            }
            set
            {
                _Editor = value;
                RaisePropertyChanged(() => Editor);
            }
        }
        #endregion

        #region 属性 IsVisible
        private bool _IsVisible = true;
        [XmlAttribute("IsVisible"), DefaultValue(true)]
        public bool IsVisible
        {
            get
            {
                return _IsVisible;
            }
            set
            {
                _IsVisible = value;
                RaisePropertyChanged(() => IsVisible);
            }
        }
        #endregion
    }

    
}
