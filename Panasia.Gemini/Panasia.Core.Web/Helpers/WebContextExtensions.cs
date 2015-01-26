using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Panasia.Core.App;

namespace Panasia.Core.Web
{
    public static class WebContextExtensions
    {
        public static Dictionary<string, object> CreateDictionary(this CommandParameterCollection parameters, object context)
        {
            Dictionary<string, object> viewModel = new Dictionary<string, object>();
            foreach (var p in parameters)
            {
                viewModel.Add(p.Name, p.GetValue(context));
            }
            return viewModel;
        }
    }
}
