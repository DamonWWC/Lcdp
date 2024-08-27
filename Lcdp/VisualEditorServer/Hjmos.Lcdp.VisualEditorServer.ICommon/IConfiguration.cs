namespace Hjmos.Lcdp.VisualEditorServer.ICommon
{
    /// <summary>
    /// 表示配置文件
    /// </summary>
    public interface IConfiguration
    {
        /// <summary>
        /// 读取配置项
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string Read(string key);
    }
}
