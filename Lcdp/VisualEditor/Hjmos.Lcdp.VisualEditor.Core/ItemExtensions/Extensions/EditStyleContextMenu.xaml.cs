using Hjmos.Lcdp.VisualEditor.Core.Services;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Xml;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    public partial class EditStyleContextMenu
    {
        private readonly DesignItem _designItem;

        public EditStyleContextMenu(DesignItem designItem)
        {
            _designItem = designItem;

            this.InitializeComponent();
        }

        private void Click_EditStyle(object sender, RoutedEventArgs e)
        {
            //var cg = designItem.OpenGroup("Edit Style");

            UIElement element = _designItem.View;
            object defaultStyleKey = element.GetValue(DefaultStyleKeyProperty);
            Style style = Application.Current.TryFindResource(defaultStyleKey) as Style;

            MyComponentService service = _designItem.Services.Component as MyComponentService;

            MemoryStream ms = new();
            XmlTextWriter writer = new(ms, System.Text.Encoding.UTF8) { Formatting = Formatting.Indented };
            XamlWriter.Save(style, writer);

            MyDesignItem rootItem = _designItem.Context.RootItem as MyDesignItem;

            ms.Position = 0;
            StreamReader sr = new(ms);
            string xaml = sr.ReadToEnd();

            //var xamlObject = XamlParser.ParseSnippet(rootItem.XamlObject, xaml, ((MyDesignContext)this.designItem.Context).ParserSettings);

            //var styleDesignItem = service.RegisterXamlComponentRecursive(xamlObject);
            //try
            //{
            //    designItem.Properties.GetProperty("Resources").CollectionElements.Add(styleDesignItem);
            //    cg.Commit();
            //}
            //catch (Exception)
            //{
            //    cg.Abort();
            //}
        }
    }
}
