using Hjmos.Lcdp.VisualEditor.Controls.Extensions;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shell;

namespace Hjmos.Lcdp.VisualEditor.Controls.DesignerControls
{
    /// <summary>
    /// A custom control that imitates the properties of <see cref="Window"/>, but is not a top-level control.
    /// </summary>
    public class WindowClone : ContentControl
    {
        static WindowClone()
        {
            //This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowClone), new FrameworkPropertyMetadata(typeof(WindowClone)));

            IsTabStopProperty.OverrideMetadata(typeof(WindowClone), new FrameworkPropertyMetadata(SharedInstances.BoxedFalse));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(WindowClone), new FrameworkPropertyMetadata(SharedInstances<KeyboardNavigationMode>.Box(KeyboardNavigationMode.Cycle)));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(WindowClone), new FrameworkPropertyMetadata(SharedInstances<KeyboardNavigationMode>.Box(KeyboardNavigationMode.Cycle)));
            KeyboardNavigation.ControlTabNavigationProperty.OverrideMetadata(typeof(WindowClone), new FrameworkPropertyMetadata(SharedInstances<KeyboardNavigationMode>.Box(KeyboardNavigationMode.Cycle)));
            FocusManager.IsFocusScopeProperty.OverrideMetadata(typeof(WindowClone), new FrameworkPropertyMetadata(SharedInstances.BoxedTrue));
        }

        /// <summary>
        /// This property has no effect. (for compatibility with <see cref="Window"/> only).
        /// </summary>
        public bool AllowsTransparency
        {
            get => (bool)GetValue(Window.AllowsTransparencyProperty);
            set => SetValue(Window.AllowsTransparencyProperty, SharedInstances.Box(value));
        }

        /// <summary>
        /// This property has no effect. (for compatibility with <see cref="Window"/> only).
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), TypeConverter(typeof(DialogResultConverter))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value")]
        public bool? DialogResult => null;

        public ImageSource Icon
        {
            get => (ImageSource)GetValue(Window.IconProperty);
            set => SetValue(Window.IconProperty, value);
        }

        /// <summary>
        /// This property has no effect. (for compatibility with <see cref="Window"/> only).
        /// </summary>
        [TypeConverter(typeof(LengthConverter))]
        public double Left
        {
            get => (double)GetValue(Window.LeftProperty);
            set => SetValue(Window.LeftProperty, value);
        }

        /// <summary>
        /// This property has no effect. (for compatibility with <see cref="Window"/> only).
        /// </summary>
        public Window Owner { get; set; }

        /// <summary>
        /// Gets or sets the resize mode.
        /// </summary>
        public ResizeMode ResizeMode
        {
            get => (ResizeMode)GetValue(Window.ResizeModeProperty);
            set => SetValue(Window.ResizeModeProperty, value);
        }

        /// <summary>
        /// This property has no effect. (for compatibility with <see cref="Window"/> only).
        /// </summary>
        public bool ShowActivated
        {
            get => (bool)GetValue(Window.ShowActivatedProperty);
            set => SetValue(Window.ShowActivatedProperty, value);
        }

        /// <summary>
        /// This property has no effect. (for compatibility with <see cref="Window"/> only).
        /// </summary>
        public bool ShowInTaskbar
        {
            get => (bool)GetValue(Window.ShowInTaskbarProperty);
            set => SetValue(Window.ShowInTaskbarProperty, SharedInstances.Box(value));
        }

        /// <summary>
        /// Gets or sets a value that specifies whether a window will automatically size itself to fit the size of its content.
        /// </summary>
        public SizeToContent SizeToContent
        {
            get => (SizeToContent)GetValue(Window.SizeToContentProperty);
            set => SetValue(Window.SizeToContentProperty, value);
        }

        /// <summary>
        /// This property has no effect. (for compatibility with <see cref="Window"/> only).
        /// </summary>
        public TaskbarItemInfo TaskbarItemInfo
        {
            get => (TaskbarItemInfo)GetValue(Window.TaskbarItemInfoProperty);
            set => SetValue(Window.TaskbarItemInfoProperty, value);
        }

        /// <summary>
        /// The title to display in the Window's title bar.
        /// </summary>
        public string Title
        {
            get => (string)GetValue(Window.TitleProperty);
            set => SetValue(Window.TitleProperty, value);
        }

        /// <summary>
        /// This property has no effect. (for compatibility with <see cref="Window"/> only).
        /// </summary>
        [TypeConverter(typeof(LengthConverter))]
        public double Top
        {
            get => (double)GetValue(Window.TopProperty);
            set => SetValue(Window.TopProperty, value);
        }

        /// <summary>
        /// This property has no effect. (for compatibility with <see cref="Window"/> only).
        /// </summary>
        public bool Topmost
        {
            get => (bool)GetValue(Window.TopmostProperty);
            set => SetValue(Window.TopmostProperty, SharedInstances.Box(value));
        }

        /// <summary>
        /// This property has no effect. (for compatibility with <see cref="Window"/> only).
        /// </summary>
        public WindowStartupLocation WindowStartupLocation { get; set; }

        /// <summary>
        /// This property has no effect. (for compatibility with <see cref="Window"/> only).
        /// </summary>
        public WindowState WindowState
        {
            get => (WindowState)GetValue(Window.WindowStateProperty);
            set => SetValue(Window.WindowStateProperty, value);
        }

        /// <summary>
        /// This property has no effect. (for compatibility with <see cref="Window"/> only).
        /// </summary>
        public WindowStyle WindowStyle
        {
            get => (WindowStyle)GetValue(Window.WindowStyleProperty);
            set => SetValue(Window.WindowStyleProperty, value);
        }

        /// <summary>
        /// This event is never raised. (for compatibility with <see cref="Window"/> only).
        /// </summary>
        public event EventHandler Activated;

        /// <summary>
        /// This event is never raised. (for compatibility with <see cref="Window"/> only).
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// This event is never raised. (for compatibility with <see cref="Window"/> only).
        /// </summary>
        public event EventHandler Closing;

        /// <summary>
        /// This event is never raised. (for compatibility with <see cref="Window"/> only).
        /// </summary>
        public event EventHandler ContentRendered;

        /// <summary>
        /// This event is never raised. (for compatibility with <see cref="Window"/> only).
        /// </summary>
        public event EventHandler Deactivated;

        /// <summary>
        /// This event is never raised. (for compatibility with <see cref="Window"/> only).
        /// </summary>
        public event EventHandler LocationChanged;

        /// <summary>
        /// This event is never raised. (for compatibility with <see cref="Window"/> only).
        /// </summary>
        public event EventHandler SourceInitialized;

        /// <summary>
        /// This event is never raised. (for compatibility with <see cref="Window"/> only).
        /// </summary>
        public event EventHandler StateChanged;
    }

    /// <summary>
    /// A <see cref="CustomInstanceFactory"/> for <see cref="Window"/>
    /// (and derived classes, unless they specify their own <see cref="CustomInstanceFactory"/>).
    /// </summary>
    [ExtensionFor(typeof(Window))]
    public class WindowCloneExtension : CustomInstanceFactory
    {
        /// <summary>
        /// Used to create instances of <see cref="WindowClone"/>.
        /// </summary>
        public override object CreateInstance(Type type, params object[] arguments)
        {
            Debug.Assert(arguments.Length == 0);
            return new WindowClone();
        }
    }
}
