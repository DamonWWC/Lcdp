using Hjmos.Lcdp.VisualEditorServer.EFCore;
using Hjmos.Lcdp.VisualEditorServer.ICommon;

namespace Hjmos.Lcdp.VisualEditorServer.Common
{
    /// <summary>
    /// 生产数据连接对象
    /// </summary>
    public class ConnectionFactory : IConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public ConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 创建数据库上下文对象
        /// </summary>
        /// <returns></returns>
        public EFCoreContext CreateDbContext() => new(_configuration.Read("DbConnectStr"));
    }
}
