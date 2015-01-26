using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Panasia.Core;

namespace Panasia.Core.Web
{
    public class ViewFileBase : EntityBase
    {
        #region 属性 FileName
        private string _FileName = string.Empty;
        public string FileName
        {
            get
            {
                return _FileName;
            }
            set
            {
                _FileName = value;
                RaisePropertyChanged(() => FileName);
                RaisePropertyChanged(() => FullName);
            }
        }
        #endregion

        #region 属性 Root
        private string _Root = string.Empty;
        public string Root
        {
            get
            {
                return _Root;
            }
            set
            {
                _Root = value;
                RaisePropertyChanged(() => Root);
                RaisePropertyChanged(() => FullName);
            }
        }
        #endregion

        #region 属性 FullName 
        public string FullName
        {
            get
            {
                return string.Format("{0}.{1}",Root,FileName);
            } 
        }
        #endregion 

        #region 属性 Parent
        private ViewFileBase _Parent = null;
        public ViewFileBase Parent
        {
            get
            {
                return _Parent;
            }
            internal set
            {
                _Parent = value;
                RaisePropertyChanged(() => Parent);
            }
        }
        #endregion

        public virtual void Reload()
        {
        }
    }

    public class ViewFile : ViewFileBase
    {
        #region 属性 Content
        private string _Content = string.Empty;
        public string Content
        {
            get
            {
                if (string.IsNullOrEmpty(_Content))
                {
                    Reload();
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

        public override void Reload()
        {
            this.Load();
        }
    }

    public class ViewFileFolder : ViewFileBase
    {
        #region 属性 Items
        private ViewFileCollection _Items = null;
        public ViewFileCollection Items
        {
            get
            {
                if (_Items == null)
                {
                    _Items = new ViewFileCollection();
                    _Items.CollectionChanged += _Items_CollectionChanged;
                }
                return _Items;
            }
            set
            {
                if (_Items != null)
                {
                    _Items.CollectionChanged -= _Items_CollectionChanged;
                }
                _Items = value;
                if (_Items != null)
                {
                    _Items.CollectionChanged += _Items_CollectionChanged;
                }
                RaisePropertyChanged(() => Items);
            }
        }

        void _Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (ViewFileBase item in e.NewItems)
                    {
                        item.Parent = this;
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (ViewFileBase item in e.OldItems)
                    {
                        item.Parent = null;
                    }
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

        public override void Reload()
        {
            this.Load();
        }
    }

    public class ViewFileCollection : CollectionBase<ViewFileBase>
    {

    }

    static class WebFileExtension
    {
        static string _WebRoot = string.Empty;
        public static string WebRoot
        {
            get
            {
                if (string.IsNullOrEmpty(_WebRoot))
                {
                    string basePath = ApplicationConfig.GetAppSettingValue("WebRoot", "");
                    if (string.IsNullOrEmpty(basePath))
                    {
                        basePath = ServiceLocator.Current.BaseDirectory;
                        _WebRoot = ServiceLocator.Current.IsWebApp() ?
                            basePath.Substring(0, basePath.Length - 4) : basePath;
                    }
                    else
                    {
                        if(basePath.Contains(':'))
                        {
                            _WebRoot = basePath;
                        }
                        else
                        {
                            _WebRoot = ServiceLocator.Current.BaseDirectory + basePath;
                        } 
                    }
                }
                return _WebRoot;
            }
        }

        public static void Load(this ViewFile file)
        {
            var path = file.Root.Replace('/', '\\');
            if (path.StartsWith("~\\"))
            {
                path = WebRoot + path.Substring(2) + file.FileName;
            }
            
            if (System.IO.File.Exists(path))
            {
                file.Content = System.IO.File.ReadAllText(path);
            }
        }

        public static void Save(this ViewFile file)
        {
            var path = file.Root.Replace('/', '\\');
            if (path.StartsWith("~\\"))
            {
                path = WebRoot + path.Substring(2) + file.FileName;
            }
            file.Content.WriteTo(path);
        }

        public static void Load(this ViewFileFolder folder)
        {
            var path = folder.Root.Replace('/', '\\');
            if (path.StartsWith("~\\"))
            {
                path = WebRoot + path.Substring(2) + folder.FileName;
            }
            if (System.IO.Directory.Exists(path))
            {
                lock (folder)
                {
                    folder.Items.Clear();
                    foreach (var f in System.IO.Directory.GetDirectories(path))
                    {
                        var newFolder = new ViewFileFolder { Root = folder.Root + folder.FileName + "\\", FileName = f.Substring(f.LastIndexOf('\\')+1) };
                        folder.Items.Add(newFolder);
                        newFolder.Load();
                    }

                    foreach (var f in System.IO.Directory.EnumerateFiles(path)
                        .Where(p=>p.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase)))
                    {
                        var newFile = new ViewFile { Root = folder.Root + folder.FileName + "\\", FileName = f.Substring(f.LastIndexOf('\\')+1) };
                        folder.Items.Add(newFile);
                    }
                }
            }
        }
    }
}
