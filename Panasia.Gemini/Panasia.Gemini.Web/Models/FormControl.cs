using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Panasia.Gemini.Web.Models
{
    public class FormControl
    {
        public string title { get; set; }
        public string name { get; set; }
        public int width { get; set; }
        public string type { get; set; }
        public Item items { get; set; }
        public ValidateType validtype { get; set; }
    }
    public class Item
    {
        public int key{get;set;}
        public string value{get;set;}
    }
    public class ValidateType
    {
        public bool Required { get; set; }
        public bool IsMustNumber { get; set; }
    }
}