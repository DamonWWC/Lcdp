using Gu.Wpf.Geometry.Effects;
using Hjmos.Lcdp.Plugins.NccControl.DataFields;
using Hjmos.Lcdp.Plugins.NccControl.Models;
using Hjmos.Lcdp.Toolkits;
using Hjmos.Lcdp.VisualEditor.Core.ViewModels;
using Hjmos.Lcdp.VisualEditorServer.Entities.Core;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Hjmos.Lcdp.Plugins.NccControl.ViewModels
{
    public class PieStatisticsViewModel : WidgetViewModelBase
    {

        private List<MonitorStatistics> MonitorStatisticsList { get; set; }

        public PieStatisticsViewModel()
        {
            LabelPoint = chartPoint => string.Format("{1:P},{0}个", chartPoint.Y, chartPoint.Participation);
            ColorsCollection = new ColorsCollection
            {
                Color.FromRgb(48,238,154),
                Color.FromRgb(132,130,224),
                Color.FromRgb(234,204,44),
                Color.FromRgb(255,124,114)
            };

            GetData();

            // 数据字段改变，刷新组件内容
            AttachedPropertyChanged += async parameters =>
            {
                DependencyProperty dp = parameters.GetValue<DependencyProperty>("DependencyProperty");

                if (dp.Name == "DataFields")
                {
                    //PieStatisticsDataFields dataFields = parameters.GetValue<PieStatisticsDataFields>("NewValue");

                    //if (dataFields is null) return;

                    //// 标题赋值
                    //Title = dataFields.Title;


                    //if (dataFields.Address is null || string.IsNullOrEmpty(dataFields.Address.Value))
                    //{
                    //    MonitorStatisticsList = null;
                    //    GetData();
                    //    return;
                    //}

                    //// 从接口获取数据
                    //Result<List<MonitorStatistics>> _result = await WebApiHelper.GetAsync<Result<List<MonitorStatistics>>>(dataFields.Address.Value);

                    //if (_result.Code != 200)
                    //{
                    //    MessageBox.Show(_result.Msg);
                    //    return;
                    //}

                    //MonitorStatisticsList = _result.Data;





                    //SetData(MonitorStatisticsList);
                }

            };
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        private void GetData()
        {
            if (MonitorStatisticsList == null)
            {
                MonitorStatisticsList = new List<MonitorStatistics>();
            }

            SetData(MonitorStatisticsList);
        }

        private readonly object obj = new();

        private void SetData(List<MonitorStatistics> statisticsInfo)
        {

            statisticsInfo.Add(new MonitorStatistics() { Total = 100, Num = 100, Name = "1", Code = "1", Ratio = "1" });






            lock (obj)
            {
                double sum = statisticsInfo.Sum(p => p.Num);

                SeriesCollection SeriesCollection1 = new();
                int i = 0;
                foreach (MonitorStatistics item in statisticsInfo)
                {
                    PieSeries series = new()
                    {
                        Title = item.Name,
                        Values = new ChartValues<ObservableValue> { new ObservableValue(item.Num) },
                        StrokeThickness = 0,
                        //填百分比
                        Tag = item.Num / sum,
                        Effect = new AngularGradientEffect() { CenterPoint = new Point(0.5, 0.5), CentralAngle = 360d, EndColor = Colors.Transparent, StartAngle = 0d, StartColor = Colors.Blue }

                    };




                    if (i == 0)
                    {
                        series.TextPieNum = sum.ToString();
                        series.TextPieNumSize = 24;
                    }

                    i++;
                    SeriesCollection1.Add(series);
                }
                SeriesCollection = SeriesCollection1;
                DeadLine = DateTime.Now;
            }
        }


        #region Property

        private SeriesCollection _SeriesCollection;
        /// <summary>
        /// Pie图数据源
        /// </summary>
        public SeriesCollection SeriesCollection
        {
            get => _SeriesCollection;
            set => SetProperty(ref _SeriesCollection, value);
        }

        private Func<ChartPoint, string> _LabelPoint;
        public Func<ChartPoint, string> LabelPoint
        {
            get => _LabelPoint;
            set => SetProperty(ref _LabelPoint, value);
        }

        private ColorsCollection _ColorsCollection;
        public ColorsCollection ColorsCollection
        {
            get => _ColorsCollection;
            set => SetProperty(ref _ColorsCollection, value);
        }

        private DateTime _DeadLine;
        /// <summary>
        /// 截止时间
        /// </summary>
        public DateTime DeadLine
        {
            get => _DeadLine;
            set => SetProperty(ref _DeadLine, value);
        }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        private string _title = string.Empty;

        #endregion
    }

}

