using Hjmos.Lcdp.VisualEditor.Controls.Adorners;
using Hjmos.Lcdp.VisualEditor.Controls.Services;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Hjmos.Lcdp.VisualEditor.Controls.DesignerControls
{
    /// <summary>
    /// 除了特定区域外，所有东西都变灰。
    /// </summary>
    public sealed class GrayOutDesignerExceptActiveArea : FrameworkElement
    {
        private Geometry designSurfaceRectangle;
        private Geometry activeAreaGeometry;
        private Geometry combinedGeometry;
        private AdornerPanel adornerPanel;
        private IDesignPanel designPanel;
        private const double MaxOpacity = 0.3;

        public GrayOutDesignerExceptActiveArea()
        {
            this.GrayOutBrush = new SolidColorBrush(SystemColors.GrayTextColor) { Opacity = MaxOpacity };
            this.IsHitTestVisible = false;
        }

        public Brush GrayOutBrush { get; set; }

        public Geometry ActiveAreaGeometry
        {
            get => activeAreaGeometry;
            set
            {
                activeAreaGeometry = value;
                combinedGeometry = new CombinedGeometry(GeometryCombineMode.Exclude, designSurfaceRectangle, activeAreaGeometry);
            }
        }

        protected override void OnRender(DrawingContext drawingContext) => drawingContext.DrawGeometry(GrayOutBrush, null, combinedGeometry);

        private Rect currentAnimateActiveAreaRectToTarget;

        public void AnimateActiveAreaRectTo(Rect newRect)
        {
            if (newRect.Equals(currentAnimateActiveAreaRectToTarget))
                return;
            activeAreaGeometry.BeginAnimation(
                RectangleGeometry.RectProperty,
                new RectAnimation(newRect, new Duration(new TimeSpan(0, 0, 0, 0, 100))),
                HandoffBehavior.SnapshotAndReplace);
            currentAnimateActiveAreaRectToTarget = newRect;
        }

        public static void Start(ref GrayOutDesignerExceptActiveArea grayOut, ServiceContainer services, UIElement activeContainer)
        {
            Debug.Assert(activeContainer != null);
            Start(ref grayOut, services, activeContainer, new Rect(activeContainer.RenderSize));
        }

        public static void Start(ref GrayOutDesignerExceptActiveArea grayOut, ServiceContainer services, UIElement activeContainer, Rect activeRectInActiveContainer)
        {
            Debug.Assert(services != null);
            Debug.Assert(activeContainer != null);
            DesignPanel designPanel = services.GetService<IDesignPanel>() as DesignPanel;
            OptionService optionService = services.GetService<OptionService>();
            if (designPanel != null && grayOut == null && optionService != null && optionService.GrayOutDesignSurfaceExceptParentContainerWhenDragging)
            {
                grayOut = new GrayOutDesignerExceptActiveArea
                {
                    designSurfaceRectangle = new RectangleGeometry(new Rect(0, 0, ((Border)designPanel.Child).Child.RenderSize.Width, ((Border)designPanel.Child).Child.RenderSize.Height)),
                    designPanel = designPanel,
                    adornerPanel = new AdornerPanel()
                };
                grayOut.adornerPanel.Order = AdornerOrder.BehindForeground;
                grayOut.adornerPanel.SetAdornedElement(designPanel.Context.RootItem.View, null);
                grayOut.adornerPanel.Children.Add(grayOut);
                grayOut.ActiveAreaGeometry = new RectangleGeometry(activeRectInActiveContainer, 0, 0, (Transform)activeContainer.TransformToVisual(grayOut.adornerPanel.AdornedElement));
                Animate(grayOut.GrayOutBrush, Brush.OpacityProperty, MaxOpacity);
                designPanel.Adorners.Add(grayOut.adornerPanel);
            }
        }

        private static readonly TimeSpan animationTime = new(2000000);

        private static void Animate(Animatable element, DependencyProperty property, double to)
        {
            element.BeginAnimation(property, new DoubleAnimation(to, new Duration(animationTime), FillBehavior.HoldEnd), HandoffBehavior.SnapshotAndReplace);
        }

        public static void Stop(ref GrayOutDesignerExceptActiveArea grayOut)
        {
            if (grayOut != null)
            {
                Animate(grayOut.GrayOutBrush, Brush.OpacityProperty, 0);
                IDesignPanel designPanel = grayOut.designPanel;
                AdornerPanel adornerPanelToRemove = grayOut.adornerPanel;
                DispatcherTimer timer = new()
                {
                    Interval = animationTime
                };
                timer.Tick += delegate
                {
                    timer.Stop();
                    designPanel.Adorners.Remove(adornerPanelToRemove);
                };
                timer.Start();
                grayOut = null;
            }
        }
    }
}
