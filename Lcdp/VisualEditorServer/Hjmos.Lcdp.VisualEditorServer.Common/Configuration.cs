using Microsoft.Extensions.Configuration;
using System.IO;

namespace Hjmos.Lcdp.VisualEditorServer.Common
{
    /// <summary>
    /// 表示配置文件
    /// </summary>
    public class Configuration : ICommon.IConfiguration
    {
        private static readonly IConfigurationRoot _configuration;

        /// <summary>
        /// 初始化配置文件
        /// </summary>
        static Configuration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            _configuration = builder.Build();
        }

        /// <summary>
        /// 读取配置项
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Read(string key) => _configuration[key];
    }
}
