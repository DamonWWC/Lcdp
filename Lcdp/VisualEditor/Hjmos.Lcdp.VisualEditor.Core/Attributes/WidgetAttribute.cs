using System;

namespace Hjmos.Lcdp.VisualEditor.Core.Attributes
{
    /// <summary>
    /// 此特性标记组件元数据
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class WidgetAttribute : Attribute
    {
        /// <summary>
        /// 组件名称
        /// </summary>
        public string DisplayName { get; set; } = "Undefined";

        /// <summary>
        /// 组件类别
        /// </summary>
        public string Category { get; set; } = "Demo";

        /// <summary>
        /// 组件图标，FontCode
        /// </summary>
        public string Icon { get; set; } = "\ue63c";

        /// <summary>
        /// 默认宽度
        /// </summary>
        public double DefaultWidth { get; set; } = 200d;

        /// <summary>
        /// 默认高度
        /// </summary>
        public double DefaultHeight { get; set; } = 200d;

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
