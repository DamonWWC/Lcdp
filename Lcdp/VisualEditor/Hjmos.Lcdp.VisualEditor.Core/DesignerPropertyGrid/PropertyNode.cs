using System;
using System.Linq;
using System.ComponentModel;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Markup;
using Hjmos.Lcdp.VisualEditor.Core.Services;

namespace Hjmos.Lcdp.VisualEditor.Core.DesignerPropertyGrid
{
    /// <summary>
    /// 属性网格的视图-模型类。
    /// </summary>
    public class PropertyNode : INotifyPropertyChanged
    {
        static readonly object Unset = new();

        /// <summary>
        /// Gets the properties that are presented by this node.
        /// This might be multiple properties if multiple controls are selected.
        /// 获取此节点显示的属性。
        /// 如果选择了多个控件，这可能是多个属性。
        /// </summary>
        public ReadOnlyCollection<DesignItemProperty> Properties { get; private set; }

        bool raiseEvents = true;
        bool hasStringConverter;

        /// <summary>获取属性的名称</summary>
        public string Name
        {
            get
            {
                var dp = FirstProperty.DependencyProperty;
                if (dp != null)
                {
                    var dpd = DependencyPropertyDescriptor.FromProperty(dp, FirstProperty.DesignItem.ComponentType);
                    if (dpd.IsAttached)
                    {
                        // Will return the attached property name in the form of <DeclaringType>.<PropertyName>
                        return dpd.Name;
                    }
                }

                return FirstProperty.Name;
            }
        }

        /// <summary>获取此属性节点是否表示事件</summary>
        public bool IsEvent { get { return FirstProperty.IsEvent; } }

        /// <summary>获取与此属性集关联的设计上下文</summary>
        public DesignContext Context { get { return FirstProperty.DesignItem.Context; } }

        /// <summary>获取与此属性集关联的服务容器</summary>
        public ServiceContainer Services { get { return FirstProperty.DesignItem.Services; } }

        /// <summary>获取编辑此属性的编辑器控件</summary>
        public FrameworkElement Editor { get; private set; }

        /// <summary>获取第一个属性(等价于Properties[0])</summary>
        public DesignItemProperty FirstProperty { get { return Properties[0]; } }

        /// <summary>
        /// For nested property nodes, gets the parent node.
        /// </summary>
        public PropertyNode Parent { get; private set; }

        /// <summary>
        /// For nested property nodes, gets the level of this node.
        /// </summary>
        public int Level { get; private set; }

        /// <summary>
        /// Gets the category of this node.
        /// </summary>
        public Category Category { get; set; }

        /// <summary>
        /// Gets the list of child nodes.
        /// </summary>
        public ObservableCollection<PropertyNode> Children { get; private set; }

        /// <summary>
        /// Gets the list of advanced child nodes (not visible by default).
        /// </summary>
        public ObservableCollection<PropertyNode> MoreChildren { get; private set; }

        bool isExpanded;

        /// <summary>
        /// Gets whether this property node is currently expanded.
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return isExpanded;
            }
            set
            {
                isExpanded = value;
                UpdateChildren();
                RaisePropertyChanged("IsExpanded");
            }
        }

        /// <summary>
        /// Gets whether this property node has children.
        /// </summary>
        public bool HasChildren
        {
            get { return Children.Count > 0 || MoreChildren.Count > 0; }
        }

        /// <summary>
        /// Gets the description object using the IPropertyDescriptionService.
        /// </summary>
        public object Description
        {
            get
            {
                IPropertyDescriptionService s = Services.GetService<IPropertyDescriptionService>();
                if (s != null)
                {
                    return s.GetDescription(FirstProperty);
                }
                return null;
            }
        }

        /// <summary>
        /// Gets/Sets the value of this property.
        /// </summary>
        public object DesignerValue
        {
            get
            {
                if (IsAmbiguous)
                    return null;
                var result = FirstProperty.DesignerValue;
                if (result == DependencyProperty.UnsetValue)
                    return null;
                return result;
            }
            set
            {
                SetValueCore(value);
            }
        }

        /// <summary>
        /// Gets/Sets the value of this property.
        /// </summary>
        public object Value
        {
            get
            {
                if (IsAmbiguous) return null;
                var result = FirstProperty.ValueOnInstance;
                if (result == DependencyProperty.UnsetValue) return null;
                return result;
            }
            set
            {
                SetValueCore(value);
            }
        }

        /// <summary>
        /// Gets/Sets the value of this property in string form
        /// </summary>
        public string ValueString
        {
            get
            {
                if (ValueItem == null || ValueItem.Component is MarkupExtension)
                {
                    if (DesignerValue == null) return null;
                    if (hasStringConverter)
                    {
                        return FirstProperty.TypeConverter.ConvertToInvariantString(DesignerValue);
                    }
                    return "(" + DesignerValue.GetType().Name + ")";
                }
                return "(" + ValueItem.ComponentType.Name + ")";
            }
            set
            {
                // make sure we only catch specific exceptions
                // and/or show the error message to the user
                //try {
                DesignerValue = FirstProperty.TypeConverter.ConvertFromInvariantString(value);
                //} catch {
                //	OnValueOnInstanceChanged();
                //}
            }
        }

        /// <summary>
        /// Gets whether the property node is enabled for editing.
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return ValueItem == null && hasStringConverter;
            }
        }

        /// <summary>
        /// Gets whether this property was set locally.
        /// </summary>
        public bool IsSet
        {
            get
            {
                foreach (DesignItemProperty p in Properties)
                {
                    if (p.IsSet) return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Gets the color of the name.
        /// Depends on the type of the value (binding/resource/etc.)
        /// </summary>
        public Brush NameForeground
        {
            get
            {
                if (ValueItem != null)
                {
                    object component = ValueItem.Component;
                    if (component is BindingBase)
                        return Brushes.DarkGoldenrod;
                    if (component is StaticResourceExtension || component is DynamicResourceExtension)
                        return Brushes.DarkGreen;
                }
                return SystemColors.WindowTextBrush;
            }
        }

        /// <summary>
        /// Returns the DesignItem that owns the property (= the DesignItem that is currently selected).
        /// Returns null if multiple DesignItems are selected.
        /// </summary>
        public DesignItem ValueItem
        {
            get
            {
                if (Properties.Count == 1)
                {
                    return FirstProperty.Value;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets whether the property value is ambiguous (multiple controls having different values are selected).
        /// </summary>
        public bool IsAmbiguous
        {
            get
            {
                foreach (DesignItemProperty p in Properties)
                {
                    if (p != FirstProperty && !Equals(p.ValueOnInstance, FirstProperty.ValueOnInstance))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private bool isVisible;

        /// <summary>
        /// Gets/Sets whether the property is visible.
        /// </summary>
        public bool IsVisible
        {
            get => isVisible;
            set
            {
                isVisible = value;
                RaisePropertyChanged("IsVisible");
            }
        }

        /// <summary>
        /// Gets whether resetting the property is possible.
        /// </summary>
        public bool CanReset => IsSet;

        /// <summary>
        /// Resets the property.
        /// </summary>
        public void Reset() => SetValueCore(Unset);

        /// <summary>
        /// Replaces the value of this node with a new binding.
        /// </summary>
        public void CreateBinding()
        {
            DesignerValue = new Binding();
            IsExpanded = true;
        }

        private void SetValueCore(object value)
        {
            raiseEvents = false;
            if (value == Unset)
            {
                foreach (DesignItemProperty p in Properties)
                {
                    p.Reset();
                }
            }
            else
            {
                foreach (DesignItemProperty p in Properties)
                {
                    if (p.DesignItem.Component is TextBlock && p.DependencyProperty == TextBlock.TextProperty)
                    {
                        p.DesignItem.ContentProperty.CollectionElements.Clear();
                    }
                    p.SetValue(value);
                }
            }
            raiseEvents = true;
            OnValueChanged();
        }

        void OnValueChanged()
        {
            RaisePropertyChanged("IsSet");
            RaisePropertyChanged("Value");
            RaisePropertyChanged("DesignerValue");
            RaisePropertyChanged("ValueString");
            RaisePropertyChanged("IsAmbiguous");
            RaisePropertyChanged("FontWeight");
            RaisePropertyChanged("IsEnabled");
            RaisePropertyChanged("NameForeground");

            UpdateChildren();
        }

        private void OnValueOnInstanceChanged()
        {
            RaisePropertyChanged("Value");
            RaisePropertyChanged("DesignerValue");
            RaisePropertyChanged("ValueString");
        }

        /// <summary>
        /// Creates a new PropertyNode instance.
        /// </summary>
        public PropertyNode()
        {
            Children = new ObservableCollection<PropertyNode>();
            MoreChildren = new ObservableCollection<PropertyNode>();
        }

        private PropertyNode(DesignItemProperty[] properties, PropertyNode parent) : this()
        {
            this.Parent = parent;
            this.Level = parent == null ? 0 : parent.Level + 1;
            Load(properties);
        }

        /// <summary>
        /// Initializes this property node with the specified properties.
        /// </summary>
        public void Load(DesignItemProperty[] properties)
        {
            if (this.Properties != null)
            {
                // detach events from old properties
                foreach (DesignItemProperty property in this.Properties)
                {
                    property.ValueChanged -= new EventHandler(property_ValueChanged);
                    property.ValueOnInstanceChanged -= new EventHandler(property_ValueOnInstanceChanged);
                }
            }

            this.Properties = new ReadOnlyCollection<DesignItemProperty>(properties);

            if (Editor == null)
                Editor = EditorManager.CreateEditor(FirstProperty);

            foreach (DesignItemProperty property in properties)
            {
                property.ValueChanged += new EventHandler(property_ValueChanged);
                property.ValueOnInstanceChanged += new EventHandler(property_ValueOnInstanceChanged);
            }

            hasStringConverter =
                FirstProperty.TypeConverter.CanConvertFrom(typeof(string)) &&
                FirstProperty.TypeConverter.CanConvertTo(typeof(string));

            OnValueChanged();
        }

        void property_ValueOnInstanceChanged(object sender, EventArgs e)
        {
            if (raiseEvents) OnValueOnInstanceChanged();
        }

        void property_ValueChanged(object sender, EventArgs e)
        {
            if (raiseEvents) OnValueChanged();
        }

        private void UpdateChildren()
        {
            Children.Clear();
            MoreChildren.Clear();

            if (Parent == null || Parent.IsExpanded)
            {
                if (ValueItem != null)
                {
                    IComponentPropertyService service = ValueItem.Services.GetService<IComponentPropertyService>();
                    System.Collections.Generic.IEnumerable<PropertyNode> list = service.GetAvailableProperties(ValueItem)
                        .OrderBy(d => d.Name)
                        .Select(d => new PropertyNode(new[] { ValueItem.Properties.GetProperty(d) }, this));

                    foreach (PropertyNode node in list)
                    {
                        if (Metadata.IsBrowsable(node.FirstProperty))
                        {
                            node.IsVisible = true;
                            if (Metadata.IsPopularProperty(node.FirstProperty))
                            {
                                Children.Add(node);
                            }
                            else
                            {
                                MoreChildren.Add(node);
                            }
                        }
                    }
                }
            }

            RaisePropertyChanged("HasChildren");
        }

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Occurs when a property has changed. Used to support WPF data binding.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }
}
