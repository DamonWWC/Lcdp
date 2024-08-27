using Hjmos.Lcdp.VisualEditorServer.EFCore;

namespace Hjmos.Lcdp.VisualEditorServer.ICommon
{
    /// <summary>
    /// 生产数据连接对象
    /// </summary>
    public interface IConnectionFactory
    {
        /// <summary>
        /// 创建数据库上下文对象
        /// </summary>
        /// <returns></returns>
        EFCoreContext CreateDbContext();
    }
}
