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
    public class BasicColumnViewModel : WidgetViewModelBase
    {

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

                         // 获取Series数组
                         JArray jSeries = JArray.Parse(jObject["Series"].ToString());

                         // 转化为ColumnSeries
                         List<ColumnSeries> columnSeriesList = jSeries.Select(p =>
                         {
                             var chartValues = new ChartValues<double>();

                             chartValues.AddRange(p["Values"].ToObject<List<double>>().ToArray());

                             ColumnSeries columnSeries = new() { Values = chartValues };

                             columnSeries.Title = p["Title"]?.ToString() ?? columnSeries.Title;

                             return columnSeries;
                         }).ToList();

                         BasicColumnModel.SeriesCollection = new();
                         BasicColumnModel.SeriesCollection.AddRange(columnSeriesList);

                         BasicColumnModel.Labels = JArray.Parse(jObject["Labels"].ToString()).ToObject<List<string>>().ToArray();
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
