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
    public class StackedColumnExampleViewModel : WidgetViewModelBase
    {

        public StackedColumnExampleModel StackedColumnExampleModel { get; set; } = new();

        public StackedColumnExampleViewModel()
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

                         List<StackedColumnSeries> columnSeriesList = jSeries.Select(p =>
                         {
                             var chartValues = new ChartValues<double>();

                             chartValues.AddRange(p["Values"].ToObject<List<double>>().ToArray());

                             StackedColumnSeries columnSeries = new() { Values = chartValues };

                             columnSeries.DataLabels = p["DataLabels"] is null ? columnSeries.DataLabels : bool.Parse(p["DataLabels"].ToString());

                             return columnSeries;
                         }).ToList();

                         StackedColumnExampleModel.SeriesCollection = new();
                         StackedColumnExampleModel.SeriesCollection.AddRange(columnSeriesList);

                         StackedColumnExampleModel.Labels =JArray.Parse(jObject["Labels"].ToString()).ToObject<List<string>>().ToArray();
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
