using Hjmos.Lcdp.Plugins.CS6.Models.LiveChartExample;
using Hjmos.Lcdp.VisualEditor.Core.ViewModels;
using LiveCharts;
using LiveCharts.Wpf;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Hjmos.Lcdp.Plugins.CS6.ViewModels.LiveChartExample
{
    public class BasicLineViewModel : WidgetViewModelBase
    {

        public BasicLineExampleModel BasicLineExampleModel { get; set; } = new();

        public BasicLineViewModel()
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

                         JArray jSeries = JArray.Parse(jObject["Series"].ToString());

                         List<LineSeries> lineSeriesList = jSeries.Select(p =>
                         {
                             var chartValues = new ChartValues<double>();

                             chartValues.AddRange(p["Values"].ToObject<List<double>>().ToArray());

                             LineSeries lineSeries = new() { Values = chartValues };

                             lineSeries.Title = p["Title"]?.ToString() ?? lineSeries.Title;

                             // LineSmoothness： 0 straight lines, 1 really smooth lines
                             lineSeries.LineSmoothness = p["LineSmoothness"] is null ? lineSeries.LineSmoothness : double.Parse(p["LineSmoothness"].ToString());

                             return lineSeries;
                         }).ToList();

                         BasicLineExampleModel.SeriesCollection = new();
                         BasicLineExampleModel.SeriesCollection.AddRange(lineSeriesList);

                         BasicLineExampleModel.Labels = JArray.Parse(jObject["Labels"].ToString()).ToObject<List<string>>().ToArray();
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
