using Hjmos.Lcdp.VisualEditor.Core.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Hjmos.Lcdp.VisualEditor.Core.OutlineView
{
    public interface IOutlineNode
    {
        ISelectionService SelectionService { get; }
        bool IsExpanded { get; set; }
        DesignItem DesignItem { get; set; }
        ServiceContainer Services { get; }
        bool IsSelected { get; set; }
        bool IsDesignTimeVisible { get; set; }
        bool IsDesignTimeLocked { get; }
        string Name { get; }
        bool CanInsert(IEnumerable<IOutlineNode> nodes, IOutlineNode after, bool copy);
        void Insert(IEnumerable<IOutlineNode> nodes, IOutlineNode after, bool copy);
        ObservableCollection<IOutlineNode> Children { get; }
    }
}
