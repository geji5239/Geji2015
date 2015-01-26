using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Panasia.Core;

namespace Panasia.Core.Contents
{
    public class FileBase : EntityBase, IFile
    {
        #region 属性 Name
        protected string _Name = "";
        [XmlAttribute("Name"), DefaultValue("")]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                RaisePropertyChanged(() => Name);
                ResetFullName();
            }
        }
        #endregion

        #region 属性 Description
        private string _Description = "";
        [XmlAttribute("Description"), DefaultValue("")]
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = value;
                RaisePropertyChanged(() => Description);
            }
        }
        #endregion

        #region 属性 FullName
        protected string _FullName = "";
        [XmlIgnore()]
        public string FullName
        {
            get
            {
                return _FullName;
            }
            set
            {
                _FullName = value;
                RaisePropertyChanged(() => FullName);
            }
        }
        #endregion

        #region 属性 Folder
        private ContentFolder _Folder = null;
        [XmlIgnore()]
        public ContentFolder Folder
        {
            get
            {
                return _Folder;
            }
            internal set
            {
                _Folder = value;
                RaisePropertyChanged(() => Folder);
                OnFolderChanged();
            }
        }
        #endregion

        #region 属性 IsExists
        private bool _IsExists = false;
        [XmlIgnore()]
        public bool IsExists
        {
            get
            {
                return _IsExists;
            }
            set
            {
                if(_IsExists == value)
                {
                    return;
                }
                _IsExists = value;
                RaisePropertyChanged(() => IsExists);
                OnIsExistsChanged();
            }
        }
        #endregion

        #region 属性 LastChanged
        private DateTime _LastChanged = DateHelper.MinDate;
        [XmlIgnore()]
        public DateTime LastChanged
        {
            get
            {
                return _LastChanged;
            }
            set
            {
                _LastChanged = value;
                RaisePropertyChanged(() => LastChanged);
            }
        }
        #endregion

        public virtual void Load()
        {
            CheckExists();
        }

        public virtual bool CheckExists()
        {            
            var file = ContentLib.Current.RootPath+FullName;

            if(this is ContentFolder)
            {
                IsExists = System.IO.Directory.Exists(file);
                if (IsExists)
                {
                    var info = new System.IO.DirectoryInfo(file);
                    LastChanged = info.LastWriteTime;
                }
                else
                {
                    LastChanged = DateHelper.MinDate;
                }
            }
            else
            {
                IsExists = System.IO.File.Exists(file);
                if (IsExists)
                {
                    var info = new System.IO.FileInfo(file);
                    LastChanged = info.LastWriteTime;
                }
                else
                {
                    LastChanged = DateHelper.MinDate;
                }
            } 
            return IsExists;
        }

        internal string GetPath()
        { 
            return  ContentLib.Current.RootPath+FullName; 
        }

        protected virtual void OnIsExistsChanged()
        {

        }
        
        protected virtual void OnFolderChanged()
        {
            ResetFullName();
        }

        internal virtual void ResetFullName()
        {
            if (Folder == null)
            {
                FullName = "";
            }
            else
            {
                FullName = Folder.FullName + "\\" + Name;
            }
        }
    }

    public class ContentFileBase : FileBase, IContentFile
    {
        #region 属性 ID
        private string _ID = "";
        [XmlAttribute("ID"), DefaultValue("")]
        public string ID
        {
            get
            {
                if (string.IsNullOrEmpty(_ID))
                {
                    _ID = Guid.NewGuid().ToString();
                }
                return _ID;
            }
            set
            {
                _ID = value;
                RaisePropertyChanged(() => ID);
            }
        }
        #endregion
        
        #region 属性 Content
        protected string _Content = "";
        [XmlIgnore()]
        public string Content
        {
            get
            {
                if(string.IsNullOrEmpty(_Content))
                {
                    _Content = LoadContent();
                }
                return _Content;
            }
            set
            {
                _Content = value;
                RaisePropertyChanged(() => Content);
            }
        }
        #endregion 

        public void SaveContent()
        {
            if(Folder==null)
            {
                return;
            }

            var folderPath = Folder.GetPath();
            if(!System.IO.Directory.Exists(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }
            var path = GetPath();
            Content.WriteTo(path);
        }

        public string LoadContent()
        {
            if(CheckExists())
            {
                var path = GetPath();                 
                return path.ReadFileText();
            }
            return "";
        }
    }
}
