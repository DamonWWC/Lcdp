using Hjmos.Lcdp.VisualEditor.Core.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Core
{
    public class ReadOnlyTextPropertyEditor : PropertyEditorBase
    {
        public override FrameworkElement CreateElement(PropertyItem propertyItem) => new TextBox
        {
            IsReadOnly = true,
            Background = new SolidColorBrush(Color.FromRgb(60, 60, 60)),
            Foreground = Brushes.White,
            BorderThickness = new Thickness(0)
        };

        public override DependencyProperty GetDependencyProperty() => TextBox.TextProperty;

        public override BindingMode GetBindingMode(PropertyItem propertyItem) => BindingMode.OneWay;

        protected override IValueConverter GetConverter(PropertyItem propertyItem) => ResourceHelper.GetResourceInternal<IValueConverter>("Object2StringConverter");
    }
}
