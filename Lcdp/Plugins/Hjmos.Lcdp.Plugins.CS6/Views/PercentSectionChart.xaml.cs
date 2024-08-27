using Hjmos.Lcdp.Plugins.CS6.Models;
using System.Collections.Generic;
using System.Windows;

namespace Hjmos.Lcdp.Plugins.CS6.Views
{
    // 百分比线段图表
    public partial class PercentSectionChart
    {
        public PercentSectionChart() => InitializeComponent();


        /// <summary>
        /// 默认颜色列表
        /// </summary>
        public string[] Colors
        {
            get => (string[])GetValue(ColorsProperty);
            set => SetValue(ColorsProperty, value);
        }

        public static readonly DependencyProperty ColorsProperty =
            DependencyProperty.Register("Colors", typeof(string[]), typeof(PercentSectionChart), new PropertyMetadata(default, (d, e) =>
            {
                PercentSectionChart percentSectionChart = (PercentSectionChart)d;
                percentSectionChart.AssignColor();
            }));

        /// <summary>
        /// 序列标签
        /// </summary>
        public string[] Labels
        {
            get => (string[])GetValue(LabelsProperty);
            set => SetValue(LabelsProperty, value);
        }

        public static readonly DependencyProperty LabelsProperty =
            DependencyProperty.Register("Labels", typeof(string[]), typeof(PercentSectionChart), new PropertyMetadata(default, (d, e) => { }));


        /// <summary>
        /// 图表序列
        /// </summary>
        public List<PercentSection> Series
        {
            get => (List<PercentSection>)GetValue(SeriesProperty);
            set => SetValue(SeriesProperty, value);
        }


        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register("Series", typeof(List<PercentSection>), typeof(PercentSectionChart), new PropertyMetadata(default, (d, e) =>
            {
                PercentSectionChart percentSectionChart = (PercentSectionChart)d;
                percentSectionChart.AssignColor();
            }));

        /// <summary>
        /// 需要显示的行数
        /// </summary>
        public int RowCount
        {
            get => (int)GetValue(RowCountProperty);
            set => SetValue(RowCountProperty, value);
        }

        public static readonly DependencyProperty RowCountProperty =
            DependencyProperty.Register("RowCount", typeof(int), typeof(PercentSectionChart), new PropertyMetadata(3, (d, e) => { }));

        /// <summary>
        /// 线段间隔
        /// </summary>
        public double Gap
        {
            get => (double)GetValue(GapProperty);
            set => SetValue(GapProperty, value);
        }

        public static readonly DependencyProperty GapProperty =
            DependencyProperty.Register("Gap", typeof(double), typeof(PercentSectionChart), new PropertyMetadata(3d, (d, e) => { }));

        /// <summary>
        /// 分配颜色
        /// </summary>
        private void AssignColor()
        {
            if (Series is null) return;
            if (Colors is null) return;
            if (Labels is null) return;

            Series.ForEach(s =>
            {
                for (int i = 0; i < s.Sections.Count; i++)
                {
                    s.Sections[i].Color = Colors[i];
                    s.Sections[i].Label = Labels[i];
                }
            });
        }
    }
}
