using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panasia.Core.Sys
{
    public static class ExceptionExtensions
    {
        public static Exception CreateException(this string message)
        {
            return new Exception(message);
        }
    }
}
