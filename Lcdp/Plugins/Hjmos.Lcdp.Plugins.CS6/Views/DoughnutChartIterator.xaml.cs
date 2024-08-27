using Hjmos.Lcdp.Plugins.CS6.Helpers;
using Hjmos.Lcdp.VisualEditor.Core.Attributes;

namespace Hjmos.Lcdp.Plugins.CS6.Views
{
    [Widget(Category = "长沙6组件", DisplayName = "环形图组", DefaultWidth = 500d)]
    public partial class DoughnutChartIterator
    {
        public DoughnutChartIterator()
        {
            InitializeComponent();

            WidgetLoaded +=() => SampleDataHelper.FillDataFromJsonFile(this, "JsonSample.DoughnutChartIterator.json");
        }
    }
}
