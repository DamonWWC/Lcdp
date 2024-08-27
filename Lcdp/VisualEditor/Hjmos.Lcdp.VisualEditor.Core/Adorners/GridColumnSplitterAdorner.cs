using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Core.Adorners
{
    public sealed class GridColumnSplitterAdorner : GridSplitterAdorner
    {
        static GridColumnSplitterAdorner()
        {
            // 覆盖默认样式
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridColumnSplitterAdorner), new FrameworkPropertyMetadata(typeof(GridColumnSplitterAdorner)));
            // 覆盖默认光标，使用东西向双头光标
            CursorProperty.OverrideMetadata(typeof(GridColumnSplitterAdorner), new FrameworkPropertyMetadata(Cursors.SizeWE));
        }

        internal GridColumnSplitterAdorner(GridRailAdorner rail, DesignItem gridItem, DesignItem firstRow, DesignItem secondRow) : base(rail, gridItem, firstRow, secondRow) { }

        /// <summary>
        /// 获取列分割器的坐标位置
        /// </summary>
        /// <param name="point">相对于Grid的鼠标位置</param>
        /// <returns>分割器相对于Grid的X轴坐标</returns>
        protected override double GetCoordinate(Point point) => point.X;

        protected override void RememberOriginalSize()
        {
            // 第一二列的列定义
            ColumnDefinition r1 = (ColumnDefinition)FirstRow.Component;
            ColumnDefinition r2 = (ColumnDefinition)SecondRow.Component;
            // 第一二列的GridLength
            Original1 = (GridLength)r1.GetValue(ColumnDefinition.WidthProperty);
            Original2 = (GridLength)r2.GetValue(ColumnDefinition.WidthProperty);
            // 第一二列的宽度
            OriginalPixelSize1 = r1.ActualWidth;
            OriginalPixelSize2 = r2.ActualWidth;
        }

        /// <summary>
        /// 列的宽度依赖属性
        /// </summary>
        protected override DependencyProperty RowColumnSizeProperty => ColumnDefinition.WidthProperty;
    }
}
