using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    [ExtensionFor(typeof(object))]
    public class OnlyDeletePlacementBehavior : BehaviorExtension, IPlacementBehavior
    {
        static OnlyDeletePlacementBehavior() { }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (ExtendedItem.Component is Panel || ExtendedItem.Component is Control || ExtendedItem.Component is Border || ExtendedItem.Component is Viewbox || ExtendedItem.Component is TextBlock)
                return;

            ExtendedItem.AddBehavior(typeof(IPlacementBehavior), this);
        }

        public virtual bool CanPlace(IEnumerable<DesignItem> childItems, PlacementType type, PlacementAlignment position) => type == PlacementType.Delete;

        public virtual void BeginPlacement(PlacementOperation operation) { }

        public virtual void EndPlacement(PlacementOperation operation) { }

        public virtual Rect GetPosition(PlacementOperation operation, DesignItem item) => new();

        public Rect GetPositionRelativeToContainer(PlacementOperation operation, DesignItem item) => new();

        public virtual void BeforeSetPosition(PlacementOperation operation) { }

        public virtual bool CanPlaceItem(PlacementInformation info) => false;

        public virtual void SetPosition(PlacementInformation info) { }

        public virtual bool CanLeaveContainer(PlacementOperation operation) => true;

        public virtual void LeaveContainer(PlacementOperation operation)
        {
            foreach (PlacementInformation item in operation.PlacedItems)
            {
                if (item.Item.ParentProperty.IsCollection)
                {
                    item.Item.ParentProperty.CollectionElements.Remove(item.Item);
                }
                else
                {
                    item.Item.ParentProperty.Reset();
                }
            }
        }

        private static readonly InfoTextEnterArea infoTextEnterArea;

        public virtual bool CanEnterContainer(PlacementOperation operation, bool shouldAlwaysEnter) => false;

        public virtual void EnterContainer(PlacementOperation operation) { }

        public virtual Point PlacePoint(Point point) => new Point();
    }
}
