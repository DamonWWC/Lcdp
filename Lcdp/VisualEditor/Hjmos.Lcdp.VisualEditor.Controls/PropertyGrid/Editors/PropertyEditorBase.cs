using System.Windows;
using System.Windows.Data;

namespace Hjmos.Lcdp.VisualEditor.Controls
{
    public abstract class PropertyEditorBase : DependencyObject
    {
        public abstract FrameworkElement CreateElement(PropertyItem propertyItem);

        public virtual void CreateBinding(PropertyItem propertyItem, DependencyObject element)
        {
            // 如果是附加属性，加上()
            string propertyName = propertyItem.PropertyName.Contains(".") ? $"({propertyItem.PropertyName})" : propertyItem.PropertyName;
            Binding binding;
            if (propertyItem.SourceProperty != null)
            {
                binding = new()
                {
                    Path = new PropertyPath(propertyItem.SourceProperty),
                    Source = propertyItem.Value,
                    Mode = GetBindingMode(propertyItem),
                    UpdateSourceTrigger = GetUpdateSourceTrigger(propertyItem),
                    Converter = GetConverter(propertyItem)
                };
            }
            else
            {
                binding = new(propertyName)
                {
                    Source = propertyItem.Value,
                    Mode = GetBindingMode(propertyItem),
                    UpdateSourceTrigger = GetUpdateSourceTrigger(propertyItem),
                    Converter = GetConverter(propertyItem)
                };

            }

            BindingOperations.SetBinding(element, GetDependencyProperty(), binding);
        }

        public abstract DependencyProperty GetDependencyProperty();

        public virtual BindingMode GetBindingMode(PropertyItem propertyItem) => propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;

        public virtual UpdateSourceTrigger GetUpdateSourceTrigger(PropertyItem propertyItem) => UpdateSourceTrigger.PropertyChanged;

        protected virtual IValueConverter GetConverter(PropertyItem propertyItem) => null;
    }
}
