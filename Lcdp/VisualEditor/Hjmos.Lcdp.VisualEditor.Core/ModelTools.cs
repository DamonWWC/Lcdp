using Hjmos.Lcdp.VisualEditor.Core.Enums;
using Hjmos.Lcdp.VisualEditor.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Xps.Serialization;

namespace Hjmos.Lcdp.VisualEditor.Core
{
    /// <summary>
    /// 使用设计器DOM的静态帮助器方法。
    /// </summary>
    public static class ModelTools
    {
        /// <summary>
        /// 比较a和b在模型文件中的位置。
        /// </summary>
        public static int ComparePositionInModelFile(DesignItem a, DesignItem b)
        {
            // 首先记住a的所有父属性
            HashSet<DesignItemProperty> aProps = new();
            DesignItem tmp = a;
            while (tmp != null)
            {
                aProps.Add(tmp.ParentProperty);
                tmp = tmp.Parent;
            }

            // 现在沿着b的父树向上走，直到找到一个匹配的属性
            tmp = b;
            while (tmp != null)
            {
                DesignItemProperty prop = tmp.ParentProperty;
                if (aProps.Contains(prop))
                {
                    if (prop.IsCollection)
                    {
                        return prop.CollectionElements.IndexOf(a).CompareTo(prop.CollectionElements.IndexOf(b));
                    }
                    else
                    {
                        return 0;
                    }
                }
                tmp = tmp.Parent;
            }
            return 0;
        }

        /// <summary>
        /// 获取指定的设计项是否在其所属的文档中。
        /// </summary>
        /// <returns>对于活动对象为True，对于删除对象为false。</returns>
        public static bool IsInDocument(DesignItem item)
        {
            DesignItem rootItem = item.Context.RootItem;
            while (item != null)
            {
                if (item == rootItem) return true;
                item = item.Parent;
            }
            return false;
        }

        /// <summary>
        /// 获取是否可以删除指定的组件。
        /// </summary>
        public static bool CanDeleteComponents(ICollection<DesignItem> items)
        {
            IPlacementBehavior b = PlacementOperation.GetPlacementBehavior(items);
            return b != null && b.CanPlace(items, PlacementType.Delete, PlacementAlignment.Center);
        }

        public static bool CanSelectComponent(DesignItem item) => item.View != null;

        /// <summary>
        /// 从父容器中删除指定的组件
        /// 如果删除的组件是当前选中的组件，则删除前取消选中
        /// </summary>
        public static void DeleteComponents(ICollection<DesignItem> deleteItems)
        {
            if (deleteItems.Count > 0)
            {
                //var changeGroup = deleteItems.First().OpenGroup("Delete Items");
                try
                {
                    var itemsGrpParent = deleteItems.GroupBy(x => x.Parent);
                    // 根据父类来分组处理
                    foreach (var itemsList in itemsGrpParent)
                    {
                        List<DesignItem> items = itemsList.ToList();
                        DesignItem parent = items.First().Parent;

                        // 定义一个放置类型为删除的放置操作
                        PlacementOperation operation = PlacementOperation.Start(items, PlacementType.Delete);
                        try
                        {
                            // 取消选中
                            ISelectionService selectionService = items.First().Services.Selection;
                            selectionService.SetSelectedComponents(items, SelectionTypes.Remove);

                            // 如果在删除某些组件后选择为空，则选择被删除组件的父组件
                            if (selectionService.SelectionCount == 0 && !items.Contains(parent))
                            {
                                selectionService.SetSelectedComponents(new[] { parent });
                            }

                            //// 删除设计项的名称(原设计估计是为了支持重做，可以不用删除)
                            //foreach (DesignItem designItem in items)
                            //{
                            //    designItem.Name = null;
                            //}

                            // 触发组件移除事件
                            var service = parent.Services.Component as MyComponentService;
                            foreach (DesignItem item in items)
                            {
                                service.RaiseComponentRemoved(item);
                            }

                            // 删除正在放置的项
                            operation.DeleteItemsAndCommit();
                        }
                        catch (Exception e)
                        {
                            operation.Abort();
                            throw e;
                        }
                    }
                    //changeGroup.Commit();
                }
                catch
                {
                    //changeGroup.Abort();
                    throw;
                }
            }
        }

        public static void CreateVisualTree(this UIElement element)
        {
            try
            {
                var fixedDoc = new FixedDocument();
                var pageContent = new PageContent();
                var fixedPage = new FixedPage();
                fixedPage.Children.Add(element);
                (pageContent as IAddChild).AddChild(fixedPage);
                fixedDoc.Pages.Add(pageContent);

                var f = new XpsSerializerFactory();
                var w = f.CreateSerializerWriter(new MemoryStream());
                w.Write(fixedDoc);

                fixedPage.Children.Remove(element);
            }
            catch (Exception)
            { }
        }

        internal static Size GetDefaultSize(DesignItem createdItem)
        {
            var defS = Metadata.GetDefaultSize(createdItem.ComponentType, false);
            if (defS != null)
                return defS.Value;

            CreateVisualTree(createdItem.View);

            var s = createdItem.View.DesiredSize;

            var newS = Metadata.GetDefaultSize(createdItem.ComponentType, true);

            if (newS.HasValue)
            {
                if (!(s.Width > 5) && newS.Value.Width > 0)
                    s.Width = newS.Value.Width;

                if (!(s.Height > 5) && newS.Value.Height > 0)
                    s.Height = newS.Value.Height;
            }

            if (double.IsNaN(s.Width) && GetWidth(createdItem.View) > 0)
            {
                s.Width = GetWidth(createdItem.View);
            }
            if (double.IsNaN(s.Height) && GetWidth(createdItem.View) > 0)
            {
                s.Height = GetHeight(createdItem.View);
            }

            return s;
        }

        public static double GetWidth(UIElement element)
        {
            double v = (double)element.GetValue(FrameworkElement.WidthProperty);
            if (double.IsNaN(v))
                return element.RenderSize.Width;
            else
                return v;
        }

        public static double GetHeight(UIElement element)
        {
            double v = (double)element.GetValue(FrameworkElement.HeightProperty);
            if (double.IsNaN(v))
                return element.RenderSize.Height;
            else
                return v;
        }

        public static void Resize(DesignItem item, double newWidth, double newHeight)
        {
            if (newWidth != GetWidth(item.View))
            {
                if (double.IsNaN(newWidth))
                    item.Properties.GetProperty(FrameworkElement.WidthProperty).Reset();
                else
                    item.Properties.GetProperty(FrameworkElement.WidthProperty).SetValue(newWidth);
            }
            if (newHeight != GetHeight(item.View))
            {
                if (double.IsNaN(newHeight))
                    item.Properties.GetProperty(FrameworkElement.HeightProperty).Reset();
                else
                    item.Properties.GetProperty(FrameworkElement.HeightProperty).SetValue(newHeight);
            }
        }


        private class ItemPos
        {
            public HorizontalAlignment HorizontalAlignment { get; set; }

            public VerticalAlignment VerticalAlignment { get; set; }

            public double Xmin { get; set; }

            public double Xmax { get; set; }

            public double Ymin { get; set; }

            public double Ymax { get; set; }

            public DesignItem DesignItem { get; set; }
        }

        private static ItemPos GetItemPos(PlacementOperation operation, DesignItem designItem)
        {
            var itemPos = new ItemPos() { DesignItem = designItem };

            var pos = operation.CurrentContainerBehavior.GetPosition(operation, designItem);
            itemPos.Xmin = pos.X;
            itemPos.Xmax = pos.X + pos.Width;
            itemPos.Ymin = pos.Y;
            itemPos.Ymax = pos.Y + pos.Height;

            return itemPos;
        }

        public static Tuple<DesignItem, Rect> WrapItemsNewContainer(IEnumerable<DesignItem> items, Type containerType, bool doInsert = true)
        {
            var collection = items;

            var _context = collection.First().Context as MyDesignContext;

            var container = collection.First().Parent;

            if (collection.Any(x => x.Parent != container))
                return null;

            //Change Code to use the Placment Operation!
            var placement = container.Extensions.OfType<IPlacementBehavior>().FirstOrDefault();
            if (placement == null)
                return null;

            var operation = PlacementOperation.Start(items.ToList(), PlacementType.Move);

            var newInstance = _context.Services.ExtensionManager.CreateInstanceWithCustomInstanceFactory(containerType, null);
            DesignItem newPanel = _context.Services.Component.RegisterComponentForDesigner(newInstance);

            List<ItemPos> itemList = new List<ItemPos>();

            int? firstIndex = null;

            foreach (var item in collection)
            {
                itemList.Add(GetItemPos(operation, item));
                //var pos = placement.GetPosition(null, item);
                if (container.Component is Canvas)
                {
                    item.Properties.GetAttachedProperty(Canvas.RightProperty).Reset();
                    item.Properties.GetAttachedProperty(Canvas.LeftProperty).Reset();
                    item.Properties.GetAttachedProperty(Canvas.TopProperty).Reset();
                    item.Properties.GetAttachedProperty(Canvas.BottomProperty).Reset();
                }
                else if (container.Component is Grid)
                {
                    item.Properties.GetProperty(FrameworkElement.HorizontalAlignmentProperty).Reset();
                    item.Properties.GetProperty(FrameworkElement.VerticalAlignmentProperty).Reset();
                    item.Properties.GetProperty(FrameworkElement.MarginProperty).Reset();
                }

                if (item.ParentProperty.IsCollection)
                {
                    var parCol = item.ParentProperty.CollectionElements;
                    if (!firstIndex.HasValue)
                        firstIndex = parCol.IndexOf(item);
                    parCol.Remove(item);
                }
                else
                {
                    item.ParentProperty.Reset();
                }
            }

            var xmin = itemList.Min(x => x.Xmin);
            var xmax = itemList.Max(x => x.Xmax);
            var ymin = itemList.Min(x => x.Ymin);
            var ymax = itemList.Max(x => x.Ymax);

            foreach (var item in itemList)
            {
                if (newPanel.Component is Canvas)
                {
                    if (item.HorizontalAlignment == HorizontalAlignment.Right)
                    {
                        item.DesignItem.Properties.GetAttachedProperty(Canvas.RightProperty).SetValue(xmax - item.Xmax);
                    }
                    else
                    {
                        item.DesignItem.Properties.GetAttachedProperty(Canvas.LeftProperty).SetValue(item.Xmin - xmin);
                    }

                    if (item.VerticalAlignment == VerticalAlignment.Bottom)
                    {
                        item.DesignItem.Properties.GetAttachedProperty(Canvas.BottomProperty).SetValue(ymax - item.Ymax);
                    }
                    else
                    {
                        item.DesignItem.Properties.GetAttachedProperty(Canvas.TopProperty).SetValue(item.Ymin - ymin);
                    }

                    newPanel.ContentProperty.CollectionElements.Add(item.DesignItem);

                }
                else if (newPanel.Component is Grid)
                {
                    Thickness thickness = new Thickness(0);
                    if (item.HorizontalAlignment == HorizontalAlignment.Right)
                    {
                        item.DesignItem.Properties.GetProperty(FrameworkElement.HorizontalAlignmentProperty).SetValue(HorizontalAlignment.Right);
                        thickness.Right = xmax - item.Xmax;
                    }
                    else
                    {
                        item.DesignItem.Properties.GetProperty(FrameworkElement.HorizontalAlignmentProperty).SetValue(HorizontalAlignment.Left);
                        thickness.Left = item.Xmin - xmin;
                    }

                    if (item.VerticalAlignment == VerticalAlignment.Bottom)
                    {
                        item.DesignItem.Properties.GetProperty(FrameworkElement.VerticalAlignmentProperty).SetValue(VerticalAlignment.Bottom);
                        thickness.Bottom = ymax - item.Ymax;
                    }
                    else
                    {
                        item.DesignItem.Properties.GetProperty(FrameworkElement.VerticalAlignmentProperty).SetValue(VerticalAlignment.Top);
                        thickness.Top = item.Ymin - ymin;
                    }

                    item.DesignItem.Properties.GetProperty(FrameworkElement.MarginProperty).SetValue(thickness);

                    newPanel.ContentProperty.CollectionElements.Add(item.DesignItem);

                }
                else if (newPanel.Component is Viewbox)
                {
                    newPanel.ContentProperty.SetValue(item.DesignItem);
                }
                else if (newPanel.Component is ContentControl)
                {
                    newPanel.ContentProperty.SetValue(item.DesignItem);
                }
            }

            if (doInsert)
            {
                PlacementOperation operation2 = PlacementOperation.TryStartInsertNewComponents(
                    container,
                    new[] { newPanel },
                    new[] { new Rect(xmin, ymin, xmax - xmin, ymax - ymin).Round() },
                    PlacementType.AddItem
                );

                if (items.Count() == 1 && container.ContentProperty != null && container.ContentProperty.IsCollection)
                {
                    container.ContentProperty.CollectionElements.Remove(newPanel);
                    container.ContentProperty.CollectionElements.Insert(firstIndex.Value, newPanel);
                }

                operation2.Commit();

                _context.Services.Selection.SetSelectedComponents(new[] { newPanel });
            }

            operation.Commit();

            return new Tuple<DesignItem, Rect>(newPanel, new Rect(xmin, ymin, xmax - xmin, ymax - ymin).Round());
        }

        public static void UnwrapItemsFromContainer(DesignItem container)
        {
            var collection = container.ContentProperty.CollectionElements.ToList();

            var newPanel = container.Parent;

            if (collection.Any(x => x.Parent != container))
                return;

            //Change Code to use the Placment Operation!
            var placement = container.Extensions.OfType<IPlacementBehavior>().FirstOrDefault();
            if (placement == null)
                return;

            var operation = PlacementOperation.Start(collection.ToList(), PlacementType.Move);

            List<ItemPos> itemList = new();

            int? firstIndex = null;

            var containerPos = GetItemPos(operation, container);

            foreach (var item in collection)
            {
                itemList.Add(GetItemPos(operation, item));
                if (container.Component is Canvas)
                {
                    item.Properties.GetAttachedProperty(Canvas.RightProperty).Reset();
                    item.Properties.GetAttachedProperty(Canvas.LeftProperty).Reset();
                    item.Properties.GetAttachedProperty(Canvas.TopProperty).Reset();
                    item.Properties.GetAttachedProperty(Canvas.BottomProperty).Reset();
                }
                else if (container.Component is Grid)
                {
                    item.Properties.GetProperty(FrameworkElement.HorizontalAlignmentProperty).Reset();
                    item.Properties.GetProperty(FrameworkElement.VerticalAlignmentProperty).Reset();
                    item.Properties.GetProperty(FrameworkElement.MarginProperty).Reset();
                }

                if (item.ParentProperty.IsCollection)
                {
                    var parCol = item.ParentProperty.CollectionElements;
                    if (!firstIndex.HasValue)
                        firstIndex = parCol.IndexOf(item);
                    parCol.Remove(item);
                }
                else
                {
                    item.ParentProperty.Reset();
                }
            }

            newPanel.ContentProperty.CollectionElements.Remove(container);

            foreach (var item in itemList)
            {
                if (newPanel.Component is Canvas)
                {
                    if (item.HorizontalAlignment == HorizontalAlignment.Right)
                    {
                        item.DesignItem.Properties.GetAttachedProperty(Canvas.RightProperty).SetValue(containerPos.Xmax - item.Xmax);
                    }
                    else
                    {
                        item.DesignItem.Properties.GetAttachedProperty(Canvas.LeftProperty).SetValue(item.Xmin + containerPos.Xmin);
                    }

                    if (item.VerticalAlignment == VerticalAlignment.Bottom)
                    {
                        item.DesignItem.Properties.GetAttachedProperty(Canvas.BottomProperty).SetValue(containerPos.Ymax - item.Ymax);
                    }
                    else
                    {
                        item.DesignItem.Properties.GetAttachedProperty(Canvas.TopProperty).SetValue(item.Ymin + containerPos.Ymin);
                    }

                    newPanel.ContentProperty.CollectionElements.Add(item.DesignItem);

                }
                else if (newPanel.Component is Grid)
                {
                    Thickness thickness = new Thickness(0);
                    if (item.HorizontalAlignment == HorizontalAlignment.Right)
                    {
                        item.DesignItem.Properties.GetProperty(FrameworkElement.HorizontalAlignmentProperty).SetValue(HorizontalAlignment.Right);
                        thickness.Right = containerPos.Xmax - item.Xmax;
                    }
                    else
                    {
                        item.DesignItem.Properties.GetProperty(FrameworkElement.HorizontalAlignmentProperty).SetValue(HorizontalAlignment.Left);
                        thickness.Left = item.Xmin;
                    }

                    if (item.VerticalAlignment == VerticalAlignment.Bottom)
                    {
                        item.DesignItem.Properties.GetProperty(FrameworkElement.VerticalAlignmentProperty).SetValue(VerticalAlignment.Bottom);
                        thickness.Bottom = containerPos.Ymax - item.Ymax;
                    }
                    else
                    {
                        item.DesignItem.Properties.GetProperty(FrameworkElement.VerticalAlignmentProperty).SetValue(VerticalAlignment.Top);
                        thickness.Top = item.Ymin;
                    }

                    item.DesignItem.Properties.GetProperty(FrameworkElement.MarginProperty).SetValue(thickness);

                    newPanel.ContentProperty.CollectionElements.Add(item.DesignItem);

                }
                else if (newPanel.Component is Viewbox)
                {
                    newPanel.ContentProperty.SetValue(item.DesignItem);
                }
                else if (newPanel.Component is ContentControl)
                {
                    newPanel.ContentProperty.SetValue(item.DesignItem);
                }
            }

            operation.Commit();
        }

        public static void ApplyTransform(DesignItem designItem, Transform transform, bool relative = true, DependencyProperty transformProperty = null)
        {
            //var changeGroup = designItem.OpenGroup("Apply Transform");

            transformProperty = transformProperty ?? FrameworkElement.RenderTransformProperty;
            Transform oldTransform = null;
            if (designItem.Properties.GetProperty(transformProperty).IsSet)
            {
                oldTransform = designItem.Properties.GetProperty(transformProperty).ValueOnInstance as Transform;
            }

            if (oldTransform is MatrixTransform)
            {
                var mt = oldTransform as MatrixTransform;
                var tg = new TransformGroup();
                if (mt.Matrix.OffsetX != 0 && mt.Matrix.OffsetY != 0)
                    tg.Children.Add(new TranslateTransform() { X = mt.Matrix.OffsetX, Y = mt.Matrix.OffsetY });
                if (mt.Matrix.M11 != 0 && mt.Matrix.M22 != 0)
                    tg.Children.Add(new ScaleTransform() { ScaleX = mt.Matrix.M11, ScaleY = mt.Matrix.M22 });

                var angle = Math.Atan2(mt.Matrix.M21, mt.Matrix.M11) * 180 / Math.PI;
                if (angle != 0)
                    tg.Children.Add(new RotateTransform() { Angle = angle });
                //if (mt.Matrix.M11 != 0 && mt.Matrix.M22 != 0)
                //	tg.Children.Add(new SkewTransform(){ ScaleX = mt.Matrix.M11, ScaleY = mt.Matrix.M22 });
            }
            else if (oldTransform != null && oldTransform.GetType() != transform.GetType())
            {
                var tg = new TransformGroup();
                var tgDes = designItem.Services.Component.RegisterComponentForDesigner(tg);
                tgDes.ContentProperty.CollectionElements.Add(designItem.Services.Component.GetDesignItem(oldTransform));
                designItem.Properties.GetProperty(FrameworkElement.RenderTransformProperty).SetValue(tg);
                oldTransform = tg;
            }



            if (transform is RotateTransform)
            {
                var rotateTransform = transform as RotateTransform;

                if (oldTransform is RotateTransform || oldTransform == null)
                {
                    if (rotateTransform.Angle != 0)
                    {
                        designItem.Properties.GetProperty(transformProperty).SetValue(transform);
                        var angle = rotateTransform.Angle;
                        if (relative && oldTransform != null)
                        {
                            angle = rotateTransform.Angle + ((RotateTransform)oldTransform).Angle;
                        }
                        // TODO: 旋转的时候这里报错了
                        DesignItemProperty v = designItem.Properties.GetProperty(transformProperty);
                        DesignItem value = v.Value;
                        value.Properties.GetProperty(RotateTransform.AngleProperty).SetValue(angle);
                        if (rotateTransform.CenterX != 0.0)
                            designItem.Properties.GetProperty(transformProperty).Value.Properties.GetProperty(RotateTransform.CenterXProperty).SetValue(rotateTransform.CenterX);
                        if (rotateTransform.CenterY != 0.0)
                            designItem.Properties.GetProperty(transformProperty).Value.Properties.GetProperty(RotateTransform.CenterYProperty).SetValue(rotateTransform.CenterY);

                        if (oldTransform == null)
                            designItem.Properties.GetProperty(UIElement.RenderTransformOriginProperty).SetValue(new Point(0.5, 0.5));
                    }
                    else
                    {
                        designItem.Properties.GetProperty(transformProperty).Reset();
                        designItem.Properties.GetProperty(UIElement.RenderTransformOriginProperty).Reset();
                    }
                }
                else if (oldTransform is TransformGroup)
                {
                    var tg = oldTransform as TransformGroup;
                    var rot = tg.Children.FirstOrDefault(x => x is RotateTransform);
                    if (rot != null)
                    {
                        designItem.Services.Component.GetDesignItem(tg).ContentProperty.CollectionElements.Remove(designItem.Services.Component.GetDesignItem(rot));
                    }
                    if (rotateTransform.Angle != 0)
                    {
                        var des = designItem.Services.Component.GetDesignItem(transform);
                        if (des == null)
                            des = designItem.Services.Component.RegisterComponentForDesigner(transform);
                        designItem.Services.Component.GetDesignItem(tg).ContentProperty.CollectionElements.Add(des);
                        if (oldTransform == null)
                            designItem.Properties.GetProperty(UIElement.RenderTransformOriginProperty).SetValue(new Point(0.5, 0.5));
                    }
                }
                else
                {
                    if (rotateTransform.Angle != 0)
                    {
                        designItem.Properties.GetProperty(transformProperty).SetValue(transform);
                        if (oldTransform == null)
                            designItem.Properties.GetProperty(UIElement.RenderTransformOriginProperty).SetValue(new Point(0.5, 0.5));
                    }
                }
            }

            ((DesignPanel)designItem.Services.DesignPanel).AdornerLayer.UpdateAdornersForElement(designItem.View, true);

            //changeGroup.Commit();
        }

        public static void StretchItems(IEnumerable<DesignItem> items, StretchDirection stretchDirection)
        {
            IEnumerable<DesignItem> collection = items;

            DesignItem container = collection.First().Parent;

            if (collection.Any(x => x.Parent != container))
                return;

            IPlacementBehavior placement = container.Extensions.OfType<IPlacementBehavior>().FirstOrDefault();
            if (placement == null)
                return;

            //var changeGroup = container.OpenGroup("StretchItems");

            double w = GetWidth(collection.First().View);
            double h = GetHeight(collection.First().View);

            foreach (DesignItem item in collection.Skip(1))
            {
                switch (stretchDirection)
                {
                    case StretchDirection.Width:
                        {
                            if (!double.IsNaN(w))
                                item.Properties.GetProperty(FrameworkElement.WidthProperty).SetValue(w);
                        }
                        break;
                    case StretchDirection.Height:
                        {
                            if (!double.IsNaN(h))
                                item.Properties.GetProperty(FrameworkElement.HeightProperty).SetValue(h);
                        }
                        break;
                }
            }

            //changeGroup.Commit();
        }

        public static void ArrangeItems(IEnumerable<DesignItem> items, ArrangeDirection arrangeDirection)
        {
            IEnumerable<DesignItem> collection = items;

            MyDesignContext _context = collection.First().Context as MyDesignContext;

            DesignItem container = collection.First().Parent;

            if (collection.Any(x => x.Parent != container))
                return;

            IPlacementBehavior placement = container.Extensions.OfType<IPlacementBehavior>().FirstOrDefault();
            if (placement == null)
                return;

            PlacementOperation operation = PlacementOperation.Start(items.ToList(), PlacementType.Move);

            List<ItemPos> itemList = new();
            foreach (DesignItem item in collection)
            {
                itemList.Add(GetItemPos(operation, item));
            }

            double xmin = itemList.Min(x => x.Xmin);
            double xmax = itemList.Max(x => x.Xmax);
            double mpos = (xmax - xmin) / 2 + xmin;
            double ymin = itemList.Min(x => x.Ymin);
            double ymax = itemList.Max(x => x.Ymax);
            double ympos = (ymax - ymin) / 2 + ymin;

            foreach (DesignItem item in collection)
            {
                switch (arrangeDirection)
                {
                    case ArrangeDirection.Left:
                        {
                            if (container.Component is Canvas)
                            {
                                if (!item.Properties.GetAttachedProperty(Canvas.RightProperty).IsSet)
                                {
                                    item.Properties.GetAttachedProperty(Canvas.LeftProperty).SetValue(xmin);
                                }
                                else
                                {
                                    double pos = (double)((Panel)item.Parent.Component).ActualWidth - (xmin + (double)((FrameworkElement)item.Component).ActualWidth);
                                    item.Properties.GetAttachedProperty(Canvas.RightProperty).SetValue(pos);
                                }
                            }
                            else if (container.Component is Grid)
                            {
                                if ((HorizontalAlignment)item.Properties.GetProperty(FrameworkElement.HorizontalAlignmentProperty).ValueOnInstance != HorizontalAlignment.Right)
                                {
                                    Thickness margin = (Thickness)item.Properties.GetProperty(FrameworkElement.MarginProperty).ValueOnInstance;
                                    margin.Left = xmin;
                                    item.Properties.GetProperty(FrameworkElement.MarginProperty).SetValue(margin);
                                }
                                else
                                {
                                    double pos = (double)((Panel)item.Parent.Component).ActualWidth - (xmin + (double)((FrameworkElement)item.Component).ActualWidth);
                                    Thickness margin = (Thickness)item.Properties.GetProperty(FrameworkElement.MarginProperty).ValueOnInstance;
                                    margin.Right = pos;
                                    item.Properties.GetProperty(FrameworkElement.MarginProperty).SetValue(margin);
                                }
                            }
                        }
                        break;
                    case ArrangeDirection.HorizontalMiddle:
                        {
                            if (container.Component is Canvas)
                            {
                                if (!item.Properties.GetAttachedProperty(Canvas.RightProperty).IsSet)
                                {
                                    if (!item.Properties.GetAttachedProperty(Canvas.RightProperty).IsSet)
                                    {
                                        item.Properties.GetAttachedProperty(Canvas.LeftProperty).SetValue(mpos - ((FrameworkElement)item.Component).ActualWidth / 2);
                                    }
                                    else
                                    {
                                        double pp = mpos - ((FrameworkElement)item.Component).ActualWidth / 2;
                                        double pos = (double)((Panel)item.Parent.Component).ActualWidth - pp - ((FrameworkElement)item.Component).ActualWidth;
                                        item.Properties.GetAttachedProperty(Canvas.RightProperty).SetValue(pos);
                                    }
                                }
                            }
                            else if (container.Component is Grid)
                            {
                                if ((HorizontalAlignment)item.Properties.GetProperty(FrameworkElement.HorizontalAlignmentProperty).ValueOnInstance != HorizontalAlignment.Right)
                                {
                                    Thickness margin = (Thickness)item.Properties.GetProperty(FrameworkElement.MarginProperty).ValueOnInstance;
                                    margin.Left = mpos - ((FrameworkElement)item.Component).ActualWidth / 2;
                                    item.Properties.GetProperty(FrameworkElement.MarginProperty).SetValue(margin);
                                }
                                else
                                {
                                    double pp = mpos - ((FrameworkElement)item.Component).ActualWidth / 2;
                                    double pos = (double)((Panel)item.Parent.Component).ActualWidth - pp - ((FrameworkElement)item.Component).ActualWidth;
                                    Thickness margin = (Thickness)item.Properties.GetProperty(FrameworkElement.MarginProperty).ValueOnInstance;
                                    margin.Right = pos;
                                    item.Properties.GetProperty(FrameworkElement.MarginProperty).SetValue(margin);
                                }
                            }
                        }
                        break;
                    case ArrangeDirection.Right:
                        {
                            if (container.Component is Canvas)
                            {
                                if (!item.Properties.GetAttachedProperty(Canvas.RightProperty).IsSet)
                                {
                                    double pos = xmax - (double)((FrameworkElement)item.Component).ActualWidth;
                                    item.Properties.GetAttachedProperty(Canvas.LeftProperty).SetValue(pos);
                                }
                                else
                                {
                                    double pos = (double)((Panel)item.Parent.Component).ActualWidth - xmax;
                                    item.Properties.GetAttachedProperty(Canvas.RightProperty).SetValue(pos);
                                }
                            }
                            else if (container.Component is Grid)
                            {
                                if ((HorizontalAlignment)item.Properties.GetProperty(FrameworkElement.HorizontalAlignmentProperty).ValueOnInstance != HorizontalAlignment.Right)
                                {
                                    double pos = xmax - (double)((FrameworkElement)item.Component).ActualWidth;
                                    Thickness margin = (Thickness)item.Properties.GetProperty(FrameworkElement.MarginProperty).ValueOnInstance;
                                    margin.Left = pos;
                                    item.Properties.GetProperty(FrameworkElement.MarginProperty).SetValue(margin);
                                }
                                else
                                {
                                    double pos = (double)((Panel)item.Parent.Component).ActualWidth - xmax;
                                    Thickness margin = (Thickness)item.Properties.GetProperty(FrameworkElement.MarginProperty).ValueOnInstance;
                                    margin.Right = pos;
                                    item.Properties.GetProperty(FrameworkElement.MarginProperty).SetValue(margin);
                                }
                            }
                        }
                        break;
                    case ArrangeDirection.Top:
                        {
                            if (container.Component is Canvas)
                            {
                                if (!item.Properties.GetAttachedProperty(Canvas.BottomProperty).IsSet)
                                {
                                    item.Properties.GetAttachedProperty(Canvas.TopProperty).SetValue(ymin);
                                }
                                else
                                {
                                    double pos = (double)((Panel)item.Parent.Component).ActualHeight - (ymin + (double)((FrameworkElement)item.Component).ActualHeight);
                                    item.Properties.GetAttachedProperty(Canvas.BottomProperty).SetValue(pos);
                                }
                            }
                            else if (container.Component is Grid)
                            {
                                if ((VerticalAlignment)item.Properties.GetProperty(FrameworkElement.VerticalAlignmentProperty).ValueOnInstance != VerticalAlignment.Bottom)
                                {
                                    item.Properties.GetAttachedProperty(Canvas.TopProperty).SetValue(ymin);
                                    Thickness margin = (Thickness)item.Properties.GetProperty(FrameworkElement.MarginProperty).ValueOnInstance;
                                    margin.Top = ymin;
                                    item.Properties.GetProperty(FrameworkElement.MarginProperty).SetValue(margin);
                                }
                                else
                                {
                                    double pos = (double)((Panel)item.Parent.Component).ActualHeight - (ymin + (double)((FrameworkElement)item.Component).ActualHeight);
                                    Thickness margin = (Thickness)item.Properties.GetProperty(FrameworkElement.MarginProperty).ValueOnInstance;
                                    margin.Bottom = pos;
                                    item.Properties.GetProperty(FrameworkElement.MarginProperty).SetValue(margin);
                                }
                            }
                        }
                        break;
                    case ArrangeDirection.VerticalMiddle:
                        {
                            if (container.Component is Canvas)
                            {
                                if (!item.Properties.GetAttachedProperty(Canvas.BottomProperty).IsSet)
                                {
                                    item.Properties.GetAttachedProperty(Canvas.TopProperty).SetValue(ympos - ((FrameworkElement)item.Component).ActualHeight / 2);
                                }
                                else
                                {
                                    double pp = mpos - ((FrameworkElement)item.Component).ActualHeight / 2;
                                    double pos = (double)((Panel)item.Parent.Component).ActualHeight - pp - ((FrameworkElement)item.Component).ActualHeight;
                                    item.Properties.GetAttachedProperty(Canvas.BottomProperty).SetValue(pos);
                                }
                            }
                            else if (container.Component is Grid)
                            {
                                if ((VerticalAlignment)item.Properties.GetProperty(FrameworkElement.VerticalAlignmentProperty).ValueOnInstance != VerticalAlignment.Bottom)
                                {
                                    Thickness margin = (Thickness)item.Properties.GetProperty(FrameworkElement.MarginProperty).ValueOnInstance;
                                    margin.Top = ympos - ((FrameworkElement)item.Component).ActualHeight / 2;
                                    item.Properties.GetProperty(FrameworkElement.MarginProperty).SetValue(margin);
                                }
                                else
                                {
                                    double pp = mpos - ((FrameworkElement)item.Component).ActualHeight / 2;
                                    double pos = (double)((Panel)item.Parent.Component).ActualHeight - pp - ((FrameworkElement)item.Component).ActualHeight;
                                    Thickness margin = (Thickness)item.Properties.GetProperty(FrameworkElement.MarginProperty).ValueOnInstance;
                                    margin.Bottom = pos;
                                    item.Properties.GetProperty(FrameworkElement.MarginProperty).SetValue(margin);
                                }
                            }
                        }
                        break;
                    case ArrangeDirection.Bottom:
                        {
                            if (container.Component is Canvas)
                            {
                                if (!item.Properties.GetAttachedProperty(Canvas.BottomProperty).IsSet)
                                {
                                    double pos = ymax - (double)((FrameworkElement)item.Component).ActualHeight;
                                    item.Properties.GetAttachedProperty(Canvas.TopProperty).SetValue(pos);
                                }
                                else
                                {
                                    double pos = (double)((Panel)item.Parent.Component).ActualHeight - ymax;
                                    item.Properties.GetAttachedProperty(Canvas.BottomProperty).SetValue(pos);
                                }
                            }
                            else if (container.Component is Grid)
                            {
                                if ((VerticalAlignment)item.Properties.GetProperty(FrameworkElement.VerticalAlignmentProperty).ValueOnInstance != VerticalAlignment.Bottom)
                                {
                                    double pos = ymax - (double)((FrameworkElement)item.Component).ActualHeight;
                                    Thickness margin = (Thickness)item.Properties.GetProperty(FrameworkElement.MarginProperty).ValueOnInstance;
                                    margin.Top = pos;
                                    item.Properties.GetProperty(FrameworkElement.MarginProperty).SetValue(margin);
                                }
                                else
                                {
                                    double pos = (double)((Panel)item.Parent.Component).ActualHeight - ymax;
                                    Thickness margin = (Thickness)item.Properties.GetProperty(FrameworkElement.MarginProperty).ValueOnInstance;
                                    margin.Bottom = pos;
                                    item.Properties.GetProperty(FrameworkElement.MarginProperty).SetValue(margin);
                                }
                            }
                        }
                        break;
                }
            }

            operation.Commit();
        }

        //		public static class Path {
        //			
        //			public static PathGeometry ConvertToPathGeometry(this TextBlock textBlock)
        //			{
        //				//var ft = new FormatedText();
        //				return null;
        //			}
        //			
        //			public static PathGeometry ConvertToPathGeometry(this Rectangle rectangle)
        //			{
        //				return null;
        //			}
        //			
        //			public static PathGeometry ConvertToPathGeometry(this Ellipse ellipse)
        //			{
        //				return null;
        //			}
        //		}
    }
}
