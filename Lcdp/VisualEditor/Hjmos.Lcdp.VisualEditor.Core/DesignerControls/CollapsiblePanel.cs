using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Hjmos.Lcdp.VisualEditor.Core.DesignerControls
{
    /// <summary>
    /// 允许动态折叠面板的内容。
    /// </summary>
    public class CollapsiblePanel : ContentControl
    {
        static CollapsiblePanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CollapsiblePanel), new FrameworkPropertyMetadata(typeof(CollapsiblePanel)));
            FocusableProperty.OverrideMetadata(typeof(CollapsiblePanel), new FrameworkPropertyMetadata(SharedInstances.BoxedFalse));
        }

        public static readonly DependencyProperty IsCollapsedProperty =
            DependencyProperty.Register("IsCollapsed", typeof(bool), typeof(CollapsiblePanel),
                new UIPropertyMetadata(false, new PropertyChangedCallback(OnIsCollapsedChanged)));

        public bool IsCollapsed
        {
            get => (bool)GetValue(IsCollapsedProperty);
            set => SetValue(IsCollapsedProperty, value);
        }

        public static readonly DependencyProperty CollapseOrientationProperty =
            DependencyProperty.Register("CollapseOrientation", typeof(Orientation), typeof(CollapsiblePanel), new FrameworkPropertyMetadata(Orientation.Vertical));

        public Orientation CollapseOrientation
        {
            get => (Orientation)GetValue(CollapseOrientationProperty);
            set => SetValue(CollapseOrientationProperty, value);
        }

        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(CollapsiblePanel), new UIPropertyMetadata(TimeSpan.FromMilliseconds(250)));

        /// <summary>
        /// The duration in milliseconds of the animation.
        /// 以毫秒为单位的动画持续时间。
        /// </summary>
        public TimeSpan Duration
        {
            get => (TimeSpan)GetValue(DurationProperty);
            set => SetValue(DurationProperty, value);
        }

        protected internal static readonly DependencyProperty AnimationProgressProperty = DependencyProperty.Register(
            "AnimationProgress", typeof(double), typeof(CollapsiblePanel),
            new FrameworkPropertyMetadata(SharedInstances.BoxedDouble1));

        /// <summary>
        /// Value between 0 and 1 specifying how far the animation currently is.
        /// 0到1之间的值，指定当前动画的距离。
        /// </summary>
        protected internal double AnimationProgress
        {
            get => (double)GetValue(AnimationProgressProperty);
            set => SetValue(AnimationProgressProperty, value);
        }

        protected internal static readonly DependencyProperty AnimationProgressXProperty = DependencyProperty.Register(
            "AnimationProgressX", typeof(double), typeof(CollapsiblePanel),
            new FrameworkPropertyMetadata(SharedInstances.BoxedDouble1));

        /// <summary>
        /// Value between 0 and 1 specifying how far the animation currently is.
        /// 0到1之间的值，指定当前动画的距离。
        /// </summary>
        protected internal double AnimationProgressX
        {
            get => (double)GetValue(AnimationProgressXProperty);
            set => SetValue(AnimationProgressXProperty, value);
        }

        protected internal static readonly DependencyProperty AnimationProgressYProperty = DependencyProperty.Register(
            "AnimationProgressY", typeof(double), typeof(CollapsiblePanel),
            new FrameworkPropertyMetadata(SharedInstances.BoxedDouble1));

        /// <summary>
        /// Value between 0 and 1 specifying how far the animation currently is.
        /// 0到1之间的值，指定当前动画的距离。
        /// </summary>
        protected internal double AnimationProgressY
        {
            get => (double)GetValue(AnimationProgressYProperty);
            set => SetValue(AnimationProgressYProperty, value);
        }

        private static void OnIsCollapsedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as CollapsiblePanel).SetupAnimation((bool)e.NewValue);

        void SetupAnimation(bool isCollapsed)
        {
            if (this.IsLoaded)
            {
                // If the animation is already running, calculate remaining portion of the time
                // 如果动画已经在运行，计算剩余的时间
                double currentProgress = AnimationProgress;
                if (!isCollapsed)
                {
                    currentProgress = 1.0 - currentProgress;
                }

                DoubleAnimation animation = new();
                animation.To = isCollapsed ? 0.0 : 1.0;
                animation.Duration = TimeSpan.FromSeconds(Duration.TotalSeconds * currentProgress);
                animation.FillBehavior = FillBehavior.HoldEnd;

                this.BeginAnimation(AnimationProgressProperty, animation);
                if (CollapseOrientation == Orientation.Horizontal)
                {
                    this.BeginAnimation(AnimationProgressXProperty, animation);
                    this.AnimationProgressY = 1.0;
                }
                else
                {
                    this.AnimationProgressX = 1.0;
                    this.BeginAnimation(AnimationProgressYProperty, animation);
                }
            }
            else
            {
                this.AnimationProgress = isCollapsed ? 0.0 : 1.0;
                this.AnimationProgressX = (CollapseOrientation == Orientation.Horizontal) ? this.AnimationProgress : 1.0;
                this.AnimationProgressY = (CollapseOrientation == Orientation.Vertical) ? this.AnimationProgress : 1.0;
            }
        }
    }

    sealed class CollapsiblePanelProgressToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double)
                return (double)value > 0 ? Visibility.Visible : Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
    }

    public class SelfCollapsingPanel : CollapsiblePanel
    {
        public static readonly DependencyProperty CanCollapseProperty =
            DependencyProperty.Register("CanCollapse", typeof(bool), typeof(SelfCollapsingPanel),
                                        new FrameworkPropertyMetadata(SharedInstances.BoxedFalse, new PropertyChangedCallback(OnCanCollapseChanged)));

        public bool CanCollapse
        {
            get => (bool)GetValue(CanCollapseProperty);
            set => SetValue(CanCollapseProperty, value);
        }

        static void OnCanCollapseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SelfCollapsingPanel panel = (SelfCollapsingPanel)d;
            if ((bool)e.NewValue)
            {
                if (!panel.HeldOpenByMouse)
                    panel.IsCollapsed = true;
            }
            else
            {
                panel.IsCollapsed = false;
            }
        }

        bool HeldOpenByMouse => IsMouseOver || IsMouseCaptureWithin;

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (CanCollapse && !HeldOpenByMouse)
                IsCollapsed = true;
        }

        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            base.OnLostMouseCapture(e);
            if (CanCollapse && !HeldOpenByMouse)
                IsCollapsed = true;
        }
    }
}
