using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Panasia.Core;

namespace Panasia.Core.Contents
{
    public class ContentFolder : FileBase
    {
        private static object _LoadLock = new object();

        #region 属性 Folders
        private ContentFolderCollection _Folders = null;
        [XmlArray("Folders"), XmlArrayItem("Folder", typeof(ContentFolder))]
        public ContentFolderCollection Folders
        {
            get
            {
                if (_Folders == null)
                {
                    _Folders = new ContentFolderCollection();
                    _Folders.CollectionChanged += _Files_CollectionChanged;
                }
                return _Folders;
            }
            set
            {
                if (_Folders != null)
                {
                    _Folders.CollectionChanged -= _Files_CollectionChanged;
                }
                _Folders = value;
                RaisePropertyChanged(() => Folders);
                if (_Folders != null)
                {
                    _Folders.CollectionChanged += _Files_CollectionChanged;
                    SetFolder(_Folders);
                }
                RaisePropertyChanged(() => Items);
            }
        }
        #endregion

        #region 属性 Files
        private ContentFileCollection _Files = null;
        [XmlArray("Files")]
        [XmlArrayItem("RazorFile", typeof(RazorTemplate))]
        public ContentFileCollection Files
        {
            get
            {
                if (_Files == null)
                {
                    _Files = new ContentFileCollection();
                    _Files.CollectionChanged += _Files_CollectionChanged;
                }
                return _Files;
            }
            set
            {
                if (_Files != null)
                {
                    _Files.CollectionChanged -= _Files_CollectionChanged;
                }
                _Files = value;
                RaisePropertyChanged(() => Files);
                if (_Files != null)
                {
                    _Files.CollectionChanged += _Files_CollectionChanged;
                    SetFolder(_Files);
                }
                RaisePropertyChanged(() => Items);
            }
        }

        void _Files_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (FileBase file in e.NewItems)
                    {
                        file.Folder = this;
                    }
                    RaisePropertyChanged(() => Items);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (FileBase file in e.OldItems)
                    {
                        file.Folder = null;
                    }
                    RaisePropertyChanged(() => Items);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region 属性 Items
        [XmlIgnore()]
        public IEnumerable<FileBase> Items
        {
            get
            {
                return GetItems();
            }
        }

        private IEnumerable<FileBase> GetItems()
        {
            foreach (var folder in Folders.OrderBy(f => f.FullName))
            {
                yield return folder;
            }

            foreach (var file in Files.OrderBy(f => f.FullName))
            {
                yield return file;
            }
        }
        #endregion
        
        public FileBase GetFile(string path)
        {
            if(path.Contains('/'))
            {
                return GetFile(path.Replace('/', '\\'));
            }

            if(path.StartsWith("\\"))
            {
                path = path.TrimStart('\\');
            }
            if(path.EndsWith("\\"))
            {
                path = path.TrimEnd('\\');
            }
            var index = path.IndexOf('\\');
            var name = index > 0 ? path.Substring(0, index) : path;

            var item = GetItems().FirstOrDefault(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if(item==null)
            {
                return null;
            }
            if(index>0)
            {
                return (item is ContentFolder)? (item as ContentFolder).GetFile(path.Substring(index+1))
                    :null;
            }
            return item;
        }

        public override void Load()
        {
            lock (_LoadLock)
            {
                if (!CheckExists())
                {
                    return;
                }

                foreach (var folder in Folders)
                {
                    folder.Load();
                }
                foreach (var file in Files)
                {
                    file.Load();
                }
            }
        }

        private void SetFolder(IEnumerable<FileBase> files)
        {
            foreach (var file in files)
            {
                file.Folder = this;
            }
        }

        protected override void OnIsExistsChanged()
        {
            base.OnIsExistsChanged();

            foreach (var folder in Folders)
            {

            }
        }

        protected override void OnFolderChanged()
        {
            base.OnFolderChanged();
            SetFolder(Folders);
            SetFolder(Files);
        }

        internal override void ResetFullName()
        {
            base.ResetFullName();
            Folders.ForEach(f => f.ResetFullName());
            Files.ForEach(f => f.ResetFullName());
        }
    }

    public class ContentFolderCollection : CollectionBase<ContentFolder>
    {

    }

    public class ContentFileCollection : CollectionBase<ContentFileBase>
    {

    }
}
