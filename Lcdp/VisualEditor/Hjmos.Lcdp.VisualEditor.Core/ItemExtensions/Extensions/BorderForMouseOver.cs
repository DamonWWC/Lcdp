using Hjmos.Lcdp.VisualEditor.Core.Adorners;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    /// <summary>
    /// 鼠标悬停的时候显示一个边框
    /// </summary>
    [ExtensionFor(typeof(FrameworkElement))]
    [ExtensionServer(typeof(MouseOverExtensionServer))]
    public class BorderForMouseOver : AdornerProvider
    {
        private readonly AdornerPanel _adornerPanel;

        public BorderForMouseOver()
        {
            // 放置在背景层
            _adornerPanel = new AdornerPanel { Order = AdornerOrder.Background };
            // 添加到装饰器集合
            this.Adorners.Add(_adornerPanel);
            // 创建一个浅蓝色边框
            Border border = new()
            {
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.DodgerBlue, // #FF1E90FF
                Margin = new Thickness(-2)
            };
            // 设置放置的位置
            AdornerPanel.SetPlacement(border, AdornerPlacement.FillContent);
            // 添加到装饰面板
            _adornerPanel.Children.Add(border);
        }
    }
}
