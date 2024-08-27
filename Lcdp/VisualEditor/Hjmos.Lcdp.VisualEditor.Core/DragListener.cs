using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Core
{
    public delegate void DragHandler(DragListener drag);

    public class DragListener
    {
        static DragListener() => InputManager.Current.PostProcessInput += new ProcessInputEventHandler(PostProcessInput);

        public Transform Transform { get; set; }

        public DragListener(IInputElement target)
        {
            Target = target;

            Target.PreviewMouseLeftButtonDown += Target_MouseDown;
            Target.PreviewMouseMove += Target_MouseMove;
            Target.PreviewMouseLeftButtonUp += Target_MouseUp;
        }

        public void ExternalStart() => Target_MouseDown(null, null);

        public void ExternalMouseMove(MouseEventArgs e) => Target_MouseMove(null, e);

        public void ExternalStop() => Target_MouseUp(null, null);

        static DragListener CurrentListener;

        static void PostProcessInput(object sender, ProcessInputEventArgs e)
        {
            if (CurrentListener != null)
            {
                if (e.StagingItem.Input is KeyEventArgs a && a.Key == Key.Escape)
                {
                    Mouse.Capture(null);
                    CurrentListener.IsDown = false;
                    CurrentListener.IsCanceled = true;
                    CurrentListener.Complete();
                }
            }
        }

        private void Target_MouseDown(object sender, MouseButtonEventArgs e)
        {
            StartPoint = Mouse.GetPosition(null);
            CurrentPoint = StartPoint;
            DeltaDelta = new Vector();
            IsDown = true;
            IsCanceled = false;
            MouseDown?.Invoke(this);
        }

        private void Target_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsDown)
            {
                DeltaDelta = e.GetPosition(null) - CurrentPoint;
                CurrentPoint += DeltaDelta;

                if (!IsActive)
                {
                    if (Math.Abs(Delta.X) >= SystemParameters.MinimumHorizontalDragDistance ||
                        Math.Abs(Delta.Y) >= SystemParameters.MinimumVerticalDragDistance)
                    {
                        IsActive = true;
                        CurrentListener = this;

                        Started?.Invoke(this);
                    }
                }

                if (IsActive && Changed != null)
                {
                    Changed(this);
                }
            }
        }

        private void Target_MouseUp(object sender, MouseButtonEventArgs e)
        {
            IsDown = false;
            if (IsActive)
            {
                Complete();
            }
        }

        private void Complete()
        {
            IsActive = false;
            CurrentListener = null;

            Completed?.Invoke(this);
        }

        public event DragHandler MouseDown;
        public event DragHandler Started;
        public event DragHandler Changed;
        public event DragHandler Completed;

        public IInputElement Target { get; private set; }
        public Point StartPoint { get; private set; }
        public Point CurrentPoint { get; private set; }
        public Vector DeltaDelta { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsDown { get; private set; }
        public bool IsCanceled { get; private set; }

        public Vector Delta
        {
            get
            {
                if (Transform != null)
                {
                    Matrix matrix = Transform.Value;
                    matrix.Invert();
                    return matrix.Transform(CurrentPoint - StartPoint);
                }
                return CurrentPoint - StartPoint;
            }
        }
    }
}
