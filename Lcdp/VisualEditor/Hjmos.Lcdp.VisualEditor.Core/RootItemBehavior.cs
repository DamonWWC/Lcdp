using System;
using System.Windows;
using System.Collections.Generic;
using System.Diagnostics;

namespace Hjmos.Lcdp.VisualEditor.Core
{
    /// <summary>
	/// 初始化根项目的不同行为。
	/// <remarks>不能是一个扩展，因为根项目可以是任何类型</remarks>
    /// </summary>
    public class RootItemBehavior : IRootPlacementBehavior
    {
        private DesignItem _rootItem;

        public void Intialize(DesignContext context)
        {
            Debug.Assert(context.RootItem != null);
            this._rootItem = context.RootItem;
            _rootItem.AddBehavior(typeof(IRootPlacementBehavior), this);
        }

        public bool CanPlace(IEnumerable<DesignItem> childItems, PlacementType type, PlacementAlignment position)
        {
            return type == PlacementType.Resize && (position == PlacementAlignment.Right || position == PlacementAlignment.BottomRight || position == PlacementAlignment.Bottom);
        }

        public void BeginPlacement(PlacementOperation operation)
        {
        }

        public void EndPlacement(PlacementOperation operation)
        {
        }

        public Rect GetPosition(PlacementOperation operation, DesignItem childItem)
        {
            UIElement child = childItem.View;
            return new Rect(0, 0, ModelTools.GetWidth(child), ModelTools.GetHeight(child));
        }

        public void BeforeSetPosition(PlacementOperation operation) { }

        public void SetPosition(PlacementInformation info)
        {
            UIElement element = info.Item.View;
            Rect newPosition = info.Bounds;
            if (newPosition.Right != ModelTools.GetWidth(element))
            {
                info.Item.Properties[FrameworkElement.WidthProperty].SetValue(newPosition.Right);
            }
            if (newPosition.Bottom != ModelTools.GetHeight(element))
            {
                info.Item.Properties[FrameworkElement.HeightProperty].SetValue(newPosition.Bottom);
            }
        }

        public bool CanLeaveContainer(PlacementOperation operation) => false;

        public void LeaveContainer(PlacementOperation operation) => throw new NotImplementedException();

        public bool CanEnterContainer(PlacementOperation operation, bool shouldAlwaysEnter) => false;

        public void EnterContainer(PlacementOperation operation) => throw new NotImplementedException();

        public Point PlacePoint(Point point) => point;
    }
}
