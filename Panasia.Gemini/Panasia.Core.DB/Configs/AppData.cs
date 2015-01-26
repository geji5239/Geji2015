using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Xml.Serialization; 

namespace Panasia.Core
{

    [XmlRoot("AppData")]
    public class AppData : ConfigFile
    {
        private const string DefaultConfigFileName = "DBConfig.xml";
        private const string ConfigFileNameKey = "DBConfigFile";
        
        #region 属性 CurrentConfigFileName
        private string _CurrentConfigFileName = DefaultConfigFileName;
        [XmlIgnore]
        public string CurrentConfigFileName
        {
            get
            {
                return _CurrentConfigFileName;
            }
            private set
            {
                _CurrentConfigFileName = value;
                RaisePropertyChanged("CurrentConfigFileName");
            }
        }
        #endregion

        #region 属性 Modules
        private AppModuleCollection _Modules = null;
        [XmlArray("Modules"), XmlArrayItem("Module", typeof(AppModule))]
        public AppModuleCollection Modules
        {
            get
            {
                if (_Modules == null)
                {
                    _Modules = new AppModuleCollection();
                }
                return _Modules;
            }
            set
            {
                _Modules = value;
                RaisePropertyChanged("Modules");
            }
        }
        #endregion

        #region 属性 CodeSetting
        [XmlIgnore()]
        public CodeSetting CodeSetting
        {
            get
            {
                return CodeSetting.Current;
            }
            set
            {
            }
        }
        #endregion

        #region 属性 UpdateMode
        private UpdateMode _UpdateMode = UpdateMode.Default;
        [XmlAttribute("UpdateMode")]
        public UpdateMode UpdateMode
        {
            get
            {
                return _UpdateMode;
            }
            set
            {
                _UpdateMode = value;
                RaisePropertyChanged("UpdateMode");
            }
        }
        #endregion

        #region 属性 UpdateTime
        private DateTime _UpdateTime = DateTime.Now;
        [XmlAttribute("UpdateTime")]
        public DateTime UpdateTime
        {
            get
            {
                return _UpdateTime;
            }
            set
            {
                _UpdateTime = value;
                RaisePropertyChanged("UpdateTime");
            }
        }
        #endregion

        #region AppData的实例
        private static object _CurrentLock = new object();
        private static AppData _Current = null;
        public static AppData Current
        {
            get
            {
                if(_Current==null)
                {
                    lock (_CurrentLock)
                    {
                        if(_Current==null)
                        {
                            _Current = Load();
                        }
                    }
                }
                return _Current;
            }
        }
        #endregion
         
        public void Init()
        {
            foreach (var module in Modules)
            {
                module.Init();
            }
        }

        #region 配置加载及保存 
        public static AppData Load(string fileName = "")
        {
            var appdata = ConfigFile.Load<AppData>(DefaultConfigFileName, ConfigFileNameKey);
            appdata.Init();
            return appdata;
        } 
        #endregion
    }
}
