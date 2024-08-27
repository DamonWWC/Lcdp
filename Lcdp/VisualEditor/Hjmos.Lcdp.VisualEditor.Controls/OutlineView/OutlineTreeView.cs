using Hjmos.Lcdp.VisualEditor.Controls.UIExtensions;
using System.Collections.Generic;

namespace Hjmos.Lcdp.VisualEditor.Controls.OutlineView
{
    public class OutlineTreeView : DragTreeView
    {
        protected override bool CanInsert(DragTreeViewItem target, DragTreeViewItem[] items, DragTreeViewItem after, bool copy)
        {
            UpdateCustomNodes(items);
            return (target.DataContext as IOutlineNode).CanInsert(_customOutlineNodes, after == null ? null : after.DataContext as IOutlineNode, copy);
        }

        protected override void Insert(DragTreeViewItem target, DragTreeViewItem[] items, DragTreeViewItem after, bool copy)
        {
            UpdateCustomNodes(items);
            (target.DataContext as IOutlineNode).Insert(_customOutlineNodes, after == null ? null : after.DataContext as IOutlineNode, copy);
        }

        // Need to do this through a seperate List since previously LINQ queries apparently disconnected DataContext;bug in .NET 4.0
        private List<IOutlineNode> _customOutlineNodes;

        private void UpdateCustomNodes(IEnumerable<DragTreeViewItem> items)
        {
            _customOutlineNodes = new List<IOutlineNode>();
            foreach (DragTreeViewItem item in items)
                _customOutlineNodes.Add(item.DataContext as IOutlineNode);
        }

        public override bool ShouldItemBeVisible(DragTreeViewItem dragTreeViewitem)
        {
            IOutlineNode node = dragTreeViewitem.DataContext as IOutlineNode;
            return string.IsNullOrEmpty(Filter) || node.Services.GetService<IOutlineNodeNameService>().GetOutlineNodeName(node.DesignItem).ToLower().Contains(Filter.ToLower());
        }

        protected override void SelectOnly(DragTreeViewItem item)
        {
            base.SelectOnly(item);

            IOutlineNode node = item.DataContext as IOutlineNode;

            if (node.DesignItem != null)
            {
                DesignSurface surface = node.DesignItem.View.TryFindParent<DesignSurface>();
                // TODO:暂时注释
                //if (surface != null)
                //    surface.ScrollIntoView(node.DesignItem);
            }
        }
    }
}
