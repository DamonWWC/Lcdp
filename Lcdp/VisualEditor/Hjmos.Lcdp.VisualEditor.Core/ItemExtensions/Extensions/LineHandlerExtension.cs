using Hjmos.Lcdp.Helpers;
using Hjmos.Lcdp.VisualEditor.Core.Adorners;
using Hjmos.Lcdp.VisualEditor.Core.Controls;
using Hjmos.Lcdp.VisualEditor.Core.Thumbs;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    [ExtensionFor(typeof(Line), OverrideExtensions = new Type[] { typeof(ResizeThumbExtension), typeof(SelectedElementRectangleExtension), typeof(CanvasPositionExtension), typeof(QuickOperationMenuExtension), typeof(RotateThumbExtension), typeof(RenderTransformOriginExtension), typeof(SkewThumbExtension) })]
    public class LineHandlerExtension : LineExtensionBase
    {
        private double CurrentX2;
        private double CurrentY2;
        private double CurrentLeft;
        private double CurrentTop;

        //Size oldSize;
        private ZoomControl zoom;

        public DragListener DragListener { get; private set; }

        protected DesignerThumb CreateThumb(PlacementAlignment alignment, Cursor cursor)
        {
            DesignerThumb designerThumb = new() { Alignment = alignment, Cursor = cursor, IsPrimarySelection = true };
            AdornerPanel.SetPlacement(designerThumb, Place(designerThumb, alignment));

            adornerPanel.Children.Add(designerThumb);

            DragListener = new DragListener(designerThumb);
            DragListener.Started += Drag_Started;
            DragListener.Changed += Drag_Changed;
            DragListener.Completed += Drag_Completed;

            return designerThumb;
        }

        Bounds CalculateDrawing(double x, double y, double left, double top, double xleft, double xtop)
        {

            double theta = (180 / Math.PI) * Math.Atan2(y, x);
            double verticaloffset = Math.Abs(90 - Math.Abs(theta));
            if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
            {
                if (Math.Abs(theta) < 45 || Math.Abs(theta) > 135)
                {
                    y = 0;
                    top = xtop;
                }
                else if (verticaloffset < 45)
                {
                    x = 0;
                    left = xleft;
                }
            }
            else if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                if (verticaloffset < 10)
                {
                    x = 0;
                    left = xleft;
                }
                else if (Math.Abs(theta) < 10 || Math.Abs(theta) > 170)
                {
                    y = 0;
                    top = xtop;
                }
            }

            SetSurfaceInfo(0, 3, Math.Round((180 / Math.PI) * Math.Atan2(y, x), 0).ToString());
            return new Bounds { X = Math.Round(x, 1), Y = Math.Round(y, 1), Left = Math.Round(left, 1), Top = Math.Round(top, 1) };
        }

        #region eventhandlers


        // TODO: 从这里删除所有的隐藏/显示扩展
        protected virtual void Drag_Started(DragListener drag)
        {
            Line al = ExtendedItem.View as Line;
            CurrentX2 = al.X2;
            CurrentY2 = al.Y2;
            CurrentLeft = al.Margin.Left;
            CurrentTop = al.Margin.Top;
            if (ExtendedItem.Parent.View is Canvas)
            {
                var lp = (double)al.GetValue(Canvas.LeftProperty);
                if (!double.IsNaN(lp))
                    CurrentLeft += lp;
                var tp = (double)al.GetValue(Canvas.TopProperty);
                if (!double.IsNaN(tp))
                    CurrentTop += tp;
            }

            var designPanel = ExtendedItem.Services.DesignPanel as DesignPanel;
            zoom = designPanel.TryFindParent<ZoomControl>();

            if (resizeBehavior != null)
                operation = PlacementOperation.Start(extendedItemArray, PlacementType.Resize);
            else
            {
                //changeGroup = this.ExtendedItem.Context.OpenGroup("Resize", extendedItemArray);
            }
            _isResizing = true;

            (drag.Target as DesignerThumb).IsPrimarySelection = false;
        }

        protected virtual void Drag_Changed(DragListener drag)
        {
            Line al = ExtendedItem.View as Line;

            var alignment = (drag.Target as DesignerThumb).Alignment;
            var info = operation.PlacedItems[0];
            double dx = 0;
            double dy = 0;

            if (zoom != null)
            {
                dx = drag.Delta.X * (1 / zoom.CurrentZoom);
                dy = drag.Delta.Y * (1 / zoom.CurrentZoom);
            }

            double top, left, x, y, xtop, xleft;

            if (alignment == PlacementAlignment.TopLeft)
            {

                // 正常的值
                x = CurrentX2 - dx;
                y = CurrentY2 - dy;
                top = CurrentTop + dy;
                left = CurrentLeft + dx;

                // 按下键时使用的值
                xtop = CurrentTop + CurrentY2;
                xleft = CurrentLeft + CurrentX2;

            }
            else
            {
                x = CurrentX2 + dx;
                y = CurrentY2 + dy;
                top = xtop = CurrentTop;
                left = xleft = CurrentLeft;
            }

            Bounds position = CalculateDrawing(x, y, left, top, xleft, xtop);

            ExtendedItem.Properties.GetProperty(Line.X1Property).SetValue(0);
            ExtendedItem.Properties.GetProperty(Line.Y1Property).SetValue(0);
            ExtendedItem.Properties.GetProperty(Line.X2Property).SetValue(position.X);
            ExtendedItem.Properties.GetProperty(Line.Y2Property).SetValue(position.Y);

            if (operation != null)
            {
                var result = info.OriginalBounds;
                result.X = position.Left;
                result.Y = position.Top;
                result.Width = Math.Abs(position.X);
                result.Height = Math.Abs(position.Y);

                info.Bounds = result.Round();
                operation.CurrentContainerBehavior.BeforeSetPosition(operation);
                operation.CurrentContainerBehavior.SetPosition(info);

                //				var p = operation.CurrentContainerBehavior.PlacePoint(new Point(position.X, position.Y));
                //				ExtendedItem.Properties.GetProperty(Line.X2Property).SetValue(p.X);
                //				ExtendedItem.Properties.GetProperty(Line.Y2Property).SetValue(p.Y);
            }

            (drag.Target as DesignerThumb).InvalidateArrange();
            ResetWidthHeightProperties();
        }

        protected virtual void Drag_Completed(DragListener drag)
        {
            if (operation != null)
            {
                if (drag.IsCanceled) operation.Abort();
                else
                {
                    ResetWidthHeightProperties();

                    operation.Commit();
                }
                operation = null;
            }
            else
            {
                //if (drag.IsCanceled)
                //    changeGroup.Abort();
                //else
                //    changeGroup.Commit();
                //changeGroup = null;
            }

            _isResizing = false;
            (drag.Target as DesignerThumb).IsPrimarySelection = true;
            HideSizeAndShowHandles();
        }

        /// <summary>
        /// 每当在画布上选择一个行时调用，记住装饰器是为每个行对象创建的，并且永远不会销毁
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            resizeThumbs = new DesignerThumb[]
            {
                CreateThumb(PlacementAlignment.TopLeft, Cursors.Cross),
                CreateThumb(PlacementAlignment.BottomRight, Cursors.Cross)
            };

            extendedItemArray[0] = this.ExtendedItem;

            Invalidate();

            this.ExtendedItem.PropertyChanged += OnPropertyChanged;
            resizeBehavior = PlacementOperation.GetPlacementBehavior(extendedItemArray);
            UpdateAdornerVisibility();
        }

        #endregion
    }
}
