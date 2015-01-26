/*
 * singba:这个类是仿照PRISM框架来实现MEF组件的组合
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel.Composition.ReflectionModel;

namespace Panasia.Core
{
    public interface IInitializationMeta
    {
        int Priority { get; }
    }

    public static class ServiceExtensions
    {
        public static void Log(this object sender, string message, Category category = Category.Info, Priority priority = Priority.None)
        {
            if (category == Category.Debug)
            {
                Debug.WriteLine("Debug{0}\t{1}", DateTime.Now.ToString("mm:ss.fff"), message);
            }
            ServiceLocator.Current.Log.Log(sender, message, category, priority);
        }
    }

    public class ServiceLocator : IDisposable
    {
        public const string ActionInitContractName = "Initialization";
        public const string ActionDisposeContractName = "Disposed";
        public const string ActionMetaPriority = "Priority";

        public const string PluginPath = "plugins";
        public const string ExtensionComponents = "ExtensionComponents";
        private CompositionContainer _container;
        private readonly Logger _logger = null;

        private bool _CheckIsWebService = false;
        private bool IsWebService = false;
        private IEnumerable<Lazy<Action, IInitializationMeta>> _InitializationActions;
        private IEnumerable<Lazy<Action, IInitializationMeta>> _DisposedActions;

        public AggregateCatalog Catalog { get; private set; }

        private Dictionary<string, Assembly> _Assemblies = new Dictionary<string, Assembly>();

        private ServiceLocator()
        {
            Catalog = new AggregateCatalog();

            string apppath = BaseDirectory;
            LoadDefaultAssemblies(apppath);

            _container = new CompositionContainer(Catalog);
            _logger = _container.GetExportedValue<Logger>();
            _container.ComposeExportedValue<CompositionContainer>(_container);

            var logActions = _container.GetExports<Action<object, string, Category, Priority>>("LoggerAction");
            _logger.LogAdded += (sender, e) =>
            {
                if (logActions != null)
                {
                    foreach (var action in logActions)
                    {
                        try
                        {
                            if (action.Value != null)
                            {
                                action.Value(sender, e.Message, e.Category, e.Priority);
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            };

            _InitializationActions = _container.GetExports<Action, IInitializationMeta>("Initialization");
            _DisposedActions = _container.GetExports<Action, IInitializationMeta>("Disposed");

            //_container.ComposeParts(this);

        }

        #region MEF组件加载 

        #region 加载默认的组件
        private void LoadDefaultAssemblies(string apppath)
        {

            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => !(
                a.FullName.StartsWith("Telerik")
                || a.FullName.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase)
                || a.FullName.StartsWith("system", StringComparison.OrdinalIgnoreCase)
                || a.FullName.StartsWith("Newton", StringComparison.OrdinalIgnoreCase)
                || a.FullName.StartsWith("mscorlib", StringComparison.OrdinalIgnoreCase)));

            foreach (var asm in assemblies)
            {
                TryLoadAssembly(asm);
            }

            Action<string> addPath = (p) =>
            {
                var pp = apppath + p;
                if (!System.IO.Directory.Exists(pp))
                {
                    return;
                    //如果没有目录，也不存在插件dll
                    //System.IO.Directory.CreateDirectory(pp);
                }
                //*.*修改为*.dll

                var files = System.IO.Directory.GetFiles(pp, "*.dll");
                foreach (var file in files)
                {
                    TryLoadAssembly(file);
                }
            };

            addPath("");
            addPath(PluginPath);
        }
        #endregion

        #region 加载单个dll
        private void TryLoadAssembly(string file)
        {
            try
            {
                var asm = Assembly.LoadFile(file);
                TryLoadAssembly(asm);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("加载{0}出错:{1}", file, ex.ToString()));
            }
        }

        private void TryLoadAssembly(Assembly asm)
        {
            if (_Assemblies.ContainsKey(asm.FullName))
            {
                return;
            }
            try
            {
                var ac = new AssemblyCatalog(asm);
                Catalog.Catalogs.Add(ac);
                _Assemblies.Add(asm.FullName, asm);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("加载{0}出错:{1}", asm.FullName, ex.ToString()));
            }
        }
        #endregion
        #endregion

        static ServiceLocator _Current = new ServiceLocator();
        public static ServiceLocator Current
        {
            get
            {
                return _Current;
            }
        }

        bool _InitDone = false;
        public void Init()
        {
            if ((!_InitDone) && _InitializationActions != null)
            {
                foreach (var item in _InitializationActions.OrderBy(a => a.Metadata.Priority))
                {
                    try
                    {
                        item.Value();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                _InitDone = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool flag = true)
        {
            if (_DisposedActions != null)
            {
                foreach (var item in _DisposedActions.OrderBy(a => a.Metadata.Priority))
                {
                    try
                    {
                        item.Value();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("退出异常:{0}", ex.ToString());
                    }
                }
            }
            _container.Dispose();
        }

        public void ComposeParts(params object[] obj)
        {
            _container.ComposeParts(obj);
        }

        public CompositionContainer GetContainer()
        {
            return _container;
        }

        public ILog Log
        {
            get
            {
                return _logger;
            }
        }

        public IEnumerable<KeyValuePair<Type, IDictionary<string, object>>> GetExportedTypes<T>(string contractName)
        {
            if (Catalog == null)
            {
                yield break;
            }
            foreach (var part in Catalog.Parts)
            {
                foreach (var def in part.ExportDefinitions)
                {
                    if (def.ContractName.Equals(contractName) &&
                        def.Metadata.ContainsKey("ExportTypeIdentity") &&
                    def.Metadata["ExportTypeIdentity"].Equals(typeof(T).FullName))
                    {
                        yield return new KeyValuePair<Type, IDictionary<string, object>>(ReflectionModelServices.GetPartType(part).Value,
                            def.Metadata);
                    }
                }
            }
        }


        public T GetInstance<T>()
        {
            return _container.GetExportedValue<T>();
        }

        public bool IsWebApp()
        {
            if (_CheckIsWebService == false)
            {
                IsWebService = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile.EndsWith("web.config", StringComparison.OrdinalIgnoreCase);
            }
            return IsWebService;
        }
        string _BaseDirectory = null;
        /// <summary>
        /// 如果是Web项目返回的是bin目录，如果是win项目，返回是BaseDirectory,文本最后有\\
        /// </summary>
        public string BaseDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(_BaseDirectory))
                {
                    var exe = AppDomain.CurrentDomain.BaseDirectory;
                    _BaseDirectory = (exe.EndsWith("\\") ? exe : exe + "\\") + (IsWebApp() ? "bin\\" : "");
                }
                return _BaseDirectory;
            }
        }
    }
}
