using LiveCharts;
using Prism.Mvvm;

namespace Hjmos.Lcdp.Plugins.CS6.Models.LiveChartExample
{
    public class StackedColumnExampleModel : BindableBase
    {
        public SeriesCollection SeriesCollection
        {
            get => _seriesCollection;
            set => SetProperty(ref _seriesCollection, value);
        }
        private SeriesCollection _seriesCollection;

        public string[] Labels
        {
            get => _labels;
            set => SetProperty(ref _labels, value);
        }
        private string[] _labels;
    }
}
