using Hjmos.Lcdp.VisualEditor.Core.BaseClass;
using System;
using System.Windows.Media.Imaging;

namespace Hjmos.Lcdp.Plugins.DemoControl.Views.Samples
{

    public partial class PictureSample : SampleBase
    {
        public PictureSample()
        {
            InitializeComponent();

            Loaded += delegate
            {
                if (WidgetType.ToString().Contains("AlarmSolve.LineNet.WholeView"))
                {
                    pic.Source = new BitmapImage(new Uri("pack://application:,,,/Hjmos.Lcdp.Plugins.DemoControl;component/Images/background.png"));
                }
                else if (WidgetType.ToString().Contains("Hjmos.Ncc.EmergencyCommandV1.Views.AlarmMonitor.AlarmWarnStatisticsView"))
                {
                    pic.Source = new BitmapImage(new Uri("pack://application:,,,/Hjmos.Lcdp.VisualEditor.Assets;component/Images/预警统计.png"));
                }
                else if (WidgetType.ToString().Contains("Hjmos.Ncc.EmergencyCommandV1.Views.AlarmMonitor.EmergencyStatisticsView"))
                {
                    pic.Source = new BitmapImage(new Uri("pack://application:,,,/Hjmos.Lcdp.VisualEditor.Assets;component/Images/突发统计.png"));
                }
                else if (WidgetType.ToString().Contains("Hjmos.Ncc.EmergencyCommandV1.Views.AlarmMonitor.EventTrendInRecentSevenDaysView"))
                {
                    pic.Source = new BitmapImage(new Uri("pack://application:,,,/Hjmos.Lcdp.VisualEditor.Assets;component/Images/事件趋势.png"));
                }

            };

        }
    }
}
