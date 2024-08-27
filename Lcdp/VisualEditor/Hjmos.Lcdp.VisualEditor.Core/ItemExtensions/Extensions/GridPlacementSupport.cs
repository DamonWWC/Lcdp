using Hjmos.Lcdp.VisualEditor.Core.DesignerControls;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    /// <summary>
    /// 为<see cref="Grid"/>提供<see cref="IPlacementBehavior"/>行为
    /// </summary>
    [ExtensionFor(typeof(Grid), OverrideExtension = typeof(DefaultPlacementBehavior))]
    public sealed class GridPlacementSupport : SnaplinePlacementBehavior
    {
        private Grid _grid;
        /// <summary>已进入新的容器</summary>
        private bool _enteredIntoNewContainer;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            _grid = ExtendedItem.Component as Grid;
        }

        /// <summary>
        /// 获取列相对于Grid的偏移量
        /// </summary>
        /// <param name="index">列索引</param>
        /// <returns></returns>
        private double GetColumnOffset(int index)
        {
            // 当网格没有列时，我们仍然需要为index=0和grid返回0。宽度指数= 1
            if (index == 0)
                return 0;
            else if (index < _grid.ColumnDefinitions.Count)
                return _grid.ColumnDefinitions[index].Offset;
            else
                return _grid.ActualWidth;
        }

        /// <summary>
        /// 获取行相对于Grid的偏移量
        /// </summary>
        /// <param name="index">行索引</param>
        /// <returns></returns>
        private double GetRowOffset(int index)
        {
            if (index == 0)
                return 0;
            else if (index < _grid.RowDefinitions.Count)
                return _grid.RowDefinitions[index].Offset;
            else
                return _grid.ActualHeight;
        }

        /// <summary>一个很小的量，TODO：暂时不清楚计算列的时候为什么要用，看起来不用也行，可能是考虑到一些吸附或者对齐像素的功能导致的误差？还是考虑到刚好坐标在线上的问题？</summary>
        private const double epsilon = 0.00000001;

        /// <summary>
        /// 根据X轴坐标获取列索引（如果跟网格线重叠，就选前一列？）
        /// </summary>
        /// <param name="x">X轴坐标</param>
        /// <returns></returns>
        private int GetColumnIndex(double x)
        {
            if (_grid.ColumnDefinitions.Count == 0)
                return 0;

            for (int i = 1; i < _grid.ColumnDefinitions.Count; i++)
            {
                if (x < _grid.ColumnDefinitions[i].Offset - epsilon)
                    return i - 1;
            }
            return _grid.ColumnDefinitions.Count - 1;
        }

        /// <summary>
        /// 根据Y轴坐标获取行索引（如果跟网格线重叠，就选前一行？）
        /// </summary>
        /// <param name="y">Y轴坐标</param>
        /// <returns></returns>
        private int GetRowIndex(double y)
        {
            if (_grid.RowDefinitions.Count == 0)
                return 0;
            for (int i = 1; i < _grid.RowDefinitions.Count; i++)
            {
                if (y < _grid.RowDefinitions[i].Offset - epsilon)
                    return i - 1;
            }
            return _grid.RowDefinitions.Count - 1;
        }

        /// <summary>
        /// 根据X轴坐标获取列索引（如果跟网格线重叠，就选后一列？）
        /// </summary>
        /// <param name="x">X轴坐标</param>
        /// <returns></returns>
        private int GetEndColumnIndex(double x)
        {
            if (_grid.ColumnDefinitions.Count == 0)
                return 0;
            for (int i = 1; i < _grid.ColumnDefinitions.Count; i++)
            {
                if (x <= _grid.ColumnDefinitions[i].Offset + epsilon)
                    return i - 1;
            }
            return _grid.ColumnDefinitions.Count - 1;
        }

        /// <summary>
        /// 根据Y轴坐标获取行索引（如果跟网格线重叠，就选前一行？）
        /// </summary>
        /// <param name="y">Y轴坐标</param>
        /// <returns></returns>
        private int GetEndRowIndex(double y)
        {
            if (_grid.RowDefinitions.Count == 0)
                return 0;
            for (int i = 1; i < _grid.RowDefinitions.Count; i++)
            {
                if (y <= _grid.RowDefinitions[i].Offset + epsilon)
                    return i - 1;
            }
            return _grid.RowDefinitions.Count - 1;
        }

        /// <summary>
        /// 添加容器吸附性
        /// </summary>
        /// <param name="containerRect"></param>
        /// <param name="horizontalMap"></param>
        /// <param name="verticalMap"></param>
        protected override void AddContainerSnaplines(Rect containerRect, List<Snapline> horizontalMap, List<Snapline> verticalMap)
        {
            Grid grid = ExtendedItem.View as Grid;
            double offset = 0;
            foreach (RowDefinition r in grid.RowDefinitions)
            {
                offset += r.ActualHeight;
                horizontalMap.Add(new Snapline() { RequireOverlap = false, Offset = offset, Start = offset, End = containerRect.Right });
                if (SnaplineMargin > 0)
                {
                    horizontalMap.Add(new Snapline() { RequireOverlap = false, Offset = offset - SnaplineMargin, Start = offset, End = containerRect.Right });
                    horizontalMap.Add(new Snapline() { RequireOverlap = false, Offset = offset + SnaplineMargin, Start = offset, End = containerRect.Right });
                }

            }
            offset = 0;
            foreach (ColumnDefinition c in grid.ColumnDefinitions)
            {
                offset += c.ActualWidth;
                verticalMap.Add(new Snapline() { RequireOverlap = false, Offset = offset, Start = containerRect.Top, End = containerRect.Bottom });
                if (SnaplineMargin > 0)
                {
                    verticalMap.Add(new Snapline() { RequireOverlap = false, Offset = offset - SnaplineMargin, Start = containerRect.Top, End = containerRect.Bottom });
                    verticalMap.Add(new Snapline() { RequireOverlap = false, Offset = offset + SnaplineMargin, Start = containerRect.Top, End = containerRect.Bottom });
                }
            }
        }

        /// <summary>
        /// 设置设计项的列位置和跨越合并数
        /// </summary>
        /// <param name="item"></param>
        /// <param name="column"></param>
        /// <param name="columnSpan"></param>
        private static void SetColumn(DesignItem item, int column, int columnSpan)
        {
            Debug.Assert(item != null && column >= 0 && columnSpan > 0);
            item.Properties.GetAttachedProperty(Grid.ColumnProperty).SetValue(column);
            if (columnSpan == 1)
            {
                item.Properties.GetAttachedProperty(Grid.ColumnSpanProperty).Reset();
            }
            else
            {
                item.Properties.GetAttachedProperty(Grid.ColumnSpanProperty).SetValue(columnSpan);
            }
        }

        /// <summary>
        /// 设置设计项的行位置和跨越合并数
        /// </summary>
        /// <param name="item"></param>
        /// <param name="row"></param>
        /// <param name="rowSpan"></param>
        private static void SetRow(DesignItem item, int row, int rowSpan)
        {
            Debug.Assert(item != null && row >= 0 && rowSpan > 0);
            item.Properties.GetAttachedProperty(Grid.RowProperty).SetValue(row);
            if (rowSpan == 1)
            {
                item.Properties.GetAttachedProperty(Grid.RowSpanProperty).Reset();
            }
            else
            {
                item.Properties.GetAttachedProperty(Grid.RowSpanProperty).SetValue(rowSpan);
            }
        }

        /// <summary>
        /// TODO：好像是显示水平对齐的辅助线条
        /// </summary>
        /// <param name="itemBounds"></param>
        /// <param name="availableSpaceRect"></param>
        /// <returns></returns>
        private static HorizontalAlignment SuggestHorizontalAlignment(Rect itemBounds, Rect availableSpaceRect)
        {
            bool isLeft = itemBounds.Left < availableSpaceRect.Left + availableSpaceRect.Width / 4;
            bool isRight = itemBounds.Right > availableSpaceRect.Right - availableSpaceRect.Width / 4;
            return isLeft && isRight ? HorizontalAlignment.Stretch : isRight ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }

        /// <summary>
        /// TODO：好像是显示垂直对齐的辅助线条
        /// </summary>
        /// <param name="itemBounds"></param>
        /// <param name="availableSpaceRect"></param>
        /// <returns></returns>
        private static VerticalAlignment SuggestVerticalAlignment(Rect itemBounds, Rect availableSpaceRect)
        {
            bool isTop = itemBounds.Top < availableSpaceRect.Top + availableSpaceRect.Height / 4;
            bool isBottom = itemBounds.Bottom > availableSpaceRect.Bottom - availableSpaceRect.Height / 4;
            return isTop && isBottom ? VerticalAlignment.Stretch : isBottom ? VerticalAlignment.Bottom : VerticalAlignment.Top;
        }

        /// <summary>
        /// 进入容器触发的方法
        /// </summary>
        /// <param name="operation"></param>
        public override void EnterContainer(PlacementOperation operation)
        {
            _enteredIntoNewContainer = true;
            _grid.UpdateLayout();
            base.EnterContainer(operation);

            if (operation.Type == PlacementType.PasteItem)
            {
                foreach (PlacementInformation info in operation.PlacedItems)
                {
                    Thickness margin = (Thickness)info.Item.Properties.GetProperty(FrameworkElement.MarginProperty).ValueOnInstance;
                    HorizontalAlignment horizontalAlignment = (HorizontalAlignment)info.Item.Properties.GetProperty(FrameworkElement.HorizontalAlignmentProperty).ValueOnInstance;
                    VerticalAlignment verticalAlignment = (VerticalAlignment)info.Item.Properties.GetProperty(FrameworkElement.VerticalAlignmentProperty).ValueOnInstance;

                    if (horizontalAlignment == HorizontalAlignment.Left)
                        margin.Left += PlacementOperation.PasteOffset;
                    else if (horizontalAlignment == HorizontalAlignment.Right)
                        margin.Right -= PlacementOperation.PasteOffset;
                    if (verticalAlignment == VerticalAlignment.Top)
                        margin.Top += PlacementOperation.PasteOffset;
                    else if (verticalAlignment == VerticalAlignment.Bottom)
                        margin.Bottom -= PlacementOperation.PasteOffset;

                    info.Item.Properties.GetProperty(FrameworkElement.MarginProperty).SetValue(margin);
                }
            }
        }

        /// <summary>画面变灰，除了特定区域</summary>
        private GrayOutDesignerExceptActiveArea _grayOut;

        /// <summary>
        /// 结束放置元素后调用
        /// </summary>
        /// <param name="operation"></param>
        public override void EndPlacement(PlacementOperation operation)
        {
            GrayOutDesignerExceptActiveArea.Stop(ref _grayOut);
            _enteredIntoNewContainer = false;
            base.EndPlacement(operation);
        }

        /// <summary>
        /// 设置元素位置
        /// </summary>
        /// <param name="info"></param>
        public override void SetPosition(PlacementInformation info)
        {
            base.SetPosition(info);
            int leftColumnIndex = GetColumnIndex(info.Bounds.Left);
            int rightColumnIndex = GetEndColumnIndex(info.Bounds.Right);
            if (rightColumnIndex < leftColumnIndex) rightColumnIndex = leftColumnIndex;
            SetColumn(info.Item, leftColumnIndex, rightColumnIndex - leftColumnIndex + 1);
            int topRowIndex = GetRowIndex(info.Bounds.Top);
            int bottomRowIndex = GetEndRowIndex(info.Bounds.Bottom);
            if (bottomRowIndex < topRowIndex) bottomRowIndex = topRowIndex;
            SetRow(info.Item, topRowIndex, bottomRowIndex - topRowIndex + 1);

            Rect availableSpaceRect = new(
                new Point(GetColumnOffset(leftColumnIndex), GetRowOffset(topRowIndex)),
                new Point(GetColumnOffset(rightColumnIndex + 1), GetRowOffset(bottomRowIndex + 1))
            );

            if (info.Item == Services.Selection.PrimarySelection)
            {
                // 只用于主选择：
                if (_grayOut != null)
                {
                    _grayOut.AnimateActiveAreaRectTo(availableSpaceRect);
                }
                else
                {
                    GrayOutDesignerExceptActiveArea.Start(ref _grayOut, this.Services, this.ExtendedItem.View, availableSpaceRect);
                }
            }

            HorizontalAlignment ha = (HorizontalAlignment)info.Item.Properties[FrameworkElement.HorizontalAlignmentProperty].ValueOnInstance;
            VerticalAlignment va = (VerticalAlignment)info.Item.Properties[FrameworkElement.VerticalAlignmentProperty].ValueOnInstance;
            if (_enteredIntoNewContainer)
            {
                ha = SuggestHorizontalAlignment(info.Bounds, availableSpaceRect);
                va = SuggestVerticalAlignment(info.Bounds, availableSpaceRect);
            }
            info.Item.Properties[FrameworkElement.HorizontalAlignmentProperty].SetValue(ha);
            info.Item.Properties[FrameworkElement.VerticalAlignmentProperty].SetValue(va);

            Thickness margin = new(0, 0, 0, 0);
            if (ha is HorizontalAlignment.Left or HorizontalAlignment.Stretch)
                margin.Left = info.Bounds.Left - GetColumnOffset(leftColumnIndex);
            if (va is VerticalAlignment.Top or VerticalAlignment.Stretch)
                margin.Top = info.Bounds.Top - GetRowOffset(topRowIndex);
            if (ha is HorizontalAlignment.Right or HorizontalAlignment.Stretch)
                margin.Right = GetColumnOffset(rightColumnIndex + 1) - info.Bounds.Right;
            if (va is VerticalAlignment.Bottom or VerticalAlignment.Stretch)
                margin.Bottom = GetRowOffset(bottomRowIndex + 1) - info.Bounds.Bottom;
            info.Item.Properties[FrameworkElement.MarginProperty].SetValue(margin);

            if (ha == HorizontalAlignment.Stretch)
                info.Item.Properties[FrameworkElement.WidthProperty].Reset();
            //else
            //    info.Item.Properties[FrameworkElement.WidthProperty].SetValue(info.Bounds.Width);

            if (va == VerticalAlignment.Stretch)
                info.Item.Properties[FrameworkElement.HeightProperty].Reset();
            //else
            //    info.Item.Properties[FrameworkElement.HeightProperty].SetValue(info.Bounds.Height);
        }

        /// <summary>
        /// 元素离开容器调用
        /// </summary>
        /// <param name="operation"></param>
        public override void LeaveContainer(PlacementOperation operation)
        {
            GrayOutDesignerExceptActiveArea.Stop(ref _grayOut);
            base.LeaveContainer(operation);
            foreach (PlacementInformation info in operation.PlacedItems)
            {
                if (info.Item.ComponentType == typeof(ColumnDefinition))
                {
                    // TODO: 将已删除列的宽度与前一列的宽度合并
                    this.ExtendedItem.Properties["ColumnDefinitions"].CollectionElements.Remove(info.Item);
                }
                else if (info.Item.ComponentType == typeof(RowDefinition))
                {
                    this.ExtendedItem.Properties["RowDefinitions"].CollectionElements.Remove(info.Item);
                }
                else
                {
                    info.Item.Properties.GetAttachedProperty(Grid.RowProperty).Reset();
                    info.Item.Properties.GetAttachedProperty(Grid.ColumnProperty).Reset();
                    info.Item.Properties.GetAttachedProperty(Grid.RowSpanProperty).Reset();
                    info.Item.Properties.GetAttachedProperty(Grid.ColumnSpanProperty).Reset();

                    HorizontalAlignment ha = (HorizontalAlignment)info.Item.Properties[FrameworkElement.HorizontalAlignmentProperty].ValueOnInstance;
                    VerticalAlignment va = (VerticalAlignment)info.Item.Properties[FrameworkElement.VerticalAlignmentProperty].ValueOnInstance;

                    if (ha == HorizontalAlignment.Stretch)
                        info.Item.Properties[FrameworkElement.WidthProperty].SetValue(info.Bounds.Width);
                    if (va == VerticalAlignment.Stretch)
                        info.Item.Properties[FrameworkElement.HeightProperty].SetValue(info.Bounds.Height);
                }
            }
        }
    }
}
