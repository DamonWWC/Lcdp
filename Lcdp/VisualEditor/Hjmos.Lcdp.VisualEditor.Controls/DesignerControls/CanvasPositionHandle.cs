using Hjmos.Lcdp.VisualEditor.Controls.Adorners;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Hjmos.Lcdp.VisualEditor.Controls.DesignerControls
{
    /// <summary>
    /// 显示网格中控件的边距的装饰器。
    /// </summary>
    public class CanvasPositionHandle : MarginHandle
    {
        static CanvasPositionHandle()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CanvasPositionHandle), new FrameworkPropertyMetadata(typeof(CanvasPositionHandle)));
            HandleLengthOffset = 2;
        }

        private Path line1;
        private Path line2;

        public override void OnApplyTemplate()
        {
            line1 = GetTemplateChild("line1") as Path;
            line2 = GetTemplateChild("line2") as Path;

            base.OnApplyTemplate();
        }

        readonly Canvas canvas;
        readonly DesignItem adornedControlItem;
        readonly AdornerPanel adornerPanel;
        readonly HandleOrientation orientation;
        readonly FrameworkElement adornedControl;

        /// <summary> This grid contains the handle line and the endarrow. 这个网格包含手柄线和内箭头。</summary>
        //		Grid lineArrow;

        private DependencyPropertyDescriptor leftDescriptor;
        private DependencyPropertyDescriptor rightDescriptor;
        private DependencyPropertyDescriptor topDescriptor;
        private DependencyPropertyDescriptor bottomDescriptor;
        private DependencyPropertyDescriptor widthDescriptor;
        private DependencyPropertyDescriptor heightDescriptor;

        public CanvasPositionHandle(DesignItem adornedControlItem, AdornerPanel adornerPanel, HandleOrientation orientation)
        {
            Debug.Assert(adornedControlItem != null);
            this.adornedControlItem = adornedControlItem;
            this.adornerPanel = adornerPanel;
            this.orientation = orientation;

            Angle = (double)orientation;

            canvas = adornedControlItem.Parent.Component as Canvas;
            adornedControl = adornedControlItem.Component as FrameworkElement;
            Stub = new MarginStub(this);
            ShouldBeVisible = true;

            leftDescriptor = DependencyPropertyDescriptor.FromProperty(Canvas.LeftProperty, adornedControlItem.Component.GetType());
            leftDescriptor.AddValueChanged(adornedControl, OnPropertyChanged);
            rightDescriptor = DependencyPropertyDescriptor.FromProperty(Canvas.RightProperty, adornedControlItem.Component.GetType());
            rightDescriptor.AddValueChanged(adornedControl, OnPropertyChanged);
            topDescriptor = DependencyPropertyDescriptor.FromProperty(Canvas.TopProperty, adornedControlItem.Component.GetType());
            topDescriptor.AddValueChanged(adornedControl, OnPropertyChanged);
            bottomDescriptor = DependencyPropertyDescriptor.FromProperty(Canvas.BottomProperty, adornedControlItem.Component.GetType());
            bottomDescriptor.AddValueChanged(adornedControl, OnPropertyChanged);
            widthDescriptor = DependencyPropertyDescriptor.FromProperty(Control.WidthProperty, adornedControlItem.Component.GetType());
            widthDescriptor.AddValueChanged(adornedControl, OnPropertyChanged);
            heightDescriptor = DependencyPropertyDescriptor.FromProperty(Control.WidthProperty, adornedControlItem.Component.GetType());
            heightDescriptor.AddValueChanged(adornedControl, OnPropertyChanged);
            BindAndPlaceHandle();
        }


        void OnPropertyChanged(object sender, EventArgs e) => BindAndPlaceHandle();

        /// <summary>
        /// Gets/Sets the angle by which the Canvas display has to be rotated
        /// 获取/设置Canvas显示必须旋转的角度
        /// </summary>
        public override double TextTransform
        {
            get
            {
                if ((double)orientation == 90 || (double)orientation == 180)
                    return 180;
                if ((double)orientation == 270)
                    return 0;
                return (double)orientation;
            }
        }

        /// <summary>
        /// Binds the <see cref="MarginHandle.HandleLength"/> to the margin and place the handles.
        /// 绑定<see cref="MarginHandle.HandleLength"/>到margin，并放置手柄。
        /// </summary>
        void BindAndPlaceHandle()
        {
            if (!adornerPanel.Children.Contains(this))
                adornerPanel.Children.Add(this);
            if (!adornerPanel.Children.Contains(Stub))
                adornerPanel.Children.Add(Stub);
            RelativePlacement placement = new();
            switch (orientation)
            {
                case HandleOrientation.Left:
                    {
                        var wr = (double)leftDescriptor.GetValue(adornedControl);
                        if (double.IsNaN(wr))
                        {
                            wr = (double)rightDescriptor.GetValue(adornedControl);
                            wr = canvas.ActualWidth - (PlacementOperation.GetRealElementSize(adornedControl).Width + wr);
                        }
                        else
                        {
                            if (line1 != null)
                            {
                                line1.StrokeDashArray.Clear();
                                line2.StrokeDashArray.Clear();
                            }
                        }
                        this.HandleLength = wr;
                        placement = new RelativePlacement(HorizontalAlignment.Left, VerticalAlignment.Center);
                        placement.XOffset = -HandleLengthOffset;
                        break;
                    }
                case HandleOrientation.Top:
                    {
                        var wr = (double)topDescriptor.GetValue(adornedControl);
                        if (double.IsNaN(wr))
                        {
                            wr = (double)bottomDescriptor.GetValue(adornedControl);
                            wr = canvas.ActualHeight - (PlacementOperation.GetRealElementSize(adornedControl).Height + wr);
                        }
                        else
                        {
                            if (line1 != null)
                            {
                                line1.StrokeDashArray.Clear();
                                line2.StrokeDashArray.Clear();
                            }
                        }
                        this.HandleLength = wr;
                        placement = new RelativePlacement(HorizontalAlignment.Center, VerticalAlignment.Top);
                        placement.YOffset = -HandleLengthOffset;
                        break;
                    }
                case HandleOrientation.Right:
                    {
                        var wr = (double)rightDescriptor.GetValue(adornedControl);
                        if (double.IsNaN(wr))
                        {
                            wr = (double)leftDescriptor.GetValue(adornedControl);
                            wr = canvas.ActualWidth - (PlacementOperation.GetRealElementSize(adornedControl).Width + wr);
                        }
                        else
                        {
                            if (line1 != null)
                            {
                                line1.StrokeDashArray.Clear();
                                line2.StrokeDashArray.Clear();
                            }
                        }
                        this.HandleLength = wr;
                        placement = new RelativePlacement(HorizontalAlignment.Right, VerticalAlignment.Center);
                        placement.XOffset = HandleLengthOffset;
                        break;
                    }
                case HandleOrientation.Bottom:
                    {
                        var wr = (double)bottomDescriptor.GetValue(adornedControl);
                        if (double.IsNaN(wr))
                        {
                            wr = (double)topDescriptor.GetValue(adornedControl);
                            wr = canvas.ActualHeight - (PlacementOperation.GetRealElementSize(adornedControl).Height + wr);
                        }
                        else
                        {
                            if (line1 != null)
                            {
                                line1.StrokeDashArray.Clear();
                                line2.StrokeDashArray.Clear();
                            }
                        }
                        this.HandleLength = wr;
                        placement = new RelativePlacement(HorizontalAlignment.Center, VerticalAlignment.Bottom);
                        placement.YOffset = HandleLengthOffset;
                        break;
                    }
            }

            AdornerPanel.SetPlacement(this, placement);
            this.Visibility = Visibility.Visible;
        }
    }
}
