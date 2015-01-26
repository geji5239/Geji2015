/*
 * 这个是为了动态扩展控件的配置而改进的，暂时没有使用
 */ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Panasia.Core.Web
{
    public class ControlBox:ControlBase
    {
        #region 属性 ControlType
        private string _ControlType = string.Empty;
        [XmlAttribute("ControlType")]
        public string ControlType
        {
            get
            {
                return _ControlType;
            }
            set
            {
                _ControlType = value;
                RaisePropertyChanged(() => ControlType);
            }
        }
        #endregion

        #region 属性 ControlDoc
        private string _ControlDoc = string.Empty;
        [XmlText]
        public string ControlDoc
        {
            get
            {
                return _ControlDoc;
            }
            set
            {
                _ControlDoc = value;
                RaisePropertyChanged(() => ControlDoc);
            }
        }
        #endregion
    }
}
