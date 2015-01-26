using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;


namespace Panasia.Core.Email
{
    [XmlRoot("MostConfig")]
    [Serializable()]
    public class MailConfig
    {
        #region attribute Type
        // Type
        private string _Type = "AutoEmail";

        /// <summary> 
        /// Type 
        /// </summary> 
        ///[DataMember()]
        [XmlAttribute("Type")]
        public string Type
        {
            get
            {
                return _Type;
            }
            set
            {
                if (!_Type.Equals(value))
                {
                    _Type = value;
                    ///RaisePropertyChanged("Type");
                }
            }
        }
        #endregion //attribute Type
        
        #region attribute Events
        // Events
        private List<EventItem> _Events = new List<EventItem>();

        /// <summary> 
        /// Events 
        /// </summary> 
        ///[DataMember()]
        [XmlArray("EventMails"), XmlArrayItem("EventItem",typeof(EventItem))]
        public List<EventItem> Events
        {
            get
            {
                return _Events;
            }
            set
            {
                if (!_Events.Equals(value))
                {
                    _Events = value;
                    ///RaisePropertyChanged("Events");
                }
            }
        }
        #endregion //attribute Events
    }
}
