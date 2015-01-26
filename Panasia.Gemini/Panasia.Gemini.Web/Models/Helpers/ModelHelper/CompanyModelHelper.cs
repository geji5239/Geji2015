using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Panasia.Gemini.Web.Models.CompanyPageModels;


namespace Panasia.Gemini.Web.Models.Helpers.ModelHelper
{
    public static class CompanyModelHelper
    {
                public static org_Company ToAddCompany(this CompanyItem CompanyItem)
                {
                    org_Company Company = CompanyItem.ToCompany();
                    return Company;
                }
                public static CompanyItem ToCompanyItem(this org_Company Company)
                {
                    return new CompanyItem(Company);
                }
                public static org_Company ToUpdateCompany(this CompanyItem CompanyItem)
                {
                    org_Company Company = CompanyItem.ToCompany();
                    return Company;
                }
    }
}