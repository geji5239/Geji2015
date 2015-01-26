using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Panasia.Core;
using Panasia.Core.App;
using Panasia.Core.Wpf;

namespace Panasia.Core.Web.Controls
{
    /// <summary>
    /// ViewActionCommandControl.xaml 的交互逻辑
    /// </summary>
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    [UIControl("CommandControls", "ReportActionCommand")]
    public partial class ReportActionCommandControl : UserControl
    { 
        public ReportActionCommandControl()
        {
            InitializeComponent(); 
            var reportPath = ServiceLocator.Current.BaseDirectory +
                ApplicationConfig.GetAppSettingValue("ReportPath", "..\\Reports\\");  
            reportFiles.ItemsSource = GetReportFiles(reportPath);
        } 

        private IEnumerable<string> GetReportFiles(string path,string root="")
        {
            if(!System.IO.Directory.Exists(path))
            {
                yield break;
            }

            foreach(var file in System.IO.Directory.GetFiles(path)
                .Where(f=>f.EndsWith(".rdlc", StringComparison.OrdinalIgnoreCase)))
            {
                var fileName = file.Substring(path.Length);
                yield return string.IsNullOrEmpty(root) ? fileName : (root + "\\" + fileName);
            }

            foreach(var p in System.IO.Directory.GetDirectories(path))
            {
                var pName = p.Substring(p.LastIndexOf("\\")+1);
                foreach (var f in GetReportFiles(p, string.IsNullOrEmpty(root) ? pName : (root + "\\" + pName)))
                {
                    yield return f;
                }
            }
        }
    }
}
