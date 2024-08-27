using Hjmos.Lcdp.VisualEditor.Controls.Adorners;
using Hjmos.Lcdp.VisualEditor.Controls.Extensions2;

namespace Hjmos.Lcdp.VisualEditor.Controls.Thumbs
{
    internal sealed class MultiPointThumb : DesignerThumb
    {
        private int _index;

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

        public AdornerPlacement AdornerPlacement { get; set; }
    }
}
