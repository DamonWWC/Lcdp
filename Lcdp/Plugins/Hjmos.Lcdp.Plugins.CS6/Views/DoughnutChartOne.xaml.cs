using Hjmos.Lcdp.Plugins.CS6.Helpers;
using Hjmos.Lcdp.VisualEditor.Core.Attributes;

namespace Hjmos.Lcdp.Plugins.CS6.Views
{
    [Widget(Category = "长沙6组件", DisplayName = "环形图01", DefaultWidth = 500d)]
    public partial class DoughnutChartOne
    {
        public DoughnutChartOne()
        {
            InitializeComponent();

            WidgetLoaded +=() => SampleDataHelper.FillDataFromJsonFile(this, "JsonSample.DoughnutChartOne.json");
        }
    }
}
