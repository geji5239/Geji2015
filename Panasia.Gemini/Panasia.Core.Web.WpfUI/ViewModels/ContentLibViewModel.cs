using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Panasia.Core;
using Panasia.Core.App;
using Panasia.Core.Wpf;
using Panasia.Core.Contents;

namespace Panasia.Core.Web
{
    [PageModel("ContentLib")]
    public class ContentLibViewModel : Panasia.Core.Wpf.PageModel
    {
        #region 属性 ContentLib
        public ContentLib ContentLib
        {
            get
            {
                return ContentLib.Current;
            }
        }
        #endregion

        #region 属性 SelectedItem
        private object _SelectedItem = null;
        public object SelectedItem
        {
            get
            {
                return _SelectedItem;
            }
            set
            {
                _SelectedItem = value;
                RaisePropertyChanged(() => SelectedItem);
            }
        }
        #endregion

        #region LoadFolderCommand
        private RelayCommand _LoadFolderCommand = null;
        public RelayCommand LoadFolderCommand
        {
            get
            {
                if (_LoadFolderCommand == null)
                {
                    _LoadFolderCommand = new RelayCommand(OnLoadFolder, CanLoadFolder);
                }
                return _LoadFolderCommand;
            }
        }

        private bool CanLoadFolder(object parameter)
        {
            return true;
        }

        private void OnLoadFolder(object parameter)
        {
            if (parameter != null && parameter is ContentFolder)
            {
                var folder = parameter as ContentFolder;
                ResetFolder(folder);
            }
            else if (ContentLib.FolderRoot.Folders.Count == 0)
            {
                var folder = ContentLib.FolderRoot;
                ResetFolder(folder);
            }
        }

        private void ResetFolder(ContentFolder folder)
        {
            folder.Folders.Clear();
            folder.Files.Clear();
            var path = ContentLib.GetRootPath() + folder.FullName.TrimStart('\\');
            if (System.IO.Directory.Exists(path))
            {
                foreach (var f in System.IO.Directory.GetDirectories(path))
                {
                    var newItem = ContentLib.CreateFolder(folder);
                    newItem.Name = f.Substring(f.LastIndexOf('\\') + 1);
                    newItem.Description = "自动加载";

                    ResetFolder(newItem);
                }

                foreach (var f in System.IO.Directory.GetFiles(path))
                {
                    var newItem = ContentLib.CreateFile(folder);
                    newItem.Name = f.Substring(f.LastIndexOf('\\') + 1);
                    newItem.Description = "自动加载";
                }
            }
        }
        #endregion

        #region AddFolderCommand
        private RelayCommand _AddFolderCommand = null;
        public RelayCommand AddFolderCommand
        {
            get
            {
                if (_AddFolderCommand == null)
                {
                    _AddFolderCommand = new RelayCommand(OnAddFolder, CanAddFolder);
                }
                return _AddFolderCommand;
            }
        }

        private bool CanAddFolder(object parameter)
        {
            return true;
        }

        private void OnAddFolder(object parameter)
        {
            if (parameter == null)
            {
                var item = ContentLib.Current.CreateFolder(ContentLib.Current.FolderRoot);
                SelectedItem = item;
                return;
            }
            if (parameter is ContentFolder)
            {
                var item = ContentLib.Current.CreateFolder(parameter as ContentFolder);
                SelectedItem = item;
                return;
            }
            if (parameter is ContentFileBase)
            {
                var file = parameter as ContentFileBase;
                if (file.Folder != null)
                {
                    var item = ContentLib.Current.CreateFolder(file.Folder);
                    SelectedItem = item;
                    return;
                }
            }
            else
            {
                var item = ContentLib.Current.CreateFolder(ContentLib.Current.FolderRoot);
                SelectedItem = item;
                return;
            }
        }
        #endregion

        #region AddFileCommand
        private RelayCommand _AddFileCommand = null;
        public RelayCommand AddFileCommand
        {
            get
            {
                if (_AddFileCommand == null)
                {
                    _AddFileCommand = new RelayCommand(OnAddFile, CanAddFile);
                }
                return _AddFileCommand;
            }
        }

        private bool CanAddFile(object parameter)
        {
            return parameter != null && parameter is ContentFolder;
        }

        private void OnAddFile(object parameter)
        {
            var item = ContentLib.Current.CreateFile(parameter as ContentFolder);
            SelectedItem = item;
        }
        #endregion

        #region SaveFileCommand
        private RelayCommand _SaveFileCommand = null;
        public RelayCommand SaveFileCommand
        {
            get
            {
                if (_SaveFileCommand == null)
                {
                    _SaveFileCommand = new RelayCommand(OnSaveFile, CanSaveFile);
                }
                return _SaveFileCommand;
            }
        }

        private bool CanSaveFile(object parameter)
        {
            return parameter != null && parameter is RazorTemplate;
        }

        private void OnSaveFile(object parameter)
        {
            var file = parameter as RazorTemplate;
            file.SaveContent();

            ContentLib.Current.Save();
        }
        #endregion

        #region TestFileCommand
        private RelayCommand _TestFileCommand = null;
        public RelayCommand TestFileCommand
        {
            get
            {
                if (_TestFileCommand == null)
                {
                    _TestFileCommand = new RelayCommand(OnTestFile, CanTestFile);
                }
                return _TestFileCommand;
            }
        }

        private bool CanTestFile(object parameter)
        {
            return parameter != null && parameter is RazorTemplate;
        }

        private void OnTestFile(object parameter)
        {

        }
        #endregion

        #region OpenFileCommand
        private RelayCommand _OpenFileCommand = null;
        public RelayCommand OpenFileCommand
        {
            get
            {
                if (_OpenFileCommand == null)
                {
                    _OpenFileCommand = new RelayCommand(OnOpenFile, CanOpenFile);
                }
                return _OpenFileCommand;
            }
        }

        private bool CanOpenFile(object parameter)
        {
            return parameter!=null;
        }

        private void OnOpenFile(object parameter)
        {
            SelectedItem = parameter;
        }
        #endregion

        public override void Open(Dictionary<string, string> paras, Action<bool> callBack)
        {
            base.Open(paras, callBack);

            if (ContentLib.FolderRoot.Folders.Count == 0 && ContentLib.FolderRoot.Files.Count == 0)
            {
                LoadViews();
            }
        }

        private void LoadViews()
        {
        }
    }
}
