using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Panasia.Core;

namespace Panasia.Core.Web
{
    public static class StringExtensions
    {
        public static string FormatXml(this string xml)
        {
            string result = "";

            MemoryStream mStream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode);
            XmlDocument document = new XmlDocument();

            try
            {
                // Load the XmlDocument with the XML.
                document.LoadXml(xml);

                writer.Formatting = Formatting.Indented;

                // Write the XML into a formatting XmlTextWriter
                document.WriteContentTo(writer);
                writer.Flush();
                mStream.Flush();

                // Have to rewind the MemoryStream in order to read
                // its contents.
                mStream.Position = 0;

                // Read MemoryStream contents into a StreamReader.
                StreamReader sReader = new StreamReader(mStream);

                // Extract the text from the StreamReader.
                String FormattedXML = sReader.ReadToEnd();

                result = FormattedXML;
            }
            catch (XmlException ex)
            {
                return xml;
            }

            mStream.Close();

            return result;
        }

        public static string ToFormatHtml(this string htmlContent, string space = "    ")
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                int level = 0;
                var index = 0;
                var indexEnd = 0;
                Action addLine = () =>
                {
                    if (sb.Length > 0)
                    {
                        sb.Append("\r\n");
                    }
                };

                Action addSpace = () =>
                {

                    if (level == 0)
                        return;
                    for (int i = 0; i < level; i++)
                    {
                        sb.Append(space);
                    }
                };

                var contentIndex = 0;
                index = htmlContent.IndexOf('<', index);

                Action<int> appendContent = (len) =>
                {
                    var content = htmlContent.Substring(contentIndex, len).Trim();
                    if (content.Length > 0)
                    {
                        if (content[0] == '<')
                        {
                            addSpace();
                        }
                        sb.Append(content);
                        addLine();
                    }
                    contentIndex += len;
                };

                Action findNext = () =>
                {
                    if (indexEnd < htmlContent.Length - 1)
                    {
                        index = htmlContent.IndexOf("<", indexEnd + 1);
                    }
                    else
                    {
                        index = htmlContent.Length;
                    }
                };

                bool tagend = true;
                while (index < htmlContent.Length)
                {
                    Console.WriteLine(sb.ToString());
                    if (index < 0)
                    {
                        break;
                    }

                    var len = index - contentIndex;
                    if (len > 0)
                    {
                        appendContent(len);
                        if (!tagend)
                        {
                            level++;
                        }
                    }

                    tagend = false;

                    if (htmlContent[index + 1] == '/')
                    {
                        level -= 1;
                        indexEnd = htmlContent.IndexOf('>', index + 1);
                        if (indexEnd > 0)
                        {
                            len = indexEnd - contentIndex + 1;
                            appendContent(len);
                            findNext();
                            tagend = true;
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (htmlContent[index + 1] == '!' && htmlContent[index + 2] == '-' && htmlContent[index + 3] == '-')
                    {
                        indexEnd = htmlContent.IndexOf("-->", index + 1);
                        if (indexEnd > 0)
                        {
                            len = indexEnd - contentIndex + 3;
                            appendContent(len);
                            findNext();
                            tagend = true;
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }

                    indexEnd = htmlContent.IndexOf(">", index + 1);
                    if (indexEnd < 0)
                    {
                        break;
                    }
                    if (htmlContent[indexEnd - 1] == '/')
                    {
                        len = indexEnd - contentIndex + 1;
                        appendContent(len);
                        findNext();
                        tagend = true;
                        continue;
                    }

                    findNext();
                    if (index < htmlContent.Length)
                    {
                        if (htmlContent[index + 1] == '/')
                        {
                            indexEnd = htmlContent.IndexOf('>', index + 1);
                            if (indexEnd > 0)
                            {
                                len = indexEnd - contentIndex + 1;
                                appendContent(len);
                                findNext();
                                tagend = true;
                                continue;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }

                sb.Append(htmlContent, contentIndex, htmlContent.Length - contentIndex);

                return sb.ToString();
            }
            catch (Exception ex)
            {
                "FormatHtml".Log(ex.ToString());
                return htmlContent;
            }
        }
    
        public static string ReplaceParameters(this string source,Func<string,string> getValue)
        {
            if(string.IsNullOrEmpty(source))
            {
                return source;
            }
            int pIndex = source.IndexOf("{@");
            if(pIndex<0)
            {
                return source;
            }
            StringBuilder sb = new StringBuilder();
            Dictionary<string, string> values = new Dictionary<string, string>();
            int pEnd = 0;
            int sIndex = 0;
            while(pIndex>=0)
            {
                pEnd = source.IndexOf("}", pIndex);
                if(pEnd<pIndex)
                {
                    throw new Exception("参数错误:" + source);
                }
                var name = source.Substring(pIndex + 2, pEnd - pIndex - 2);
                string value = "";
                if(!values.ContainsKey(name))
                {
                    value = getValue(name);
                    values[name] = value;
                }
                else
                {
                    value = values[name];
                }
                sb.Append(source.Substring(sIndex, pIndex - sIndex));
                sb.Append(value);
                sIndex = pEnd + 1;
                if(sIndex>source.Length-3)
                {
                    break;
                }
                pIndex = source.IndexOf("{@",sIndex);
            }
            if(sIndex<source.Length)
            {
                sb.Append(source.Substring(sIndex));
            }
            return sb.ToString();
        }
    }
}
