using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Core
{
    /// <summary>
    /// 存储在<see cref="PlacementOperation"/>期间单个项的放置信息。
    /// </summary>
    public sealed class PlacementInformation
    {
        /// <summary>
        /// 精度范围：设计器将边界舍入到这个数字，以避免浮点错误。
        /// Value: 0
        /// </summary>
        public const int BoundsPrecision = 0;

        /// <summary>
        /// 被放置的项
        /// </summary>
        public DesignItem Item { get; }

        /// <summary>
        /// 该PlacementInformation所属的<see cref="PlacementOperation"/>。
        /// </summary>
        public PlacementOperation Operation { get; }

        internal PlacementInformation(DesignItem item, PlacementOperation operation)
        {
            this.Item = item;
            this.Operation = operation;
        }

        /// <summary>
        /// 获取/设置原始边界。
        /// </summary>
        public Rect OriginalBounds { get; set; }

        /// <summary>
        /// 获取/设置项的当前边界。
        /// </summary>
        public Rect Bounds { get; set; }

        /// <summary>
        /// 获取/设置用于启动操作的调整大小拇指的对齐方式。
        /// </summary>
        public PlacementAlignment? ResizeThumbAlignment { get; set; }

        /// <inheritdoc/>
        public override string ToString() => "[PlacementInformation OriginalBounds=" + OriginalBounds + " Bounds=" + Bounds + " Item=" + Item + "]";
    }
}
