using Hjmos.Lcdp.VisualEditor.Core.Helpers;
using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Core.Attached
{

    public static class AdaptiveAttached
    {
        public static readonly DependencyProperty AdaptiveProperty = DependencyProperty.RegisterAttached("Adaptive", typeof(bool), typeof(AdaptiveAttached),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.NotDataBindable,
            (d, e) =>
            {
                FrameworkElement element = d as FrameworkElement;

                if (GetAdaptive(element))
                {
                    // 组件和容器宽高相等
                    AdaptiveHelper.AdaptiveParent(element);
                }
                else
                {
                    // 还原组件宽高为200（TODO: 应该还原为原来宽高，或者默认宽高）
                    element.SetValue(FrameworkElement.WidthProperty, 200d);
                    element.SetValue(FrameworkElement.HeightProperty, 200d);
                }
            }));

        public static bool GetAdaptive(DependencyObject d) => (bool)d.GetValue(AdaptiveProperty);

        public static void SetAdaptive(DependencyObject d, bool value) => d.SetValue(AdaptiveProperty, value);
    }
}
