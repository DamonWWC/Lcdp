namespace Hjmos.Lcdp.VisualEditor.Core
{
    /// <summary>
    /// 根元素的行为接口（item.Parent为null的元素）
    /// 代替IPlacementBehavior来支持调整根元素的大小。
    /// </summary>
    public interface IRootPlacementBehavior : IPlacementBehavior { }
}
