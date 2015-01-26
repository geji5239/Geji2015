using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panasia.Core.Contents
{
    public interface IFile
    {
        string Name { get; set; }
        string FullName { get; set; }
        string Description { get; set; }
        bool IsExists { get; set; }
    }
}
