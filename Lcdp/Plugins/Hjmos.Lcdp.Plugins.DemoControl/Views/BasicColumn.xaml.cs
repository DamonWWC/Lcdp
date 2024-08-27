using Hjmos.Lcdp.Plugins.DemoControl.DataFields;
using Hjmos.Lcdp.VisualEditor.Core.Attributes;

namespace Hjmos.Lcdp.Plugins.DemoControl.Views
{
    [Widget(Category = "示例组件", DisplayName = "柱状图", DefaultWidth = 500d)]
    [DataFields(typeof(BasicColumnDataFields))]
    public partial class BasicColumn
    {
        public BasicColumn()
        {
            InitializeComponent();
        }
    }
}
