using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Panasia.Core;

namespace Panasia.Core.AppTest
{
    public class TestParameter:EntityBase
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

        #region 属性 Path
        private string _Path = string.Empty;
        [XmlAttribute("Path"), DefaultValue("")]
        public string Path
        {
            get
            {
                return _Path;
            }
            set
            {
                _Path = value;
                RaisePropertyChanged(() => Path);
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
    } 

    public class TestParameterCollection:CollectionBase<TestParameter>
    {
        public TestParameter this[string name]
        {
            get
            {
                return Items.FirstOrDefault(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
