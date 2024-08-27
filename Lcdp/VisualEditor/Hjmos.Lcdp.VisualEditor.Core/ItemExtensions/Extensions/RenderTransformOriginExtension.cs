using Hjmos.Lcdp.VisualEditor.Core.Adorners;
using Hjmos.Lcdp.VisualEditor.Core.DesignerControls;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    /// <summary>
    /// 渲染变换原点的扩展
    /// </summary>
    [ExtensionServer(typeof(OnlyOneItemSelectedExtensionServer))]
    [ExtensionFor(typeof(FrameworkElement))]
    public class RenderTransformOriginExtension : SelectionAdornerProvider
    {
        /// <summary>装饰器面板</summary>
        private readonly AdornerPanel _adornerPanel;
        /// <summary>变换原点的Thumb</summary>
        private RenderTransformOriginThumb _renderTransformOriginThumb;

        // 仅包含ExtendedItem元素的数组
        private readonly DesignItem[] extendedItemArray = new DesignItem[1];

        public RenderTransformOriginExtension()
        {
            // 创建一个放置在前景层的装饰面板
            _adornerPanel = new AdornerPanel { Order = AdornerOrder.Foreground };
            this.Adorners.Add(_adornerPanel);

            CreateRenderTransformOriginThumb();
        }

        /// <summary>
        /// 创建渲染变换的原点
        /// </summary>
        private void CreateRenderTransformOriginThumb()
        {
            // 变换原点的Thumb
            _renderTransformOriginThumb = new RenderTransformOriginThumb { Cursor = Cursors.Hand };

            AdornerPanel.SetPlacement(_renderTransformOriginThumb,
                                      new RelativePlacement(HorizontalAlignment.Left, VerticalAlignment.Top)
                                      {
                                          XRelativeToContentWidth = renderTransformOrigin.X,
                                          YRelativeToContentHeight = renderTransformOrigin.Y
                                      });
            _adornerPanel.Children.Add(_renderTransformOriginThumb);

            _renderTransformOriginThumb.DragDelta += new DragDeltaEventHandler(RenderTransformOriginThumb_DragDelta);
            _renderTransformOriginThumb.DragCompleted += new DragCompletedEventHandler(RenderTransformOriginThumb_DragCompleted);
        }

        private void RenderTransformOriginThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            ExtendedItem.Properties.GetProperty(UIElement.RenderTransformOriginProperty).SetValue(new Point(Math.Round(renderTransformOrigin.X, 4), Math.Round(renderTransformOrigin.Y, 4)));
        }

        void RenderTransformOriginThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (AdornerPanel.GetPlacement(_renderTransformOriginThumb) is not RelativePlacement p) return;

            Vector pointAbs = _adornerPanel.RelativeToAbsolute(new Vector(p.XRelativeToContentWidth, p.YRelativeToContentHeight));
            Vector pointAbsNew = pointAbs + new Vector(e.HorizontalChange, e.VerticalChange);
            Vector pRel = _adornerPanel.AbsoluteToRelative(pointAbsNew);
            renderTransformOrigin = new Point(pRel.X, pRel.Y);

            this.ExtendedItem.View.SetValue(UIElement.RenderTransformOriginProperty, renderTransformOrigin);
        }

        private Point renderTransformOrigin = new(0.5, 0.5);
        private DependencyPropertyDescriptor renderTransformOriginPropertyDescriptor;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            extendedItemArray[0] = this.ExtendedItem;
            this.ExtendedItem.PropertyChanged += OnPropertyChanged;

            if (this.ExtendedItem.Properties.GetProperty(UIElement.RenderTransformOriginProperty).IsSet)
            {
                renderTransformOrigin = (Point)this.ExtendedItem.Properties.GetProperty(UIElement.RenderTransformOriginProperty).ValueOnInstance;
            }

            AdornerPanel.SetPlacement(_renderTransformOriginThumb,
                                      new RelativePlacement(HorizontalAlignment.Left, VerticalAlignment.Top)
                                      {
                                          XRelativeToContentWidth = renderTransformOrigin.X,
                                          YRelativeToContentHeight = renderTransformOrigin.Y
                                      });

            renderTransformOriginPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(FrameworkElement.RenderTransformOriginProperty, typeof(FrameworkElement));
            renderTransformOriginPropertyDescriptor.AddValueChanged(this.ExtendedItem.Component, OnRenderTransformOriginPropertyChanged);
        }

        private void OnRenderTransformOriginPropertyChanged(object sender, EventArgs e)
        {
            Point pRel = renderTransformOrigin;
            if (this.ExtendedItem.Properties.GetProperty(UIElement.RenderTransformOriginProperty).IsSet)
                pRel = (Point)this.ExtendedItem.Properties.GetProperty(UIElement.RenderTransformOriginProperty).ValueOnInstance;

            AdornerPanel.SetPlacement(_renderTransformOriginThumb,
                                      new RelativePlacement(HorizontalAlignment.Left, VerticalAlignment.Top)
                                      {
                                          XRelativeToContentWidth = pRel.X,
                                          YRelativeToContentHeight = pRel.Y
                                      });

        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e) { }

        protected override void OnRemove()
        {
            renderTransformOriginPropertyDescriptor.RemoveValueChanged(this.ExtendedItem.Component, OnRenderTransformOriginPropertyChanged);

            base.OnRemove();
        }
    }
}
