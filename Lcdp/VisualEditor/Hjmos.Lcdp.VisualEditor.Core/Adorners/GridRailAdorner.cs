using Hjmos.Lcdp.VisualEditor.Core.DesignerControls;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Core.Adorners
{
    /// <summary>
    /// 显示在网格旁边的条状轨道装饰器，用于创建新行/列。
    /// </summary>
    public class GridRailAdorner : FrameworkElement
    {
        static GridRailAdorner()
        {
            // 条状轨道的颜色 #351E90FF a 53 r 30 g 144 b 255
            _bgBrush = new SolidColorBrush(Color.FromArgb(0x35, 0x1E, 0x90, 0xff));
            // 使对象不可修改
            _bgBrush.Freeze();
        }

        /// <summary>被装饰的网格设计项</summary>
        private readonly DesignItem _gridItem;
        /// <summary>被装饰的网格实例</summary>
        private readonly Grid _grid;
        /// <summary>装饰器面板</summary>
        private readonly AdornerPanel _adornerPanel;
        /// <summary>预览网格分割器（跟随鼠标移动的分割指示器）</summary>
        private readonly GridSplitterAdorner _previewAdorner;
        /// <summary>轨道的方向</summary>
        private readonly Orientation _orientation;
        /// <summary>网格单位选择器</summary>
        private readonly GridUnitSelector _unitSelector;
        /// <summary>轨道的颜色</summary>
        private static readonly SolidColorBrush _bgBrush;
        /// <summary>指示是否显示网格单位选择器  TODO：可以做成属性，在修改值的时候，自动切换单位选择器的隐藏状态</summary>
        private bool _displayUnitSelector;
        /// <summary>轨道宽度</summary>
        public const double RailSize = 10;
        /// <summary>轨道和Grid的距离</summary>
        public const double RailDistance = 6;
        /// <summary>分割指示器的宽度（就是三角形箭头的宽度）</summary>
        public const double SplitterWidth = 10;

        public GridRailAdorner(DesignItem gridItem, AdornerPanel adornerPanel, Orientation orientation)
        {
            Debug.Assert(gridItem != null);
            Debug.Assert(adornerPanel != null);

            _gridItem = gridItem;
            _grid = gridItem.Component as Grid;
            _adornerPanel = adornerPanel;
            _orientation = orientation;
            _displayUnitSelector = false;
            _unitSelector = new GridUnitSelector(this) { Orientation = _orientation, Visibility = Visibility.Hidden };

            // 把网格单位选择器添加到装饰器面板
            _adornerPanel.Children.Add(_unitSelector);

            // 设置轨道宽度、创建预览分割器
            if (_orientation == Orientation.Horizontal)
            {
                this.Height = RailSize;
                _previewAdorner = new GridColumnSplitterAdorner(this, gridItem, null, null) { IsPreview = true, IsHitTestVisible = false };
            }
            else
            {
                this.Width = RailSize;
                _previewAdorner = new GridRowSplitterAdorner(this, gridItem, null, null) { IsPreview = true, IsHitTestVisible = false };
            }

        }

        /// <summary>
        /// 自定义装饰器的渲染
        /// </summary>
        /// <param name="drawingContext"></param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            // 轨道方向水平
            if (_orientation == Orientation.Horizontal)
            {
                // 绘制水平条状轨道
                Rect bgRect = new(0, 0, _grid.ActualWidth, RailSize);
                drawingContext.DrawRectangle(_bgBrush, null, bgRect);

                // 获取网格的列定义
                DesignItemProperty colCollection = _gridItem.Properties["ColumnDefinitions"];

                // 在轨道上标记每一列的宽度
                foreach (DesignItem colItem in colCollection.CollectionElements)
                {
                    ColumnDefinition column = colItem.Component as ColumnDefinition;
                    if (column.ActualWidth < 0) continue;
                    GridLength len = (GridLength)column.GetValue(ColumnDefinition.WidthProperty);

                    // 文本内容
                    FormattedText text = new(GridLengthToText(len), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Sergio UI"), 10, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip)
                    {
                        // 文本居中对齐
                        TextAlignment = TextAlignment.Center
                    };

                    // 绘制文本
                    drawingContext.DrawText(text, new Point(column.Offset + column.ActualWidth / 2, 0));
                }
            }
            // 轨道方向垂直
            else
            {
                // 绘制垂直条状轨道
                Rect bgRect = new(0, 0, RailSize, _grid.ActualHeight);
                drawingContext.DrawRectangle(_bgBrush, null, bgRect);

                // 获取网格的行定义
                DesignItemProperty rowCollection = _gridItem.Properties["RowDefinitions"];

                // 在轨道上标记每一行的高度
                foreach (DesignItem rowItem in rowCollection.CollectionElements)
                {
                    RowDefinition row = rowItem.Component as RowDefinition;
                    if (row.ActualHeight < 0) continue;
                    GridLength len = (GridLength)row.GetValue(RowDefinition.HeightProperty);

                    // 文本内容
                    FormattedText text = new(GridLengthToText(len), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Sergio UI"), 10, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip)
                    {
                        // 文本居中对齐
                        TextAlignment = TextAlignment.Center
                    };
                    // 文本附加旋转变换
                    drawingContext.PushTransform(new RotateTransform(-90));
                    // 绘制文本
                    drawingContext.DrawText(text, new Point((row.Offset + row.ActualHeight / 2) * -1, 0));
                    // 清除附加的变换效果，避免影响后续绘制
                    drawingContext.Pop();
                }
            }
        }

        #region 处理鼠标事件以添加新的行/列

        /// <summary>
        /// 鼠标进入
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            // 十字线光标
            this.Cursor = Cursors.Cross;

            // 为单位选择器提供相对摆放属性的类 TODO:鼠标进入的方法可以用不到这个位置帮助类，但是这里留着可以用来测试
            RelativePlacement rpUnitSelector = new();

            // 轨道方向垂直
            if (_orientation == Orientation.Vertical)
            {
                // 将要插入分割线的位置
                double insertionPosition = e.GetPosition(this).Y;
                // 当前行：分割线所在的行
                RowDefinition current = _grid.RowDefinitions.FirstOrDefault(r => insertionPosition >= r.Offset && insertionPosition <= (r.Offset + r.ActualHeight));
                if (current != null)
                {
                    // 获取当前行对应的设计项
                    DesignItem component = _gridItem.Services.Component.GetDesignItem(current);

                    // TODO:
                    if (component == null)
                    {
                        component = _gridItem.Services.Component.RegisterComponentForDesigner(current);
                    }
                    // X轴偏移量：-(轨道宽度 + 边距) * 2.75 - 6    TODO: 这是啥意思？
                    rpUnitSelector.XOffset = -(RailSize + RailDistance) * 2.75 - 6;
                    // 宽度偏移量：轨道宽度 + 边距 TODO: 这是啥意思？
                    rpUnitSelector.WidthOffset = RailSize + RailDistance;
                    // 相对宽度因子  TODO: 这是啥意思？
                    rpUnitSelector.WidthRelativeToContentWidth = 1;
                    // 高度偏移量：55  TODO: 这是啥意思？
                    rpUnitSelector.HeightOffset = 55;
                    // Y轴偏移量：当前行左侧坐标 + 当前行高度 / 2 - 25  TODO: 这是啥意思？
                    rpUnitSelector.YOffset = current.Offset + current.ActualHeight / 2 - 25;
                    // 网格单位选择器附加到当前行
                    _unitSelector.SelectedItem = component;
                    // 根据当前行，设置网格单位选择器的单位类型
                    _unitSelector.Unit = ((GridLength)component.Properties[RowDefinition.HeightProperty].ValueOnInstance).GridUnitType;
                    // 指示是否显示网格单位选择器
                    _displayUnitSelector = true;
                }
                else
                {
                    _displayUnitSelector = false;
                }
            }
            // 轨道方向水平
            else
            {
                // 将要插入分割线的位置
                double insertionPosition = e.GetPosition(this).X;
                // 当前列：分割线所在的列
                ColumnDefinition current = _grid.ColumnDefinitions.FirstOrDefault(r => insertionPosition >= r.Offset && insertionPosition <= (r.Offset + r.ActualWidth));
                if (current != null)
                {
                    // 获取当前列对应的设计项
                    DesignItem component = _gridItem.Services.Component.GetDesignItem(current);

                    // TODO:
                    if (component == null)
                    {
                        component = _gridItem.Services.Component.RegisterComponentForDesigner(current);
                    }
                    Debug.Assert(component != null);
                    // Y轴偏移量：-(轨道高度 + 边距) * 2.20 - 6    TODO: 这是啥意思？
                    rpUnitSelector.YOffset = -(RailSize + RailDistance) * 2.20 - 6;
                    // 高度偏移量：轨道高度 + 边距 TODO: 这是啥意思？
                    rpUnitSelector.HeightOffset = RailSize + RailDistance;
                    // 相对高度因子  TODO: 这是啥意思？
                    rpUnitSelector.HeightRelativeToContentHeight = 1;
                    // 宽度偏移量：75  TODO: 这是啥意思？
                    rpUnitSelector.WidthOffset = 75;
                    // X轴偏移量：当前列顶部坐标 + 当前列宽度 / 2 - 35  TODO: 这是啥意思？
                    rpUnitSelector.XOffset = current.Offset + current.ActualWidth / 2 - 35;
                    // 网格单位选择器附加到当前列
                    _unitSelector.SelectedItem = component;
                    // 根据当前列，设置网格单位选择器的单位类型  TODO:这里的WidthProperty回去不到，还没有放到Properties中
                    _unitSelector.Unit = ((GridLength)component.Properties[ColumnDefinition.WidthProperty].ValueOnInstance).GridUnitType;
                    // 指示是否显示网格单位选择器
                    _displayUnitSelector = true;
                }
                else
                {
                    _displayUnitSelector = false;
                }
            }

            // 显示网格单位选择器
            if (_displayUnitSelector)
                _unitSelector.Visibility = Visibility.Visible;

            // 把预览分割器添加到装饰器面板
            if (!_adornerPanel.Children.Contains(_previewAdorner))
                _adornerPanel.Children.Add(_previewAdorner);
        }

        /// <summary>
        /// 鼠标移动
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            // 提供相对摆放属性的类
            RelativePlacement rp = new();
            // 为单位选择器提供相对摆放属性的类
            RelativePlacement rpUnitSelector = new();

            // 轨道方向垂直
            if (_orientation == Orientation.Vertical)
            {
                // 将要插入分割线的位置
                double insertionPosition = e.GetPosition(this).Y;
                // 当前行：分割线所在的行
                RowDefinition current = _grid.RowDefinitions.FirstOrDefault(r => insertionPosition >= r.Offset && insertionPosition <= (r.Offset + r.ActualHeight));

                // X轴偏移量：-(轨道宽度 + 边距)  TODO: 这是啥意思？
                rp.XOffset = -(RailSize + RailDistance);
                // 宽度偏移量：轨道宽度 + 边距 TODO: 这是啥意思？
                rp.WidthOffset = RailSize + RailDistance;
                // 相对宽度因子  TODO: 这是啥意思？
                rp.WidthRelativeToContentWidth = 1;
                // 高度偏移量：分割器的宽度  TODO: 这是啥意思？
                rp.HeightOffset = SplitterWidth;
                // Y轴偏移量：轨道的Y坐标 - 分割器的宽度 / 2  TODO: 这是啥意思？
                rp.YOffset = e.GetPosition(this).Y - SplitterWidth / 2;
                if (current != null)
                {
                    // 获取当前行对应的设计项
                    DesignItem component = _gridItem.Services.Component.GetDesignItem(current);
                    // X轴偏移量：-(轨道宽度 + 边距) * 2.75 - 6    TODO: 这是啥意思？
                    rpUnitSelector.XOffset = -(RailSize + RailDistance) * 3.25 - 6;
                    // 增加宽度：轨道宽度 + 边距 TODO: 这是啥意思？
                    rpUnitSelector.WidthOffset = RailSize + RailDistance;
                    // 相对宽度因子  TODO: 这是啥意思？
                    rpUnitSelector.WidthRelativeToContentWidth = 1;
                    // 增加高度：55  TODO: 这是啥意思？
                    rpUnitSelector.HeightOffset = 120;
                    // Y轴偏移量：当前行左侧坐标 + 当前行高度 / 2 - 25  TODO: 这是啥意思？
                    rpUnitSelector.YOffset = current.Offset + current.ActualHeight / 2 - 60;
                    // 网格单位选择器附加到当前行
                    _unitSelector.SelectedItem = component;
                    // 根据当前行，设置网格单位选择器的单位类型
                    _unitSelector.Unit = ((GridLength)component.Properties[RowDefinition.HeightProperty].ValueOnInstance).GridUnitType;
                    // 指示是否显示网格单位选择器
                    _displayUnitSelector = true;
                }
                else
                {
                    _displayUnitSelector = false;
                }
            }
            // 轨道方向水平
            else
            {
                // 将要插入分割线的位置
                double insertionPosition = e.GetPosition(this).X;
                // 当前列：分割线所在的列
                ColumnDefinition current = _grid.ColumnDefinitions.FirstOrDefault(r => insertionPosition >= r.Offset && insertionPosition <= (r.Offset + r.ActualWidth));

                // Y轴偏移量：-(轨道高度 + 边距)   TODO: 这是啥意思？
                rp.YOffset = -(RailSize + RailDistance);
                // 高度偏移量：轨道高度 + 边距 TODO: 这是啥意思？
                rp.HeightOffset = RailSize + RailDistance;
                // 相对高度因子  TODO: 这是啥意思？
                rp.HeightRelativeToContentHeight = 1;
                // 宽度偏移量：分割器的宽度  TODO: 这是啥意思？
                rp.WidthOffset = SplitterWidth;
                // X轴偏移量：轨道的X坐标 - 分割器的宽度 / 2  TODO: 这是啥意思？
                rp.XOffset = e.GetPosition(this).X - SplitterWidth / 2;

                if (current != null)
                {
                    // 获取当前列对应的设计项
                    DesignItem component = _gridItem.Services.Component.GetDesignItem(current);
                    Debug.Assert(component != null);
                    // Y轴偏移量：-(轨道高度 + 边距) * 2.20 - 6    TODO: 这是啥意思？
                    rpUnitSelector.YOffset = -(RailSize + RailDistance) * 3.3d - 6;
                    // 增加高度：轨道高度 + 边距
                    rpUnitSelector.HeightOffset = RailSize + RailDistance;
                    // 相对高度因子（应该是相对于原高度比例的意思）
                    rpUnitSelector.HeightRelativeToContentHeight = 1;
                    // 增加宽度：75
                    rpUnitSelector.WidthOffset = 90;
                    // X轴偏移量：当前列顶部坐标 + 当前列宽度 / 2 - 35  TODO: 这是啥意思？
                    rpUnitSelector.XOffset = current.Offset + current.ActualWidth / 2 - 45;
                    // 网格单位选择器附加到当前列
                    _unitSelector.SelectedItem = component;
                    // 根据当前列，设置网格单位选择器的单位类型
                    _unitSelector.Unit = ((GridLength)component.Properties[ColumnDefinition.WidthProperty].ValueOnInstance).GridUnitType;
                    // 指示是否显示网格单位选择器
                    _displayUnitSelector = true;
                }
                else
                {
                    _displayUnitSelector = false;
                }
            }
            // 调整预览分割器的位置
            AdornerPanel.SetPlacement(_previewAdorner, rp);
            // 调整网格单位选择器的位置
            if (_displayUnitSelector)
                AdornerPanel.SetPlacement(_unitSelector, rpUnitSelector);
        }

        /// <summary>
        /// 鼠标离开
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            // 强制更新鼠标光标
            Mouse.UpdateCursor();

            // 如果鼠标未停留在单位选择器上
            if (!_unitSelector.IsMouseOver)
            {
                // 隐藏单位选择器
                _unitSelector.Visibility = Visibility.Hidden;
                // 标记为隐藏状态
                _displayUnitSelector = false;
            }
            // 从装饰器面板移除预览分割器
            if (_adornerPanel.Children.Contains(_previewAdorner))
                _adornerPanel.Children.Remove(_previewAdorner);
        }

        /// <summary>
        /// 按下鼠标左键
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            e.Handled = true;

            // 获取焦点
            Focus();
            // 从装饰器面板移除预览分割器
            _adornerPanel.Children.Remove(_previewAdorner);

            // 轨道方向垂直
            if (_orientation == Orientation.Vertical)
            {
                // 将要插入分割线的位置
                double insertionPosition = e.GetPosition(this).Y;

                // 获取网格的行定义
                DesignItemProperty rowCollection = _gridItem.Properties["RowDefinitions"];

                // 当前行
                DesignItem currentRow = null;

                //using (ChangeGroup changeGroup = gridItem.OpenGroup("Split grid row"))
                //{

                // 获取当前行
                if (rowCollection.CollectionElements.Count == 0)
                {
                    // 添加一个RowDefinition的设计项作为第一行
                    DesignItem firstRow = _gridItem.Services.Component.RegisterComponentForDesigner(new RowDefinition());
                    rowCollection.CollectionElements.Add(firstRow);
                    // 让WPF分配firstRow.ActualHeight
                    _grid.UpdateLayout();

                    // 当前行为第一行
                    currentRow = firstRow;
                }
                else
                {
                    // 当前行：分割线所在的行
                    RowDefinition current = _grid.RowDefinitions.FirstOrDefault(r => insertionPosition >= r.Offset && insertionPosition <= (r.Offset + r.ActualHeight));
                    // 获取当前行对应的设计项
                    if (current != null)
                        currentRow = _gridItem.Services.Component.GetDesignItem(current);
                }

                // 如果网格设计项获取不到当前行，就从网格实例中附加一个
                if (currentRow == null)
                    currentRow = _gridItem.Services.Component.GetDesignItem(_grid.RowDefinitions.Last());

                // 网格单位选择器附加到当前行
                _unitSelector.SelectedItem = currentRow;

                for (int i = 0; i < _grid.RowDefinitions.Count; i++)
                {
                    RowDefinition row = _grid.RowDefinitions[i];

                    if (row.Offset > insertionPosition) continue;
                    if (row.Offset + row.ActualHeight < insertionPosition) continue;

                    // 分割行

                    // 旧的行高
                    GridLength oldLength = (GridLength)row.GetValue(RowDefinition.HeightProperty);
                    // 获取分割后的行高
                    SplitLength(oldLength, insertionPosition - row.Offset, row.ActualHeight, out GridLength newLength1, out GridLength newLength2);
                    // 获取一个新行的设计项
                    DesignItem newRowDefinition = _gridItem.Services.Component.RegisterComponentForDesigner(new RowDefinition());
                    // 行定义中加入新行
                    rowCollection.CollectionElements.Insert(i + 1, newRowDefinition);
                    // 调整原行长度
                    rowCollection.CollectionElements[i].Properties[RowDefinition.HeightProperty].SetValue(newLength1);
                    // 设置新行长度
                    newRowDefinition.Properties[RowDefinition.HeightProperty].SetValue(newLength2);
                    // 更新布局
                    _grid.UpdateLayout();
                    // 修复分割后网格子元素的行标记
                    FixIndicesAfterSplit(i, Grid.RowProperty, Grid.RowSpanProperty, insertionPosition);
                    // 更新布局
                    _grid.UpdateLayout();
                    //changeGroup.Commit();
                    break;
                }
                //}
            }
            // 轨道方向水平
            else
            {
                // 将要插入分割线的位置
                double insertionPosition = e.GetPosition(this).X;
                // 获取网格的列定义
                DesignItemProperty columnCollection = _gridItem.Properties["ColumnDefinitions"];

                // 当前列
                DesignItem currentColumn = null;

                //using (ChangeGroup changeGroup = gridItem.OpenGroup("Split grid column"))
                //{

                // 获取当前列
                if (columnCollection.CollectionElements.Count == 0)
                {
                    // 添加一个RowDefinition的设计项作为第一列
                    DesignItem firstColumn = _gridItem.Services.Component.RegisterComponentForDesigner(new ColumnDefinition());
                    columnCollection.CollectionElements.Add(firstColumn);
                    // 让WPF分配firstRow.ActualWidth
                    _grid.UpdateLayout();

                    // 当前列为第一列
                    currentColumn = firstColumn;
                }
                else
                {
                    // 当前列：分割线所在的列
                    ColumnDefinition current = _grid.ColumnDefinitions.FirstOrDefault(r => insertionPosition >= r.Offset && insertionPosition <= (r.Offset + r.ActualWidth));
                    if (current != null)
                        currentColumn = _gridItem.Services.Component.GetDesignItem(current);
                }

                // 如果网格设计项获取不到当前列，就从网格实例中附加一个
                if (currentColumn == null)
                    currentColumn = _gridItem.Services.Component.GetDesignItem(_grid.ColumnDefinitions.Last());

                // 网格单位选择器附加到当前列
                _unitSelector.SelectedItem = currentColumn;

                for (int i = 0; i < _grid.ColumnDefinitions.Count; i++)
                {
                    ColumnDefinition column = _grid.ColumnDefinitions[i];

                    if (column.Offset > insertionPosition) continue;
                    if (column.Offset + column.ActualWidth < insertionPosition) continue;

                    // 分割列

                    // 旧的列宽
                    GridLength oldLength = (GridLength)column.GetValue(ColumnDefinition.WidthProperty);
                    // 获取分割后的列宽
                    SplitLength(oldLength, insertionPosition - column.Offset, column.ActualWidth, out GridLength newLength1, out GridLength newLength2);
                    // 获取一个新列的设计项
                    DesignItem newColumnDefinition = _gridItem.Services.Component.RegisterComponentForDesigner(new ColumnDefinition());
                    // 列定义中加入新列
                    columnCollection.CollectionElements.Insert(i + 1, newColumnDefinition);
                    // 调整原列长度
                    columnCollection.CollectionElements[i].Properties[ColumnDefinition.WidthProperty].SetValue(newLength1);
                    // 设置新列长度
                    newColumnDefinition.Properties[ColumnDefinition.WidthProperty].SetValue(newLength2);
                    // 更新布局
                    _grid.UpdateLayout();
                    // 修复分割后网格子元素的列标记
                    FixIndicesAfterSplit(i, Grid.ColumnProperty, Grid.ColumnSpanProperty, insertionPosition);
                    //changeGroup.Commit();
                    // 更新布局
                    _grid.UpdateLayout();
                    break;
                }
                //}
            }
            InvalidateVisual();
        }

        /// <summary>
        /// 获取分割后的行/列长度
        /// </summary>
        /// <param name="oldLength">旧的长度</param>
        /// <param name="insertionPosition">鼠标位置</param>
        /// <param name="oldActualValue">旧的实际长度</param>
        /// <param name="newLength1">分割后新的长度1</param>
        /// <param name="newLength2">分割后新的长度2</param>
        static void SplitLength(GridLength oldLength, double insertionPosition, double oldActualValue, out GridLength newLength1, out GridLength newLength2)
        {
            // 将自动长度替换为绝对长度
            if (oldLength.IsAuto)
            {
                oldLength = new GridLength(oldActualValue);
            }
            // 计算分割位置的百分比
            double percentage = insertionPosition / oldActualValue;
            // 分割后新的长度1
            newLength1 = new GridLength(oldLength.Value * percentage, oldLength.GridUnitType);
            // 分割后新的长度2
            newLength2 = new GridLength(oldLength.Value - newLength1.Value, oldLength.GridUnitType);
        }

        /// <summary>
        /// 修复分割后网格子元素的行/列标记(如：Row和RowSpan属性)
        /// </summary>
        /// <param name="splitIndex">被分割行/列的索引</param>
        /// <param name="idxProperty">行/列位置索引附加属性</param>
        /// <param name="spanProperty">跨越合并数量附加属性</param>
        /// <param name="insertionPostion">分割的鼠标位置</param>
        private void FixIndicesAfterSplit(int splitIndex, DependencyProperty idxProperty, DependencyProperty spanProperty, double insertionPostion)
        {
            // 轨道方向水平
            if (_orientation == Orientation.Horizontal)
            {
                // 在拆分列中增加所有控件的ColSpan，在以后的列中增加所有控件的Column:
                foreach (DesignItem child in _gridItem.Properties["Children"].CollectionElements)
                {
                    // 获取元素相对于网格的坐标
                    Point topLeft = child.View.TranslatePoint(new Point(0, 0), _grid);
                    // 获取元素的边距
                    Thickness margin = (Thickness)child.Properties[MarginProperty].ValueOnInstance;
                    // 获取原来的列索引
                    int start = (int)child.Properties.GetAttachedProperty(idxProperty).ValueOnInstance;
                    // 获取跨越合并数
                    int span = (int)child.Properties.GetAttachedProperty(spanProperty).ValueOnInstance;

                    // 子元素位于拆分列中
                    if (start <= splitIndex && splitIndex < start + span)
                    {
                        // 子元素宽度
                        double width = (double)child.Properties[ActualWidthProperty].ValueOnInstance;
                        // 分割后子元素完全在第一列
                        if (insertionPostion >= topLeft.X + width)
                            continue;
                        // 分割后子元素在第一二列之间
                        if (insertionPostion > topLeft.X)
                            // 跨列+1
                            child.Properties.GetAttachedProperty(spanProperty).SetValue(span + 1);
                        // 分割后子元素在第二列
                        else
                        {
                            // 列索引+1
                            child.Properties.GetAttachedProperty(idxProperty).SetValue(start + 1);
                            // 左边距为原来边距减去分割位置（这样子元素视觉上就不会移动）
                            margin.Left = topLeft.X - insertionPostion;
                            // 重新设置子元素设计项的边距
                            child.Properties[MarginProperty].SetValue(margin);
                        }
                    }
                    // 子元素位于拆分列之后的列
                    else if (start > splitIndex)
                    {
                        // 列索引+1
                        child.Properties.GetAttachedProperty(idxProperty).SetValue(start + 1);
                    }
                }
            }
            // 轨道方向垂直
            else
            {
                // 在拆分行中增加所有控件的RowSpan，在以后的列中增加所有控件的Row:
                foreach (DesignItem child in _gridItem.Properties["Children"].CollectionElements)
                {
                    // 获取元素相对于网格的坐标
                    Point topLeft = child.View.TranslatePoint(new Point(0, 0), _grid);
                    // 获取元素的边距
                    Thickness margin = (Thickness)child.Properties[MarginProperty].ValueOnInstance;
                    // 获取原来的行索引
                    int start = (int)child.Properties.GetAttachedProperty(idxProperty).ValueOnInstance;
                    // 获取跨越合并数
                    int span = (int)child.Properties.GetAttachedProperty(spanProperty).ValueOnInstance;

                    // 子元素位于拆分行中
                    if (start <= splitIndex && splitIndex < start + span)
                    {
                        // 子元素高度
                        double height = (double)child.Properties[ActualHeightProperty].ValueOnInstance;
                        // 分割后子元素完全在第一行
                        if (insertionPostion >= topLeft.Y + height)
                            continue;
                        // 分割后子元素在第一二行之间
                        if (insertionPostion > topLeft.Y)
                            child.Properties.GetAttachedProperty(spanProperty).SetValue(span + 1);
                        // 分割后子元素在第二行
                        else
                        {
                            // 行索引+1
                            child.Properties.GetAttachedProperty(idxProperty).SetValue(start + 1);
                            // 上边距为原来边距减去分割位置（这样子元素视觉上就不会移动）
                            margin.Top = topLeft.Y - insertionPostion;
                            // 重新设置子元素设计项的边距
                            child.Properties[MarginProperty].SetValue(margin);
                        }
                    }
                    // 子元素位于拆分行之后的行
                    else if (start > splitIndex)
                    {
                        // 行索引+1
                        child.Properties.GetAttachedProperty(idxProperty).SetValue(start + 1);
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// GridLength的文本显示
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        private string GridLengthToText(GridLength len)
        {
            switch (len.GridUnitType)
            {
                case GridUnitType.Auto:
                    return "Auto";
                case GridUnitType.Star:
                    return len.Value == 1 ? "*" : Math.Round(len.Value, 2) + "*";
                case GridUnitType.Pixel:
                    return Math.Round(len.Value, 2) + "px";
            }
            return string.Empty;
        }

        /// <summary>
        /// 设置网格长度单位类型
        /// </summary>
        /// <param name="unit"></param>
        public void SetGridLengthUnit(GridUnitType unit)
        {
            DesignItem item = _unitSelector.SelectedItem;
            // 更新网格布局
            _grid.UpdateLayout();

            Debug.Assert(item != null);

            // 轨道方向垂直
            if (_orientation == Orientation.Vertical)
            {
                SetGridLengthUnit(unit, item, RowDefinition.HeightProperty);
            }
            // 轨道方向水平
            else
            {
                SetGridLengthUnit(unit, item, ColumnDefinition.WidthProperty);
            }
            _grid.UpdateLayout();
            // 重绘
            InvalidateVisual();
        }

        /// <summary>
        /// 设置网格长度单位
        /// </summary>
        /// <param name="unit">单位</param>
        /// <param name="item">当前设计项</param>
        /// <param name="property">要设置单位的依赖属性</param>
        private void SetGridLengthUnit(GridUnitType unit, DesignItem item, DependencyProperty property)
        {
            // 要设置单位的属性
            DesignItemProperty itemProperty = item.Properties[property];
            // 旧的长度
            GridLength oldValue = (GridLength)itemProperty.ValueOnInstance;
            // 根据单位获取新的长度
            GridLength value = GetNewGridLength(unit, oldValue);

            if (value != oldValue)
            {
                // 为属性分配新的长度
                itemProperty.SetValue(value);
            }
        }

        /// <summary>
        /// 根据单位获取新的长度
        /// </summary>
        /// <param name="unit">单位</param>
        /// <param name="oldValue">原来的实际长度</param>
        /// <returns></returns>
        private GridLength GetNewGridLength(GridUnitType unit, GridLength oldValue)
        {
            if (unit == GridUnitType.Auto)
            {
                return GridLength.Auto;
            }
            return new GridLength(oldValue.Value, unit);
        }
    }
}
