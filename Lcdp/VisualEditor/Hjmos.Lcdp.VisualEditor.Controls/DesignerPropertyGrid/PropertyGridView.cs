using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Controls.DesignerPropertyGrid
{
    [TemplatePart(Name = "PART_Thumb", Type = typeof(Thumb))]
    public class PropertyGridView : Control
    {
        static PropertyGridView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridView), new FrameworkPropertyMetadata(typeof(PropertyGridView)));
        }

        public PropertyGridView() : this(null)
        {
        }

        public PropertyGridView(IPropertyGrid pg)
        {
            PropertyGrid = pg ?? new PropertyGrid();
            DataContext = PropertyGrid;
        }

        private Thumb thumb;
        public override void OnApplyTemplate()
        {
            thumb = GetTemplateChild("PART_Thumb") as Thumb;

            thumb.DragDelta += new DragDeltaEventHandler(thumb_DragDelta);

            base.OnApplyTemplate();
        }

        static PropertyContextMenu propertyContextMenu = new PropertyContextMenu();

        public IPropertyGrid PropertyGrid { get; private set; }

        public static readonly DependencyProperty FirstColumnWidthProperty =
            DependencyProperty.Register("FirstColumnWidth", typeof(double), typeof(PropertyGridView),
                                        new PropertyMetadata(120.0));

        public double FirstColumnWidth
        {
            get { return (double)GetValue(FirstColumnWidthProperty); }
            set { SetValue(FirstColumnWidthProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(IEnumerable<DesignItem>), typeof(PropertyGridView));

        public IEnumerable<DesignItem> SelectedItems
        {
            get { return (IEnumerable<DesignItem>)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == SelectedItemsProperty)
            {
                PropertyGrid.SelectedItems = SelectedItems;
            }
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            var ancestors = (e.OriginalSource as DependencyObject).GetVisualAncestors();
            Border row = ancestors.OfType<Border>().FirstOrDefault(b => b.Name == "uxPropertyNodeRow");
            if (row == null) return;

            PropertyNode node = row.DataContext as PropertyNode;
            if (node.IsEvent) return;

            PropertyContextMenu contextMenu = new PropertyContextMenu();
            contextMenu.DataContext = node;
            contextMenu.Placement = PlacementMode.Bottom;
            contextMenu.HorizontalOffset = -30;
            contextMenu.PlacementTarget = row;
            contextMenu.IsOpen = true;
        }

        void thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            FirstColumnWidth = Math.Max(0, FirstColumnWidth + e.HorizontalChange);
        }
    }
}
