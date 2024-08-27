using Hjmos.Lcdp.VisualEditor.Controls.Adorners;
using Hjmos.Lcdp.VisualEditor.Controls.DesignerControls;
using Hjmos.Lcdp.VisualEditor.Controls.Extensions;
using Hjmos.Lcdp.VisualEditor.Controls.UIExtensions;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Hjmos.Lcdp.VisualEditor.Controls.Extensions2
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
            editor = new InPlaceEditor(ExtendedItem);
            editor.DataContext = element;
            editor.Visibility = Visibility.Hidden; // Hide the editor first, It's visibility is governed by mouse events.  首先隐藏编辑器，它的可见性由鼠标事件控制。

            placement = new RelativePlacement(HorizontalAlignment.Left, VerticalAlignment.Top);
            adornerPanel.Children.Add(editor);
            Adorners.Add(adornerPanel);

            designPanel = ExtendedItem.Services.GetService<IDesignPanel>() as DesignPanel;
            Debug.Assert(designPanel != null);

            /* Add mouse event handlers 添加鼠标事件处理程序 */
            designPanel.PreviewMouseLeftButtonDown += MouseDown;
            designPanel.MouseLeftButtonUp += MouseUp;
            designPanel.PreviewMouseMove += MouseMove;

            /* To update the position of Editor in case of resize operation 在进行调整大小操作时，更新编辑器的位置 */
            ExtendedItem.PropertyChanged += PropertyChanged;

            eventsAdded = true;
        }

        /// <summary>
        /// Checks whether heigth/width have changed and updates the position of editor
        /// 检查高度/宽度是否改变，并更新编辑器的位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (textBlock != null)
            {
                if (e.PropertyName == "Width")
                {
                    placement.XOffset = Mouse.GetPosition((IInputElement)element).X - Mouse.GetPosition(textBlock).X - 2.8;
                    editor.MaxWidth = Math.Max((ModelTools.GetWidth(element) - placement.XOffset), 0);
                }
                if (e.PropertyName == "Height")
                {
                    placement.YOffset = Mouse.GetPosition((IInputElement)element).Y - Mouse.GetPosition(textBlock).Y - 1;
                    editor.MaxHeight = Math.Max((ModelTools.GetHeight(element) - placement.YOffset), 0);
                }
                AdornerPanel.SetPlacement(editor, placement);
            }
        }

        /// <summary>
        /// Places the handle from a calculated offset using Mouse Positon
        /// 通过计算鼠标位置偏移量放置手柄
        /// </summary>
        /// <param name="text"></param>
        /// <param name="e"></param>
        void PlaceEditor(Visual text, MouseEventArgs e)
        {
            textBlock = text as TextBlock;
            Debug.Assert(textBlock != null);

            /* Gets the offset between the top-left corners of the element and the editor 获取元素的左上角与编辑器之间的偏移量*/
            placement.XOffset = e.GetPosition(element).X - e.GetPosition(textBlock).X - 2.8;
            placement.YOffset = e.GetPosition(element).Y - e.GetPosition(textBlock).Y - 1;
            placement.XRelativeToAdornerWidth = 0;
            placement.XRelativeToContentWidth = 0;
            placement.YRelativeToAdornerHeight = 0;
            placement.YRelativeToContentHeight = 0;

            /* Change data context of the editor to the TextBlock 将编辑器的数据上下文更改为TextBlock */
            editor.DataContext = textBlock;

            /* Set MaxHeight and MaxWidth so that editor doesn't cross the boundaries of the control 设置MaxHeight和MaxWidth，使编辑器不会跨越控件的边界 */
            editor.SetBinding(FrameworkElement.WidthProperty, new Binding("ActualWidth"));
            editor.SetBinding(FrameworkElement.HeightProperty, new Binding("ActualHeight"));

            /* Hides the TextBlock in control because of some minor offset in placement, overlaping makes text look fuzzy */
            textBlock.Visibility = Visibility.Hidden; // 
            AdornerPanel.SetPlacement(editor, placement);

            RemoveBorder(); // Remove the highlight border.
        }

        /// <summary>
        /// Aborts the editing. This aborts the underlying change group of the editor
        /// </summary>
        public void AbortEdit()
        {
            editor.AbortEditing();
        }

        /// <summary>
        /// Starts editing once again. This aborts the underlying change group of the editor
        /// </summary>
        public void StartEdit()
        {
            editor.StartEditing();
        }

        #region MouseEvents
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
                if (numClicks > 0)
                {
                    if (isMouseDown &&
                        ((Current - Start).X > SystemParameters.MinimumHorizontalDragDistance
                         || (Current - Start).Y > SystemParameters.MinimumVerticalDragDistance))
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
                    foreach (var extension in ExtendedItem.Extensions)
                    {
                        if (!(extension is InPlaceEditorExtension) && !(extension is SelectedElementRectangleExtension))
                        {
                            ExtendedItem.RemoveExtension(extension);
                        }
                    }
                    editor.Visibility = Visibility.Visible;
                }
            }
            else
            { // Clicked outside the Text - > hide the editor and make the actual text visible again
                RemoveEventsAndShowControl();
                this.ExtendedItem.ReapplyAllExtensions();
            }

            isMouseDown = false;
            isGettingDragged = false;
        }

        #endregion

        #region HighlightBorder
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
