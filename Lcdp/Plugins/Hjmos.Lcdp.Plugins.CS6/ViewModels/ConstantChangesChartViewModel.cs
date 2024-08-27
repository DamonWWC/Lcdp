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
    public class ConstantChangesChartViewModel : WidgetViewModelBase
    {

        public ConstantChangesChartModel ChartModel { get; set; } = new();

        public ConstantChangesChartViewModel()
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
                         JObject jObject = JObject.Parse(json);

                         JArray jSeries = JArray.Parse(jObject["series"].ToString());

                         List<LineSeries> lineSeriesList = jSeries.Select(p =>
                         {
                             var chartValues = new ChartValues<MeasureModel>(p["values"].ToObject<List<MeasureModel>>().ToArray());

                             LineSeries lineSeries = new()
                             {
                                 Values = chartValues,
                                 PointGeometry = null,
                                 PointGeometrySize = 0,
                                 StrokeThickness = 2,
                                 Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(p["color"].ToString())),
                                 Fill = new SolidColorBrush(Colors.Transparent)
                             };
                             lineSeries.Title = p["title"]?.ToString() ?? lineSeries.Title;

                             lineSeries.StrokeDashArray = p["strokeDashArray"] is null ? lineSeries.StrokeDashArray : new DoubleCollection(Array.ConvertAll(p["strokeDashArray"].ToString().Split(','), double.Parse));

                             // LineSmoothness： 0 straight lines, 1 really smooth lines
                             lineSeries.LineSmoothness = p["lineSmoothness"] is null ? lineSeries.LineSmoothness : double.Parse(p["lineSmoothness"].ToString());

                             return lineSeries;
                         }).ToList();

                         ChartModel.SeriesCollection = new();
                         ChartModel.SeriesCollection.AddRange(lineSeriesList);
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
