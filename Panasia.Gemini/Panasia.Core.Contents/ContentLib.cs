using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Panasia.Core;

namespace Panasia.Core.Contents
{
    [XmlRoot("ContentLib")]
    public class ContentLib : ConfigFile
    {
        const string ConfigFileKey = "ContentLibConfigFile";
        const string ContentRootKey = "ContentRoot";
        private static object _LoadLock = new object();
        private static Dictionary<string, RazorTemplate> _RazorTemplates = new Dictionary<string, RazorTemplate>();

        #region 属性 FolderRoot
        private ContentFolder _FolderRoot = null;
        [XmlElement("FolderRoot")]
        public ContentFolder FolderRoot
        {
            get
            {
                if (_FolderRoot == null)
                {
                    _FolderRoot = new ContentFolder
                    {
                    };
                }
                return _FolderRoot;
            }
            set
            {
                _FolderRoot = value;
                RaisePropertyChanged(() => FolderRoot);
            }
        }
        #endregion

        #region 属性 RootPath
        private string _RootPath = "";
        [XmlIgnore()]
        public string RootPath
        {
            get
            {
                if (string.IsNullOrEmpty(_RootPath))
                {
                    _RootPath = GetRootPath();
                }
                return _RootPath;
            }
            private set
            {
                _RootPath = value;
                RaisePropertyChanged(() => RootPath);
            }
        }
        #endregion

        #region 属性 RazorFiles
        private IEnumerable<RazorTemplate> _RazorFiles = null;
        [XmlIgnore]
        public IEnumerable<RazorTemplate> RazorFiles
        {
            get
            {
                if (_RazorFiles == null)
                {
                    _RazorFiles = _RazorTemplates.Values.OrderBy(f => f.FullName);
                }
                return _RazorFiles;
            }
        }

        private void ResetRazorFiles()
        {
            _RazorFiles = _RazorTemplates.Values.OrderBy(f => f.FullName);
            RaisePropertyChanged("RazorFiles");
        }
        #endregion

        #region 属性 Current
        private static ContentLib _Current = null;
        public static ContentLib Current
        {
            get
            {
                if (_Current == null)
                {
                    lock (_LoadLock)
                    {
                        if (_Current == null)
                        {
                            ReloadCurrent();
                        }
                    }
                }
                return _Current;
            }
        }
        #endregion

        [Export("Initialization")]
        [ExportMetadata("Priority",1)]
        public static void Init()
        {
            "ContentLib".Log("Load ContentLib");
            "ContentLib".Log(string.Format("Load ContentLib:{0}", _RazorTemplates.Count));
        }
        
        #region 回收
        [DisposeAction(1)]
        public static void Clear()
        {
            lock (_LoadLock)
            {
                if (_Current != null)
                {
                    _RazorTemplates.Clear();
                    _Current = null;
                }
            }
        }
        #endregion

        internal static void AddRazorTemplate(RazorTemplate templateItem)
        {
            _RazorTemplates[templateItem.ID] = templateItem;
        }

        public static RazorTemplate GetRazorTemplate(string id)
        {
            return _RazorTemplates.GetDictionaryValue(id, null);
        }

        public static void ReloadCurrent()
        {
            _Current = Load<ContentLib>("ContentLib.xml", ConfigFileKey);
            if (_Current != null && _Current.FolderRoot != null)
            {
                _Current.FolderRoot.Load();
            }
        }

        public static string GetRootPath()
        {
            var root = ContentRootKey.GetAppSettingValue("");
            if (string.IsNullOrEmpty(root))
            {
                root = ServiceLocator.Current.BaseDirectory;
            }
            else if (!(root.Contains(':')))
            {
                root = ServiceLocator.Current.BaseDirectory +
                    (root.EndsWith("\\") ? root : (root + "\\"));
            }
            return root;
        }

        #region 文件操作
        public RazorTemplate CreateFile(ContentFolder folder)
        {
            RazorTemplate file = new RazorTemplate { Name= "新文件.cshtml"};
            folder.Files.Add(file);
            _RazorTemplates.Add(file.ID, file);
            ResetRazorFiles();
            return file;
        }

        public ContentFolder CreateFolder(ContentFolder folder)
        {
            ContentFolder subFolder = new ContentFolder { Name = "新文件夹" };
            folder.Folders.Add(subFolder);
            return subFolder;
        }

        #endregion
    }

}
