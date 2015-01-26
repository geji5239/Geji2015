using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Panasia.Gemini.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("share",
                "share/{funcName}",
                new { controller = "Share", action = "Data", funcName = UrlParameter.Optional });
            
            routes.MapRoute("page",
                "page/{pageID}/{actionName}",
                new { controller = "Page", action = "Index", pageID = UrlParameter.Optional, actionName = UrlParameter.Optional });
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Main", id = UrlParameter.Optional }
            );
        }
    }
}