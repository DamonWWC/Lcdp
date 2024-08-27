using Prism.Mvvm;
using System.Collections.Generic;
using System.Linq;

namespace Hjmos.Lcdp.Plugins.CS6.Models
{
    public class PercentSection : BindableBase
    {
        public PercentSection(string station, double[] values)
        {
            this.Station = station;
            values.ToList().ForEach(x => Sections.Add(new Sections() { Value = x }));
        }

        /// <summary>
        /// 车站名称
        /// </summary>
        public string Station { get; }


        /// <summary>
        /// 总客流
        /// </summary>
        public double Total => Sections.Sum(x => x.Value);

        /// <summary>
        /// 线段列表
        /// </summary>
        public List<Sections> Sections { get; set; } = new();
    }

    public class Sections : BindableBase
    {
        public string Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }
        private string _color;

        public string Label
        {
            get => _label;
            set => SetProperty(ref _label, value);
        }
        private string _label;

        public double Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }
        private double _value;
    }
}
