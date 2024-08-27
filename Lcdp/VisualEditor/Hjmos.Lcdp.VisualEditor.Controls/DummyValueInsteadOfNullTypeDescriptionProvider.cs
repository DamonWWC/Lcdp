using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;

namespace Hjmos.Lcdp.VisualEditor.Controls
{

    /// <summary>
    /// 通过使用TypeDescriptionProvider，我们可以拦截所有使用PropertyDescriptor的属性访问
    /// WpfDesign.XamlDom使用PropertyDescriptor来访问属性(附加的属性除外)，甚至DesignItemProperty/XamlProperty也是如此
    /// 当实际值是虚拟值时，ValueOnInstance将报告为空
    /// </summary>
    public sealed class DummyValueInsteadOfNullTypeDescriptionProvider : TypeDescriptionProvider
    {
        private readonly string _propertyName;
        private readonly object _dummyValue;

        public DummyValueInsteadOfNullTypeDescriptionProvider(TypeDescriptionProvider existingProvider, string propertyName, object dummyValue) : base(existingProvider)
        {
            this._propertyName = propertyName;
            this._dummyValue = dummyValue;
        }

        /// <inheritdoc/>
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance) => new ShadowTypeDescriptor(this, base.GetTypeDescriptor(objectType, instance));

        /// <summary>
        /// 阴影类型描述器
        /// TODO: 这里的阴影是指图形的阴影，还是指隐藏的属性或者假的属性？
        /// </summary>
        private sealed class ShadowTypeDescriptor : CustomTypeDescriptor
        {
            private readonly DummyValueInsteadOfNullTypeDescriptionProvider _parent;

            public ShadowTypeDescriptor(DummyValueInsteadOfNullTypeDescriptionProvider parent, ICustomTypeDescriptor existingDescriptor) : base(existingDescriptor) => this._parent = parent;

            public override PropertyDescriptorCollection GetProperties() => Filter(base.GetProperties());

            public override PropertyDescriptorCollection GetProperties(Attribute[] attributes) => Filter(base.GetProperties(attributes));

            private PropertyDescriptorCollection Filter(PropertyDescriptorCollection properties)
            {
                PropertyDescriptor property = properties[_parent._propertyName];
                if (property != null)
                {
                    if ((properties as IDictionary).IsReadOnly)
                    {
                        properties = new PropertyDescriptorCollection(properties.Cast<PropertyDescriptor>().ToArray());
                    }
                    properties.Remove(property);
                    properties.Add(new ShadowPropertyDescriptor(_parent, property));
                }
                return properties;
            }
        }

        /// <summary>
        /// 阴影属性描述器
        /// </summary>
        private sealed class ShadowPropertyDescriptor : PropertyDescriptor
        {
            readonly DummyValueInsteadOfNullTypeDescriptionProvider _parent;
            readonly PropertyDescriptor _baseDescriptor;

            public ShadowPropertyDescriptor(DummyValueInsteadOfNullTypeDescriptionProvider parent, PropertyDescriptor existingDescriptor) : base(existingDescriptor)
            {
                this._parent = parent;
                this._baseDescriptor = existingDescriptor;
            }

            public override Type ComponentType => _baseDescriptor.ComponentType;

            public override bool IsReadOnly => _baseDescriptor.IsReadOnly;

            public override Type PropertyType => _baseDescriptor.PropertyType;

            public override bool CanResetValue(object component) => _baseDescriptor.CanResetValue(component);

            public override object GetValue(object component)
            {
                object value = _baseDescriptor.GetValue(component);
                return value == _parent._dummyValue ? null : value;
            }

            public override void ResetValue(object component) => _baseDescriptor.SetValue(component, _parent._dummyValue);

            public override void SetValue(object component, object value) => _baseDescriptor.SetValue(component, value ?? _parent._dummyValue);

            public override bool ShouldSerializeValue(object component)
            {
                return _baseDescriptor.ShouldSerializeValue(component) && _baseDescriptor.GetValue(component) != _parent._dummyValue;
            }
        }
    }
}
