using Hjmos.Lcdp.Plugins.CS6.DataFields;
using Hjmos.Lcdp.VisualEditor.Core.Attributes;

namespace Hjmos.Lcdp.Plugins.CS6.Views
{
    [Widget(Category = "长沙6组件", DisplayName = "节标题", DefaultWidth = 800d, DefaultHeight = 40d)]
    [DataFields(typeof(SectionHeaderDataFields))]
    public partial class SectionHeader
    {
        public SectionHeader()
        {
            InitializeComponent();
        }
    }
}
