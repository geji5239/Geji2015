using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;

namespace Panasia.Core
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false), MetadataAttribute]
    public class SystemParameterFunctionAttribute:ExportAttribute
    {
        public const string ExportContractName = "SystemParameterFunction";

        public string Name { get; private set; }

        public SystemParameterFunctionAttribute(string name)
            : base(ExportContractName, typeof(Func<string, object>))
        {
            Name = name;
        }
    }

    public interface ISystemParameterFunctionMetadata
    {
        string Name { get; }
    }
}
