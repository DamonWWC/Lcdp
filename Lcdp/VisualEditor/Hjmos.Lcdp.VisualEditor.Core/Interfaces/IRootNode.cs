namespace Hjmos.Lcdp.VisualEditor.Core.Interfaces
{
    /// <summary>
    /// 页面根节点接口
    /// </summary>
    public interface IRootNode : IUnitNode
    {
        /// <summary>
        /// 名称
        /// </summary>
        string DisplayName { get; set; }
    }
}
