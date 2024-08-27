using Hjmos.Lcdp.VisualEditor.Controls.Entities;
using System.Collections.ObjectModel;
using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Controls.Attached
{

    public static class ParameterMappingAttached
    {
        public static readonly DependencyProperty ParameterMappingProperty = DependencyProperty.RegisterAttached("ParameterMapping", typeof(ObservableCollection<ParameterMapping>), typeof(ParameterMappingAttached),
            new FrameworkPropertyMetadata(default, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.NotDataBindable));

        public static ObservableCollection<ParameterMapping> GetParameterMapping(DependencyObject d) => (ObservableCollection<ParameterMapping>)d.GetValue(ParameterMappingProperty);

        public static void SetParameterMapping(DependencyObject d, ObservableCollection<ParameterMapping> value) => d.SetValue(ParameterMappingProperty, value);
    }
}