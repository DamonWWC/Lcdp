using Hjmos.Lcdp.Plugins.CS6.Models;
using Hjmos.Lcdp.VisualEditor.Core.ViewModels;
using LiveCharts;
using LiveCharts.Wpf;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Hjmos.Lcdp.Plugins.CS6.ViewModels
{
    public class BasicColumnViewModel : WidgetViewModelBase
    {
        /// <summary>
        /// 类型选择
        /// </summary>
        public List<string> TypeList
        {
            get { return _typeList; }
            set { SetProperty(ref _typeList, value); }
        }
        private List<string> _typeList;

        /// <summary>
        /// 选中的类型
        /// </summary>
        public string SelectedType
        {
            get => _selectedType;
            set
            {
                if (SetProperty(ref _selectedType, value))
                {
                    GenerateSeries(_jSeries);
                }
            }
        }
        private string _selectedType;


        /// <summary>
        /// 选项卡是否可见
        /// </summary>
        public Visibility TabVisibility
        {
            get { return _tabVisibility; }
            set { SetProperty(ref _tabVisibility, value); }
        }
        private Visibility _tabVisibility = Visibility.Hidden;

        /// <summary>
        /// 是否在柱形上显示标签
        /// </summary>
        public bool ShowDataLabel
        {
            get => _showDataLabel;
            set
            {
                if (SetProperty(ref _showDataLabel, value))
                {
                    GenerateSeries(_jSeries);
                }
            }
        }
        private bool _showDataLabel = false;


        /// <summary>
        /// 是否显示Y轴标签
        /// PS：直接ShowLabel = false时X轴标签会显示不完全，所以使用Formatter来控制Y轴隐藏
        /// </summary>
        public bool ShowYLabels
        {
            get => _showYLabels;
            set
            {
                if (SetProperty(ref _showYLabels, value))
                {
                    BasicColumnModel.Formatter = _showYLabels ? value => value.ToString("#") : value => " ";
                }
            }
        }
        private bool _showYLabels = true;


        private List<JToken> _jSeries;


        public BasicColumnModel BasicColumnModel { get; set; } = new();

        public BasicColumnViewModel()
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

                         BasicColumnModel.Labels = JArray.Parse(jObject["labels"].ToString()).ToObject<List<string>>().ToArray();

                         // 获取Series数组
                         // 注意：获取series要放在获取configs之前，因为SelectedType赋值的时候，会触发GenerateSeries
                         _jSeries = JArray.Parse(jObject["series"].ToString()).ToList();

                         // 获取配置
                         if (jObject["configs"] is not null)
                         {
                             JArray jConfigs = JArray.Parse(jObject["configs"].ToString());

                             TypeList = jConfigs.Select(p => p["value"].ToString()).ToList();
                             SelectedType = TypeList[0];
                             TabVisibility = Visibility.Visible;
                         }
                         else
                         {
                             TypeList = null;
                             SelectedType = string.Empty;
                             TabVisibility = Visibility.Hidden;
                         }

                         GenerateSeries(_jSeries);
                     }
                     catch (Exception)
                     {
                         MessageBox.Show("JSON字符串有误");
                         return;
                     }
                 }
             };
        }

        // 根据JSON生成图表序列
        private void GenerateSeries(List<JToken> newSeries)
        {

            if (newSeries is null) return;

            if (!string.IsNullOrEmpty(SelectedType))
            {
                newSeries = _jSeries.Where(p => p["title"].ToString() == SelectedType).ToList();
            }

            // 转化为ColumnSeries
            List<ColumnSeries> _columnSeriesList = newSeries?.Select(p =>
            {
                var chartValues = new ChartValues<double>(p["values"].ToObject<List<double>>().ToArray());

                double width = double.Parse((p["width"] ??= 13).ToString());
                double padding = double.Parse((p["padding"] ??= 5).ToString());

                ColumnSeries columnSeries = new()
                {
                    Values = chartValues,
                    MaxColumnWidth = width,
                    ColumnPadding = padding,
                    RadiusX = 2,
                    RadiusY = 2,                              // 填充线性渐变
                    Fill = new LinearGradientBrush()
                    {
                        GradientStops = new GradientStopCollection()
                        {
                            // TODO: 需要判断Color字符串是否为空，空给一个默认颜色
                            new GradientStop() { Offset = 0, Color = (Color)ColorConverter.ConvertFromString(p["startColor"].ToString()) },
                            new GradientStop() { Offset = 1, Color = (Color)ColorConverter.ConvertFromString(p["endColor"].ToString()) }
                        },
                        StartPoint = new Point(0.5, 0),
                        EndPoint = new Point(0.5, 1)
                    },
                    LabelsPosition = BarLabelPosition.Top,
                    DataLabels = _showDataLabel,
                    Foreground = Brushes.White,
                    FontSize = 16,
                    FontWeight = FontWeights.Medium
                };
                columnSeries.Title = p["title"]?.ToString() ?? columnSeries.Title;

                return columnSeries;
            }).ToList();


            BasicColumnModel.SeriesCollection = new();
            BasicColumnModel.SeriesCollection.AddRange(_columnSeriesList);

        }
    }
}
