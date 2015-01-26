using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace Panasia.Core
{
    public sealed class XmlRawItem : IXmlSerializable
    {
        private string Document { get; set; }

        public XmlRawItem()
        {
        }

        public XmlRawItem(string document)
        {
            Document = document;
        }

        #region xml serilize
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            Document = reader.ReadInnerXml();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteRaw(string.IsNullOrWhiteSpace(Document) ? "" : Document);
        }

        public override string ToString()
        {
            return Document;
        }
        #endregion

        public static implicit operator XmlRawItem(string doc)
        {
            return new XmlRawItem(doc);
        }

        public static implicit operator string(XmlRawItem rawItem)
        {
            return rawItem.Document;
        }
    }
}