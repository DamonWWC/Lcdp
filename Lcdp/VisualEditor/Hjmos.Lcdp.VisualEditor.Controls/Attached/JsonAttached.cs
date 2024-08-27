using Hjmos.Lcdp.Common;
using Hjmos.Lcdp.VisualEditor.Core.Interface;
using System.ComponentModel;
using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Controls.Attached
{
    public class JsonAttached
    {
        public static readonly DependencyProperty JsonProperty = DependencyProperty.RegisterAttached("Json", typeof(string), typeof(JsonAttached),
            new FrameworkPropertyMetadata(default, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.NotDataBindable,
                (d, e) =>
                {
                    DependencyPropertyDescriptor descriptor = DependencyPropertyDescriptor.FromName(JsonProperty.Name, typeof(JsonAttached), d.GetType());

                    // 事件传参
                    IEventParameters parameters = new EventParameters {
                        { "DependencyProperty", JsonProperty },
                        { "Descriptor", descriptor },
                        { "NewValue", e.NewValue }
                    };

                    (d as IWidget).RaiseAttachedPropertyChanged(parameters);
                })
            );

        public static string GetJson(DependencyObject d) => (string)d.GetValue(JsonProperty);

        public static void SetJson(DependencyObject d, string value) => d.SetValue(JsonProperty, value);
    }
}
