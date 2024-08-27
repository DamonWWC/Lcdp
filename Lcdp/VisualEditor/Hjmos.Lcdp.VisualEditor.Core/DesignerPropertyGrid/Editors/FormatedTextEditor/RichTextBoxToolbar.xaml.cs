using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;

namespace Hjmos.Lcdp.VisualEditor.Core.DesignerPropertyGrid.Editors.FormatedTextEditor
{

    public partial class RichTextBoxToolbar
    {
        public RichTextBoxToolbar()
        {
            this.InitializeComponent();

            cmbFontFamily.SelectionChanged += (s, e) =>
            {
                if (cmbFontFamily.SelectedValue != null && RichTextBox != null)
                {
                    TextRange tr = new TextRange(RichTextBox.Selection.Start, RichTextBox.Selection.End);
                    var value = cmbFontFamily.SelectedValue;
                    tr.ApplyPropertyValue(TextElement.FontFamilyProperty, value);
                }
            };

            cmbFontSize.SelectionChanged += (s, e) =>
            {
                if (cmbFontSize.SelectedValue != null && RichTextBox != null)
                {
                    TextRange tr = new TextRange(RichTextBox.Selection.Start, RichTextBox.Selection.End);
                    var value = ((ComboBoxItem)cmbFontSize.SelectedValue).Content.ToString();
                    tr.ApplyPropertyValue(TextElement.FontSizeProperty, double.Parse(value));
                }
            };

            cmbFontSize.AddHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler((s, e) =>
            {
                if (!string.IsNullOrEmpty(cmbFontSize.Text) && RichTextBox != null)
                {
                    TextRange tr = new TextRange(RichTextBox.Selection.Start, RichTextBox.Selection.End);
                    tr.ApplyPropertyValue(TextElement.FontSizeProperty, double.Parse(cmbFontSize.Text));
                }
            }));
        }

        public void SetValuesFromTextBlock(TextBlock textBlock)
        {
            cmbFontFamily.Text = textBlock.FontFamily.ToString();
            cmbFontSize.Text = textBlock.FontSize.ToString();
        }

        public RichTextBox RichTextBox
        {
            get { return (RichTextBox)GetValue(RichTextBoxProperty); }
            set { SetValue(RichTextBoxProperty, value); }
        }

        public static readonly DependencyProperty RichTextBoxProperty =
            DependencyProperty.Register("RichTextBox", typeof(RichTextBox), typeof(RichTextBoxToolbar),
                new PropertyMetadata(null));
    }
}
