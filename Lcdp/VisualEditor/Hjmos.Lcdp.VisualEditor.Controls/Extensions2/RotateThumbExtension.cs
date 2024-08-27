using Hjmos.Lcdp.VisualEditor.Controls.Adorners;
using Hjmos.Lcdp.VisualEditor.Controls.DesignerControls;
using Hjmos.Lcdp.VisualEditor.Controls.Extensions;
using Hjmos.Lcdp.VisualEditor.Controls.Thumbs;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Controls.Extensions2
{
    /// <summary>
    /// 组件顶部的旋转手柄
    /// </summary>
    [ExtensionServer(typeof(OnlyOneItemSelectedExtensionServer))]
    [ExtensionFor(typeof(FrameworkElement))]
    public sealed class RotateThumbExtension : SelectionAdornerProvider
    {
        private readonly AdornerPanel adornerPanel;
        private readonly Thumb thumb;

        /// <summary>一个仅包含this.ExtendedItem元素的数组</summary>
        readonly DesignItem[] extendedItemArray = new DesignItem[1];
        private IPlacementBehavior resizeBehavior;
        private PlacementOperation operation;

        public RotateThumbExtension()
        {
            adornerPanel = new AdornerPanel { Order = AdornerOrder.Foreground };
            this.Adorners.Add(adornerPanel);

            thumb = CreateRotateThumb();
        }

        DesignerThumb CreateRotateThumb()
        {
            DesignerThumb rotateThumb = new RotateThumb { Cursor = Cursors.Hand };
            rotateThumb.Cursor = ZoomControl.GetCursor("Images/rotate.cur");
            rotateThumb.Alignment = PlacementAlignment.Top;
            AdornerPanel.SetPlacement(rotateThumb, new RelativePlacement(HorizontalAlignment.Center, VerticalAlignment.Top) { WidthRelativeToContentWidth = 1, HeightOffset = 0 });
            adornerPanel.Children.Add(rotateThumb);

            DragListener drag = new DragListener(rotateThumb);
            drag.Started += Drag_Rotate_Started;
            drag.Changed += Drag_Rotate_Changed;
            drag.Completed += Drag_Rotate_Completed;
            return rotateThumb;
        }

        #region Rotate

        private Point centerPoint;
        private UIElement parent;
        private Vector startVector;
        private RotateTransform rotateTransform;
        private double initialAngle;
        private DesignItem rtTransform;

        private void Drag_Rotate_Started(DragListener drag)
        {
            var designerItem = this.ExtendedItem.Component as FrameworkElement;
            this.parent = VisualTreeHelper.GetParent(designerItem) as UIElement;
            this.centerPoint = designerItem.TranslatePoint(
                new Point(designerItem.ActualWidth * designerItem.RenderTransformOrigin.X, designerItem.ActualHeight * designerItem.RenderTransformOrigin.Y),
                this.parent);

            Point startPoint = Mouse.GetPosition(this.parent);
            this.startVector = Point.Subtract(startPoint, this.centerPoint);

            if (this.rotateTransform == null)
            {
                this.initialAngle = 0;
            }
            else
            {
                this.initialAngle = this.rotateTransform.Angle;
            }

            rtTransform = this.ExtendedItem.Properties[FrameworkElement.RenderTransformProperty].Value;

            operation = PlacementOperation.Start(extendedItemArray, PlacementType.Resize);
        }

        private void Drag_Rotate_Changed(DragListener drag)
        {
            Point currentPoint = Mouse.GetPosition(this.parent);
            Vector deltaVector = Point.Subtract(currentPoint, this.centerPoint);

            double angle = Vector.AngleBetween(this.startVector, deltaVector);

            var destAngle = this.initialAngle + Math.Round(angle, 0);

            if (!Keyboard.IsKeyDown(Key.LeftCtrl))
                destAngle = ((int)destAngle / 15) * 15;

            ModelTools.ApplyTransform(this.ExtendedItem, new RotateTransform() { Angle = destAngle }, false);
        }

        void Drag_Rotate_Completed(DragListener drag) => operation.Commit();

        #endregion

        protected override void OnInitialized()
        {
            if (this.ExtendedItem.Component is WindowClone)
                return;
            base.OnInitialized();
            extendedItemArray[0] = this.ExtendedItem;
            this.ExtendedItem.PropertyChanged += OnPropertyChanged;
            this.Services.Selection.PrimarySelectionChanged += OnPrimarySelectionChanged;
            resizeBehavior = PlacementOperation.GetPlacementBehavior(extendedItemArray);
            OnPrimarySelectionChanged(null, null);

            var designerItem = this.ExtendedItem.Component as FrameworkElement;
            this.rotateTransform = designerItem.RenderTransform as RotateTransform;
            if (this.rotateTransform == null && designerItem.RenderTransform is TransformGroup tg)
            {
                this.rotateTransform = tg.Children.FirstOrDefault(x => x is RotateTransform) as RotateTransform;
            }

        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) { }

        protected override void OnRemove()
        {
            this.ExtendedItem.PropertyChanged -= OnPropertyChanged;
            this.Services.Selection.PrimarySelectionChanged -= OnPrimarySelectionChanged;
            base.OnRemove();
        }

        private void OnPrimarySelectionChanged(object sender, EventArgs e)
        {
            bool isPrimarySelection = this.Services.Selection.PrimarySelection == this.ExtendedItem;
            foreach (RotateThumb g in adornerPanel.Children)
            {
                g.IsPrimarySelection = isPrimarySelection;
            }
        }
    }
}
