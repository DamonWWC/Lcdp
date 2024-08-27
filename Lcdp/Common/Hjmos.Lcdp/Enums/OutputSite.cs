using System;
using System.ComponentModel;

namespace Hjmos.Lcdp.Enums
{
    /// <summary>
    /// 调试信息输出位置
    /// </summary>
    [Flags]
    public enum OutputSite
    {
        [Description("Debug输出窗口")]
        Debug = 1,
        [Description("Debug和Release输出窗口")]
        Trace = 2,
        [Description("控制台")]
        Console = 4
    }
}
