using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Panasia.Core.Email
{    
    [XmlRoot("SendItem")]
    [Serializable()]
	public class SendItem
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
                _Name = value;  
            }
        }
        #endregion //attribute Name

        #region attribute Priority
        // Priority
        private System.Net.Mail.MailPriority _Priority =  System.Net.Mail.MailPriority.High;

        /// <summary> 
        /// Priority 
        /// </summary> 
        ///[DataMember()]
        [XmlAttribute("Priority")]
        public System.Net.Mail.MailPriority Priority
        {
            get
            {
                return _Priority;
            }
            set
            { 
                _Priority = value; 
            }
        }
        #endregion //attribute Priority

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
                _Remark = value;  
            }
        }
        #endregion //attribute Remark

        #region attribute From
        // From
        private List<EmailItem> _From = null;

        /// <summary> 
        /// From 
        /// </summary> 
        ///[DataMember()]
        [XmlArray("From"), XmlArrayItem("EmailItem", typeof(EmailItem))]
        public List<EmailItem> From
        {
            get
            {
                if(_From ==null)
                {
                    _From = new List<EmailItem>();
                }
                return _From;
            }
            set
            {
                _From = value;
            }
        }
        #endregion //attribute From

        #region attribute To
        // To
        private List<EmailItem> _To = null;

        /// <summary> 
        /// To 
        /// </summary> 
        ///[DataMember()]
        [XmlArray("To"), XmlArrayItem("EmailItem",typeof(EmailItem))]
        public List<EmailItem> To
        {
            get
            {
                if(_To ==null)
                {
                    _To = new List<EmailItem>();
                }
                return _To;
            }
            set
            { 
                _To = value; 
            }
        }
        #endregion //attribute To

        #region attribute Cc
        // Cc
        private List<EmailItem> _Cc = null;

        /// <summary> 
        /// Cc 
        /// </summary> 
        ///[DataMember()]
        [XmlArray("Cc"), XmlArrayItem("EmailItem", typeof(EmailItem))]
        public List<EmailItem> Cc
        {
            get
            {
                if(_Cc == null)
                {
                    _Cc = new List<EmailItem>();
                }
                return _Cc;
            }
            set
            { 
                _Cc = value; 
            }
        }
        #endregion //attribute Cc

        #region attribute BCc
        // BCc
        private List<EmailItem> _BCc = null;

        /// <summary> 
        /// BCc 
        /// </summary> 
        ///[DataMember()]
        [XmlArray("BCc"), XmlArrayItem("EmailItem", typeof(EmailItem))]
        public List<EmailItem> BCc
        {
            get
            {
                if(_BCc == null)
                {
                    _BCc = new List<EmailItem>();
                }
                return _BCc;
            }
            set
            { 
                _BCc = value; 
            }
        }
        #endregion //attribute BCc

        #region attribute Subject
        // Subject
        private Subject _Subject = null;

        /// <summary> 
        /// Subject 
        /// </summary> 
        ///[DataMember()]
        [XmlElement("Subject")]
        public Subject Subject
        {
            get
            {
                if(_Subject ==null)
                {
                    _Subject = new Subject();
                }
                return _Subject;
            }
            set
            { 
                _Subject = value;  
            }
        }
        #endregion //attribute Subject

        #region attribute Host
        // Host
        private Host _Host = null;

        /// <summary> 
        /// Host 
        /// </summary> 
        ///[DataMember()]
        [XmlElement("Host")]
        public Host Host
        {
            get
            {
                if(_Host == null)
                {
                    _Host = new Host();
                }
                return _Host;
            }
            set
            { 
                _Host = value;  
            }
        }
        #endregion //attribute Host

        #region attribute Credentials
        // Credentials
        private Credentials _Credentials = null;

        /// <summary> 
        /// Credentials 
        /// </summary> 
        ///[DataMember()]
        [XmlElement("Credentials")]
        public Credentials Credentials
        {
            get
            {
                if(_Credentials==null)
                {
                    _Credentials = new Credentials();
                }
                return _Credentials;
            }
            set
            { 
                _Credentials = value;  
            }
        }
        #endregion //attribute Credentials

        #region attribute Body
        // Body
        private Body _Body = null;

        /// <summary> 
        /// Body 
        /// </summary> 
        ///[DataMember()]
        [XmlElement("Body")]
        public Body Body
        {
            get
            {
                if(_Body == null)
                {
                    _Body = new Body();
                }
                return _Body;
            }
            set
            { 
                _Body = value;  
            }
        }
        #endregion //attribute Body

	}
}
