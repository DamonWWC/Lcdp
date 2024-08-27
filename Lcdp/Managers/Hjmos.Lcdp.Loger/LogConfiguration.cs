using log4net.Config;
using System.IO;

namespace Hjmos.Lcdp.Loger
{
    /// <summary>
    /// Log配置设置
    /// </summary>
    public class LogConfiguration
    {
        /// <summary>
        /// 初始化配置文件
        /// </summary>
        public void Init() => XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.config"));
    }
}
