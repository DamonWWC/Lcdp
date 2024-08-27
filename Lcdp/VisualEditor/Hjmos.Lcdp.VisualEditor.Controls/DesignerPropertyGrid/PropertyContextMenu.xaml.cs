using Hjmos.Lcdp.VisualEditor.Controls.Themes;
using System;
using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Controls.DesignerPropertyGrid
{
    public partial class PropertyContextMenu
    {
        public PropertyContextMenu()
        {
            InitializeComponent();
        }

        public PropertyNode PropertyNode
        {
            get { return DataContext as PropertyNode; }
        }

        void Click_Reset(object sender, RoutedEventArgs e)
        {
            PropertyNode.Reset();
        }

        void Click_Binding(object sender, RoutedEventArgs e)
        {
            PropertyNode.CreateBinding();
        }

        void Click_CustomExpression(object sender, RoutedEventArgs e)
        {
        }

        void Click_ConvertToLocalValue(object sender, RoutedEventArgs e)
        {
        }

        void Click_SaveAsResource(object sender, RoutedEventArgs e)
        {
        }
    }
}
