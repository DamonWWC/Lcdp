using Hjmos.Lcdp.VisualEditor.Core.Adorners;
using Hjmos.Lcdp.VisualEditor.Core.ItemExtensions;

namespace Hjmos.Lcdp.VisualEditor.Core.Thumbs
{
    internal sealed class MultiPointThumb : DesignerThumb
    {
        
        public int Index
        {
            get => _index;
            set
            {
                _index = value;
                if (AdornerPlacement is PointTrackerPlacementSupport p)
                    p.Index = value;
            }
        }
        private int _index;

        public AdornerPlacement AdornerPlacement { get; set; }
    }
}
