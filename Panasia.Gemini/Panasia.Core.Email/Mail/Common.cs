using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Xsl; 

namespace Panasia.Core.Email
{
    static class Common
    {
        public static object GetDeserializeObjectFromFile(Type objtype, string xmlfile)
        {
            XmlSerializer deserializer = new XmlSerializer(objtype);
            TextReader textReader = new StreamReader(xmlfile);
            object obj = deserializer.Deserialize(textReader);
            textReader.Close();
            return obj;
        }

        public static string XslTrans(string source, string transfile)
        {
            XslCompiledTransform xtrans = new XslCompiledTransform();
            xtrans.Load(transfile);

            XmlDocument newdoc = new XmlDocument();
            newdoc.LoadXml(source);
            return XslTrans(xtrans, newdoc);
        }

        public static string XslTrans(XslCompiledTransform xtrans, XmlDocument xdoc)
        {
            System.IO.StringWriter result = new System.IO.StringWriter();
            xtrans.Transform(xdoc, null, result);
            return result.ToString();
        }

        /// <summary>
        /// serialize to xml
        /// </summary>
        /// <param name="xobj"></param>
        /// <param name="root"></param>
        /// <returns></returns>
        public static string ToXString(this object xobj)
        {
            string name = string.Empty;

            System.Attribute[] attrs = System.Attribute.GetCustomAttributes(xobj.GetType());
            if (attrs.Where(a => a is System.Xml.Serialization.XmlRootAttribute).Count() > 0)
            {
                System.Xml.Serialization.XmlRootAttribute attr = (System.Xml.Serialization.XmlRootAttribute)attrs.Where(a => a is System.Xml.Serialization.XmlRootAttribute).FirstOrDefault();
                name = attr.ElementName;
            }
            else
            {
                int index = xobj.GetType().FullName.LastIndexOf('.');
                name = xobj.GetType().FullName.Substring(index >= 0 ? index + 1 : 0);
            }
            return ToXmlString(xobj, name);
        }

        private static string ToXmlString(this object obj, string root)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(obj.GetType());
            System.IO.StringWriter textWriter = new System.IO.StringWriter();
            serializer.Serialize(textWriter, obj);

            string strtemp = textWriter.ToString();
            int index = strtemp.IndexOf("<" + root);
            return index > 0 ? strtemp.Substring(index) : strtemp;
        }

        public static T ToDeserializeObject<T>(this string xmlstring)
        {
            if (string.IsNullOrEmpty(xmlstring))
                return default(T);

            try
            {
                System.Xml.Serialization.XmlSerializer deserializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                System.IO.StringReader textReader = new System.IO.StringReader(xmlstring);
                object obj = deserializer.Deserialize(textReader);
                textReader.Close();
                return (T)obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
