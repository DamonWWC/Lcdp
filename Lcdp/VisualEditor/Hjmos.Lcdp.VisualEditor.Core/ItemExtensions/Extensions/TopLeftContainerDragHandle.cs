﻿using Hjmos.Lcdp.VisualEditor.Core.Adorners;
using Hjmos.Lcdp.VisualEditor.Core.Controls;
using Hjmos.Lcdp.VisualEditor.Core.Services;
using System.Windows;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    /// <summary>
    /// 为元素显示的拖动手柄
    /// </summary>
    [ExtensionServer(typeof(OnlyOneItemSelectedExtensionServer))]
    [ExtensionFor(typeof(FrameworkElement))]
    public class TopLeftContainerDragHandle : AdornerProvider
    {
        /// <summary/>
        public TopLeftContainerDragHandle()
        {
            ContainerDragHandle rect = new();

            rect.PreviewMouseDown += delegate (object sender, MouseButtonEventArgs e)
            {
                //Services.Selection.SetSelectedComponents(new DesignItem[] { this.ExtendedItem }, SelectionTypes.Auto);
                new DragMoveMouseGesture(this.ExtendedItem, false).Start(this.ExtendedItem.Services.DesignPanel, e);
                e.Handled = true;
            };

            RelativePlacement p = new(HorizontalAlignment.Left, VerticalAlignment.Top)
            {
                XOffset = -7,
                YOffset = -7
            };

            AddAdorner(p, AdornerOrder.Background, rect);
        }
    }

}
