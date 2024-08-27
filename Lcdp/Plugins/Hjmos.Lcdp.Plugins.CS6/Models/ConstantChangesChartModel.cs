using LiveCharts;
using Prism.Mvvm;
using System;

namespace Hjmos.Lcdp.Plugins.CS6.Models
{
    public class ConstantChangesChartModel : BindableBase
    {
        public SeriesCollection SeriesCollection
        {
            get => _seriesCollection;
            set => SetProperty(ref _seriesCollection, value);
        }
        private SeriesCollection _seriesCollection;

        /// <summary>
        /// Y轴标签格式
        /// </summary>
        public Func<double, string> Formatter => value => value.ToString("#");

        /// <summary>
        /// X轴标签格式
        /// </summary>
        public Func<double, string> DateTimeFormatter => value => value <= 0 ? "00:00" : new DateTime((long)value).ToString("HH:mm");


        /// <summary>
        /// X轴单位
        /// </summary>
        public double AxisUnit => TimeSpan.TicksPerMinute;

        /// <summary>
        /// X轴步长
        /// </summary>
        public double AxisStep => TimeSpan.FromMinutes(10).Ticks;
    }
}
