using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;


namespace Panasia.Core.Email
{
    [Serializable()]
    public class EventItem
    {
        #region attribute Name
        // Name
        private string _Name = string.Empty;

        /// <summary> 
        /// Name 
        /// </summary> 
        ///[DataMember()]
        [XmlAttribute("Name")]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (!_Name.Equals(value))
                {
                    _Name = value;
                    ///RaisePropertyChanged("Name");
                }
            }
        }
        #endregion //attribute Name

        #region attribute Code
        // Code
        private string _Code = string.Empty;

        /// <summary> 
        /// Code 
        /// </summary> 
        ///[DataMember()]
        [XmlAttribute("Code")]
        public string Code
        {
            get
            {
                return _Code;
            }
            set
            {
                if (!_Code.Equals(value))
                {
                    _Code = value;
                    ///RaisePropertyChanged("Code");
                }
            }
        }
        #endregion //attribute Code

        #region attribute SendedCode
        // SendedCode
        private string _SendedCode = string.Empty;

        /// <summary> 
        /// SendedCode 
        /// </summary> 
        ///[DataMember()]
        [XmlAttribute("SendedCode")]
        public string SendedCode
        {
            get
            {
                return _SendedCode;
            }
            set
            {
                if (!_SendedCode.Equals(value))
                {
                    _SendedCode = value;
                    ///RaisePropertyChanged("SendedCode");
                }
            }
        }
        #endregion //attribute SendedCode

        #region attribute MailSetting
        // MailSetting
        private string _MailSetting = string.Empty;

        /// <summary> 
        /// MailSetting 
        /// </summary> 
        ///[DataMember()]
        [XmlAttribute("MailSetting")]
        public string MailSetting
        {
            get
            {
                return _MailSetting;
            }
            set
            { 
                _MailSetting = value;  
            }
        }
        #endregion //attribute MailSetting
        
        #region attribute XslFile
        // XslFile
        private string _XslFile = string.Empty;

        /// <summary> 
        /// XslFile 
        /// </summary> 
        ///[DataMember()]
        [XmlAttribute("XslFile")]
        public string XslFile
        {
            get
            {
                return _XslFile;
            }
            set
            { 
                _XslFile = value; 
            }
        }
        #endregion //attribute XslFile

        #region attribute Remark
        // Remark
        private string _Remark = string.Empty;

        /// <summary> 
        /// Remark 
        /// </summary> 
        ///[DataMember()]
        [XmlAttribute("Remark")]
        public string Remark
        {
            get
            {
                return _Remark;
            }
            set
            {
                if (!_Remark.Equals(value))
                {
                    _Remark = value;
                    ///RaisePropertyChanged("Remark");
                }
            }
        }
        #endregion //attribute Remark
    }
}
