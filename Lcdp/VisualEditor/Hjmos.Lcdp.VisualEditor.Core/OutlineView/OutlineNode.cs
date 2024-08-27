using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Hjmos.Lcdp.VisualEditor.Core.OutlineView
{
    public class OutlineNode : OutlineNodeBase
    {
        //TODO: Reset with DesignContext
        static Dictionary<DesignItem, IOutlineNode> outlineNodes = new Dictionary<DesignItem, IOutlineNode>();

        protected OutlineNode(DesignItem designitem) : base(designitem)
        {
            UpdateChildren();
            SelectionService.SelectionChanged += new EventHandler<DesignItemCollectionEventArgs>(Selection_SelectionChanged);
        }

        protected OutlineNode(string name) : base(name) { }

        static OutlineNode() => DummyPlacementType = PlacementType.Register("DummyPlacement");

        public static IOutlineNode Create(DesignItem designItem)
        {
            IOutlineNode node = null;
            if (designItem != null && !outlineNodes.TryGetValue(designItem, out node))
            {
                node = new OutlineNode(designItem);
                outlineNodes[designItem] = node;
            }
            return node;
        }

        private void Selection_SelectionChanged(object sender, DesignItemCollectionEventArgs e)
        {
            IsSelected = DesignItem.Services.Selection.IsComponentSelected(DesignItem);
        }


        protected override void UpdateChildren()
        {
            Children.Clear();

            foreach (DesignItemProperty prp in DesignItem.AllSetProperties)
            {
                if (prp.Name != DesignItem.ContentPropertyName)
                {
                    if (prp.Value != null)
                    {
                        IOutlineNode propertyNode = PropertyOutlineNode.Create(prp);
                        IOutlineNode node = Create(prp.Value);
                        propertyNode.Children.Add(node);
                        Children.Add(propertyNode);
                    }
                }
            }
            if (DesignItem.ContentPropertyName != null)
            {
                DesignItemProperty content = DesignItem.ContentProperty;
                if (content.IsCollection)
                {
                    UpdateChildrenCore(content.CollectionElements);
                }
                else
                {
                    if (content.Value != null)
                    {
                        if (!UpdateChildrenCore(new[] { content.Value }))
                        {
                            IOutlineNode propertyNode = PropertyOutlineNode.Create(content);
                            IOutlineNode node = Create(content.Value);
                            propertyNode.Children.Add(node);
                            Children.Add(propertyNode);
                        }
                    }
                }
            }
        }

        protected override void UpdateChildrenCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (object oldItem in e.OldItems)
                {
                    IOutlineNode item = Children.FirstOrDefault(x => x.DesignItem == oldItem);
                    if (item != null)
                    {
                        Children.Remove(item);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                UpdateChildrenCore(e.NewItems.Cast<DesignItem>(), e.NewStartingIndex);
            }
        }

        private bool UpdateChildrenCore(IEnumerable<DesignItem> items, int index = -1)
        {
            bool retVal = false;
            foreach (DesignItem item in items)
            {
                if (ModelTools.CanSelectComponent(item))
                {
                    if (Children.All(x => x.DesignItem != item))
                    {
                        IOutlineNode node = Create(item);
                        if (index > -1)
                        {
                            Children.Insert(index++, node);
                            retVal = true;
                        }
                        else
                        {
                            Children.Add(node);
                            retVal = true;
                        }
                    }
                }
                else
                {
                    DesignItemProperty content = item.ContentProperty;
                    if (content != null)
                    {
                        if (content.IsCollection)
                        {
                            UpdateChildrenCore(content.CollectionElements);
                            retVal = true;
                        }
                        else
                        {
                            if (content.Value != null)
                            {
                                UpdateChildrenCore(new[] { content.Value });
                                retVal = true;
                            }
                        }
                    }
                }
            }

            return retVal;
        }
    }
}
