using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Panasia.Core;
using Panasia.Core.App;
using Panasia.Core.Wpf;

namespace Panasia.Core.AppTest
{
    public class TestDocViewModel:EntityBase
    {
        #region 属性 TestDoc
        private TestDoc _TestDoc = TestDoc.Current;
        public TestDoc TestDoc
        {
            get
            { 
                return _TestDoc;
            } 
        }
        #endregion

        #region 属性 SelectedGroup
        private TestGroup _SelectedGroup = null;
        public TestGroup SelectedGroup
        {
            get
            {
                return _SelectedGroup;
            }
            set
            {
                _SelectedGroup = value;
                RaisePropertyChanged(() => SelectedGroup);
            }
        }
        #endregion

        #region 属性 SelectedJob
        private TestJob _SelectedJob = null;
        public TestJob SelectedJob
        {
            get
            {
                return _SelectedJob;
            }
            set
            {
                _SelectedJob = value;
                RaisePropertyChanged(() => SelectedJob);
            }
        }
        #endregion

        #region 进度
        #region 属性 TotalCount
        private int _TotalCount = 0;
        public int TotalCount
        {
            get
            {
                return _TotalCount;
            }
            set
            {
                _TotalCount = value;
                RaisePropertyChanged(() => TotalCount);
            }
        }
        #endregion

        #region 属性 DoneCount
        private int _DoneCount = 0;
        public int DoneCount
        {
            get
            {
                return _DoneCount;
            }
            set
            {
                _DoneCount = value;
                RaisePropertyChanged(() => DoneCount);
            }
        }
        #endregion

        #region 属性 IsTesting
        private bool _IsTesting = false;
        public bool IsTesting
        {
            get
            {
                return _IsTesting;
            }
            set
            {
                _IsTesting = value;
                RaisePropertyChanged(() => IsTesting);
            }
        }
        #endregion

        private void SetProgress(int done)
        {
            DoneCount = done; 
        }
        #endregion

        #region AddGroupCommand
        private RelayCommand _AddGroupCommand = null;
        public RelayCommand AddGroupCommand
        {
            get
            {
                if (_AddGroupCommand == null)
                {
                    _AddGroupCommand = new RelayCommand(OnAddGroup, CanAddGroup);
                }
                return _AddGroupCommand;
            }
        }

        private bool CanAddGroup(object parameter)
        {
            return true;
        }

        private void OnAddGroup(object parameter)
        {
            var group = new TestGroup { Name = "new group" };
            TestDoc.Current.Groups.Add(group);
        }
        #endregion

        #region AddTestCommand
        private RelayCommand _AddTestCommand = null;
        public RelayCommand AddTestCommand
        {
            get
            {
                if (_AddTestCommand == null)
                {
                    _AddTestCommand = new RelayCommand(OnAddTest, CanAddTest);
                }
                return _AddTestCommand;
            }
        }

        private bool CanAddTest(object parameter)
        {
            return SelectedGroup!=null;
        }

        private void OnAddTest(object parameter)
        {
            var group = SelectedGroup;
            SelectTestTypeWindow win = new SelectTestTypeWindow(type =>
            {
                var job = new TestJob { Name = "new name", TestType = type };
                group.Items.Add(job);
            });
            win.ShowDialog();            
        }
        #endregion 

        #region MoveUpCommand
        private RelayCommand _MoveUpCommand = null;
        public RelayCommand MoveUpCommand
        {
            get
            {
                if (_MoveUpCommand == null)
                {
                    _MoveUpCommand = new RelayCommand(OnMoveUp, CanMoveUp);
                }
                return _MoveUpCommand;
            }
        }

        private bool CanMoveUp(object parameter)
        {
            return SelectedGroup!=null && SelectedJob!=null && (SelectedGroup.Items.IndexOf(SelectedJob)>0);
        }

        private void OnMoveUp(object parameter)
        {
            SelectedGroup.Items.MoveUp(SelectedJob);
        }
        #endregion

        #region MoveDownCommand
        private RelayCommand _MoveDownCommand = null;
        public RelayCommand MoveDownCommand
        {
            get
            {
                if (_MoveDownCommand == null)
                {
                    _MoveDownCommand = new RelayCommand(OnMoveDown, CanMoveDown);
                }
                return _MoveDownCommand;
            }
        }

        private bool CanMoveDown(object parameter)
        {
            return SelectedGroup != null && SelectedJob != null && (SelectedGroup.Items.IndexOf(SelectedJob) < (SelectedGroup.Items.Count-1) );
        }

        private void OnMoveDown(object parameter)
        {
            SelectedGroup.Items.MoveDown(SelectedJob);
        }
        #endregion

        #region TestCommand
        private RelayCommand _TestCommand = null;
        public RelayCommand TestCommand
        {
            get
            {
                if (_TestCommand == null)
                {
                    _TestCommand = new RelayCommand(OnTest, CanTest);
                }
                return _TestCommand;
            }
        }

        private bool CanTest(object parameter)
        {
            return true;
        }

        private void OnTest(object parameter)
        {
            foreach(var group in TestDoc.Groups)
            {
                foreach(var test in group.Items)
                {
                    test.Execute();
                }
            }
        }
        #endregion

        #region TestItemCommand
        private RelayCommand _TestItemCommand = null;
        public RelayCommand TestItemCommand
        {
            get
            {
                if (_TestItemCommand == null)
                {
                    _TestItemCommand = new RelayCommand(OnTestItem, CanTestItem);
                }
                return _TestItemCommand;
            }
        }

        private bool CanTestItem(object parameter)
        {
            return SelectedJob !=null;
        }

        private void OnTestItem(object parameter)
        {
            SelectedJob.Execute();
        }
        #endregion
    }
}
