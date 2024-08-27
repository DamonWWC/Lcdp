using Hjmos.Lcdp.VisualEditor.Core.Adorners;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    /// <summary>
    /// 为不可见控件附件边框
    /// </summary>
    [ExtensionFor(typeof(Panel))]
    [ExtensionFor(typeof(Border))]
    [ExtensionFor(typeof(ContentControl))]
    [ExtensionFor(typeof(Viewbox))]
    public class BorderForInvisibleControl : PermanentAdornerProvider
    {
        private AdornerPanel adornerPanel;
        private AdornerPanel cachedAdornerPanel;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (ExtendedItem.Component is Border)
            {
                ExtendedItem.PropertyChanged += delegate { UpdateAdorner(); };
            }

            // 如果组件是一个ContentControl，那它必须就是ContentControl类型，而不是派生类型，如Label和Button
            if (ExtendedItem.Component is not ContentControl || ExtendedItem.Component.GetType() == typeof(ContentControl))
            {
                UpdateAdorner();

                if (ExtendedItem.Component is UIElement element)
                {
                    element.IsVisibleChanged += delegate { UpdateAdorner(); };
                }
            }
        }

        private void UpdateAdorner()
        {
            if (ExtendedItem.Component is UIElement element)
            {
                Border border = element as Border;
                if (element.IsVisible && (border == null || IsAnyBorderEdgeInvisible(border)))
                {
                    CreateAdorner();

                    if (border != null)
                    {
                        Border adornerBorder = adornerPanel.Children[0] as Border;

                        adornerBorder.BorderThickness = IsBorderBrushInvisible(border) ? new Thickness(1)
                            : new Thickness(border.BorderThickness.Left > 0 ? 0 : 1,
                                            border.BorderThickness.Top > 0 ? 0 : 1,
                                            border.BorderThickness.Right > 0 ? 0 : 1,
                                            border.BorderThickness.Bottom > 0 ? 0 : 1);
                    }
                }
                else
                {
                    RemoveAdorner();
                }
            }
        }

        private bool IsAnyBorderEdgeInvisible(Border border)
        {
            return IsBorderBrushInvisible(border) || border.BorderThickness.Left == 0 || border.BorderThickness.Top == 0 || border.BorderThickness.Right == 0 || border.BorderThickness.Bottom == 0;
        }

        private bool IsBorderBrushInvisible(Border border) => border.BorderBrush == null || border.BorderBrush == Brushes.Transparent;

        private void CreateAdorner()
        {
            if (adornerPanel == null)
            {
                if (cachedAdornerPanel == null)
                {
                    cachedAdornerPanel = new AdornerPanel { Order = AdornerOrder.Background };
                    Border border = new()
                    {
                        BorderThickness = new Thickness(1),
                        BorderBrush = new SolidColorBrush(Color.FromRgb(0xCC, 0xCC, 0xCC)),
                        IsHitTestVisible = false
                    };
                    AdornerPanel.SetPlacement(border, AdornerPlacement.FillContent);
                    cachedAdornerPanel.Children.Add(border);
                }

                adornerPanel = cachedAdornerPanel;
                Adorners.Add(adornerPanel);
            }
        }

        private void RemoveAdorner()
        {
            if (adornerPanel != null)
            {
                Adorners.Remove(adornerPanel);
                adornerPanel = null;
            }
        }
    }
}
