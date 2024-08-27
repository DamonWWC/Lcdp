using System.Windows;
using System.Windows.Controls;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    [ExtensionFor(typeof(ContentControl))]
    public class ContentControlInitializer : DefaultInitializer
    {
        public override void InitializeDefaults(DesignItem item)
        {

            // 不是每个内容控件都可以将文本作为内容(如：WPF工具包的ZoomBox)
            if (item.Component is Button)
            {
                DesignItemProperty contentProperty = item.Properties["Content"];
                if (contentProperty.ValueOnInstance == null)
                {
                    contentProperty.SetValue(item.ComponentType.Name);
                }
            }

            DesignItemProperty verticalAlignmentProperty = item.Properties["VerticalAlignment"];
            if (verticalAlignmentProperty.ValueOnInstance == null)
            {
                verticalAlignmentProperty.SetValue(VerticalAlignment.Center);
            }

            DesignItemProperty horizontalAlignmentProperty = item.Properties["HorizontalAlignment"];
            if (horizontalAlignmentProperty.ValueOnInstance == null)
            {
                horizontalAlignmentProperty.SetValue(HorizontalAlignment.Center);
            }
        }
    }

    [ExtensionFor(typeof(TextBlock))]
    public class TextBlockInitializer : DefaultInitializer
    {
        public override void InitializeDefaults(DesignItem item)
        {
            DesignItemProperty textProperty = item.Properties["Text"];
            if (textProperty.ValueOnInstance == null || textProperty.ValueOnInstance.ToString() == "")
            {
                textProperty.SetValue(item.ComponentType.Name);
                item.Properties[FrameworkElement.WidthProperty].Reset();
                item.Properties[FrameworkElement.HeightProperty].Reset();
            }

            DesignItemProperty verticalAlignmentProperty = item.Properties["VerticalAlignment"];
            if (verticalAlignmentProperty.ValueOnInstance == null)
            {
                verticalAlignmentProperty.SetValue(VerticalAlignment.Center);
            }

            DesignItemProperty horizontalAlignmentProperty = item.Properties["HorizontalAlignment"];
            if (horizontalAlignmentProperty.ValueOnInstance == null)
            {
                horizontalAlignmentProperty.SetValue(HorizontalAlignment.Center);
            }
        }
    }

    [ExtensionFor(typeof(HeaderedContentControl), OverrideExtension = typeof(ContentControlInitializer))]
    public class HeaderedContentControlInitializer : DefaultInitializer
    {
        public override void InitializeDefaults(DesignItem item)
        {
            DesignItemProperty headerProperty = item.Properties["Header"];
            if (headerProperty.ValueOnInstance == null)
            {
                headerProperty.SetValue(item.ComponentType.Name);
            }

            DesignItemProperty contentProperty = item.Properties["Content"];
            if (contentProperty.ValueOnInstance == null)
            {
                contentProperty.SetValue(new PanelInstanceFactory().CreateInstance(typeof(Canvas)));
            }
        }
    }
}
