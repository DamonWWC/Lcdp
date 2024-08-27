using Hjmos.Lcdp.Common;
using Hjmos.Lcdp.VisualEditor.Controls.BaseClass;
using Hjmos.Lcdp.VisualEditor.Core.Interface;
using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Controls.Attached
{

    public static class DataFieldsAttached
    {
        public static readonly DependencyProperty DataFieldsProperty = DependencyProperty.RegisterAttached("DataFields", typeof(DataFieldsBase), typeof(DataFieldsAttached),
            new FrameworkPropertyMetadata(default, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.NotDataBindable,
                (d, e) =>
                {
                    // 事件传参
                    IEventParameters parameters = new EventParameters {
                        { "DependencyProperty", DataFieldsProperty },
                        { "NewValue", e.NewValue }
                    };

                    (d as IWidget).RaiseAttachedPropertyChanged(parameters);
                })
            );

        public static DataFieldsBase GetDataFields(DependencyObject d) => (DataFieldsBase)d.GetValue(DataFieldsProperty);

        public static void SetDataFields(DependencyObject d, DataFieldsBase value) => d.SetValue(DataFieldsProperty, value);
    }
}