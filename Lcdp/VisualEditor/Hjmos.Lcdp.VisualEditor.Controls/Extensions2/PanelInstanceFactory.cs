using Hjmos.Lcdp.VisualEditor.Controls.Extensions;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Controls.Extensions2
{
    /// <summary>
    /// Instance factory used to create Panel instances.
    /// Sets the panels Brush to a transparent brush, and modifies the panel's type descriptor so that
    /// the property value is reported as null when the transparent brush is used, and
    /// setting the Brush to null actually restores the transparent brush.
    /// </summary>
    [ExtensionFor(typeof(Panel))]
    public sealed class PanelInstanceFactory : CustomInstanceFactory
    {
        Brush _transparentBrush = new SolidColorBrush(Colors.Transparent);

        /// <summary>
        /// Creates an instance of the specified type, passing the specified arguments to its constructor.
        /// </summary>
        public override object CreateInstance(Type type, params object[] arguments)
        {
            object instance = base.CreateInstance(type, arguments);
            Panel panel = instance as Panel;
            if (panel != null)
            {
                if (panel.Background == null)
                {
                    panel.Background = _transparentBrush;
                }
                TypeDescriptionProvider provider = new DummyValueInsteadOfNullTypeDescriptionProvider(
                    TypeDescriptor.GetProvider(panel), "Background", _transparentBrush);
                TypeDescriptor.AddProvider(provider, panel);
            }
            return instance;
        }
    }

    [ExtensionFor(typeof(HeaderedContentControl))]
    public sealed class HeaderedContentControlInstanceFactory : CustomInstanceFactory
    {
        Brush _transparentBrush = new SolidColorBrush(Colors.Transparent);

        /// <summary>
        /// Creates an instance of the specified type, passing the specified arguments to its constructor.
        /// </summary>
        public override object CreateInstance(Type type, params object[] arguments)
        {
            object instance = base.CreateInstance(type, arguments);
            Control control = instance as Control;
            if (control != null)
            {
                if (control.Background == null)
                {
                    control.Background = _transparentBrush;
                }
                TypeDescriptionProvider provider = new DummyValueInsteadOfNullTypeDescriptionProvider(
                    TypeDescriptor.GetProvider(control), "Background", _transparentBrush);
                TypeDescriptor.AddProvider(provider, control);
            }
            return instance;
        }
    }

    [ExtensionFor(typeof(ItemsControl))]
    public sealed class TransparentControlsInstanceFactory : CustomInstanceFactory
    {
        Brush _transparentBrush = new SolidColorBrush(Colors.Transparent);

        /// <summary>
        /// Creates an instance of the specified type, passing the specified arguments to its constructor.
        /// </summary>
        public override object CreateInstance(Type type, params object[] arguments)
        {
            object instance = base.CreateInstance(type, arguments);
            Control control = instance as Control;
            if (control != null && (
                type == typeof(ItemsControl)))
            {
                if (control.Background == null)
                {
                    control.Background = _transparentBrush;
                }

                TypeDescriptionProvider provider = new DummyValueInsteadOfNullTypeDescriptionProvider(
                    TypeDescriptor.GetProvider(control), "Background", _transparentBrush);
                TypeDescriptor.AddProvider(provider, control);
            }
            return instance;
        }
    }

    [ExtensionFor(typeof(Border))]
    public sealed class BorderInstanceFactory : CustomInstanceFactory
    {
        Brush _transparentBrush = new SolidColorBrush(Colors.Transparent);

        /// <summary>
        /// Creates an instance of the specified type, passing the specified arguments to its constructor.
        /// </summary>
        public override object CreateInstance(Type type, params object[] arguments)
        {
            object instance = base.CreateInstance(type, arguments);
            Border panel = instance as Border;
            if (panel != null)
            {
                if (panel.Background == null)
                {
                    panel.Background = _transparentBrush;
                }
                TypeDescriptionProvider provider = new DummyValueInsteadOfNullTypeDescriptionProvider(
                    TypeDescriptor.GetProvider(panel), "Background", _transparentBrush);
                TypeDescriptor.AddProvider(provider, panel);
            }
            return instance;
        }
    }
}
