using Hjmos.Lcdp.VisualEditor.Core.Adorners;
using Hjmos.Lcdp.VisualEditor.Core.Thumbs;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    /// <summary>
    /// 用于Line、Polyline和Polygon扩展类的基类
    /// </summary>
    public class LineExtensionBase : SelectionAdornerProvider
    {
        /// <summary>
        /// Used instead of Rect to allow negative values on "Width" and "Height" (here called X and Y).
        /// 代替矩形，允许“宽度”和“高度”为负值（这里叫X和Y）
        /// </summary>
        protected class Bounds
        {
            public double X, Y, Left, Top;
        }

        protected AdornerPanel adornerPanel;
        protected IEnumerable resizeThumbs;

        /// <summary>一个只包含this.ExtendedItem元素的数组</summary>
        protected readonly DesignItem[] extendedItemArray = new DesignItem[1];

        protected IPlacementBehavior resizeBehavior;
        protected PlacementOperation operation;
        //protected ChangeGroup changeGroup;
        private readonly Canvas _surface;
        protected bool _isResizing;
        private TextBlock _text;
        //private DesignPanel designPanel;

        /// <summary>
        /// 获取该扩展是否正在调整任何元素的大小。
        /// </summary>
        public bool IsResizing => _isResizing;

        /// <summary>
        /// 在创建时添加装饰层
        /// </summary>
        public LineExtensionBase()
        {
            _surface = new Canvas();
            adornerPanel = new AdornerPanel() { MinWidth = 10, MinHeight = 10 };
            adornerPanel.Order = AdornerOrder.Foreground;
            adornerPanel.Children.Add(_surface);
            Adorners.Add(adornerPanel);
        }

        #region 事件处理器


        protected void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) => UpdateAdornerVisibility();


        protected override void OnRemove()
        {
            this.ExtendedItem.PropertyChanged -= OnPropertyChanged;
            base.OnRemove();
        }

        #endregion

        protected void UpdateAdornerVisibility()
        {
            foreach (DesignerThumb r in resizeThumbs)
            {
                bool isVisible = resizeBehavior != null && resizeBehavior.CanPlace(extendedItemArray, PlacementType.Resize, r.Alignment);
                r.Visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
            }
        }

        /// <summary>
        /// Places resize thumbs at their respective positions and streches out thumbs which are at the center of outline to extend resizability across the whole outline
        /// </summary>
        /// <param name="designerThumb"></param>
        /// <param name="alignment"></param>
        /// <param name="index">if using a polygon or multipoint adorner this is the index of the point in the Points array</param>
        /// <returns></returns>
        protected PointTrackerPlacementSupport Place(DesignerThumb designerThumb, PlacementAlignment alignment, int index = -1)
        {
            PointTrackerPlacementSupport placement = new(ExtendedItem.View as Shape, alignment, index);
            return placement;
        }

        /// <summary>
        /// 强制重绘形状
        /// </summary>
        protected void Invalidate()
        {
            if (ExtendedItem.View is Shape s)
            {
                s.InvalidateVisual();
                s.BringIntoView();
            }
        }

        protected void SetSurfaceInfo(int x, int y, string s)
        {
            if (_text == null)
            {
                _text = new TextBlock() { FontSize = 8, FontStyle = FontStyles.Italic };
                _surface.Children.Add(_text);
            }

            AdornerPanel ap = _surface.Parent as AdornerPanel;

            _surface.Width = ap.Width;
            _surface.Height = ap.Height;

            _text.Text = s;
            Canvas.SetLeft(_text, x);
            Canvas.SetTop(_text, y);
        }

        protected void HideSizeAndShowHandles()
        {
            SizeDisplayExtension sizeDisplay = null;
            MarginHandleExtension marginDisplay = null;
            foreach (Extension extension in ExtendedItem.Extensions)
            {
                if (extension is SizeDisplayExtension)
                    sizeDisplay = extension as SizeDisplayExtension;
                if (extension is MarginHandleExtension)
                    marginDisplay = extension as MarginHandleExtension;
            }

            if (sizeDisplay != null)
            {
                sizeDisplay.HeightDisplay.Visibility = Visibility.Hidden;
                sizeDisplay.WidthDisplay.Visibility = Visibility.Hidden;
            }
            if (marginDisplay != null)
            {
                marginDisplay.ShowHandles();
            }
        }

        protected void ResetWidthHeightProperties()
        {
            ExtendedItem.Properties.GetProperty(FrameworkElement.HeightProperty).Reset();
            ExtendedItem.Properties.GetProperty(FrameworkElement.WidthProperty).Reset();
        }
    }
}
