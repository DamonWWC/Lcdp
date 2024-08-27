using Hjmos.Lcdp.VisualEditor.Core.Adorners;
using Hjmos.Lcdp.VisualEditor.Core.Controls;
using Hjmos.Lcdp.VisualEditor.Core.DesignerControls;
using Hjmos.Lcdp.VisualEditor.Core.Thumbs;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    /// <summary>
    /// 组件顶部的旋转手柄
    /// </summary>
    [ExtensionServer(typeof(OnlyOneItemSelectedExtensionServer))]
    [ExtensionFor(typeof(FrameworkElement))]
    public sealed class RotateThumbExtension : SelectionAdornerProvider
    {
        private readonly AdornerPanel _adornerPanel;
        private readonly Thumb _thumb;

        /// <summary>一个仅包含this.ExtendedItem元素的数组</summary>
        readonly DesignItem[] _extendedItemArray = new DesignItem[1];
        private IPlacementBehavior _resizeBehavior;
        private PlacementOperation _operation;

        public RotateThumbExtension()
        {
            _adornerPanel = new AdornerPanel { Order = AdornerOrder.Foreground };
            this.Adorners.Add(_adornerPanel);

            _thumb = CreateRotateThumb();
        }

        DesignerThumb CreateRotateThumb()
        {
            DesignerThumb rotateThumb = new RotateThumb { Cursor = Cursors.Hand };
            rotateThumb.Cursor = ZoomControl.GetCursor("Images/rotate.cur");
            rotateThumb.Alignment = PlacementAlignment.Top;
            AdornerPanel.SetPlacement(rotateThumb, new RelativePlacement(HorizontalAlignment.Center, VerticalAlignment.Top) { WidthRelativeToContentWidth = 1, HeightOffset = 0 });
            _adornerPanel.Children.Add(rotateThumb);

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

            _operation = PlacementOperation.Start(_extendedItemArray, PlacementType.Resize);
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

        void Drag_Rotate_Completed(DragListener drag) => _operation.Commit();

        #endregion

        protected override void OnInitialized()
        {
            if (this.ExtendedItem.Component is WindowClone)
                return;
            base.OnInitialized();
            _extendedItemArray[0] = this.ExtendedItem;
            this.ExtendedItem.PropertyChanged += OnPropertyChanged;
            this.Services.Selection.PrimarySelectionChanged += OnPrimarySelectionChanged;
            _resizeBehavior = PlacementOperation.GetPlacementBehavior(_extendedItemArray);
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
            foreach (RotateThumb g in _adornerPanel.Children)
            {
                g.IsPrimarySelection = isPrimarySelection;
            }
        }
    }
}
