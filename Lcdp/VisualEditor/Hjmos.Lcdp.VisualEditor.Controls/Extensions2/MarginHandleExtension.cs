using Hjmos.Lcdp.VisualEditor.Controls.Adorners;
using Hjmos.Lcdp.VisualEditor.Controls.DesignerControls;
using Hjmos.Lcdp.VisualEditor.Controls.Extensions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Controls.Extensions2
{
    [ExtensionFor(typeof(FrameworkElement))]
    [ExtensionServer(typeof(PrimarySelectionExtensionServer))]
    public class MarginHandleExtension : AdornerProvider
    {
        private MarginHandle[] _handles;
        private MarginHandle _leftHandle, _topHandle, _rightHandle, _bottomHandle;
        private Grid _grid;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            // 如果元素有父节点
            if (this.ExtendedItem.Parent != null)
            {
                // 如果元素父节点的类型的Grid
                //if (this.ExtendedItem.Parent.ComponentType == typeof(Grid))
                if (typeof(Grid).IsAssignableFrom(this.ExtendedItem.Parent.ComponentType))
                {
                    FrameworkElement extendedControl = this.ExtendedItem.Component as FrameworkElement;
                    AdornerPanel adornerPanel = new();

                    // 如果元素在网格中旋转/倾斜，则不会出现边距手柄
                    if (extendedControl.LayoutTransform != null && extendedControl.LayoutTransform.Value == Matrix.Identity && extendedControl.RenderTransform.Value == Matrix.Identity)
                    {
                        _grid = this.ExtendedItem.Parent.View as Grid;

                        // 创建边距手柄
                        _handles = new[]
                        {
                            _leftHandle = new MarginHandle(ExtendedItem, adornerPanel, HandleOrientation.Left),
                            _topHandle = new MarginHandle(ExtendedItem, adornerPanel, HandleOrientation.Top),
                            _rightHandle = new MarginHandle(ExtendedItem, adornerPanel, HandleOrientation.Right),
                            _bottomHandle = new MarginHandle(ExtendedItem, adornerPanel, HandleOrientation.Bottom),
                        };
                        // 为手柄注册鼠标事件
                        foreach (MarginHandle handle in _handles)
                        {
                            handle.MouseLeftButtonDown += OnMouseDown;
                            handle.Stub.PreviewMouseLeftButtonDown += OnMouseDown;
                        }
                    }

                    if (adornerPanel != null)
                        this.Adorners.Add(adornerPanel);
                }
            }
        }

        #region Change margin through handle/stub
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            var row = (int)this.ExtendedItem.Properties.GetAttachedProperty(Grid.RowProperty).ValueOnInstance;
            var rowSpan = (int)this.ExtendedItem.Properties.GetAttachedProperty(Grid.RowSpanProperty).ValueOnInstance;

            var column = (int)this.ExtendedItem.Properties.GetAttachedProperty(Grid.ColumnProperty).ValueOnInstance;
            var columnSpan = (int)this.ExtendedItem.Properties.GetAttachedProperty(Grid.ColumnSpanProperty).ValueOnInstance;

            var margin = (Thickness)this.ExtendedItem.Properties[FrameworkElement.MarginProperty].ValueOnInstance;

            var point = this.ExtendedItem.View.TranslatePoint(new Point(), _grid);
            var position = new Rect(point, PlacementOperation.GetRealElementSize(this.ExtendedItem.View));
            MarginHandle handle = null;
            if (sender is MarginHandle)
                handle = sender as MarginHandle;
            if (sender is MarginStub)
                handle = ((MarginStub)sender).Handle;
            if (handle != null)
            {
                switch (handle.Orientation)
                {
                    case HandleOrientation.Left:
                        if (_rightHandle.Visibility == Visibility.Visible)
                        {
                            if (_leftHandle.Visibility == Visibility.Visible)
                            {
                                margin.Left = 0;
                                this.ExtendedItem.Properties[FrameworkElement.WidthProperty].SetValue(position.Width);
                                this.ExtendedItem.Properties[FrameworkElement.HorizontalAlignmentProperty].SetValue(HorizontalAlignment.Right);
                            }
                            else
                            {
                                var leftMargin = position.Left - GetColumnOffset(column);
                                margin.Left = leftMargin;
                                this.ExtendedItem.Properties[FrameworkElement.HorizontalAlignmentProperty].Reset();
                                this.ExtendedItem.Properties[FrameworkElement.WidthProperty].Reset();
                            }
                        }
                        else
                        {
                            if (_leftHandle.Visibility == Visibility.Visible)
                            {
                                margin.Left = 0;
                                var rightMargin = GetColumnOffset(column + columnSpan) - position.Right;
                                margin.Right = rightMargin;

                                this.ExtendedItem.Properties[FrameworkElement.WidthProperty].SetValue(position.Width);
                                this.ExtendedItem.Properties[FrameworkElement.HorizontalAlignmentProperty].SetValue(HorizontalAlignment.Right);
                            }
                            else
                            {
                                var leftMargin = position.Left - GetColumnOffset(column);
                                margin.Left = leftMargin;
                                this.ExtendedItem.Properties[FrameworkElement.HorizontalAlignmentProperty].SetValue(HorizontalAlignment.Left);
                            }
                        }
                        break;
                    case HandleOrientation.Top:
                        if (_bottomHandle.Visibility == Visibility.Visible)
                        {
                            if (_topHandle.Visibility == Visibility.Visible)
                            {
                                margin.Top = 0;
                                this.ExtendedItem.Properties[FrameworkElement.HeightProperty].SetValue(position.Height);
                                this.ExtendedItem.Properties[FrameworkElement.VerticalAlignmentProperty].SetValue(VerticalAlignment.Bottom);
                            }
                            else
                            {
                                var topMargin = position.Top - GetRowOffset(row);
                                margin.Top = topMargin;
                                this.ExtendedItem.Properties[FrameworkElement.VerticalAlignmentProperty].Reset();
                                this.ExtendedItem.Properties[FrameworkElement.HeightProperty].Reset();
                            }
                        }
                        else
                        {
                            if (_topHandle.Visibility == Visibility.Visible)
                            {
                                margin.Top = 0;
                                var bottomMargin = GetRowOffset(row + rowSpan) - position.Bottom;
                                margin.Bottom = bottomMargin;

                                this.ExtendedItem.Properties[FrameworkElement.HeightProperty].SetValue(position.Height);
                                this.ExtendedItem.Properties[FrameworkElement.VerticalAlignmentProperty].SetValue(VerticalAlignment.Bottom);
                            }
                            else
                            {
                                var topMargin = position.Top - GetRowOffset(row);
                                margin.Top = topMargin;
                                this.ExtendedItem.Properties[FrameworkElement.VerticalAlignmentProperty].SetValue(VerticalAlignment.Top);
                            }
                        }
                        break;
                    case HandleOrientation.Right:
                        if (_leftHandle.Visibility == Visibility.Visible)
                        {
                            if (_rightHandle.Visibility == Visibility.Visible)
                            {
                                margin.Right = 0;
                                this.ExtendedItem.Properties[FrameworkElement.WidthProperty].SetValue(position.Width);
                                this.ExtendedItem.Properties[FrameworkElement.HorizontalAlignmentProperty].SetValue(HorizontalAlignment.Left);
                            }
                            else
                            {
                                var rightMargin = GetColumnOffset(column + columnSpan) - position.Right;
                                margin.Right = rightMargin;
                                this.ExtendedItem.Properties[FrameworkElement.HorizontalAlignmentProperty].Reset();
                                this.ExtendedItem.Properties[FrameworkElement.WidthProperty].Reset();
                            }
                        }
                        else
                        {
                            if (_rightHandle.Visibility == Visibility.Visible)
                            {
                                margin.Right = 0;
                                var leftMargin = position.Left - GetColumnOffset(column);
                                margin.Left = leftMargin;

                                this.ExtendedItem.Properties[FrameworkElement.WidthProperty].SetValue(position.Width);
                                this.ExtendedItem.Properties[FrameworkElement.HorizontalAlignmentProperty].SetValue(HorizontalAlignment.Left);
                            }
                            else
                            {
                                var rightMargin = GetColumnOffset(column + columnSpan) - position.Right;
                                margin.Right = rightMargin;
                                this.ExtendedItem.Properties[FrameworkElement.HorizontalAlignmentProperty].SetValue(HorizontalAlignment.Right);
                            }
                        }
                        break;
                    case HandleOrientation.Bottom:
                        if (_topHandle.Visibility == Visibility.Visible)
                        {
                            if (_bottomHandle.Visibility == Visibility.Visible)
                            {
                                margin.Bottom = 0;
                                this.ExtendedItem.Properties[FrameworkElement.HeightProperty].SetValue(position.Height);
                                this.ExtendedItem.Properties[FrameworkElement.VerticalAlignmentProperty].SetValue(VerticalAlignment.Top);
                            }
                            else
                            {
                                var bottomMargin = GetRowOffset(row + rowSpan) - position.Bottom;
                                margin.Bottom = bottomMargin;
                                this.ExtendedItem.Properties[FrameworkElement.VerticalAlignmentProperty].Reset();
                                this.ExtendedItem.Properties[FrameworkElement.HeightProperty].Reset();
                            }
                        }
                        else
                        {
                            if (_bottomHandle.Visibility == Visibility.Visible)
                            {
                                margin.Bottom = 0;
                                var topMargin = position.Top - GetRowOffset(row);
                                margin.Top = topMargin;

                                this.ExtendedItem.Properties[FrameworkElement.HeightProperty].SetValue(position.Height);
                                this.ExtendedItem.Properties[FrameworkElement.VerticalAlignmentProperty].SetValue(VerticalAlignment.Top);
                            }
                            else
                            {
                                var bottomMargin = GetRowOffset(row + rowSpan) - position.Bottom;
                                margin.Bottom = bottomMargin;
                                this.ExtendedItem.Properties[FrameworkElement.VerticalAlignmentProperty].SetValue(VerticalAlignment.Bottom);
                            }
                        }
                        break;
                }
            }
            this.ExtendedItem.Properties[FrameworkElement.MarginProperty].SetValue(margin);
        }

        private double GetColumnOffset(int index)
        {
            if (_grid != null)
            {
                // 当网格没有列时，我们仍然需要为index=0返回0，为index=1返回grid.Width
                if (index == 0)
                    return 0;
                if (index < _grid.ColumnDefinitions.Count)
                    return _grid.ColumnDefinitions[index].Offset;
                return _grid.ActualWidth;
            }
            return 0;
        }

        private double GetRowOffset(int index)
        {
            if (_grid != null)
            {
                if (index == 0)
                    return 0;
                if (index < _grid.RowDefinitions.Count)
                    return _grid.RowDefinitions[index].Offset;
                return _grid.ActualHeight;
            }
            return 0;
        }

        #endregion

        /// <summary>
        /// 隐藏操作手柄
        /// </summary>
        public void HideHandles()
        {
            if (_handles != null)
            {
                foreach (MarginHandle handle in _handles)
                {
                    handle.ShouldBeVisible = false;
                    handle.Visibility = Visibility.Hidden;
                }
            }
        }

        /// <summary>
        /// 显示操作手柄
        /// </summary>
        public void ShowHandles()
        {
            if (_handles != null)
            {
                foreach (MarginHandle handle in _handles)
                {
                    handle.ShouldBeVisible = true;
                    handle.Visibility = Visibility.Visible;
                    handle.DecideVisiblity(handle.HandleLength);
                }
            }
        }
    }
}
