using System;

namespace Hjmos.Lcdp.ILoger
{
    /// <summary>
    /// 日志操作帮助类
    /// </summary>
    public interface ILogHelper
    {
        #region Debug

        /// <summary>
        /// 写入调试日志，记录类名和调试信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        void Debug(object source, string msg);

        /// <summary>
        /// 写入调试日志，记录类名、方法名和调试信息
        /// 方法名需要调用堆栈获取，影响性能，如无必要记录方法名，可以调用其他重载
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        void Debug(string msg);

        /// <summary>
        /// 写入调试日志，记录类名、调试信息和异常信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        void Debug(object source, string msg, Exception exception);

        /// <summary>
        /// 写入调试日志，记录类名、方法名和调试信息、异常信息
        /// 方法名需要调用堆栈获取，影响性能，如无必要记录方法名，可以调用其他重载
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        void Debug(string msg, Exception exception);

        #endregion

        #region Info

        /// <summary>
        /// 写入信息日志，记录类名和调试信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        void Info(object source, string msg);

        /// <summary>
        /// 写入信息日志，记录类名、方法名和调试信息
        /// 方法名需要调用堆栈获取，影响性能，如无必要记录方法名，可以调用其他重载
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        void Info(string msg);

        /// <summary>
        /// 写入信息日志，记录类名、调试信息和异常信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        void Info(object source, string msg, Exception exception);

        /// <summary>
        /// 写入信息日志，记录类名、方法名和调试信息、异常信息
        /// 方法名需要调用堆栈获取，影响性能，如无必要记录方法名，可以调用其他重载
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        void Info(string msg, Exception exception);

        #endregion

        #region Warn

        /// <summary>
        /// 写入警告日志，记录类名和调试信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        void Warn(object source, string msg);

        /// <summary>
        /// 写入警告日志，记录类名、方法名和调试信息
        /// 方法名需要调用堆栈获取，影响性能，如无必要记录方法名，可以调用其他重载
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        void Warn(string msg);

        /// <summary>
        /// 写入警告日志，记录类名、调试信息和异常信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        void Warn(object source, string msg, Exception exception);

        /// <summary>
        /// 写入警告日志，记录类名、方法名和调试信息、异常信息
        /// 方法名需要调用堆栈获取，影响性能，如无必要记录方法名，可以调用其他重载
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        void Warn(string msg, Exception exception);

        #endregion

        #region Error

        /// <summary>
        /// 写入错误日志，记录类名和调试信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        void Error(object source, string msg);

        /// <summary>
        /// 写入错误日志，记录类名、方法名和调试信息
        /// 方法名需要调用堆栈获取，影响性能，如无必要记录方法名，可以调用其他重载
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        void Error(string msg);

        /// <summary>
        /// 写入错误日志，记录类名、调试信息和异常信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        void Error(object source, string msg, Exception exception);

        /// <summary>
        /// 写入错误日志，记录类名、方法名和调试信息、异常信息
        /// 方法名需要调用堆栈获取，影响性能，如无必要记录方法名，可以调用其他重载
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        void Error(string msg, Exception exception);

        #endregion

        #region Fatal

        /// <summary>
        /// 写入失败日志，记录类名和调试信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        void Fatal(object source, string msg);

        /// <summary>
        /// 写入失败日志，记录类名、方法名和调试信息
        /// 方法名需要调用堆栈获取，影响性能，如无必要记录方法名，可以调用其他重载
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        void Fatal(string msg);

        /// <summary>
        /// 写入失败日志，记录类名、调试信息和异常信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        void Fatal(object source, string msg, Exception exception);

        /// <summary>
        /// 写入失败日志，记录类名、方法名和调试信息、异常信息
        /// 方法名需要调用堆栈获取，影响性能，如无必要记录方法名，可以调用其他重载
        /// </summary>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        void Fatal(string msg, Exception exception);

        #endregion
    }
}
