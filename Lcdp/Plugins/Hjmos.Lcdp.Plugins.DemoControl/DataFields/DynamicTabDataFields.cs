using Hjmos.Lcdp.VisualEditor.Core.BaseClass;

namespace Hjmos.Lcdp.Plugins.DemoControl.DataFields
{
    public class DynamicTabDataFields : DataFieldsBase
    {
        public string KeyField { get; set; } = "Tab1,Tab2";

        public string ValueField { get; set; } = "Layer2,Layer3";

        public string EventField { get; set; } = "rob,fire";
    }
}
