using Hjmos.Lcdp.Plugins.CS6.Helpers;
using Hjmos.Lcdp.Plugins.CS6.ViewModels;
using Hjmos.Lcdp.VisualEditor.Core.Attributes;
using System.Windows;

namespace Hjmos.Lcdp.Plugins.CS6.Views
{
    [Widget(Category = "长沙6组件", DisplayName = "柱形图", DefaultWidth = 500d)]
    public partial class BasicColumn
    {

        public BasicColumn()
        {
            InitializeComponent();

            ShowDataLabel = false;
            ShowYLabels = true;

            WidgetLoaded += () => SampleDataHelper.FillDataFromJsonFile(this, "JsonSample.BasicColumn.json");
        }

        /// <summary>
        /// 是否在柱形上显示标签
        /// </summary>
        public bool ShowDataLabel
        {
            get => (bool)GetValue(ShowDataLabelProperty);
            set => SetValue(ShowDataLabelProperty, value);
        }


        public static readonly DependencyProperty ShowDataLabelProperty =
            DependencyProperty.Register("ShowDataLabel", typeof(bool), typeof(BasicColumn), new PropertyMetadata(false, (d, e) =>
            {
                BasicColumn basicColumn = (BasicColumn)d;
                (basicColumn.DataContext as BasicColumnViewModel).ShowDataLabel = (bool)e.NewValue;
            }));


        /// <summary>
        /// 是否显示Y轴标签
        /// </summary>
        public bool ShowYLabels
        {
            get => (bool)GetValue(ShowYLabelsProperty);
            set => SetValue(ShowYLabelsProperty, value);
        }

        public static readonly DependencyProperty ShowYLabelsProperty =
            DependencyProperty.Register("ShowYLabels", typeof(bool), typeof(BasicColumn), new PropertyMetadata(true, (d, e) =>
            {
                BasicColumn basicColumn = (BasicColumn)d;
                (basicColumn.DataContext as BasicColumnViewModel).ShowYLabels = (bool)e.NewValue;
            }));

    }
}
