using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Panasia.Gemini.Web.Models
{
    public class Column
    {
        public string title { get; set; }
        public string field { get; set; }
        public int width { get; set; }
        public int rowspan { get; set; }
        public int colspan { get; set; }
        public string align { get; set; }
        public string halign { get; set; }
        public bool sortable { get; set; }
        public string order { get; set; }
        public bool resizable { get; set; }
        public bool hidden { get; set; }
        public Func<Column> formatter { get; set; }
        public Func<Column> sorter { get; set; }
        public bool checkbox { get; set; }

    }
}