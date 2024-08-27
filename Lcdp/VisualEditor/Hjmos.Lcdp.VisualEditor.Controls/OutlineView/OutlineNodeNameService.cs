namespace Hjmos.Lcdp.VisualEditor.Controls.OutlineView
{
    public class OutlineNodeNameService : IOutlineNodeNameService
    {
        public OutlineNodeNameService() { }


        public string GetOutlineNodeName(DesignItem designItem)
        {
            if (designItem == null)
                return "";

            return string.IsNullOrEmpty(designItem.Name) ? designItem.ComponentType.Name : designItem.ComponentType.Name + " (" + designItem.Name + ")";
        }
    }
}
