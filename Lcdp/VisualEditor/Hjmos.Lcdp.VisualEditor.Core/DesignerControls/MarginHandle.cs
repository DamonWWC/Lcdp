using Hjmos.Lcdp.VisualEditor.Core.Adorners;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Core.DesignerControls
{
    /// <summary>
    /// 显示网格中控件的边距的装饰器。
    /// </summary>
    public class MarginHandle : Control
    {
        /// <summary>将手柄放置到具有一定偏移量的位置，这样手柄就不会干扰选择轮廓。</summary>
        public static double HandleLengthOffset = 2;

        static MarginHandle() => DefaultStyleKeyProperty.OverrideMetadata(typeof(MarginHandle), new FrameworkPropertyMetadata(typeof(MarginHandle)));

        /// <summary><see cref="HandleLength"/>的依赖属性</summary>
        public static readonly DependencyProperty HandleLengthProperty =
            DependencyProperty.Register("HandleLength", typeof(double), typeof(MarginHandle),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnHandleLengthChanged)));

        /// <summary>获取/设置边距手柄的长度。</summary>
        public double HandleLength
        {
            get => (double)GetValue(HandleLengthProperty);
            set => SetValue(HandleLengthProperty, value);
        }

        private readonly Grid _grid;
        private readonly DesignItem _adornedControlItem;
        private readonly AdornerPanel _adornerPanel;
        private readonly FrameworkElement _adornedControl;

        /// <summary>这个网格包含手柄线和箭头</summary>
        private Grid lineArrow;

        /// <summary>获取此手柄的存根</summary>
        public MarginStub Stub { get; protected set; }

        /// <summary>获取/设置手柄旋转的角度</summary>
        public double Angle { get; set; }

        /// <summary>获取/设置边距文本必须旋转的角度</summary>
        public virtual double TextTransform
        {
            get => (double)Orientation is 90 or 180 ? 180 : (double)Orientation == 270 ? 0 : (double)Orientation;
            set { }
        }

        /// <summary>当<see cref="HandleLength"/>改变时判断手柄/存根的可见性</summary>
        public static void OnHandleLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as MarginHandle).DecideVisiblity((double)e.NewValue);

        /// <summary>是否永久显示手柄</summary>
        public bool ShouldBeVisible { get; set; }

        /// <summary>是否只显示存根</summary>
        public bool DisplayOnlyStub { get; set; }

        /// <summary>获取手柄的方向</summary>
        public HandleOrientation Orientation { get; }

        protected MarginHandle() { }

        public MarginHandle(DesignItem adornedControlItem, AdornerPanel adornerPanel, HandleOrientation orientation)
        {
            Debug.Assert(adornedControlItem != null);

            _adornedControlItem = adornedControlItem;
            _adornerPanel = adornerPanel;
            this.Orientation = orientation;
            Angle = (double)orientation;
            _grid = adornedControlItem.Parent.Component as Grid;
            _adornedControl = adornedControlItem.Component as FrameworkElement;
            Stub = new MarginStub(this);
            ShouldBeVisible = true;
            BindAndPlaceHandle();

            adornedControlItem.PropertyChanged += OnPropertyChanged;
            OnPropertyChanged(this._adornedControlItem, new PropertyChangedEventArgs("HorizontalAlignment"));
            OnPropertyChanged(this._adornedControlItem, new PropertyChangedEventArgs("VerticalAlignment"));
        }

        /// <summary>
        /// 将<see cref="HandleLength"/>绑定到边距并放置手柄。
        /// </summary>
        private void BindAndPlaceHandle()
        {
            // 保证边距手柄和存根放置在装饰面板中
            if (!_adornerPanel.Children.Contains(this))
                _adornerPanel.Children.Add(this);
            if (!_adornerPanel.Children.Contains(Stub))
                _adornerPanel.Children.Add(Stub);


            // 把HandleLength和Margin绑定，并且设置手柄的显示位置
            RelativePlacement placement = new();
            Binding binding = new();
            binding.Source = _adornedControl;
            switch (Orientation)
            {
                case HandleOrientation.Left:
                    binding.Path = new PropertyPath("Margin.Left");
                    placement = new RelativePlacement(HorizontalAlignment.Left, VerticalAlignment.Center);
                    placement.XOffset = -HandleLengthOffset;
                    break;
                case HandleOrientation.Top:
                    binding.Path = new PropertyPath("Margin.Top");
                    placement = new RelativePlacement(HorizontalAlignment.Center, VerticalAlignment.Top);
                    placement.YOffset = -HandleLengthOffset;
                    break;
                case HandleOrientation.Right:
                    binding.Path = new PropertyPath("Margin.Right");
                    placement = new RelativePlacement(HorizontalAlignment.Right, VerticalAlignment.Center);
                    placement.XOffset = HandleLengthOffset;
                    break;
                case HandleOrientation.Bottom:
                    binding.Path = new PropertyPath("Margin.Bottom");
                    placement = new RelativePlacement(HorizontalAlignment.Center, VerticalAlignment.Bottom);
                    placement.YOffset = HandleLengthOffset;
                    break;
            }

            binding.Mode = BindingMode.TwoWay;
            SetBinding(HandleLengthProperty, binding);

            AdornerPanel.SetPlacement(this, placement);
            AdornerPanel.SetPlacement(Stub, placement);

            DecideVisiblity(this.HandleLength);
        }

        /// <summary>
        /// 决定Handle或存根的可见性（以设置的为准），并在控件靠近Grid或超出Grid时隐藏线和终端箭头
        /// </summary>		
        public void DecideVisiblity(double handleLength)
        {
            if (ShouldBeVisible)
            {
                if (!DisplayOnlyStub)
                {
                    this.Visibility = handleLength != 0.0 ? Visibility.Visible : Visibility.Hidden;
                    if (this.lineArrow != null)
                    {
                        lineArrow.Visibility = handleLength < 25 ? Visibility.Hidden : Visibility.Visible;
                    }
                    Stub.Visibility = this.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
                }
                else
                {
                    this.Visibility = Visibility.Hidden;
                    Stub.Visibility = Visibility.Visible;
                }
            }
            else
            {
                this.Visibility = Visibility.Hidden;
                Stub.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// 元素的对齐属性改变时，重新配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "HorizontalAlignment" && (Orientation == HandleOrientation.Left || Orientation == HandleOrientation.Right))
            {
                var ha = (HorizontalAlignment)_adornedControlItem.Properties[HorizontalAlignmentProperty].ValueOnInstance;
                switch (ha)
                {
                    case HorizontalAlignment.Stretch:
                        DisplayOnlyStub = false;
                        break;
                    case HorizontalAlignment.Center:
                        DisplayOnlyStub = true;
                        break;
                    default:
                        DisplayOnlyStub = ha.ToString() != Orientation.ToString();
                        break;
                }
            }

            if (e.PropertyName == "VerticalAlignment" && (Orientation == HandleOrientation.Top || Orientation == HandleOrientation.Bottom))
            {
                VerticalAlignment va = (VerticalAlignment)_adornedControlItem.Properties[VerticalAlignmentProperty].ValueOnInstance;

                switch (va)
                {
                    case VerticalAlignment.Stretch:
                        DisplayOnlyStub = false;
                        break;
                    case VerticalAlignment.Center:
                        DisplayOnlyStub = true;
                        break;
                    default:
                        DisplayOnlyStub = va.ToString() != Orientation.ToString();
                        break;
                }
            }
            DecideVisiblity(this.HandleLength);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            this.Cursor = Cursors.Hand;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            this.Cursor = Cursors.Arrow;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            lineArrow = new Grid();
            lineArrow = Template.FindName("lineArrow", this) as Grid;
            Debug.Assert(lineArrow != null);
        }
    }
}