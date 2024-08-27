using Hjmos.Lcdp.VisualEditor.Core.ViewModels;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Hjmos.Lcdp.Plugins.CS6.ViewModels
{
    public class PieChartViewModel : WidgetViewModelBase
    {
        /// <summary>
        /// 饼图序列集合
        /// </summary>
        public SeriesCollection SeriesCollection { get; set; } = new();

        /// <summary>
        /// 饼图标签格式
        /// </summary>
        public Func<ChartPoint, string> PointLabel => chartPoint => string.Format("", chartPoint.Y, chartPoint.Participation);

        /// <summary>
        /// 告警总数
        /// </summary>
        public int TotalCount
        {
            get { return _totalCount; }
            set { SetProperty(ref _totalCount, value); }
        }
        private int _totalCount;

        /// <summary>
        /// 饼图图例数据
        /// </summary>
        public List<LegendDTO> Legends { get; set; }


        public PieChartViewModel()
        {


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



                        TotalCount = int.Parse(jObject["totalCount"].ToString());


                        var _jSeries = JArray.Parse(jObject["alarmList"].ToString()).ToList();

                        _jSeries.ForEach(x =>
                        {
                            //SeriesCollection

                            var a = new PieSeries
                            {
                                Title = x["name"].ToString(),  // 标题
                                Values = new ChartValues<ObservableValue> { new ObservableValue(double.Parse(x["amount"].ToString())) },    // 饼图的数值
                                LabelPoint = PointLabel,
                                DataLabels = true,
                                StrokeThickness = 0
                            };

                            SeriesCollection.Add(a);


                        });

                    }
                    catch (Exception)
                    {
                        MessageBox.Show("JSON字符串有误");
                        return;
                    }
                }
            };

        }

        /// <summary>
        /// 图例Model
        /// </summary>
        public class LegendDTO
        {
            public string Title { get; set; }
            public string Tag { get; set; }
            public Brush Brush { get; set; }
        }
    }
}