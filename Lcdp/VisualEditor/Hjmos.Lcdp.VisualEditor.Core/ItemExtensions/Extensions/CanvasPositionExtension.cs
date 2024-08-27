using Hjmos.Lcdp.VisualEditor.Core.Adorners;
using Hjmos.Lcdp.VisualEditor.Core.DesignerControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    [ExtensionFor(typeof(FrameworkElement))]
    [ExtensionServer(typeof(OnlyOneItemSelectedExtensionServer))]
    public class CanvasPositionExtension : AdornerProvider
    {
        private MarginHandle[] _handles;
        private MarginHandle _leftHandle, _topHandle, _rightHandle, _bottomHandle;
        private Canvas _canvas;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (this.ExtendedItem.Parent != null)
            {
                //if (this.ExtendedItem.Parent.ComponentType == typeof(Canvas))
                if (typeof(Canvas).IsAssignableFrom(this.ExtendedItem.Parent.ComponentType))
                {
                    FrameworkElement extendedControl = this.ExtendedItem.Component as FrameworkElement;
                    AdornerPanel adornerPanel = new();

                    // 如果元素在网格中旋转/倾斜，则不会出现空白手柄
                    if (extendedControl.LayoutTransform.Value == Matrix.Identity && extendedControl.RenderTransform.Value == Matrix.Identity)
                    {
                        _canvas = this.ExtendedItem.Parent.View as Canvas;
                        _handles = new[]
                        {
                            _leftHandle = new CanvasPositionHandle(ExtendedItem, adornerPanel, HandleOrientation.Left),
                            _topHandle = new CanvasPositionHandle(ExtendedItem, adornerPanel, HandleOrientation.Top),
                            _rightHandle = new CanvasPositionHandle(ExtendedItem, adornerPanel, HandleOrientation.Right),
                            _bottomHandle = new CanvasPositionHandle(ExtendedItem, adornerPanel, HandleOrientation.Bottom),
                        };
                    }

                    if (adornerPanel != null)
                        this.Adorners.Add(adornerPanel);
                }
            }
        }

        public void HideHandles()
        {
            if (_handles != null)
            {
                foreach (MarginHandle handle in _handles)
                {
                    handle.ShouldBeVisible = false;
                    handle.Visibility = Visibility.Hidden;
                }
            }
        }

        public void ShowHandles()
        {
            if (_handles != null)
            {
                foreach (MarginHandle handle in _handles)
                {
                    handle.ShouldBeVisible = true;
                    handle.Visibility = Visibility.Visible;
                    handle.DecideVisiblity(handle.HandleLength);
                }
            }
        }
    }
}
