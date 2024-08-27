namespace Hjmos.Lcdp.VisualEditor.Core.Interfaces
{
    /// <summary>
    /// 图层接口
    /// </summary>
    public interface ILayer : IGroup
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
