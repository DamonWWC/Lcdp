using Gu.Wpf.Geometry.Effects;
using Hjmos.Lcdp.Plugins.CS6.Helpers;
using Hjmos.Lcdp.Plugins.CS6.Models;
using Hjmos.Lcdp.VisualEditor.Core.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Hjmos.Lcdp.Plugins.CS6.ViewModels
{
    public class DoughnutChartIteratorViewModel : WidgetViewModelBase
    {

        /// <summary>
        /// 环形图数据模型集合
        /// </summary>
        public List<DoughnutChartModel> DoughnutChartModelList
        {
            get => _doughnutChartModelList;
            set => SetProperty(ref _doughnutChartModelList, value);
        }
        private List<DoughnutChartModel> _doughnutChartModelList;

        public DoughnutChartIteratorViewModel()
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

                        // 获取Series数组
                        JArray jSeries = JArray.Parse(jObject["doughnuts"].ToString());

                        DoughnutChartModelList = jSeries.Select(p =>
                        {
                            double value = Convert.ToDouble(p["value"]);

                            //// 饼图弧度
                            //dynamic[] pieArcs = PieHelper.Pie(new double[] { value, 100 - value });

                            //// 获取线性渐变的起止方向
                            //Dictionary<string, double> coordinate = PieHelper.GetCoordinates(pieArcs[0].StartRadian, pieArcs[1].EndRadian);


                            ShaderEffect effect = new AngularGradientEffect()
                            {
                                CenterPoint = new Point(0.5, 0.5),
                                CentralAngle = value * 3.6d,
                                StartColor =  (Color)ColorConverter.ConvertFromString(p["startColor"].ToString()),
                                EndColor = (Color)ColorConverter.ConvertFromString(p["endColor"].ToString()),
                                StartAngle = 0d
                            };


                            //Brush brush = new LinearGradientBrush()
                            //{
                            //    GradientStops = new GradientStopCollection() {
                            //        new GradientStop() { Offset = 0, Color =  (Color)ColorConverter.ConvertFromString(p["startColor"].ToString())},
                            //        new GradientStop() { Offset = 1, Color =  (Color)ColorConverter.ConvertFromString(p["endColor"].ToString()) }
                            //    },
                            //    StartPoint = new Point(coordinate["X"], coordinate["Y"]),
                            //    EndPoint = new Point(coordinate["X2"], coordinate["Y2"])
                            //};

                            return new DoughnutChartModel() { ShaderEffect = effect, Value = value };

                        }).ToList();
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
