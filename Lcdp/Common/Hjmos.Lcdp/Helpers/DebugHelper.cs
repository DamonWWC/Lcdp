using Hjmos.Lcdp.Enums;
using System;

namespace Hjmos.Lcdp.Helpers
{

    /// <summary>
    /// 调试帮助类
    /// </summary>
    public class DebugHelper
    {
        /// <summary>
        /// 输出调试信息
        /// </summary>
        /// <param name="msg">需要输出的信息</param>
        /// <param name="site">枚举OutputSite的组合值</param>
        public static void WriteLine(string msg, OutputSite site)
        {
            // 写入信息到调式输出窗口
            if ((site & OutputSite.Debug) != 0) System.Diagnostics.Debug.WriteLine(msg);

            // 写入信息到调试和发布输出窗口
            if ((site & OutputSite.Trace) != 0) System.Diagnostics.Trace.WriteLine(msg);

            // 写入信息到控制台
            if ((site & OutputSite.Console) != 0) Console.WriteLine(msg);
        }

        /// <summary>
        /// 输出调试信息
        /// </summary>
        /// <param name="msg">需要输出的信息</param>
        /// <param name="flag">枚举OutputSite的组合值</param>
        public static void WriteLine(string msg, int flag = 5)
        {
            OutputSite site = (OutputSite)Enum.Parse(typeof(OutputSite), flag.ToString());

            WriteLine(msg, site);
        }

    }
}
