namespace Hjmos.Lcdp.VisualEditor.Core.Interfaces
{
    /// <summary>
    /// 根组件接口
    /// </summary>
    public interface IRoot : IGroup
    {

        /// <summary>
        /// 名称
        /// </summary>
        string DisplayName { get; set; }
    }
}
