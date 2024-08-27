using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;

namespace Hjmos.Lcdp.VisualEditor.Controls.DesignerControls
{
    public class NumericUpDown : Control
    {
        static NumericUpDown() => DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(typeof(NumericUpDown)));

        private TextBox textBox;
        private DragRepeatButton upButton;
        private DragRepeatButton downButton;

        public static readonly DependencyProperty DecimalPlacesProperty =
            DependencyProperty.Register("DecimalPlaces", typeof(int), typeof(NumericUpDown));

        public int DecimalPlaces
        {
            get => (int)GetValue(DecimalPlacesProperty);
            set => SetValue(DecimalPlacesProperty, value);
        }

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(NumericUpDown));

        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(NumericUpDown), new FrameworkPropertyMetadata(100.0));

        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double?), typeof(NumericUpDown),
                new FrameworkPropertyMetadata(SharedInstances.BoxedDouble0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public double? Value
        {
            get => (double?)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty SmallChangeProperty =
            DependencyProperty.Register("SmallChange", typeof(double), typeof(NumericUpDown),
                new FrameworkPropertyMetadata(SharedInstances.BoxedDouble1));

        public double SmallChange
        {
            get => (double)GetValue(SmallChangeProperty);
            set => SetValue(SmallChangeProperty, value);
        }

        public static readonly DependencyProperty LargeChangeProperty =
            DependencyProperty.Register("LargeChange", typeof(double), typeof(NumericUpDown),
                new FrameworkPropertyMetadata(10.0));

        public double LargeChange
        {
            get => (double)GetValue(LargeChangeProperty);
            set => SetValue(LargeChangeProperty, value);
        }

        private bool IsDragging
        {
            get => upButton.IsDragging;
            set
            {
                upButton.IsDragging = value; downButton.IsDragging = value;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            upButton = (DragRepeatButton)Template.FindName("PART_UpButton", this);
            downButton = (DragRepeatButton)Template.FindName("PART_DownButton", this);
            textBox = (TextBox)Template.FindName("PART_TextBox", this);

            upButton.Click += UpButton_Click;
            downButton.Click += DownButton_Click;

            textBox.LostFocus += (sender, e) => OnLostFocus(e);

            DragListener upDrag = new(upButton);
            DragListener downDrag = new(downButton);

            upDrag.Started += Drag_Started;
            upDrag.Changed += Drag_Changed;
            upDrag.Completed += Drag_Completed;

            downDrag.Started += Drag_Started;
            downDrag.Changed += Drag_Changed;
            downDrag.Completed += Drag_Completed;

            Print();
        }

        private void Drag_Started(DragListener drag) => OnDragStarted();

        private void Drag_Changed(DragListener drag)
        {
            IsDragging = true;
            MoveValue(-drag.DeltaDelta.Y * SmallChange);
        }

        private void Drag_Completed(DragListener drag)
        {
            IsDragging = false;
            OnDragCompleted();
        }

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsDragging) SmallDown();
        }

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsDragging) SmallUp();
        }

        protected virtual void OnDragStarted()
        {
        }

        protected virtual void OnDragCompleted()
        {
        }

        public void SmallUp()
        {
            MoveValue(SmallChange);
        }

        public void SmallDown() => MoveValue(-SmallChange);

        public void LargeUp() => MoveValue(LargeChange);

        public void LargeDown() => MoveValue(-LargeChange);

        private void MoveValue(double delta)
        {
            if (!Value.HasValue)
                return;

            double result;
            if (double.IsNaN((double)Value) || double.IsInfinity((double)Value))
            {
                SetValue(delta);
            }
            else if (double.TryParse(textBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                SetValue(result + delta);
            }
            else
            {
                SetValue((double)Value + delta);
            }
        }

        private void Print()
        {
            if (textBox != null)
            {
                textBox.Text = Value?.ToString("F" + DecimalPlaces, CultureInfo.InvariantCulture);
                textBox.CaretIndex = int.MaxValue;
            }
        }

        //wpf bug?: Value = -1 updates bindings without coercing, workaround
        //update: not derived from RangeBase - no problem
        private void SetValue(double? newValue)
        {
            newValue = CoerceValue(newValue);
            if (Value != newValue && !(Value.HasValue && double.IsNaN(Value.Value) && newValue.HasValue && double.IsNaN(newValue.Value)))
                Value = newValue;
        }

        private double? CoerceValue(double? newValue) => !newValue.HasValue ? null : Math.Max(Minimum, Math.Min((double)newValue, Maximum));

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            switch (e.Key)
            {
                case Key.Enter:
                    SetInputValue();
                    textBox.SelectAll();
                    e.Handled = true;
                    break;
                case Key.Up:
                    SmallUp();
                    e.Handled = true;
                    break;
                case Key.Down:
                    SmallDown();
                    e.Handled = true;
                    break;
                case Key.PageUp:
                    LargeUp();
                    e.Handled = true;
                    break;
                case Key.PageDown:
                    LargeDown();
                    e.Handled = true;
                    break;
                    //				case Key.Home:
                    //					Maximize();
                    //					e.Handled = true;
                    //					break;
                    //				case Key.End:
                    //					Minimize();
                    //					e.Handled = true;
                    //					break;
            }
        }

        private void SetInputValue()
        {
            if (double.TryParse(textBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
            {
                SetValue(result);
            }
            else
            {
                Print();
            }
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            SetInputValue();
        }

        //protected override void OnMouseWheel(MouseWheelEventArgs e)
        //{
        //    if (e.Delta > 0)
        //    {
        //        if (Keyboard.IsKeyDown(Key.LeftShift))
        //        {
        //            LargeUp();
        //        }
        //        else
        //        {
        //            SmallUp();
        //        }
        //    }
        //    else
        //    {
        //        if (Keyboard.IsKeyDown(Key.LeftShift))
        //        {
        //            LargeDown();
        //        }
        //        else
        //        {
        //            SmallDown();
        //        }
        //    }
        //    e.Handled = true;
        //}

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == ValueProperty)
            {
                Value = CoerceValue((double?)e.NewValue);
                Print();
            }
            else if (e.Property == SmallChangeProperty &&
                     ReadLocalValue(LargeChangeProperty) == DependencyProperty.UnsetValue)
            {
                LargeChange = SmallChange * 10;
            }
        }
    }

    public class DragRepeatButton : RepeatButton
    {
        public static readonly DependencyProperty IsDraggingProperty =
            DependencyProperty.Register("IsDragging", typeof(bool), typeof(DragRepeatButton));

        public bool IsDragging
        {
            get => (bool)GetValue(IsDraggingProperty);
            set => SetValue(IsDraggingProperty, value);
        }
    }
}
