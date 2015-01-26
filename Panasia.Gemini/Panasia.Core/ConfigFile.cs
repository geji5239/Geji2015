using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Panasia.Core
{
    public class ConfigFile:EntityBase
    {
        private const string ConfigRootKey = "ConfigRoot";

        #region 属性 CurrentConfigFileName
        private string _CurrentConfigFileName = string.Empty;
        [XmlIgnore]
        public string CurrentConfigFileName
        {
            get
            {
                return _CurrentConfigFileName;
            }
            protected set
            {
                _CurrentConfigFileName = value;
                RaisePropertyChanged("CurrentConfigFileName");
            }
        }
        #endregion

        #region 属性 ConfigRoot 
        private static string _ConfigRoot = string.Empty;
        /// <summary>
        /// 配置文件的根目录
        /// </summary>
        public static string ConfigRoot
        {
            get
            {
                if(string.IsNullOrEmpty(_ConfigRoot))
                {
                    var root = ConfigRootKey.GetAppSettingValue("");
                    if(string.IsNullOrEmpty(root))
                    {
                        _ConfigRoot =  ServiceLocator.Current.BaseDirectory;
                    }
                    else if (root.Contains(':'))
                    {
                        _ConfigRoot = root;
                    }
                    else
                    {
                        _ConfigRoot = ServiceLocator.Current.BaseDirectory + root;
                    }
                }
                return _ConfigRoot;
            } 
        }
        #endregion

        #region 配置加载及保存
        public void Save()
        {
            this.SaveXDoc(CurrentConfigFileName);
        }

        public void SaveAs(string fileName)
        {
            this.SaveXDoc(fileName);
            this.CurrentConfigFileName = fileName;
        }

        public static T Load<T>(string fileName, string configFileNameKey="") where T : ConfigFile
        {
            string file =(!string.IsNullOrEmpty(configFileNameKey)) && System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains(configFileNameKey) ?
                System.Configuration.ConfigurationManager.AppSettings[configFileNameKey] :
                fileName;
            if (file.IndexOf(":\\") < 0)
            {
                file =ConfigRoot + file;
            }
            return LoadFile<T>(file); 
        } 

        private static T LoadFile<T>(string filename) where T :ConfigFile
        {
            if (System.IO.File.Exists(filename))
            {
                var data = filename.LoadXDoc<T>();
                data.CurrentConfigFileName = filename;
                return data;
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}
