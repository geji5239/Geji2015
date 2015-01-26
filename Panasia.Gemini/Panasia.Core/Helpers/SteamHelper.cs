using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Panasia.Core
{
    public static class SteamHelper
    {
        #region 扩展方法 
        public static string ReadToEnd(this Stream stream)
        {
            if (stream == null)
            {
                return string.Empty;
            }
            using (var reader = new StreamReader(stream, Encoding.GetEncoding("utf-8")))
            {
                return reader.ReadToEnd();
            }
        }

        public static Stream ToStream(this string content)
        {
            MemoryStream stream = new MemoryStream();
            using (var sw = new StreamWriter(stream))
            {
                sw.Write(content);
                return stream;
            }
        }
        #endregion
    }
}
