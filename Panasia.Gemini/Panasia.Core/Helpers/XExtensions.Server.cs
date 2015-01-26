using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Xsl;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

namespace Panasia.Core
{
    public static partial class BaseHelper
    {
        public static void SaveXDoc(this object xobj, string filename)
        {
            int index=filename.LastIndexOf('\\');
            if (index > 0)
            {
                string path= filename.Substring(0,index);
                if(!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
            }
            XmlSerializer xs = new XmlSerializer(xobj.GetType());
            Stream stream = System.IO.File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            
            xs.Serialize(stream, xobj);
            stream.Close();
        }

        public static T LoadXDoc<T>(this string filename)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            object obj = xs.Deserialize(stream);
            stream.Close();
            return (T)obj;
        }

        public static string XslTrans(string sourcefile, string transfile)
        {
            XslCompiledTransform xtrans = new XslCompiledTransform();
            xtrans.Load(transfile);

            XmlDocument newdoc = new XmlDocument();
            newdoc.Load(sourcefile);
            return XslTrans(xtrans, newdoc);
        }

        public static string XslTrans(XslCompiledTransform xtrans, XmlDocument xdoc)
        {
            System.IO.StringWriter result = new System.IO.StringWriter();
            xtrans.Transform(xdoc, null, result);
            return result.ToString();
        }

        public static string GetStringUsingXslStream(this string source, string transtring)
        {
            XslCompiledTransform xtrans = new XslCompiledTransform();
            MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(transtring));
            xtrans.Load(XmlReader.Create(ms));

            XmlDocument newdoc = new XmlDocument();
            newdoc.LoadXml(source);
            return XslTrans(xtrans, newdoc);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="transfile"></param>
        /// <returns></returns>
        public static string GetStringUsingXslFile(this string source, string transfile)
        {
            XslCompiledTransform xtrans = new XslCompiledTransform();
            xtrans.Load(transfile);

            XmlDocument newdoc = new XmlDocument();
            newdoc.LoadXml(source);
            return XslTrans(xtrans, newdoc);
        }

        public static StreamReader ReadStream(this string filename)
        {
            StreamReader sr = new StreamReader(filename);
            return sr;
        }

        public static string ReadFileText(this string fileName)
        {
            using(var sr = System.IO.File.OpenText(fileName))
            {
                return sr.ReadToEnd();
            }
        }

        public static void WriteTo(this string content,string fileName,bool append=false)
        {
            string dir = fileName.Substring(0, fileName.LastIndexOf('\\'));
            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }
            using (var sw = append ? System.IO.File.AppendText(fileName) : System.IO.File.CreateText(fileName))
            {
                sw.Write(content);
                sw.Close();
            }
        }
        
    }
}
