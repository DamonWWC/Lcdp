using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Controls.Adorners
{
    /// <summary>
    /// 网格的行分割器
    /// </summary>
    public class GridRowSplitterAdorner : GridSplitterAdorner
    {
        static GridRowSplitterAdorner()
        {
            // 覆盖默认样式
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridRowSplitterAdorner), new FrameworkPropertyMetadata(typeof(GridRowSplitterAdorner)));
            // 覆盖默认光标，使用南北向双头光标
            CursorProperty.OverrideMetadata(typeof(GridRowSplitterAdorner), new FrameworkPropertyMetadata(Cursors.SizeNS));
        }

        internal GridRowSplitterAdorner(GridRailAdorner rail, DesignItem gridItem, DesignItem firstRow, DesignItem secondRow) : base(rail, gridItem, firstRow, secondRow) { }

        /// <summary>
        /// 获取行分割器的坐标位置
        /// </summary>
        /// <param name="point">相对于Grid的鼠标位置</param>
        /// <returns>分割器相对于Grid的Y轴坐标</returns>
        protected override double GetCoordinate(Point point) => point.Y;

        /// <summary>
        /// 记忆第一二行的原始尺寸
        /// </summary>
        protected override void RememberOriginalSize()
        {
            // 第一二行的行定义
            RowDefinition r1 = (RowDefinition)FirstRow.Component;
            RowDefinition r2 = (RowDefinition)SecondRow.Component;
            // 第一二行的GridLength
            Original1 = (GridLength)r1.GetValue(RowDefinition.HeightProperty);
            Original2 = (GridLength)r2.GetValue(RowDefinition.HeightProperty);
            // 第一二行实际高度
            OriginalPixelSize1 = r1.ActualHeight;
            OriginalPixelSize2 = r2.ActualHeight;
        }

        /// <summary>
        /// 行的高度依赖属性
        /// </summary>
        protected override DependencyProperty RowColumnSizeProperty => RowDefinition.HeightProperty;
    }
}
