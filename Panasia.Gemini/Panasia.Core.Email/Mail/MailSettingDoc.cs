using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Panasia.Core.Email
{
    [XmlRoot("Data")]
    public class MailSettingDoc
    {
        #region attribute Type
        // Type
        private string _Type = "EmailSendItem";

        /// <summary> 
        /// Type 
        /// </summary>  
        [XmlAttribute("Type")]
        public string Type
        {
            get
            {
                return _Type;
            }
            set
            { 
                _Type = value;  
            }
        }
        #endregion //attribute Type

        #region attribute SendItem
        // SendItem
        private SendItem _SendItem = new SendItem();

        /// <summary> 
        /// SendItem 
        /// </summary>  
        [XmlElement("SendItem")]
        public SendItem SendItem
        {
            get
            {
                return _SendItem;
            }
            set
            { 
                _SendItem = value; 
            }
        }
        #endregion //attribute SendItem
    }
}
