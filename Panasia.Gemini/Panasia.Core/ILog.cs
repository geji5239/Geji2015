using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Panasia.Core
{
    [Export]
    public class Logger : ILog
    {
        public event EventHandler<LogEventArgs> LogAdded;

        private Action<string, Category, Priority> callback;
        public Action<string, Category, Priority> Callback
        {
            get { return this.callback; }
            set { this.callback = value; }
        }


#pragma warning disable 0649
        [ImportMany]
        private IEnumerable<Lazy<Action<string, Category, Priority>>> LogActions;

#pragma warning restore 0649
        public Logger()
        {
            Thread thr = new Thread(new ThreadStart(Run));
            thr.Start();
            System.Diagnostics.Debug.WriteLine("日志启动.");
        }

        public void Log(object sender, string message, Category category, Priority priority)
        {
            AddAction(() => AddLog(sender, message, category, priority));
        }

        private void AddLog(object sender, string message, Category category, Priority priority)
        {
            if (Callback != null)
            {
                Callback(message, category, priority);
            }
            if (LogAdded != null)
            {
                LogAdded(sender, new LogEventArgs { Message = message, Category = category, Priority = priority });
            }
        }

        #region 日志处理队列线程

        private static object _EventsLock = new object();

        private static bool _EnableEventHandlerThread = true;

        private static ManualResetEvent _ThreadEvent = new ManualResetEvent(true);

        private ConcurrentQueue<Action> _Actions = new ConcurrentQueue<Action>();

        private void AddAction(Action action)
        {
            _Actions.Enqueue(action);
            lock (_EventsLock)
            {
                _ThreadEvent.Set();
            }
        }

        private void Run()
        {
            while (_EnableEventHandlerThread)
            {
                Action action;
                while (_Actions.TryDequeue(out action))
                {
                    try
                    {
                        action();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("执行日志方法错误:" + ex.ToString());
                    }
                }
                if (_Actions.Count == 0)
                {
                    _ThreadEvent.Reset();
                    _ThreadEvent.WaitOne();
                }
            }
            this.Log("日志停止");
        }

        [Export("Disposed", typeof(Action))]
        [ExportMetadata("Priority", 10)]
        private static void Stop()
        {
            _EnableEventHandlerThread = false;
            lock (_EventsLock)
            {
                _ThreadEvent.Set();
            }
        }
        #endregion

    }

    public interface ILog
    {
        event EventHandler<LogEventArgs> LogAdded;
        void Log(object sender, string message, Category category, Priority priority);
    }

    public class LogEventArgs : EventArgs
    {
        #region 属性 CreatedTime
        DateTime _CreatedTime = DateTime.Now;
        public DateTime CreatedTime
        {
            get
            {
                return _CreatedTime;
            }
            set
            {
                _CreatedTime = value;
            }
        }
        #endregion

        /// <summary>
        /// 信息内容
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 类别
        /// </summary>
        public Category Category { get; set; }
        /// <summary>
        /// 优先级
        /// </summary>
        public Priority Priority { get; set; }

    }

    public enum Priority
    {
        /// <summary>
        /// 没有指定
        /// </summary>
        None = 0,

        /// <summary>
        /// 高级
        /// </summary>
        High = 1,

        /// <summary>
        /// 中级
        /// </summary>
        Medium = 2,

        /// <summary>
        /// 低级
        /// </summary>
        Low = 3,
    }

    /// <summary>
    /// 错误类别
    /// </summary>
    public enum Category
    {
        /// <summary>
        /// 调试
        /// </summary>
        Debug = 0,

        /// <summary>
        /// 异常
        /// </summary>
        Exception = 1,

        /// <summary>
        /// 信息
        /// </summary>
        Info = 2,

        /// <summary>
        /// 警告
        /// </summary>
        Warn = 3,

        /// <summary>
        /// 致命错误
        /// </summary>
        Fatal = 4,
    }



}
