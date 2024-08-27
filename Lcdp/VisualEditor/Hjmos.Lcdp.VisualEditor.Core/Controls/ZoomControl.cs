using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Core.Controls
{
    /// <summary>
    /// 基于ScrollView缩放的类，扩展平移的功能
    /// </summary>
    public class ZoomControl : ZoomScrollViewer
    {
        static ZoomControl()
        {
            _panToolCursor = GetCursor("Images/PanToolCursor.cur");
            _panToolCursorMouseDown = GetCursor("Images/PanToolCursorMouseDown.cur");
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ZoomControl), new FrameworkPropertyMetadata(typeof(ZoomControl)));
        }

        public object AdditionalControls
        {
            get => GetValue(AdditionalControlsProperty);
            set => SetValue(AdditionalControlsProperty, value);
        }

        public static readonly DependencyProperty AdditionalControlsProperty =
            DependencyProperty.Register("AdditionalControls", typeof(object), typeof(ZoomControl), new PropertyMetadata(null));

        /// <summary>
        /// 根据路径加载Assets程序集下的鼠标手势
        /// </summary>
        /// <param name="path">相对路径</param>
        /// <returns>光标</returns>
        internal static Cursor GetCursor(string path)
        {
            //var a = Assembly.GetExecutingAssembly();
            Assembly a = Assembly.Load("Hjmos.Lcdp.VisualEditor.Assets");
            ResourceManager m = new(a.GetName().Name + ".g", a);
            using Stream s = m.GetStream(path.ToLowerInvariant());
            return new Cursor(s);
        }

        /// <summary>平移工具图标（手掌张开）</summary>
        private static readonly Cursor _panToolCursor;
        /// <summary>鼠标按下时的平移工具图标（手掌抓住）</summary>
        private static readonly Cursor _panToolCursorMouseDown;
        private double _startHorizontalOffset;
        private double _startVericalOffset;
        /// <summary>按下鼠标时的坐标</summary>
        private Point _startPoint;
        /// <summary>是否按下鼠标</summary>
        private bool _isMouseDown;
        /// <summary>是否平移</summary>
        private bool _pan;

        /// <summary>
        /// 键盘按下时触发
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!_pan && e.Key == Key.Space)
            {
                // 按住空格键平移
                _pan = true;
                Mouse.UpdateCursor();
            }
            base.OnKeyDown(e);
        }

        /// <summary>
        /// 键盘弹起时触发
        /// </summary>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                // 停止平移
                _pan = false;
                Mouse.UpdateCursor();
            }
            base.OnKeyUp(e);
        }

        /// <summary>
        /// 鼠标按下时触发
        /// </summary>
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (!_pan && e.MiddleButton == MouseButtonState.Pressed)
            {
                // 按住鼠标中键平移
                _pan = true;
                Mouse.UpdateCursor();
            }

            if (_pan && !e.Handled)
            {
                // 捕获鼠标
                if (Mouse.Capture(this))
                {
                    // 标记鼠标按下
                    _isMouseDown = true;
                    e.Handled = true;
                    // 纪录鼠标起始位置
                    _startPoint = e.GetPosition(this);
                    PanStart();
                    Mouse.UpdateCursor();
                }
            }
            base.OnPreviewMouseDown(e);
        }

        /// <summary>
        /// 鼠标移动时触发
        /// </summary>
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (_isMouseDown)
            {
                // 获取当前鼠标位置
                Point endPoint = e.GetPosition(this);
                PanContinue(endPoint - _startPoint);
            }
            base.OnPreviewMouseMove(e);
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            if (_pan && e.MiddleButton != MouseButtonState.Pressed && !Keyboard.IsKeyDown(Key.Space))
            {
                _pan = false;
                Mouse.UpdateCursor();
            }

            if (_isMouseDown)
            {
                _isMouseDown = false;
                ReleaseMouseCapture();
                Mouse.UpdateCursor();
            }
            base.OnPreviewMouseUp(e);
        }

        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            if (_isMouseDown)
            {
                _isMouseDown = false;
                ReleaseMouseCapture();
                Mouse.UpdateCursor();
            }
            base.OnLostMouseCapture(e);
        }

        protected override void OnQueryCursor(QueryCursorEventArgs e)
        {
            base.OnQueryCursor(e);
            if (!e.Handled && (_pan || _isMouseDown))
            {
                e.Handled = true;
                e.Cursor = _isMouseDown ? _panToolCursorMouseDown : _panToolCursor;
            }
        }

        /// <summary>
        /// 开始平移
        /// </summary>
        private void PanStart()
        {
            _startHorizontalOffset = this.HorizontalOffset;
            _startVericalOffset = this.VerticalOffset;
        }

        /// <summary>
        /// 继续平移
        /// </summary>
        /// <param name="delta">平移的量</param>
        private void PanContinue(Vector delta)
        {
            ScrollToHorizontalOffset(_startHorizontalOffset - delta.X / this.CurrentZoom);
            ScrollToVerticalOffset(_startVericalOffset - delta.Y / this.CurrentZoom);
        }
    }
}
