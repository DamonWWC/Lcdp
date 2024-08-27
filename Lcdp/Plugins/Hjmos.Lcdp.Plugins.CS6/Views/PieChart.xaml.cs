using Hjmos.Lcdp.Plugins.CS6.Helpers;
using Hjmos.Lcdp.VisualEditor.Core.Attributes;

namespace Hjmos.Lcdp.Plugins.CS6.Views
{
    [Widget(Category = "长沙6组件", DisplayName = "环形饼图", DefaultWidth = 500d)]
    public partial class PieChart
    {
        public PieChart()
        {
            InitializeComponent();

            WidgetLoaded += () => SampleDataHelper.FillDataFromJsonFile(this, "JsonSample.PieChart.json");
        }
    }
}
