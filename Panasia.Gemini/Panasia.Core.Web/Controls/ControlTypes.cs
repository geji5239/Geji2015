using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace Panasia.Core.Web
{
    public class ControlTypes
    {

    }

    public class FormControlExportAttribute:ExportAttribute
    {

    }

    public interface IFormControl
    {
        string TypeName { get; }

        string Title { get; }
    }
}
