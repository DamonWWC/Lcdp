using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Core
{
    /// <summary>
    /// 存储关于放置操作的数据。
    /// </summary>
    public sealed class PlacementOperation
    {
        /// <summary>插入组件偏移量</summary>
        public const double PasteOffset = 10;

        #region Properties
        /// <summary>将要被放置的项</summary>
        public ReadOnlyCollection<PlacementInformation> PlacedItems { get; }

        /// <summary>放置的类型</summary>
        public PlacementType Type { get; }

        /// <summary>获取放置操作是否已中止</summary>
        public bool IsAborted { get; private set; }

        /// <summary>获取放置操作是否已提交</summary>
        public bool IsCommitted { get; private set; }

        /// <summary>获取放置操作的当前容器</summary>
        public DesignItem CurrentContainer { get; private set; }

        /// <summary>获取当前容器的放置行为</summary>
        public IPlacementBehavior CurrentContainerBehavior { get; private set; }

        #endregion

        #region Container changing

        /// <summary>
        /// 更换容器。此方法假设您已经检查了是否可以更改容器。
        /// </summary>
        public void ChangeContainer(DesignItem newContainer)
        {
            if (newContainer == null)
                throw new ArgumentNullException("newContainer");

            if (IsAborted || IsCommitted)
                throw new PlacementOperationException("该操作不再运行");

            if (CurrentContainer == newContainer)
                return;

            if (!CurrentContainerBehavior.CanLeaveContainer(this))
                throw new PlacementOperationException("不能从它们的父容器中删除项");

            try
            {
                CurrentContainerBehavior.LeaveContainer(this);

                GeneralTransform transform = CurrentContainer.View.TransformToVisual(newContainer.View);

                foreach (PlacementInformation info in PlacedItems)
                {
                    info.OriginalBounds = TransformRectByMiddlePoint(transform, info.OriginalBounds);
                    info.Bounds = TransformRectByMiddlePoint(transform, info.Bounds).Round();
                }

                CurrentContainer = newContainer;
                CurrentContainerBehavior = newContainer.GetBehavior<IPlacementBehavior>();

                Debug.Assert(CurrentContainerBehavior != null);
                CurrentContainerBehavior.EnterContainer(this);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                //Abort();
                throw;
            }
        }

        private static Rect TransformRectByMiddlePoint(GeneralTransform transform, Rect r)
        {
            // 我们不想在将控件移出缩放容器时调整它的大小，我们只想正确地移动它
            Point p = new(r.Left + r.Width / 2, r.Top + r.Height / 2);
            Vector movement = transform.Transform(p) - p;
            return new Rect(r.TopLeft + movement, r.Size);
        }

        #endregion

        #region Delete Items

        /// <summary>
        /// 删除正在放置的项，并提交放置操作。
        /// </summary>
        public void DeleteItemsAndCommit()
        {
            if (IsAborted || IsCommitted)
                throw new PlacementOperationException("该操作不再运行");

            if (!CurrentContainerBehavior.CanLeaveContainer(this))
                throw new PlacementOperationException("不能从它们父容器中删除项");

            CurrentContainerBehavior.LeaveContainer(this);
            //Commit();
        }
        #endregion

        #region Start

        /// <summary>
        /// 启动一个新的放置操作，更改<paramref name="placedItems"/>的放置。  
        /// </summary>
        /// <param name="placedItems">要放置的项</param>
        /// <param name="type">描述如何完成放置的类</param>
        /// <returns>一个PlacementOperation对象。</returns>
        /// <remarks>
        /// 一旦你完成了返回的PlacementOperation，你必须调用<see cref="Abort"/>或<see cref="Commit"/>，否则ChangeGroup将保持打开状态，撤销/重做将失败!  
        /// </remarks>
        public static PlacementOperation Start(ICollection<DesignItem> placedItems, PlacementType type)
        {
            if (placedItems == null)
                throw new ArgumentNullException("placedItems");
            if (type == null)
                throw new ArgumentNullException("type");

            DesignItem[] items = placedItems.ToArray();
            if (items.Length == 0)
                throw new ArgumentException("placedItems.Length 必须大于0");

            PlacementOperation op = new(items, type);
            try
            {
                if (op.CurrentContainerBehavior == null)
                    throw new PlacementOperationException("不支持启动该操作");

                op.CurrentContainerBehavior.BeginPlacement(op);
                foreach (PlacementInformation info in op.PlacedItems)
                {
                    info.OriginalBounds = op.CurrentContainerBehavior.GetPosition(op, info.Item);
                    info.Bounds = info.OriginalBounds;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                //op.changeGroup.Abort();
                throw;
            }
            return op;
        }

        private PlacementOperation(DesignItem[] items, PlacementType type)
        {
            this.CurrentContainerBehavior = GetPlacementBehavior(items, out List<DesignItem> moveableItems, type);

            PlacementInformation[] information = new PlacementInformation[moveableItems.Count];
            for (int i = 0; i < information.Length; i++)
            {
                information[i] = new PlacementInformation(moveableItems[i], this);
            }
            this.PlacedItems = new ReadOnlyCollection<PlacementInformation>(information);
            this.Type = type;

            this.CurrentContainer = moveableItems[0].Parent;

            //this.changeGroup = moveableItems[0].Context.OpenGroup(type.ToString(), moveableItems);
        }

        /// <summary>
        /// 元素真正应该拥有的尺寸(即使其渲染较小(如空图像!))  
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static Size GetRealElementSize(UIElement element)
        {
            Size size = element.RenderSize;

            if (element is FrameworkElement ele)
            {
                if (!double.IsNaN(ele.Width))
                    size.Width = ele.Width;

                if (!double.IsNaN(ele.Height))
                    size.Height = ele.Height;

                if (size.Width < ele.MinWidth)
                    size.Width = ele.MinWidth;

                if (size.Height < ele.MinHeight)
                    size.Height = ele.MinHeight;
            }

            return size;
        }

        /// <summary>
        /// 获取与指定项关联的放置行为。  
        /// </summary>
        public static IPlacementBehavior GetPlacementBehavior(ICollection<DesignItem> items) => GetPlacementBehavior(items, out _, PlacementType.Move);

        /// <summary>
        /// 获取与指定项关联的放置行为。 
        /// </summary>
        public static IPlacementBehavior GetPlacementBehavior(ICollection<DesignItem> items, out List<DesignItem> moveableItems, PlacementType placementType)
        {
            moveableItems = new List<DesignItem>();

            if (items == null)
                throw new ArgumentNullException("items");
            if (items.Count == 0)
                return null;

            ICollection<DesignItem> possibleItems = items;
            if (!items.Any(x => x.Parent == null))
            {
                IEnumerable<IGrouping<DesignItem, DesignItem>> itemsPartentGroup = items.GroupBy(x => x.Parent);
                DesignItem parents = itemsPartentGroup.Select(x => x.Key).OrderBy(x => x.DepthLevel).First();
                possibleItems = itemsPartentGroup.Where(x => x.Key.DepthLevel == parents.DepthLevel).SelectMany(x => x).ToList();
            }

            DesignItem first = possibleItems.First();
            DesignItem parent = first.Parent;
            moveableItems.Add(first);
            foreach (DesignItem item in possibleItems.Skip(1))
            {
                if (item.Parent != parent)
                {
                    if (placementType != PlacementType.MoveAndIgnoreOtherContainers)
                    {
                        return null;
                    }
                }
                else
                    moveableItems.Add(item);
            }
            if (parent != null)
                return parent.GetBehavior<IPlacementBehavior>();
            else if (possibleItems.Count == 1)
                return first.GetBehavior<IRootPlacementBehavior>();
            else
                return null;
        }
        #endregion

        #region StartInsertNewComponents
        /// <summary>
        /// 尝试将新组件插入到容器中。
        /// </summary>
        /// <param name="container">应该作为组件的父组件的容器。</param>
        /// <param name="placedItems">要添加到容器中的组件。</param>
        /// <param name="positions">指定元素应该获得的位置的矩形。</param>
        /// <param name="type">类型</param>
        /// <returns>插入新组件的操作，如果不能插入则为空。</returns>
        public static PlacementOperation TryStartInsertNewComponents(DesignItem container, IList<DesignItem> placedItems, IList<Rect> positions, PlacementType type)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (placedItems == null)
                throw new ArgumentNullException("placedItems");
            if (positions == null)
                throw new ArgumentNullException("positions");
            if (type == null)
                throw new ArgumentNullException("type");
            if (placedItems.Count == 0)
                throw new ArgumentException("placedItems.Count 必须大于0");
            if (placedItems.Count != positions.Count)
                throw new ArgumentException("positions.Count 必须等于 placedItems.Count");

            DesignItem[] items = placedItems.ToArray();

            PlacementOperation op = new(items, type);
            try
            {
                for (int i = 0; i < items.Length; i++)
                {
                    op.PlacedItems[i].OriginalBounds = op.PlacedItems[i].Bounds = positions[i];
                }
                op.CurrentContainer = container;
                op.CurrentContainerBehavior = container.GetBehavior<IPlacementBehavior>();
                if (op.CurrentContainerBehavior == null || !op.CurrentContainerBehavior.CanEnterContainer(op, true))
                {
                    //op.changeGroup.Abort();
                    return null;
                }
                op.CurrentContainerBehavior.EnterContainer(op);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                //op.changeGroup.Abort();
                throw;
            }
            return op;
        }
        #endregion

        #region ChangeGroup handling

        /// <summary>
        /// Gets/Sets the description of the underlying change group.
        /// </summary>
        //public string Description
        //{
        //    get { return changeGroup.Title; }
        //    set { changeGroup.Title = value; }
        //}

        /// <summary>
        /// Aborts the operation.
        /// This aborts the underlying change group, reverting all changes done while the operation was running.
        /// </summary>
        public void Abort()
        {
            if (!IsAborted)
            {
                if (IsCommitted)
                    throw new PlacementOperationException("PlacementOperation 已提交");
                IsAborted = true;
                CurrentContainerBehavior.EndPlacement(this);
                //changeGroup.Abort();
            }
        }

        /// <summary>
        /// Commits the operation.
        /// This commits the underlying change group.
        /// </summary>
        public void Commit()
        {
            if (IsAborted || IsCommitted)
                throw new PlacementOperationException("PlacementOperation 已经终止/提交");
            IsCommitted = true;
            CurrentContainerBehavior.EndPlacement(this);
            //changeGroup.Commit();
        }
        #endregion
    }

    /// <summary>
    /// 放置过程中可能发生的异常
    /// </summary>
    public class PlacementOperationException : InvalidOperationException
    {
        /// <summary>
        /// 放置异常的构造函数
        /// </summary>
        /// <param name="message"></param>
        public PlacementOperationException(string message) : base(message) { }
    }
}
