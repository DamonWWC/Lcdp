using Hjmos.Lcdp.Plugins.CS6.Helpers;
using Hjmos.Lcdp.VisualEditor.Core.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Hjmos.Lcdp.Plugins.CS6.ViewModels
{
    public class DoughnutChartOneViewModel : WidgetViewModelBase
    {

        public Func<double, string> Formatter { get; set; } = value => string.Empty;

        /// <summary>
        /// 弧形颜色
        /// </summary>
        public Brush Brush
        {
            get => _brush;
            set => SetProperty(ref _brush, value);
        }
        private Brush _brush;

        public double Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }
        private double _value;

        public DoughnutChartOneViewModel()
        {
            // 数据字段改变，刷新组件内容
            AttachedPropertyChanged += parameters =>
            {
                // 获取变化的附加属性
                DependencyProperty dp = parameters.GetValue<DependencyProperty>("DependencyProperty");

                if (dp.Name == "Json")
                {
                    string json = parameters.GetValue<object>("NewValue").ToString();

                    if (string.IsNullOrEmpty(json)) return;

                    try
                    {
                        // 获取整个Json对象
                        JObject jObject = JObject.Parse(json);

                        Value = Convert.ToDouble(jObject["Value"]);

                        // 饼图弧度
                        dynamic[] pieArcs = PieHelper.Pie(new double[] { Value, 100-Value });

                        // 获取线性渐变的起止方向
                        Dictionary<string, double> coordinate = PieHelper.GetCoordinates(pieArcs[0].StartRadian, pieArcs[1].EndRadian);

                        Brush = new LinearGradientBrush()
                        {
                            GradientStops = new GradientStopCollection() {
                                new GradientStop() { Offset= 0, Color =  (Color)ColorConverter.ConvertFromString(jObject["StartColor"].ToString())},
                                new GradientStop() { Offset = 1, Color =  (Color)ColorConverter.ConvertFromString(jObject["EndColor"].ToString()) }
                            },
                            StartPoint = new Point(coordinate["X"], coordinate["Y"]),
                            EndPoint = new Point(coordinate["X2"], coordinate["Y2"])
                        };
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("JSON字符串有误");
                        return;
                    }
                }
            };

        }
    }
}
