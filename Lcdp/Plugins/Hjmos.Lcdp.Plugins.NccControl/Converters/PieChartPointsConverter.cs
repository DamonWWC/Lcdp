using LiveCharts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Hjmos.Lcdp.Plugins.NccControl.Converters
{
    public class PieChartPointsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<ChartPoint> va)
            {

                var result = va.ToList();
                return result[0].Y;

            }
            return default(string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PieChartPointMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var va1 = ((IEnumerable<ChartPoint>)values[0]).ToList();
            var va2 = (double)values[1];

            return (double)(va1[0].Instance) / va2;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}