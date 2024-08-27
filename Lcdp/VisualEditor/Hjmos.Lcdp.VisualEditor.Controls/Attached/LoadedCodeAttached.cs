using Hjmos.Lcdp.Common;
using Hjmos.Lcdp.VisualEditor.Core.Interface;
using System.ComponentModel;
using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Controls.Attached
{
    public class LoadedCodeAttached
    {
        public static readonly DependencyProperty LoadedCodeProperty = DependencyProperty.RegisterAttached("LoadedCode", typeof(string), typeof(LoadedCodeAttached),
            new FrameworkPropertyMetadata(default, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.NotDataBindable,
                (d, e) =>
                {
                    DependencyPropertyDescriptor descriptor = DependencyPropertyDescriptor.FromName(LoadedCodeProperty.Name, typeof(LoadedCodeAttached), d.GetType());

                    // 事件传参
                    IEventParameters parameters = new EventParameters {
                        { "DependencyProperty", LoadedCodeProperty },
                        { "Descriptor", descriptor },
                        { "NewValue", e.NewValue }
                    };

                    (d as IWidget).RaiseAttachedPropertyChanged(parameters);
                })
            );

        public static string GetLoadedCode(DependencyObject d) => (string)d.GetValue(LoadedCodeProperty);

        public static void SetLoadedCode(DependencyObject d, string value) => d.SetValue(LoadedCodeProperty, value);
    }
}
