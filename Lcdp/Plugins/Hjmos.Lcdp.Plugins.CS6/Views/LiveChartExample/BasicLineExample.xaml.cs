using Hjmos.Lcdp.Plugins.CS6.Helpers;
using Hjmos.Lcdp.VisualEditor.Core.Attributes;

namespace Hjmos.Lcdp.Plugins.CS6.Views.LiveChartExample
{
    [Widget(Category = "LiveChart组件", DisplayName = "折线图", DefaultWidth = 500d)]
    public partial class BasicLineExample
    {
        public BasicLineExample()
        {
            InitializeComponent();

            WidgetLoaded +=() => SampleDataHelper.FillDataFromJsonFile(this, "JsonSample.LiveChartExample.BasicLineExample.json");
        }
    }
}
