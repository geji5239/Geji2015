using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Panasia.Core.Email
{
    [Serializable()]
    public class Credentials
	{
        #region attribute UserID
        // UserID
        private string _UserID = string.Empty;

        /// <summary> 
        /// UserID 
        /// </summary> 
        ///[DataMember()]
        [XmlAttribute("UserID")]
        public string UserID
        {
            get
            {
                return _UserID;
            }
            set
            { 
                _UserID = value;  
            }
        }
        #endregion //attribute UserID

        #region attribute Password
        // Password
        private string _Password = string.Empty;

        /// <summary> 
        /// Password 
        /// </summary> 
        ///[DataMember()]
        [XmlAttribute("Password")]
        public string Password
        {
            get
            {
                return _Password;
            }
            set
            { 
                _Password = value;  
            }
        }
        #endregion //attribute Password

	}
}
