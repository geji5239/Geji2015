using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Panasia.Core.Email
{
    [Serializable()]
    public class Host
	{
        #region attribute Type
        // Type
        private HostType _Type =  HostType.Exchange;

        /// <summary> 
        /// Host Type 
        /// </summary>  
        [XmlAttribute("Type")]
        public HostType Type
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


        #region attribute Address
        // Address
        private string _Address = string.Empty;

        /// <summary> 
        /// Address 
        /// </summary>  
        [XmlAttribute("Address")]
        public string Address
        {
            get
            {
                return _Address;
            }
            set
            { 
                _Address = value; 
            }
        }
        #endregion //attribute Address

        #region attribute Port
        // Port
        private string _Port = string.Empty;

        /// <summary> 
        /// Port 
        /// </summary> 
        ///[DataMember()]
        [XmlAttribute("Port")]
        public string Port
        {
            get
            {
                return _Port;
            }
            set
            { 
                _Port = value;  
            }
        }
        #endregion //attribute Port

        #region attribute EnableSsl
        // EnableSsl
        private bool _EnableSsl = false;

        /// <summary> 
        /// EnableSsl 
        /// </summary> 
        ///[DataMember()]
        [XmlAttribute("EnableSsl")]
        public bool EnableSsl
        {
            get
            {
                return _EnableSsl;
            }
            set
            { 
                _EnableSsl = value;  
            }
        }
        #endregion //attribute EnableSsl
	}

    public enum HostType
    {
        Exchange,
        Smtp,
        Local
    }
}
