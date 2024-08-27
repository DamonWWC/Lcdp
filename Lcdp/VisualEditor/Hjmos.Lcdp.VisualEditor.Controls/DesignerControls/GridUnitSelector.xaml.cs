using Hjmos.Lcdp.VisualEditor.Controls.Adorners;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Controls.DesignerControls
{
    /// <summary>
    /// 网格单位选择器
    /// </summary>
    public partial class GridUnitSelector
    {
        /// <summary>Grid上轨道装饰器的引用</summary>
        private readonly GridRailAdorner _rail;

        public GridUnitSelector(GridRailAdorner rail)
        {
            InitializeComponent();

            _rail = rail;
        }

        /// <summary>固定</summary>
        private void FixedChecked(object sender, RoutedEventArgs e) => _rail.SetGridLengthUnit(Unit);

        /// <summary>加权比例</summary>
        private void StarChecked(object sender, RoutedEventArgs e) => _rail.SetGridLengthUnit(Unit);

        /// <summary>自动</summary>
        private void AutoChecked(object sender, RoutedEventArgs e) => _rail.SetGridLengthUnit(Unit);

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(GridUnitSelector), new FrameworkPropertyMetadata());

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public DesignItem SelectedItem { get; set; }

        public GridUnitType Unit
        {
            get => auto.IsChecked == true ? GridUnitType.Auto : star.IsChecked == true ? GridUnitType.Star : GridUnitType.Pixel;
            set
            {
                switch (value)
                {
                    case GridUnitType.Auto:
                        auto.IsChecked = true;
                        break;
                    case GridUnitType.Star:
                        star.IsChecked = true;
                        break;
                    case GridUnitType.Pixel:
                        @fixed.IsChecked = true;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 鼠标离开时隐藏网格单位选择器
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            this.Visibility = Visibility.Hidden;
        }
    }
}
