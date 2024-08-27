using Hjmos.Lcdp.VisualEditor.Controls.Adorners;
using Hjmos.Lcdp.VisualEditor.Controls.Extensions;
using Hjmos.Lcdp.VisualEditor.Controls.Thumbs;
using System;
using System.Windows;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Controls.Extensions2
{
    /// <summary>
    /// 围绕组件调整大小的thumb。
    /// </summary>
    [ExtensionServer(typeof(OnlyOneItemSelectedExtensionServer))]
    [ExtensionFor(typeof(FrameworkElement))]
    public sealed class ResizeThumbExtension : SelectionAdornerProvider
    {
        private readonly AdornerPanel adornerPanel;
        private readonly DesignerThumb[] _designerThumbs;

        /// <summary>一个只包含this.ExtendedItem的数组</summary>
        private readonly DesignItem[] extendedItemArray = new DesignItem[1];
        private IPlacementBehavior resizeBehavior;
        private PlacementOperation operation;

        /// <summary>
        /// 获取此扩展是否正在调整任何元素的大小。
        /// </summary>
        public bool IsResizing { get; private set; }

        public ResizeThumbExtension()
        {
            adornerPanel = new AdornerPanel
            {
                Order = AdornerOrder.Foreground
            };
            this.Adorners.Add(adornerPanel);

            _designerThumbs = new[] {
                CreateThumb(PlacementAlignment.TopLeft, Cursors.SizeNWSE),
                CreateThumb(PlacementAlignment.Top, Cursors.SizeNS),
                CreateThumb(PlacementAlignment.TopRight, Cursors.SizeNESW),
                CreateThumb(PlacementAlignment.Left, Cursors.SizeWE),
                CreateThumb(PlacementAlignment.Right, Cursors.SizeWE),
                CreateThumb(PlacementAlignment.BottomLeft, Cursors.SizeNESW),
                CreateThumb(PlacementAlignment.Bottom, Cursors.SizeNS),
                CreateThumb(PlacementAlignment.BottomRight, Cursors.SizeNWSE)
            };
        }

        private DesignerThumb CreateThumb(PlacementAlignment alignment, Cursor cursor)
        {
            DesignerThumb designerThumb = new ResizeThumb(cursor == Cursors.SizeNS, cursor == Cursors.SizeWE)
            {
                Cursor = cursor,
                Alignment = alignment
            };
            AdornerPanel.SetPlacement(designerThumb, Place(ref designerThumb, alignment));
            adornerPanel.Children.Add(designerThumb);

            DragListener drag = new(designerThumb);
            drag.Started += Drag_Started;
            drag.Changed += Drag_Changed;
            drag.Completed += Drag_Completed;
            return designerThumb;
        }

        /// <summary>
        /// 将调整大小的thumb放在各自的位置，并延伸thumb到轮廓的中心，以使整个轮廓可调整大小
        /// </summary>
        /// <param name="designerThumb"></param>
        /// <param name="alignment"></param>
        /// <returns></returns>
        private RelativePlacement Place(ref DesignerThumb designerThumb, PlacementAlignment alignment)
        {
            RelativePlacement placement = new RelativePlacement(alignment.Horizontal, alignment.Vertical);

            if (alignment.Horizontal == HorizontalAlignment.Center)
            {
                placement.WidthRelativeToContentWidth = 1;
                placement.HeightOffset = 6;
                designerThumb.Opacity = 0;
                return placement;
            }
            if (alignment.Vertical == VerticalAlignment.Center)
            {
                placement.HeightRelativeToContentHeight = 1;
                placement.WidthOffset = 6;
                designerThumb.Opacity = 0;
                return placement;
            }

            placement.WidthOffset = 6;
            placement.HeightOffset = 6;
            return placement;
        }

        private Size _oldSize;

        // TODO : Remove all hide/show extensions from here. 从这里删除所有的隐藏/显示扩展。
        private void Drag_Started(DragListener drag)
        {
            /* Abort editing Text if it was editing, because it interferes with the undo stack. */
            //foreach(var extension in this.ExtendedItem.Extensions){
            //	if(extension is InPlaceEditorExtension){
            //		((InPlaceEditorExtension)extension).AbortEdit();
            //	}
            //}

            drag.Transform = this.ExtendedItem.GetCompleteAppliedTransformationToView();

            _oldSize = new Size(ModelTools.GetWidth(ExtendedItem.View), ModelTools.GetHeight(ExtendedItem.View));
            if (resizeBehavior != null)
                operation = PlacementOperation.Start(extendedItemArray, PlacementType.Resize);
            else
            {
                //changeGroup = this.ExtendedItem.Context.OpenGroup("Resize", extendedItemArray);
            }
            IsResizing = true;
            ShowSizeAndHideHandles();
        }

        private void Drag_Changed(DragListener drag)
        {
            double dx = 0;
            double dy = 0;
            PlacementAlignment alignment = (drag.Target as DesignerThumb).Alignment;

            Vector delta = drag.Delta;

            if (alignment.Horizontal == HorizontalAlignment.Left) dx = -delta.X;
            if (alignment.Horizontal == HorizontalAlignment.Right) dx = delta.X;
            if (alignment.Vertical == VerticalAlignment.Top) dy = -delta.Y;
            if (alignment.Vertical == VerticalAlignment.Bottom) dy = delta.Y;

            if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) &&
                alignment.Horizontal != HorizontalAlignment.Center && alignment.Vertical != VerticalAlignment.Center)
            {
                if (dx > dy)
                    dx = dy;
                else
                    dy = dx;
            }

            double newWidth = Math.Max(0, _oldSize.Width + dx);
            double newHeight = Math.Max(0, _oldSize.Height + dy);

            if (operation.CurrentContainerBehavior is GridPlacementSupport)
            {
                HorizontalAlignment hor = (HorizontalAlignment)this.ExtendedItem.Properties[FrameworkElement.HorizontalAlignmentProperty].ValueOnInstance;
                VerticalAlignment ver = (VerticalAlignment)this.ExtendedItem.Properties[FrameworkElement.VerticalAlignmentProperty].ValueOnInstance;
                if (hor == HorizontalAlignment.Stretch)
                    this.ExtendedItem.Properties[FrameworkElement.WidthProperty].Reset();
                else
                    this.ExtendedItem.Properties.GetProperty(FrameworkElement.WidthProperty).SetValue(newWidth);

                if (ver == VerticalAlignment.Stretch)
                    this.ExtendedItem.Properties[FrameworkElement.HeightProperty].Reset();
                else
                    this.ExtendedItem.Properties.GetProperty(FrameworkElement.HeightProperty).SetValue(newHeight);
            }
            else
            {
                ModelTools.Resize(ExtendedItem, newWidth, newHeight);
            }

            if (operation != null)
            {
                PlacementInformation info = operation.PlacedItems[0];
                Rect result = info.OriginalBounds;

                if (alignment.Horizontal == HorizontalAlignment.Left)
                    result.X = Math.Min(result.Right, result.X - dx);
                if (alignment.Vertical == VerticalAlignment.Top)
                    result.Y = Math.Min(result.Bottom, result.Y - dy);
                result.Width = newWidth;
                result.Height = newHeight;

                info.Bounds = result.Round();
                info.ResizeThumbAlignment = alignment;
                operation.CurrentContainerBehavior.BeforeSetPosition(operation);
                operation.CurrentContainerBehavior.SetPosition(info);
            }
        }

        private void Drag_Completed(DragListener drag)
        {
            if (operation != null)
            {
                if (drag.IsCanceled) operation.Abort();
                else operation.Commit();
                operation = null;
            }
            else
            {
                //if (drag.IsCanceled) changeGroup.Abort();
                //else changeGroup.Commit();
                //changeGroup = null;
            }
            IsResizing = false;
            HideSizeAndShowHandles();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            extendedItemArray[0] = this.ExtendedItem;
            this.ExtendedItem.PropertyChanged += OnPropertyChanged;
            this.Services.Selection.PrimarySelectionChanged += OnPrimarySelectionChanged;
            resizeBehavior = PlacementOperation.GetPlacementBehavior(extendedItemArray);
            UpdateAdornerVisibility();
            OnPrimarySelectionChanged(null, null);
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateAdornerVisibility();
        }

        protected override void OnRemove()
        {
            this.ExtendedItem.PropertyChanged -= OnPropertyChanged;
            this.Services.Selection.PrimarySelectionChanged -= OnPrimarySelectionChanged;
            base.OnRemove();
        }

        private void OnPrimarySelectionChanged(object sender, EventArgs e)
        {
            bool isPrimarySelection = this.Services.Selection.PrimarySelection == this.ExtendedItem;
            foreach (DesignerThumb g in adornerPanel.Children)
            {
                g.IsPrimarySelection = isPrimarySelection;
            }
        }

        private void UpdateAdornerVisibility()
        {
            foreach (DesignerThumb r in _designerThumbs)
            {
                bool isVisible = resizeBehavior != null && resizeBehavior.CanPlace(extendedItemArray, PlacementType.Resize, r.Alignment);
                r.Visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
            }
        }

        // 控制显示/隐藏调整大小/边距的手柄
        private void ShowSizeAndHideHandles()
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
                sizeDisplay.HeightDisplay.Visibility = Visibility.Visible;
                sizeDisplay.WidthDisplay.Visibility = Visibility.Visible;
            }

            if (marginDisplay != null)
                marginDisplay.HideHandles();
        }

        private void HideSizeAndShowHandles()
        {
            SizeDisplayExtension sizeDisplay = null;
            MarginHandleExtension marginDisplay = null;
            foreach (var extension in ExtendedItem.Extensions)
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
    }
}
