using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Panasia.Gemini.Web.Models
{
    public class FindCompanyModel
    {
        public List<org_Company> Company { get; set; }
        public ErrorModel Error { get; set; }
    }
}