namespace Hjmos.Lcdp.VisualEditor.Core.Services
{
    /// <summary>
    /// 包含一组关于默认设计器组件的选项。  
    /// </summary>
    public sealed class OptionService
    {
        /// <summary>获取/设置在拖动/选择时设计图面是否应为灰色</summary>
        public bool GrayOutDesignSurfaceExceptParentContainerWhenDragging = true;

        /// <summary>获取/设置在使用对线布局时是否需要舍入值</summary>
        public bool SnaplinePlacementRoundValues = false;
    }
}
