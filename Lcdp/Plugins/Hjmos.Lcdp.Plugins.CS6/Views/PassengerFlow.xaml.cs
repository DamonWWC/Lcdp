using Hjmos.Lcdp.Plugins.CS6.Helpers;
using Hjmos.Lcdp.VisualEditor.Core.Attributes;

namespace Hjmos.Lcdp.Plugins.CS6.Views
{
    [Widget(Category = "长沙6组件", DisplayName = "今日累计客流TOP5", DefaultWidth = 500d)]
    public partial class PassengerFlow
    {
        public PassengerFlow()
        {
            InitializeComponent();

            //WidgetLoaded +=() => SampleDataHelper.FillDataFromJsonFile(this, "JsonSample.DoughnutChartIterator.json");
        }
    }
}
