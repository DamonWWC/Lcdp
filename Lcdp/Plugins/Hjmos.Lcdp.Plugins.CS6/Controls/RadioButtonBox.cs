using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Hjmos.Lcdp.Plugins.CS6.Controls
{
    public class RadioButtonBox : RadioButton
    {
        /// <summary>
        /// 选中时的背景色
        /// </summary>
        public Brush CheckedBackground
        {
            get { return (Brush)GetValue(CheckedBackgroundProperty); }
            set { SetValue(CheckedBackgroundProperty, value); }
        }

        public static readonly DependencyProperty CheckedBackgroundProperty =
            DependencyProperty.Register("CheckedBackground", typeof(Brush), typeof(RadioButtonBox), new PropertyMetadata(Brushes.Transparent));
    }
}
