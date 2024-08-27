using Hjmos.Lcdp.VisualEditor.Controls.Adorners;
using Hjmos.Lcdp.VisualEditor.Controls.DesignerControls;
using Hjmos.Lcdp.VisualEditor.Controls.Extensions;
using Hjmos.Lcdp.VisualEditor.Controls.UIExtensions;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Controls.Extensions2
{
    [ExtensionServer(typeof(OnlyOneItemSelectedExtensionServer))]
    [ExtensionFor(typeof(FrameworkElement))]
    public sealed class SkewThumbExtension : SelectionAdornerProvider
    {
        readonly AdornerPanel adornerPanel;
        readonly DesignItem[] extendedItemArray = new DesignItem[1];

        private AdornerLayer _adornerLayer;

        public SkewThumbExtension()
        {
            adornerPanel = new AdornerPanel { Order = AdornerOrder.BeforeForeground };
            this.Adorners.Add(adornerPanel);
        }

        #region Skew

        private Point startPoint;
        private UIElement parent;
        private SkewTransform skewTransform;
        private double skewX;
        private double skewY;
        private DesignItem rtTransform;
        private Thumb thumb1;
        private Thumb thumb2;
        PlacementOperation operation;

        private void DragX_Started(DragListener drag)
        {
            _adornerLayer = this.adornerPanel.TryFindParent<AdornerLayer>();

            var designerItem = this.ExtendedItem.Component as FrameworkElement;
            this.parent = VisualTreeHelper.GetParent(designerItem) as UIElement;

            startPoint = Mouse.GetPosition(this.parent);

            if (this.skewTransform == null)
            {
                this.skewX = 0;
                this.skewY = 0;
            }
            else
            {
                this.skewX = this.skewTransform.AngleX;
                this.skewY = this.skewTransform.AngleY;
            }

            rtTransform = this.ExtendedItem.Properties[FrameworkElement.RenderTransformProperty].Value;

            operation = PlacementOperation.Start(extendedItemArray, PlacementType.Resize);
        }

        private void DragX_Changed(DragListener drag)
        {
            Point currentPoint = Mouse.GetPosition(this.parent);
            Vector deltaVector = Point.Subtract(currentPoint, this.startPoint);

            var destAngle = (-0.5 * deltaVector.X) + skewX;

            if (destAngle == 0 && skewY == 0)
            {
                this.ExtendedItem.Properties.GetProperty(FrameworkElement.RenderTransformProperty).Reset();
                rtTransform = null;
                skewTransform = null;
            }
            else
            {
                if ((rtTransform == null) || !(rtTransform.Component is SkewTransform))
                {
                    if (!this.ExtendedItem.Properties.GetProperty(FrameworkElement.RenderTransformOriginProperty).IsSet)
                    {
                        this.ExtendedItem.Properties.GetProperty(FrameworkElement.RenderTransformOriginProperty).SetValue(new Point(0.5, 0.5));
                    }

                    if (this.skewTransform == null)
                        this.skewTransform = new SkewTransform(0, 0);
                    this.ExtendedItem.Properties.GetProperty(FrameworkElement.RenderTransformProperty).SetValue(skewTransform);
                    rtTransform = this.ExtendedItem.Properties[FrameworkElement.RenderTransformProperty].Value;
                }
                rtTransform.Properties["AngleX"].SetValue(destAngle);
            }

            _adornerLayer.UpdateAdornersForElement(this.ExtendedItem.View, true);
        }

        private void DragX_Completed(DragListener drag) => operation.Commit();

        private void DragY_Started(DragListener drag)
        {
            _adornerLayer = this.adornerPanel.TryFindParent<AdornerLayer>();

            var designerItem = this.ExtendedItem.Component as FrameworkElement;
            this.parent = VisualTreeHelper.GetParent(designerItem) as UIElement;

            startPoint = Mouse.GetPosition(this.parent);

            if (this.skewTransform == null)
            {
                this.skewX = 0;
                this.skewY = 0;
            }
            else
            {
                this.skewX = this.skewTransform.AngleX;
                this.skewY = this.skewTransform.AngleY;
            }

            rtTransform = this.ExtendedItem.Properties[FrameworkElement.RenderTransformProperty].Value;

            operation = PlacementOperation.Start(extendedItemArray, PlacementType.Resize);
        }

        private void DragY_Changed(DragListener drag)
        {
            Point currentPoint = Mouse.GetPosition(this.parent);
            Vector deltaVector = Point.Subtract(currentPoint, this.startPoint);

            var destAngle = (-0.5 * deltaVector.Y) + skewY;

            if (destAngle == 0 && skewX == 0)
            {
                this.ExtendedItem.Properties.GetProperty(FrameworkElement.RenderTransformProperty).Reset();
                rtTransform = null;
                skewTransform = null;
            }
            else
            {
                if (rtTransform == null)
                {
                    if (!this.ExtendedItem.Properties.GetProperty(FrameworkElement.RenderTransformOriginProperty).IsSet)
                    {
                        this.ExtendedItem.Properties.GetProperty(FrameworkElement.RenderTransformOriginProperty).SetValue(new Point(0.5, 0.5));
                    }

                    if (this.skewTransform == null)
                        this.skewTransform = new SkewTransform(0, 0);
                    this.ExtendedItem.Properties.GetProperty(FrameworkElement.RenderTransformProperty).SetValue(skewTransform);
                    rtTransform = this.ExtendedItem.Properties[FrameworkElement.RenderTransformProperty].Value;
                }
                rtTransform.Properties["AngleY"].SetValue(destAngle);
            }

            _adornerLayer.UpdateAdornersForElement(this.ExtendedItem.View, true);
        }

        private void DragY_Completed(DragListener drag) => operation.Commit();

        #endregion

        protected override void OnInitialized()
        {

            if (this.ExtendedItem.Component is WindowClone)
                return;
            base.OnInitialized();

            extendedItemArray[0] = this.ExtendedItem;
            this.ExtendedItem.PropertyChanged += OnPropertyChanged;

            var designerItem = this.ExtendedItem.Component as FrameworkElement;
            this.skewTransform = designerItem.RenderTransform as SkewTransform;

            if (skewTransform != null)
            {
                skewX = skewTransform.AngleX;
                skewY = skewTransform.AngleY;
            }

            thumb1 = new Thumb() { Cursor = Cursors.ScrollWE, Height = 14, Width = 4, Opacity = 1 };
            thumb2 = new Thumb() { Cursor = Cursors.ScrollNS, Width = 14, Height = 4, Opacity = 1 };

            OnPropertyChanged(null, null);

            adornerPanel.Children.Add(thumb1);
            adornerPanel.Children.Add(thumb2);

            DragListener drag1 = new DragListener(thumb1);
            drag1.Started += DragX_Started;
            drag1.Changed += DragX_Changed;
            drag1.Completed += DragX_Completed;
            DragListener drag2 = new DragListener(thumb2);
            drag2.Started += DragY_Started;
            drag2.Changed += DragY_Changed;
            drag2.Completed += DragY_Completed;
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender == null || e.PropertyName == "Width" || e.PropertyName == "Height")
            {
                AdornerPanel.SetPlacement(thumb1,
                                          new RelativePlacement(HorizontalAlignment.Center, VerticalAlignment.Top)
                                          {
                                              YOffset = 0,
                                              XOffset = -1 * PlacementOperation.GetRealElementSize(ExtendedItem.View).Width / 4
                                          });

                AdornerPanel.SetPlacement(thumb2,
                                          new RelativePlacement(HorizontalAlignment.Left, VerticalAlignment.Center)
                                          {
                                              YOffset = -1 * PlacementOperation.GetRealElementSize(ExtendedItem.View).Height / 4,
                                              XOffset = 0
                                          });

                if (this.ExtendedItem.Services.DesignPanel is DesignPanel designPanel)
                    designPanel.AdornerLayer.UpdateAdornersForElement(this.ExtendedItem.View, true);
            }
        }

        protected override void OnRemove()
        {
            this.ExtendedItem.PropertyChanged -= OnPropertyChanged;
            base.OnRemove();
        }
    }
}
