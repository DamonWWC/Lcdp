using Hjmos.Lcdp.VisualEditor.Core.Adorners;
using Hjmos.Lcdp.VisualEditor.Core.Services;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Core
{
    /// <summary>
    /// 信息文本区域
    /// </summary>
    public sealed class InfoTextEnterArea : FrameworkElement
    {
        private AdornerPanel adornerPanel;
        private IDesignPanel designPanel;

        public InfoTextEnterArea() => this.IsHitTestVisible = false;

        public Geometry ActiveAreaGeometry { get; set; }

        public static void Start(ref InfoTextEnterArea grayOut, ServiceContainer services, UIElement activeContainer, string text)
        {
            Debug.Assert(activeContainer != null);
            Start(ref grayOut, services, activeContainer, new Rect(activeContainer.RenderSize), text);
        }

        public static void Start(ref InfoTextEnterArea grayOut, ServiceContainer services, UIElement activeContainer, Rect activeRectInActiveContainer, string text)
        {
            Debug.Assert(services != null);
            Debug.Assert(activeContainer != null);
            DesignPanel designPanel = services.GetService<IDesignPanel>() as DesignPanel;
            OptionService optionService = services.GetService<OptionService>();
            if (designPanel != null && grayOut == null && optionService != null && optionService.GrayOutDesignSurfaceExceptParentContainerWhenDragging)
            {
                grayOut = new InfoTextEnterArea();
                grayOut.designPanel = designPanel;
                grayOut.adornerPanel = new AdornerPanel();
                grayOut.adornerPanel.Order = AdornerOrder.Background;
                grayOut.adornerPanel.SetAdornedElement(designPanel.Context.RootItem.View, null);
                grayOut.ActiveAreaGeometry = new RectangleGeometry(activeRectInActiveContainer, 0, 0, (Transform)activeContainer.TransformToVisual(grayOut.adornerPanel.AdornedElement));
                var tb = new TextBlock() { Text = text };
                tb.FontSize = 10;
                tb.ClipToBounds = true;
                tb.Width = ((FrameworkElement)activeContainer).ActualWidth;
                tb.Height = ((FrameworkElement)activeContainer).ActualHeight;
                tb.VerticalAlignment = VerticalAlignment.Top;
                tb.HorizontalAlignment = HorizontalAlignment.Left;
                tb.RenderTransform = (Transform)activeContainer.TransformToVisual(grayOut.adornerPanel.AdornedElement);
                grayOut.adornerPanel.Children.Add(tb);

                designPanel.Adorners.Add(grayOut.adornerPanel);
            }
        }

        public static void Stop(ref InfoTextEnterArea grayOut)
        {
            if (grayOut != null)
            {
                IDesignPanel designPanel = grayOut.designPanel;
                AdornerPanel adornerPanelToRemove = grayOut.adornerPanel;
                designPanel.Adorners.Remove(adornerPanelToRemove);
                grayOut = null;
            }
        }
    }
}
