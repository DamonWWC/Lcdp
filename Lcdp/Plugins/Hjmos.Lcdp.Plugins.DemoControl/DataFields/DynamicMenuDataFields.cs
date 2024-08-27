using Hjmos.Lcdp.VisualEditor.Core.CustomTypes;
using Hjmos.Lcdp.VisualEditor.Core.BaseClass;

namespace Hjmos.Lcdp.Plugins.DemoControl.DataFields
{
    public class DynamicMenuDataFields : DataFieldsBase
    {
        public string KeyField { get; set; } = "First Page,Second Page";

        public TextArea ValueField { get; set; }
    }
}
