namespace Hjmos.Lcdp.VisualEditor.Controls.OutlineView
{
    public class PropertyOutlineNode : OutlineNode
    {
        private readonly DesignItemProperty _property;

        protected PropertyOutlineNode(DesignItemProperty property) : base(property.Name) => _property = property;

        public override ServiceContainer Services => this._property.DesignItem.Services;

        static PropertyOutlineNode() => DummyPlacementType = PlacementType.Register("DummyPlacement");

        public static IOutlineNode Create(DesignItemProperty property) => new PropertyOutlineNode(property);
    }
}
