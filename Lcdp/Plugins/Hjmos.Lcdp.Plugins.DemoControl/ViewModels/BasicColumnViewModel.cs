using Hjmos.Lcdp.ILoger;
using Hjmos.Lcdp.Plugins.DemoControl.DataFields;
using Hjmos.Lcdp.Toolkits;
using Hjmos.Lcdp.VisualEditor.Core.Helpers;
using Hjmos.Lcdp.VisualEditor.Core.ViewModels;
using Hjmos.Lcdp.VisualEditorServer.Entities.Core;
using LiveCharts;
using LiveCharts.Wpf;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Hjmos.Lcdp.Plugins.DemoControl.ViewModels
{
    public class BasicColumnViewModel : WidgetViewModelBase
    {
        public string Desc
        {
            get => _desc;
            set => SetProperty(ref _desc, value);
        }
        private string _desc;

        public SeriesCollection SeriesCollection
        {
            get => _seriesCollection;
            set => SetProperty(ref _seriesCollection, value);
        }
        private SeriesCollection _seriesCollection = new();

        public string[] Labels
        {
            get => _labels;
            set => SetProperty(ref _labels, value);
        }
        private string[] _labels;


        public Func<double, string> Formatter
        {
            get => _formatter;
            set => SetProperty(ref _formatter, value);
        }
        private Func<double, string> _formatter;

        public BasicColumnViewModel(ILogHelper logHelper)
        {
            // 数据字段改变，刷新组件内容
            AttachedPropertyChanged += async parameters =>
            {
                if (parameters == null) return;

                DependencyProperty dp = parameters.GetValue<DependencyProperty>("DependencyProperty");

                if (dp.Name == "DataFields")
                {
                    BasicColumnDataFields dataFields = parameters.GetValue<BasicColumnDataFields>("NewValue");

                    if (dataFields is null) return;

                    // 标签字段
                    string labelField = dataFields.LabelField?.Value;
                    // 序列字段
                    string seriesField = dataFields.SeriesField?.Value;
                    // 值字段
                    string valueField = dataFields.ValueField?.Value;
                    // 地址字段
                    string address = dataFields.Address?.Value;

                    object _result = parameters.GetValue<object>("Result");




                    if (_result is null && !string.IsNullOrEmpty(address))
                    {
                        // 从接口获取数据（TODO：后续可以改成用接口ID来获取数据，接口后台可以改变）
                        _result = (await WebApiHelper.GetAsync<Result<object>>(address)).Data;
                    }

                    if (_result is null) return;

                    JArray jArray = JArray.FromObject(_result);
                    logHelper.Info(this, jArray.ToString());

                    // 获取X轴数组
                    Labels = DimensionJsonHelper.GetSingleDimensionArray<string>(labelField, jArray, x => x);

                    // 获取序列集合
                    Dictionary<string, Array> dic = DimensionJsonHelper.GetSeriesCollection(labelField, Labels, seriesField, valueField, jArray);

                    SeriesCollection.Clear();

                    // 赋值给图表的SeriesCollection
                    foreach (var item in dic)
                    {
                        ChartValues<double> cv = new();
                        foreach (double i in item.Value) cv.Add(i);

                        SeriesCollection.Add(new ColumnSeries
                        {
                            Title = item.Key,
                            Values = cv
                        });
                    };
                }
            };
            Formatter = value => value.ToString("N");
        }
    }
}
