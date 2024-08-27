using System;

namespace Hjmos.Lcdp.VisualEditor.Controls.Entities
{
    /// <summary>
    /// 表示组件面板上的一个组件项
    /// </summary>
    public class WidgetItem
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string DisplayName { get; set; } = "Undefined";

        /// <summary>
        /// 类别
        /// </summary>
        public string Category { get; set; } = "Demo";

        /// <summary>
        /// FontCode，图标
        /// </summary>
        public string Icon { get; set; } = "\ue63c";

        /// <summary>
        /// 默认高度
        /// </summary>
        public double DefaultWidth { get; set; } = 200d;

        /// <summary>
        /// 默认宽度
        /// </summary>
        public double DefaultHeight { get; set; } = 200d;

        /// <summary>
        /// 组件Type
        /// </summary>
        public Type WidgetType { get; set; }

        /// <summary>
        /// 设计时是否用图例的方式展示
        /// </summary>
        public bool RenderAsSample { get; set; } = false;

        /// <summary>
        /// 图例类型
        /// </summary>
        public string SampleFullName { get; set; }
    }
}
