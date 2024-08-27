using Hjmos.Lcdp.VisualEditor.Core.Attributes;

namespace Hjmos.Lcdp.Plugins.CS6.Views
{
    [Widget(Category = "长沙6组件", DisplayName = "高峰小时客流", DefaultWidth = 500d)]
    public partial class PeakHourFlowView
    {
        public PeakHourFlowView()
        {
            InitializeComponent();

            //WidgetLoaded +=() => SampleDataHelper.FillDataFromJsonFile(this, "JsonSample.PeakHourFlowView.json");
        }
    }
}
