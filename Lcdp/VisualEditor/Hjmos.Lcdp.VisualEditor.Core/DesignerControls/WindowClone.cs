using Hjmos.Lcdp.VisualEditor.Core.ItemExtensions;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shell;

namespace Hjmos.Lcdp.VisualEditor.Core.DesignerControls
{
    /// <summary>
    /// 模仿<see cref="Window"/>属性的自定义控件，但不是顶级控件。
    /// </summary>
    public class WindowClone : ContentControl
    {
        static WindowClone()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowClone), new FrameworkPropertyMetadata(typeof(WindowClone)));

            IsTabStopProperty.OverrideMetadata(typeof(WindowClone), new FrameworkPropertyMetadata(SharedInstances.BoxedFalse));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(WindowClone), new FrameworkPropertyMetadata(SharedInstances<KeyboardNavigationMode>.Box(KeyboardNavigationMode.Cycle)));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(WindowClone), new FrameworkPropertyMetadata(SharedInstances<KeyboardNavigationMode>.Box(KeyboardNavigationMode.Cycle)));
            KeyboardNavigation.ControlTabNavigationProperty.OverrideMetadata(typeof(WindowClone), new FrameworkPropertyMetadata(SharedInstances<KeyboardNavigationMode>.Box(KeyboardNavigationMode.Cycle)));
            FocusManager.IsFocusScopeProperty.OverrideMetadata(typeof(WindowClone), new FrameworkPropertyMetadata(SharedInstances.BoxedTrue));
        }

        /// <summary>
        /// 此属性无效 (只是为了和<see cref="Window"/>兼容)。
        /// </summary>
        public bool AllowsTransparency
        {
            get => (bool)GetValue(Window.AllowsTransparencyProperty);
            set => SetValue(Window.AllowsTransparencyProperty, SharedInstances.Box(value));
        }

        /// <summary>
        /// 此属性无效 (只是为了和<see cref="Window"/>兼容)。
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), TypeConverter(typeof(DialogResultConverter))]
        public bool? DialogResult => null;

        public ImageSource Icon
        {
            get => (ImageSource)GetValue(Window.IconProperty);
            set => SetValue(Window.IconProperty, value);
        }

        /// <summary>
        /// 此属性无效 (只是为了和<see cref="Window"/>兼容)。
        /// </summary>
        [TypeConverter(typeof(LengthConverter))]
        public double Left
        {
            get => (double)GetValue(Window.LeftProperty);
            set => SetValue(Window.LeftProperty, value);
        }

        /// <summary>
        /// 此属性无效 (只是为了和<see cref="Window"/>兼容)。
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
        /// 此属性无效 (只是为了和<see cref="Window"/>兼容)。
        /// </summary>
        public bool ShowActivated
        {
            get => (bool)GetValue(Window.ShowActivatedProperty);
            set => SetValue(Window.ShowActivatedProperty, value);
        }

        /// <summary>
        /// 此属性无效 (只是为了和<see cref="Window"/>兼容)。
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
        /// 此属性无效 (只是为了和<see cref="Window"/>兼容)。
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
        /// 此属性无效 (只是为了和<see cref="Window"/>兼容)。
        /// </summary>
        [TypeConverter(typeof(LengthConverter))]
        public double Top
        {
            get => (double)GetValue(Window.TopProperty);
            set => SetValue(Window.TopProperty, value);
        }

        /// <summary>
        /// 此属性无效 (只是为了和<see cref="Window"/>兼容)。
        /// </summary>
        public bool Topmost
        {
            get => (bool)GetValue(Window.TopmostProperty);
            set => SetValue(Window.TopmostProperty, SharedInstances.Box(value));
        }

        /// <summary>
        /// 此属性无效 (只是为了和<see cref="Window"/>兼容)。
        /// </summary>
        public WindowStartupLocation WindowStartupLocation { get; set; }

        /// <summary>
        /// 此属性无效 (只是为了和<see cref="Window"/>兼容)。
        /// </summary>
        public WindowState WindowState
        {
            get => (WindowState)GetValue(Window.WindowStateProperty);
            set => SetValue(Window.WindowStateProperty, value);
        }

        /// <summary>
        /// 此属性无效 (只是为了和<see cref="Window"/>兼容)。
        /// </summary>
        public WindowStyle WindowStyle
        {
            get => (WindowStyle)GetValue(Window.WindowStyleProperty);
            set => SetValue(Window.WindowStyleProperty, value);
        }

        /// <summary>
        /// 此事件不被触发(只是为了和<see cref="Window"/>兼容)。
        /// </summary>
        public event EventHandler Activated;

        /// <summary>
        /// 此事件不被触发(只是为了和<see cref="Window"/>兼容)。
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// 此事件不被触发(只是为了和<see cref="Window"/>兼容)。
        /// </summary>
        public event EventHandler Closing;

        /// <summary>
        /// 此事件不被触发(只是为了和<see cref="Window"/>兼容)。
        /// </summary>
        public event EventHandler ContentRendered;

        /// <summary>
        /// 此事件不被触发(只是为了和<see cref="Window"/>兼容)。
        /// </summary>
        public event EventHandler Deactivated;

        /// <summary>
        /// 此事件不被触发(只是为了和<see cref="Window"/>兼容)。
        /// </summary>
        public event EventHandler LocationChanged;

        /// <summary>
        /// 此事件不被触发(只是为了和<see cref="Window"/>兼容)。
        /// </summary>
        public event EventHandler SourceInitialized;

        /// <summary>
        /// 此事件不被触发(只是为了和<see cref="Window"/>兼容)。
        /// </summary>
        public event EventHandler StateChanged;
    }
}
