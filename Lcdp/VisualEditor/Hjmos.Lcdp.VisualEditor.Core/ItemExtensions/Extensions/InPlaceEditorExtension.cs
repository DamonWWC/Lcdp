using Hjmos.Lcdp.Helpers;
using Hjmos.Lcdp.VisualEditor.Core.Adorners;
using Hjmos.Lcdp.VisualEditor.Core.DesignerControls;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    /// <summary>
    /// 扩展In-Place编辑器来编辑设计器中包装在TexBlock下的可视化树中的任何文本
    /// </summary>
    [ExtensionFor(typeof(TextBlock))]
    public class InPlaceEditorExtension : PrimarySelectionAdornerProvider
    {
        private readonly AdornerPanel adornerPanel;
        private RelativePlacement placement;
        private InPlaceEditor editor;

        /// <summary>正在编辑的扩展元素的Visual树中的元素</summary>
        private TextBlock textBlock;
        private FrameworkElement element;
        private DesignPanel designPanel;
        private bool isGettingDragged;   // 获取/设置扩展元素是否被拖动的标志
        private bool isMouseDown;        // 获取/设置是否按下元素上的左键的标志
        private int numClicks;           // 元素上的左键点击的次数

        public InPlaceEditorExtension()
        {
            adornerPanel = new AdornerPanel();
            isGettingDragged = false;
            isMouseDown = Mouse.LeftButton == MouseButtonState.Pressed;
            numClicks = 0;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            element = ExtendedItem.Component as FrameworkElement;
            editor = new InPlaceEditor(ExtendedItem)
            {
                DataContext = element,
                Visibility = Visibility.Hidden // 首先隐藏编辑器，它的可见性由鼠标事件控制
            };

            placement = new RelativePlacement(HorizontalAlignment.Left, VerticalAlignment.Top);
            adornerPanel.Children.Add(editor);
            Adorners.Add(adornerPanel);

            designPanel = ExtendedItem.Services.GetService<IDesignPanel>() as DesignPanel;
            Debug.Assert(designPanel != null);

            /* 添加鼠标事件处理程序 */
            designPanel.PreviewMouseLeftButtonDown += MouseDown;
            designPanel.MouseLeftButtonUp += MouseUp;
            designPanel.PreviewMouseMove += MouseMove;

            /* 在进行调整大小操作时，更新编辑器的位置 */
            ExtendedItem.PropertyChanged += PropertyChanged;

            eventsAdded = true;
        }

        /// <summary>
        /// 检查高度/宽度是否改变，并更新编辑器的位置
        /// </summary>
        void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (textBlock != null)
            {
                if (e.PropertyName == "Width")
                {
                    placement.XOffset = Mouse.GetPosition(element).X - Mouse.GetPosition(textBlock).X - 2.8;
                    editor.MaxWidth = Math.Max((ModelTools.GetWidth(element) - placement.XOffset), 0);
                }
                if (e.PropertyName == "Height")
                {
                    placement.YOffset = Mouse.GetPosition(element).Y - Mouse.GetPosition(textBlock).Y - 1;
                    editor.MaxHeight = Math.Max((ModelTools.GetHeight(element) - placement.YOffset), 0);
                }
                AdornerPanel.SetPlacement(editor, placement);
            }
        }

        /// <summary>
        /// 通过计算鼠标位置偏移量放置手柄
        /// </summary>
        void PlaceEditor(Visual text, MouseEventArgs e)
        {
            textBlock = text as TextBlock;
            Debug.Assert(textBlock != null);

            /* 获取元素的左上角与编辑器之间的偏移量 */
            placement.XOffset = e.GetPosition(element).X - e.GetPosition(textBlock).X - 2.8;
            placement.YOffset = e.GetPosition(element).Y - e.GetPosition(textBlock).Y - 1;
            placement.XRelativeToAdornerWidth = 0;
            placement.XRelativeToContentWidth = 0;
            placement.YRelativeToAdornerHeight = 0;
            placement.YRelativeToContentHeight = 0;

            /* 编辑器的数据上下文更改为TextBlock */
            editor.DataContext = textBlock;

            /* 设置MaxHeight和MaxWidth，使编辑器不会跨越控件的边界 */
            editor.SetBinding(FrameworkElement.WidthProperty, new Binding("ActualWidth"));
            editor.SetBinding(FrameworkElement.HeightProperty, new Binding("ActualHeight"));

            /* 在控件中隐藏TextBlock，因为在位置上有一些轻微的偏移，重叠使文本看起来模糊 */
            textBlock.Visibility = Visibility.Hidden;
            AdornerPanel.SetPlacement(editor, placement);

            // 移除高亮边框
            RemoveBorder();
        }

        /// <summary>
        /// 中止编辑。这将中止编辑器的change group
        /// </summary>
        public void AbortEdit() => editor.AbortEditing();

        /// <summary>
        /// 重新开始编辑。这将中止编辑器的change group
        /// </summary>
        public void StartEdit() => editor.StartEditing();

        #region 鼠标事件

        DesignPanelHitTestResult result;
        Point Current;
        Point Start;

        void MouseDown(object sender, MouseEventArgs e)
        {
            result = designPanel.HitTest(e.GetPosition(designPanel), false, true, HitTestType.Default);
            if (result.ModelHit == ExtendedItem && result.VisualHit is TextBlock)
            {
                Start = Mouse.GetPosition(null);
                Current = Start;
                isMouseDown = true;
            }
            numClicks++;
        }

        void MouseMove(object sender, MouseEventArgs e)
        {
            Current += e.GetPosition(null) - Start;
            result = designPanel.HitTest(e.GetPosition(designPanel), false, true, HitTestType.Default);
            if (result.ModelHit == ExtendedItem && result.VisualHit is TextBlock)
            {
                if (numClicks > 0 && isMouseDown)
                {
                    Vector vector = Current - Start;
                    if (vector.X > SystemParameters.MinimumHorizontalDragDistance || vector.Y > SystemParameters.MinimumVerticalDragDistance)
                    {
                        isGettingDragged = true;
                        editor.Focus();
                    }
                }
                DrawBorder((FrameworkElement)result.VisualHit);
            }
            else
            {
                RemoveBorder();
            }
        }

        private void MouseUp(object sender, MouseEventArgs e)
        {
            result = designPanel.HitTest(e.GetPosition(designPanel), true, true, HitTestType.Default);
            if (((result.ModelHit == ExtendedItem && result.VisualHit is TextBlock) || (result.VisualHit != null && result.VisualHit.TryFindParent<InPlaceEditor>() == editor)) && numClicks > 0)
            {
                if (!isGettingDragged)
                {
                    PlaceEditor(ExtendedItem.View, e);
                    foreach (Extension extension in ExtendedItem.Extensions)
                    {
                        if (extension is not InPlaceEditorExtension and not SelectedElementRectangleExtension)
                        {
                            ExtendedItem.RemoveExtension(extension);
                        }
                    }
                    editor.Visibility = Visibility.Visible;
                }
            }
            else
            { // 在文本外单击 -> 隐藏编辑器并使实际文本再次可见
                RemoveEventsAndShowControl();
                this.ExtendedItem.ReapplyAllExtensions();
            }

            isMouseDown = false;
            isGettingDragged = false;
        }

        #endregion

        #region 高亮边框

        private Border _border;
        private sealed class BorderPlacement : AdornerPlacement
        {
            private readonly FrameworkElement _element;

            public BorderPlacement(FrameworkElement element)
            {
                _element = element;
            }

            public override void Arrange(AdornerPanel panel, UIElement adorner, Size adornedElementSize)
            {
                Point p = _element.TranslatePoint(new Point(), panel.AdornedElement);
                var rect = new Rect(p, _element.RenderSize);
                rect.Inflate(3, 1);
                adorner.Arrange(rect);
            }
        }

        private void DrawBorder(FrameworkElement item)
        {
            if (editor != null && editor.Visibility != Visibility.Visible)
            {
                if (adornerPanel.Children.Contains(_border))
                    adornerPanel.Children.Remove(_border);
                _border = new Border { BorderBrush = Brushes.Gray, BorderThickness = new Thickness(1.4), ToolTip = "Edit this Text", SnapsToDevicePixels = true };
                var shadow = new DropShadowEffect { Color = Colors.LightGray, ShadowDepth = 3 };
                _border.Effect = shadow;
                var bp = new BorderPlacement(item);
                AdornerPanel.SetPlacement(_border, bp);
                adornerPanel.Children.Add(_border);
            }
        }

        private void RemoveBorder()
        {
            if (adornerPanel.Children.Contains(_border))
                adornerPanel.Children.Remove(_border);
        }
        #endregion

        protected override void OnRemove()
        {
            RemoveEventsAndShowControl();
            base.OnRemove();
        }

        private bool eventsAdded;

        private void RemoveEventsAndShowControl()
        {
            editor.Visibility = Visibility.Hidden;

            if (textBlock != null)
            {
                textBlock.Visibility = Visibility.Visible;
            }

            if (eventsAdded)
            {
                eventsAdded = false;
                ExtendedItem.PropertyChanged -= PropertyChanged;
                designPanel.PreviewMouseLeftButtonDown -= MouseDown;
                designPanel.PreviewMouseMove -= MouseMove;
                designPanel.MouseLeftButtonUp -= MouseUp;
            }
        }
    }
}
