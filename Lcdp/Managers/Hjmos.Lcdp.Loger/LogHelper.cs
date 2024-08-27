using Hjmos.Lcdp.ILoger;
using log4net;
using System;
using System.Diagnostics;

namespace Hjmos.Lcdp.Loger
{
    /// <summary>
    /// 日志操作帮助类
    /// </summary>
    public class LogHelper : ILogHelper
    {
        public LogHelper() { }

        #region Debug

        /// <summary>
        /// 写入调试日志，记录类名和调试信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        public void Debug(object source, string msg)
        {
            ILog Logger = LogManager.GetLogger(source.GetType());
            Logger.Debug(msg);
        }

        /// <summary>
        /// 写入调试日志，记录类名、方法名和调试信息
        /// 方法名需要调用堆栈获取，影响性能，如无必要记录方法名，可以调用其他重载
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        public void Debug(string msg)
        {
            ILog Logger = LogManager.GetLogger(GetCurrentMethodFullName());
            Logger.Debug(msg);
        }

        /// <summary>
        /// 写入调试日志，记录类名、调试信息和异常信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        public void Debug(object source, string msg, Exception exception)
        {
            ILog Logger = LogManager.GetLogger(source.GetType());
            Logger.Debug(msg, exception);
        }

        /// <summary>
        /// 写入调试日志，记录类名、方法名和调试信息、异常信息
        /// 方法名需要调用堆栈获取，影响性能，如无必要记录方法名，可以调用其他重载
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        public void Debug(string msg, Exception exception)
        {
            ILog Logger = LogManager.GetLogger(GetCurrentMethodFullName());
            Logger.Debug(msg, exception);
        }

        #endregion

        #region Info

        /// <summary>
        /// 写入信息日志，记录类名和调试信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        public void Info(object source, string msg)
        {
            ILog Logger = LogManager.GetLogger(source.GetType());
            Logger.Info(msg);
        }

        /// <summary>
        /// 写入信息日志，记录类名、方法名和调试信息
        /// 方法名需要调用堆栈获取，影响性能，如无必要记录方法名，可以调用其他重载
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        public void Info(string msg)
        {
            ILog Logger = LogManager.GetLogger(GetCurrentMethodFullName());
            Logger.Info(msg);
        }

        /// <summary>
        /// 写入信息日志，记录类名、调试信息和异常信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        public void Info(object source, string msg, Exception exception)
        {
            ILog Logger = LogManager.GetLogger(source.GetType());
            Logger.Info(msg, exception);
        }

        /// <summary>
        /// 写入信息日志，记录类名、方法名和调试信息、异常信息
        /// 方法名需要调用堆栈获取，影响性能，如无必要记录方法名，可以调用其他重载
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        public void Info(string msg, Exception exception)
        {
            ILog Logger = LogManager.GetLogger(GetCurrentMethodFullName());
            Logger.Info(msg, exception);
        }

        #endregion

        #region Warn

        /// <summary>
        /// 写入警告日志，记录类名和调试信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        public void Warn(object source, string msg)
        {
            ILog Logger = LogManager.GetLogger(source.GetType());
            Logger.Warn(msg);
        }

        /// <summary>
        /// 写入警告日志，记录类名、方法名和调试信息
        /// 方法名需要调用堆栈获取，影响性能，如无必要记录方法名，可以调用其他重载
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        public void Warn(string msg)
        {
            ILog Logger = LogManager.GetLogger(GetCurrentMethodFullName());
            Logger.Warn(msg);
        }

        /// <summary>
        /// 写入警告日志，记录类名、调试信息和异常信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        public void Warn(object source, string msg, Exception exception)
        {
            ILog Logger = LogManager.GetLogger(source.GetType());
            Logger.Warn(msg, exception);
        }

        /// <summary>
        /// 写入警告日志，记录类名、方法名和调试信息、异常信息
        /// 方法名需要调用堆栈获取，影响性能，如无必要记录方法名，可以调用其他重载
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        public void Warn(string msg, Exception exception)
        {
            ILog Logger = LogManager.GetLogger(GetCurrentMethodFullName());
            Logger.Warn(msg, exception);
        }

        #endregion

        #region Error

        /// <summary>
        /// 写入错误日志，记录类名和调试信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        public void Error(object source, string msg)
        {
            ILog Logger = LogManager.GetLogger(source.GetType());
            Logger.Error(msg);
        }

        /// <summary>
        /// 写入错误日志，记录类名、方法名和调试信息
        /// 方法名需要调用堆栈获取，影响性能，如无必要记录方法名，可以调用其他重载
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        public void Error(string msg)
        {
            ILog Logger = LogManager.GetLogger(GetCurrentMethodFullName());
            Logger.Error(msg);
        }

        /// <summary>
        /// 写入错误日志，记录类名、调试信息和异常信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        public void Error(object source, string msg, Exception exception)
        {
            ILog Logger = LogManager.GetLogger(source.GetType());
            Logger.Error(msg, exception);
        }

        /// <summary>
        /// 写入错误日志，记录类名、方法名和调试信息、异常信息
        /// 方法名需要调用堆栈获取，影响性能，如无必要记录方法名，可以调用其他重载
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        public void Error(string msg, Exception exception)
        {
            ILog Logger = LogManager.GetLogger(GetCurrentMethodFullName());
            Logger.Error(msg, exception);
        }

        #endregion

        #region Fatal

        /// <summary>
        /// 写入失败日志，记录类名和调试信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        public void Fatal(object source, string msg)
        {
            ILog Logger = LogManager.GetLogger(source.GetType());
            Logger.Fatal(msg);
        }

        /// <summary>
        /// 写入失败日志，记录类名、方法名和调试信息
        /// 方法名需要调用堆栈获取，影响性能，如无必要记录方法名，可以调用其他重载
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        public void Fatal(string msg)
        {
            ILog Logger = LogManager.GetLogger(GetCurrentMethodFullName());
            Logger.Fatal(msg);
        }

        /// <summary>
        /// 写入失败日志，记录类名、调试信息和异常信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        public void Fatal(object source, string msg, Exception exception)
        {
            ILog Logger = LogManager.GetLogger(source.GetType());
            Logger.Fatal(msg, exception);
        }

        /// <summary>
        /// 写入失败日志，记录类名、方法名和调试信息、异常信息
        /// 方法名需要调用堆栈获取，影响性能，如无必要记录方法名，可以调用其他重载
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        public void Fatal(string msg, Exception exception)
        {
            ILog Logger = LogManager.GetLogger(GetCurrentMethodFullName());
            Logger.Fatal(msg, exception);
        }

        #endregion

        /// <summary>
        /// 通过堆栈获取记录日志的方法名称
        /// </summary>
        /// <returns></returns>
        private string GetCurrentMethodFullName()
        {
            try
            {
                int depth = 2;
                StackTrace trace = new StackTrace();
                int max = trace.GetFrames().Length;
                StackFrame frame;
                string methodName, className;
                Type classType;
                do
                {
                    frame = trace.GetFrame(depth++);
                    classType = frame.GetMethod().DeclaringType;
                    className = classType.ToString();
                } while (className.EndsWith("Exception") && depth < max);
                methodName = frame.GetMethod().Name;
                return $"{className}.{methodName}";
            }
            catch
            {
                return null;
            }
        }
    }
}
