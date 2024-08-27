using Hjmos.Lcdp.Plugins.CS6.Models;
using Hjmos.Lcdp.VisualEditor.Core.ViewModels;
using System.Collections.Generic;

namespace Hjmos.Lcdp.Plugins.CS6.ViewModels
{
    public class PassengerFlowViewModel : WidgetViewModelBase
    {
        /// <summary>
        /// 累计客流集合
        /// </summary>
        public List<PercentSection> Series
        {
            get => _series;
            set => SetProperty(ref _series, value);
        }
        private List<PercentSection> _series = new() {
            new PercentSection("涧塘", new double[] { 100, 200 }),
            new PercentSection("朝阳村", new double[] { 300, 400 }),
            new PercentSection("人民东路", new double[] { 500, 600 })
        };

        /// <summary>
        /// 序列标签
        /// </summary>
        public string[] Labels
        {
            get => _labels;
            set => SetProperty(ref _labels, value);
        }
        private string[] _labels = new[]{
            "进站",
            "出站"
        };


        public string[] Colors
        {
            get => _colors;
            set => SetProperty(ref _colors, value);
        }
        private string[] _colors = new[]{
            "#0079D8",
            "#00AD5D"
        };
    }
}
