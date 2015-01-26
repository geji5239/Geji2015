using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Panasia.Core.Email
{
    [Serializable()]
    public class Body
	{
        #region attribute IsBodyHtml
        // IsBodyHtml
        private bool _IsBodyHtml = true;

        /// <summary> 
        /// IsBodyHtml 
        /// </summary>  
        [XmlAttribute("IsBodyHtml")]
        public bool IsBodyHtml
        {
            get
            {
                return _IsBodyHtml;
            }
            set
            { 
                _IsBodyHtml = value;  
            }
        }
        #endregion //attribute IsBodyHtml
        
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

        #region attribute Content
        // Content
        private string _Content = string.Empty;

        /// <summary> 
        /// Content 
        /// </summary>  
        [XmlAttribute("Content")]
        public string Content
        {
            get
            {
                return _Content;
            }
            set
            { 
                _Content = value;  
            }
        }
        #endregion //attribute Content
	}
}
