using Hjmos.Lcdp.Common;
using Hjmos.Lcdp.VisualEditor.Core.Interface;
using System.ComponentModel;
using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Controls.Attached
{
    public class CustomXamlAttached
    {
        public static readonly DependencyProperty CustomXamlProperty = DependencyProperty.RegisterAttached("CustomXaml", typeof(string), typeof(CustomXamlAttached),
            new FrameworkPropertyMetadata(default, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.NotDataBindable,
                (d, e) =>
                {
                    DependencyPropertyDescriptor descriptor = DependencyPropertyDescriptor.FromName(CustomXamlProperty.Name, typeof(CustomXamlAttached), d.GetType());

                    // 事件传参
                    IEventParameters parameters = new EventParameters {
                        { "DependencyProperty", CustomXamlProperty },
                        { "Descriptor", descriptor },
                        { "NewValue", e.NewValue }
                    };

                    (d as IWidget).RaiseAttachedPropertyChanged(parameters);
                }
            ));

        public static string GetCustomXaml(DependencyObject d) => (string)d.GetValue(CustomXamlProperty);

        public static void SetCustomXaml(DependencyObject d, string value) => d.SetValue(CustomXamlProperty, value);
    }
}
