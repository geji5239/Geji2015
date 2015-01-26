using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Panasia.Core.Email
{
    [Serializable()]
    public class Subject
	{
        #region attribute Content
        // Content
        private string _Content = string.Empty;

        /// <summary> 
        /// Content 
        /// </summary> 
        ///[DataMember()]
        [XmlAttribute("Content")]
        public string Content
        {
            get
            {
                return _Content;
            }
            set
            {
                if (!_Content.Equals(value))
                {
                    _Content = value;
                    ///RaisePropertyChanged("Content");
                }
            }
        }
        #endregion //attribute Content

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
