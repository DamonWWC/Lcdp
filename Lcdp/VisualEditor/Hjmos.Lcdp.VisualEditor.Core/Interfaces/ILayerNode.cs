namespace Hjmos.Lcdp.VisualEditor.Core.Interfaces
{
    /// <summary>
    /// 页面图层节点接口
    /// </summary>
    public interface ILayerNode : IUnitNode
    {
        /// <summary>
        /// 名称
        /// </summary>
        string DisplayName { get; set; }

        /// <summary>
        /// 层叠索引
        /// </summary>
        int ZIndex { get; set; }
    }
}
