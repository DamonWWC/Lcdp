using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Controls.Adorners
{
    /// <summary>
    /// 网格的分割器基类
    /// </summary>
    public abstract class GridSplitterAdorner : Control
    {

        public static readonly DependencyProperty IsPreviewProperty
            = DependencyProperty.Register("IsPreview", typeof(bool), typeof(GridSplitterAdorner), new PropertyMetadata(SharedInstances.BoxedFalse));

        /// <summary>网格实例</summary>
        private readonly Grid _grid;

        /// <summary>代表网格的设计项</summary>
        private readonly DesignItem _gridItem;

        /// <summary>Grid上轨道装饰器的引用项</summary>
        private readonly GridRailAdorner _rail;

        /// <summary>表示网格的第一行/列</summary>
        protected DesignItem FirstRow { get; }

        /// <summary>表示网格的第二行/列</summary>
        protected DesignItem SecondRow { get; }

        internal GridSplitterAdorner(GridRailAdorner rail, DesignItem gridItem, DesignItem firstRow, DesignItem secondRow)
        {
            Debug.Assert(gridItem != null);

            _grid = gridItem.Component as Grid;
            _gridItem = gridItem;
            _rail = rail;
            FirstRow = firstRow;
            SecondRow = secondRow;
        }

        /// <summary>
        /// 是否为预览分割器
        /// 预览分割器是跟随鼠标移动的，非预览分割器是鼠标点击后呈现在网格线上的
        /// </summary>
        public bool IsPreview
        {
            get => (bool)GetValue(IsPreviewProperty);
            set => SetValue(IsPreviewProperty, SharedInstances.Box(value));
        }

        //ChangeGroup activeChangeGroup;

        /// <summary>鼠标起始位置（如列分割器为相对于网格的X坐标、行分割器为相对于网格的Y坐标）</summary>
        private double _mouseStartPos;

        /// <summary>是否按住鼠标左键</summary>
        private bool _mouseIsDown;

        /// <summary>
        /// 鼠标左键按下事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            e.Handled = true;
            // 捕获鼠标
            if (CaptureMouse())
            {
                // 获取焦点
                Focus();
                // 鼠标起始位置
                _mouseStartPos = GetCoordinate(e.GetPosition(_grid));
                // 是否按住鼠标左键
                _mouseIsDown = true;
            }
        }

        /// <summary>标记是否已经记录了开始移动鼠标时的初始值</summary>
        private bool _isRemembered = false;

        /// <summary>
        /// 鼠标移动事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_mouseIsDown)
            {
                // 获取鼠标位置
                double mousePos = GetCoordinate(e.GetPosition(_grid));
                //if (activeChangeGroup == null)
                //{
                // 当拖动距离超过最小拖动距离后保存原始大小（最小拖动距离用来延缓拖动，如用来避免鼠标点击时不小心移动了一个像素这种情况）
                if (!_isRemembered && Math.Abs(mousePos - _mouseStartPos) >= GetCoordinate(new Point(SystemParameters.MinimumHorizontalDragDistance, SystemParameters.MinimumVerticalDragDistance)))
                {
                    //activeChangeGroup = gridItem.OpenGroup("Change grid row/column size");
                    RememberOriginalSize();
                    _isRemembered = true;
                }
                if (_isRemembered)
                {
                    ChangeSize(mousePos - _mouseStartPos);
                }
                //}
                //if (activeChangeGroup != null)
                //{

                //}
            }
        }

        /// <summary>表示原来第一行/列的网格长度</summary>
        protected GridLength Original1 { get; set; }

        /// <summary>表示原来第二行/列的网格长度</summary>
        protected GridLength Original2 { get; set; }

        /// <summary>表示原来第一行/列的实际宽度/高度</summary>
        protected double OriginalPixelSize1 { get; set; }

        /// <summary>表示原来第二行/列的实际宽度/高度</summary>
        protected double OriginalPixelSize2 { get; set; }

        /// <summary>
        /// 获取坐标（如列分割器返回X轴坐标、行分割器返回Y轴坐标）
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected abstract double GetCoordinate(Point point);

        /// <summary>
        /// 记忆第一二行/列的原始大小
        /// </summary>
        protected abstract void RememberOriginalSize();

        /// <summary>
        /// 行/列的宽/高依赖属性
        /// </summary>
        protected abstract DependencyProperty RowColumnSizeProperty { get; }

        /// <summary>
        /// 改变网格的行/列大小
        /// </summary>
        /// <param name="delta">以像素为单位的变化量</param>
        private void ChangeSize(double delta)
        {
            // 控制移动不超出第一二行/列的范围
            if (delta < -OriginalPixelSize1)
                delta = -OriginalPixelSize1;
            if (delta > OriginalPixelSize2)
                delta = OriginalPixelSize2;

            // 如有必要，将自动长度替换为绝对长度
            if (Original1.IsAuto)
                Original1 = new GridLength(OriginalPixelSize1);
            if (Original2.IsAuto)
                Original2 = new GridLength(OriginalPixelSize2);

            // 计算新的行/列的宽/高
            GridLength new1;
            if (Original1.IsStar && OriginalPixelSize1 > 0)
                new1 = new GridLength(Original1.Value * (OriginalPixelSize1 + delta) / OriginalPixelSize1, GridUnitType.Star);
            else
                new1 = new GridLength(OriginalPixelSize1 + delta);

            GridLength new2;
            if (Original2.IsStar && OriginalPixelSize2 > 0)
                new2 = new GridLength(Original2.Value * (OriginalPixelSize2 - delta) / OriginalPixelSize2, GridUnitType.Star);
            else
                new2 = new GridLength(OriginalPixelSize2 - delta);

            // 应用新的宽/高
            FirstRow.Properties[RowColumnSizeProperty].SetValue(new1);
            SecondRow.Properties[RowColumnSizeProperty].SetValue(new2);

            // 重绘父元素（AdornerPanel）
            UIElement e = (UIElement)VisualTreeHelper.GetParent(this);
            e.InvalidateArrange();
            // 重绘轨道装饰器
            _rail.InvalidateVisual();
        }

        /// <summary>
        /// 鼠标左键放开事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            //if (activeChangeGroup != null)
            //{
            //    activeChangeGroup.Commit();
            //    activeChangeGroup = null;
            //}
            Stop();
        }

        /// <summary>
        /// 失去鼠标捕获事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLostMouseCapture(MouseEventArgs e) => Stop();

        /// <summary>
        /// 键盘按下事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            // 按下Esc键
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                Stop();
            }
        }

        /// <summary>
        /// 停止鼠标捕获，停止按住鼠标
        /// </summary>
        private void Stop()
        {
            _isRemembered = false;
            // 释放对鼠标的捕获
            ReleaseMouseCapture();
            _mouseIsDown = false;
            //if (activeChangeGroup != null)
            //{
            //    activeChangeGroup.Abort();
            //    activeChangeGroup = null;
            //}
        }
    }
}
