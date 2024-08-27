using Hjmos.Lcdp.Plugins.CS6.DataFields;
using Hjmos.Lcdp.VisualEditor.Core.Attributes;

namespace Hjmos.Lcdp.Plugins.CS6.Views
{
    [Widget(Category = "长沙6组件", DisplayName = "标签指标", DefaultWidth = 800d, DefaultHeight = 256d)]
    //[DataFields(typeof(SectionHeaderDataFields))]
    public partial class LabelIndicators
    {
        public LabelIndicators()
        {
            InitializeComponent();
        }
    }
}
