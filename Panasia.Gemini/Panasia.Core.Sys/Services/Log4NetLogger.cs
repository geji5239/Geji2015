using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Panasia.Core;

namespace Panasia.Core.Sys
{
    public class Log4netLogger
    {
        static bool _ConfigLoaded = false;

        static Log4netLogger()
        {
            try
            {
                log4net.Config.XmlConfigurator.Configure();
                _ConfigLoaded = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("初始化Log4Net配置出错：" + ex.Message);
            }
        }

        [Export("LoggerAction", typeof(Action<object, string, Category, Priority>))]
        static void Log(object sender, string message, Category category, Priority priority)
        {
            if (_ConfigLoaded)
            {
                ThreadPool.QueueUserWorkItem((state) =>
                {
                    try
                    {
                        Logger(sender, message, category);
                    }
                    catch
                    {
                    }
                });
            }
        }

        private static void Logger(object sender, string message, Category category)
        {
            log4net.ILog logger = null;
            if (sender == null)
            {
                logger = log4net.LogManager.GetLogger("Common");
            }
            else
            {
                if (sender is string)
                {
                    logger = log4net.LogManager.GetLogger(sender.ToString());
                }
                else
                {
                    logger = log4net.LogManager.GetLogger(sender.GetType());
                }
            }
            switch (category)
            {
                case Category.Debug:
                    logger.Debug(message);
                    break;
                case Category.Exception:
                    logger.Error(message);
                    break;
                case Category.Fatal:
                    logger.Fatal(message);
                    break;
                case Category.Info:
                    logger.Info(message);
                    break;
                case Category.Warn:
                    logger.Warn(message);
                    break;
            }
        }

        public static void Log_Info(string message)
        {
            var log = GetLogger();
            log.Info(message);
        }
        public static void Log_Info(string message, Exception ex)
        {
            var log = GetLogger();
            log.Info(message, ex);
        }

        public static void Log_Debug(string message)
        {
            var log = GetLogger();
            log.Debug(message);
        }
        public static void Log_Debug(string message, Exception ex)
        {
            var log = GetLogger();
            log.Debug(message, ex);
        }

        public static void Log_Warn(string message)
        {
            var log = GetLogger();
            log.Warn(message);
        }
        public static void Log_Warn(string message, Exception ex)
        {
            var log = GetLogger();
            log.Warn(message, ex);
        }

        public static void Log_Error(string message)
        {
            var log = GetLogger();
            log.Error(message);
        }
        public static void Log_Error(string message, Exception ex)
        {
            var log = GetLogger();
            log.Error(message, ex);
        }

        public static void Log_Fatal(string message)
        {
            var log = GetLogger();
            log.Fatal(message);
        }
        public static void Log_Fatal(string message, Exception ex)
        {
            var log = GetLogger();
            log.Fatal(message, ex);
        }

        private static log4net.ILog GetLogger()
        {
            log4net.ILog log = log4net.LogManager.GetLogger(GetCallerType(3));
            return log;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Type GetCallerType(int skip = 2)
        {
            return new StackFrame(skip, false).GetMethod().DeclaringType;
        }
    }
}
