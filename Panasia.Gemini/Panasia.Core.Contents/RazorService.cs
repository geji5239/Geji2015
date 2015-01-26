using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RazorEngine;
using RazorEngine.Templating;

namespace Panasia.Core.Contents
{
    public static class RazorService
    { 
        private static Dictionary<string, DateTime> _CompiledTemplates = new Dictionary<string, DateTime>();

        static RazorService()
        {
            var templateService = new TemplateService();
            List<string> nameSpaces = "Panasia.Core|Panasia.Core.Contents|Panasia.Core.App|Panasia.Core.Commands|Panasia.Core.Sys|Panasia.Core.Web"
                .Split('|').ToList();

            var imports = ApplicationConfig.GetAppSettingValue("RazorImports", "");
            if (!string.IsNullOrEmpty(imports))
            {
                imports.Split('|').ForEach((ns) =>
                {
                    var item = ns.Trim().TrimEnd(';');
                    if (!nameSpaces.Contains(item))
                    {
                        nameSpaces.Add(item);
                    }
                });
            }
            
            //ApplicationConfig.GetAppSettingValue("RazorImports","")
            //    .Split('|').ForEach((ns) =>
            //    {
            //        var item = ns.Trim().TrimEnd(';');
            //        if(!nameSpaces.Contains(item))
            //        {
            //            nameSpaces.Add(item);
            //        }
            //    });

            nameSpaces.ForEach((ns) =>
            {
                templateService.AddNamespace(ns.Trim().TrimEnd(';'));
            });

            RazorEngine.Razor.SetTemplateService(templateService);
        } 

        public static void Compile(string razorTemplate, Type modelType, string cacheName)
        {
            RazorEngine.Razor.Compile(razorTemplate, modelType, cacheName);

            lock (_CompiledTemplates)
            {
                _CompiledTemplates[cacheName] = DateTime.Now;
            }
        }

        public static void Compile(string razorTemplate, string cacheName)
        {
            RazorEngine.Razor.Compile(razorTemplate, cacheName);

            lock (_CompiledTemplates)
            {
                _CompiledTemplates[cacheName] = DateTime.Now;
            }
        }

        public static string Run(string cacheName, dynamic model)
        {
            return RazorEngine.Razor.Run(cacheName, model);
        }

        public static string Parse(string razorTemplate, dynamic model)
        {
            return RazorEngine.Razor.Parse(razorTemplate, model);
        }
    }
}
