using LiveCharts;
using Prism.Mvvm;
using System;

namespace Hjmos.Lcdp.Plugins.CS6.Models
{
    public class BasicColumnModel : BindableBase
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

        public Func<double, string> Formatter
        {
            get => _formatter;
            set => SetProperty(ref _formatter, value);
        }
        private Func<double, string> _formatter = value => value.ToString("#");

        //public Func<double, string> Formatter { get; set; } = value => value.ToString("#");
    }
}
