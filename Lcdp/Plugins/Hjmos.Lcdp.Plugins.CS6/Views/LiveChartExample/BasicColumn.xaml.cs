using Hjmos.Lcdp.Plugins.CS6.DataFields.LiveChartExample;
using Hjmos.Lcdp.Plugins.CS6.Helpers;
using Hjmos.Lcdp.VisualEditor.Core.Attributes;

namespace Hjmos.Lcdp.Plugins.CS6.Views.LiveChartExample
{
    [Widget(Category = "LiveChart组件", DisplayName = "柱形图", DefaultWidth = 500d)]
    [DataFields(typeof(BasicColumnDataFields))]
    public partial class BasicColumn
    {
        public BasicColumn()
        {
            InitializeComponent();

            WidgetLoaded +=() => SampleDataHelper.FillDataFromJsonFile(this, "JsonSample.LiveChartExample.BasicColumn.json");
        }
    }
}
