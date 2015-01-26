using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Panasia.Core;
namespace Panasia.Core.Contents
{
    public interface IContentFile :IFile, INotifyPropertyChanged
    {
        string Content { get; set; } 

        void SaveContent();

        string LoadContent();
    }

}
