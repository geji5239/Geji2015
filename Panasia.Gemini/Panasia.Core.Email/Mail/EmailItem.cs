using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Panasia.Core.Email
{
    [Serializable()]
	public class EmailItem
	{
        #region attribute Name
        // Name
        private string _Name = string.Empty;

        /// <summary> 
        /// Name 
        /// </summary>  
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
                }
            }
        }
        #endregion //attribute Name

        #region attribute Email
        // Email
        private string _Email = string.Empty;

        /// <summary> 
        /// Email 
        /// </summary>  
        [XmlAttribute("Email")]
        public string Email
        {
            get
            {
                return _Email;
            }
            set
            {
                if (!_Email.Equals(value))
                {
                    _Email = value; 
                }
            }
        }
        #endregion //attribute Email

        #region attribute Encoding
        // Encoding
        private string _Encoding = "utf-8";

        /// <summary> 
        /// Encoding 
        /// </summary>  
        [XmlAttribute("Encoding")]
        public string Encoding
        {
            get
            {
                return _Encoding;
            }
            set
            {
                if (!_Encoding.Equals(value))
                {
                    _Encoding = value; 
                }
            }
        }
        #endregion //attribute Encoding

	}
}
