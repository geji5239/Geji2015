using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Panasia.Core;
using Panasia.Core.App;

namespace Panasia.Core.AppTest
{
    public static class TestExtensions
    {
        public static object GetValue(this TestParameter parameter)
        {
            if(string.IsNullOrEmpty(parameter.Path))
            {
                return parameter.Value;
            } 
            return TestDoc.Current.GetPathValue(parameter.Path);
        }

        public static IEnumerable<object> GetNameValues(this IEnumerable<TestParameter> parameters)
        {
            foreach(var p in parameters)
            {
                yield return p.Name;
                yield return p.GetValue();
            }
        }
    }
}
