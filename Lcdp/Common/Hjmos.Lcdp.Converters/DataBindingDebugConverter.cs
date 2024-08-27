using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace Hjmos.Lcdp.Converters
{
    /// <summary>
    /// 提供一种方法来调试应用程序中有问题的数据绑定。
    /// </summary>
    public class DataBindingDebugConverter : IValueConverter
    {
        /// <summary>
        /// 每当设置或更新数据绑定值时，将中断执行。
        /// </summary>
        /// <param name="value">绑定源的值</param>
        /// <param name="targetType">绑定目标的类型</param>
        /// <param name="parameter">参数</param>
        /// <param name="culture"></param>
        /// <returns>未更改的输入值</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Debugger.IsAttached) Debugger.Break();
            return value;
        }

        /// <summary>
        /// 每当设置或更新数据绑定值时，将中断执行。
        /// </summary>
        /// <param name="value">绑定目标的值</param>
        /// <param name="targetType">要转换的类型</param>
        /// <param name="parameter">参数</param>
        /// <param name="culture"></param>
        /// <returns>未更改的输入值</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Debugger.IsAttached) Debugger.Break();
            return value;
        }
    }
}