using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web; 

namespace Panasia.Core.Sys
{
  public class UserMessage
    {
        public string MsgTitle { get; set; }
        public int Count { get; set; }
        public string MsgUrl { get; set; }
        public string MsgType { get; set; }
    }
}
