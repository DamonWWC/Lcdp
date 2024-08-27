using Prism.Mvvm;
using System;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Hjmos.Lcdp.Plugins.CS6.Models
{
    public class DoughnutChartModel : BindableBase
    {
        /// <summary>
        /// 像素着色器
        /// </summary>
        public ShaderEffect ShaderEffect
        {
            get => _shaderEffect;
            set => SetProperty(ref _shaderEffect, value);
        }
        private ShaderEffect _shaderEffect;

        /// <summary>
        /// 弧形颜色
        /// </summary>
        public Brush Brush
        {
            get => _brush;
            set => SetProperty(ref _brush, value);
        }
        private Brush _brush;

        /// <summary>
        /// 值
        /// </summary>
        public double Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }
        private double _value;

        public Func<double, string> Formatter { get; set; } = value => string.Empty;
    }
}
