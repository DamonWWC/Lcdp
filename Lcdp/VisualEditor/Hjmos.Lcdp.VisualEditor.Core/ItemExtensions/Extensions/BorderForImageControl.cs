using Hjmos.Lcdp.VisualEditor.Core.Adorners;
using Hjmos.Lcdp.VisualEditor.Core.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    [ExtensionFor(typeof(Image))]
    public class BorderForImageControl : PermanentAdornerProvider
    {
        AdornerPanel _adornerPanel;
        AdornerPanel _cachedAdornerPanel;
        Border _border;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            this.ExtendedItem.PropertyChanged += OnPropertyChanged;

            UpdateAdorner();
        }

        protected override void OnRemove()
        {
            this.ExtendedItem.PropertyChanged -= OnPropertyChanged;
            base.OnRemove();
        }

        void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender == null || e.PropertyName == "Width" || e.PropertyName == "Height")
            {
                ((DesignPanel)this.ExtendedItem.Services.DesignPanel).AdornerLayer.UpdateAdornersForElement(this.ExtendedItem.View, true);
            }
        }

        void UpdateAdorner()
        {
            if (ExtendedItem.Component is UIElement)
            {
                CreateAdorner();
            }
        }

        private void CreateAdorner()
        {
            if (_adornerPanel == null)
            {
                if (_cachedAdornerPanel == null)
                {
                    _cachedAdornerPanel = new AdornerPanel
                    {
                        Order = AdornerOrder.Background
                    };
                    _border = new Border
                    {
                        BorderThickness = new Thickness(1),
                        BorderBrush = new SolidColorBrush(Color.FromRgb(0xCC, 0xCC, 0xCC)),
                        Background = Brushes.Transparent,
                        IsHitTestVisible = true
                    };
                    _border.MouseDown += Border_MouseDown;
                    _border.MinWidth = 1;
                    _border.MinHeight = 1;

                    AdornerPanel.SetPlacement(_border, AdornerPlacement.FillContent);
                    _cachedAdornerPanel.Children.Add(_border);
                }

                _adornerPanel = _cachedAdornerPanel;
                Adorners.Add(_adornerPanel);
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!Keyboard.IsKeyDown(Key.LeftAlt) && ((Image)this.ExtendedItem.View).Source == null)
            {
                e.Handled = true;
                this.ExtendedItem.Services.Selection.SetSelectedComponents(new DesignItem[] { this.ExtendedItem }, SelectionTypes.Auto);
                ((DesignPanel)this.ExtendedItem.Services.DesignPanel).Focus();
            }
        }
    }
}
