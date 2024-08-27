using Hjmos.Lcdp.VisualEditor.Controls.DesignerPropertyGrid.Editors.FormatedTextEditor;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Controls.DesignerControls
{
    /// <summary>
    /// 支持在设计器中编辑文本
    /// </summary>
    public class InPlaceEditor : Control
    {
        static InPlaceEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(InPlaceEditor), new FrameworkPropertyMetadata(typeof(InPlaceEditor)));
        }

        /// <summary>
        /// 此属性绑定到编辑器的文本属性
        /// </summary>
        public static readonly DependencyProperty BindProperty =
            DependencyProperty.Register("Bind", typeof(string), typeof(InPlaceEditor), new FrameworkPropertyMetadata());

        public string Bind
        {
            get => (string)GetValue(BindProperty);
            set => SetValue(BindProperty, value);
        }

        private readonly DesignItem designItem;
        private ChangeGroup changeGroup;
        private RichTextBox editor;

        bool _isChangeGroupOpen;

        public InPlaceEditor(DesignItem designItem)
        {
            this.designItem = designItem;

            this.InputBindings.Add(new KeyBinding(EditingCommands.ToggleBold, Key.B, ModifierKeys.Control));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            editor = Template.FindName("PART_Editor", this) as RichTextBox; // Gets the TextBox-editor from the Template
            editor.PreviewKeyDown += delegate (object sender, KeyEventArgs e)
            {
                if (e.Key == Key.Enter && (e.KeyboardDevice.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift)
                {
                    e.Handled = true;
                }
            };
            ToolTip = "Edit the Text. Press" + Environment.NewLine + "Enter to make changes." + Environment.NewLine + "Shift+Enter to insert a newline." + Environment.NewLine + "Esc to cancel editing.";

            FormatedTextEditor.SetRichTextBoxTextFromTextBlock(editor, designItem.Component as TextBlock);
            editor.TextChanged += editor_TextChanged;
        }

        void editor_TextChanged(object sender, TextChangedEventArgs e)
        {
            FormatedTextEditor.SetTextBlockTextFromRichTextBlox(this.designItem, editor);
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);
            StartEditing();
        }

        /// <summary>
        /// Change is committed if the user releases the Escape Key.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.KeyboardDevice.Modifiers != ModifierKeys.Shift)
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        // Commit the changes to DOM.
                        if (designItem.Properties[FontFamilyProperty].ValueOnInstance != editor.FontFamily)
                            designItem.Properties[FontFamilyProperty].SetValue(editor.FontFamily);
                        if ((double)designItem.Properties[FontSizeProperty].ValueOnInstance != editor.FontSize)
                            designItem.Properties[FontSizeProperty].SetValue(editor.FontSize);
                        if ((FontStretch)designItem.Properties[FontStretchProperty].ValueOnInstance != editor.FontStretch)
                            designItem.Properties[FontStretchProperty].SetValue(editor.FontStretch);
                        if ((FontStyle)designItem.Properties[FontStyleProperty].ValueOnInstance != editor.FontStyle)
                            designItem.Properties[FontStyleProperty].SetValue(editor.FontStyle);
                        if ((FontWeight)designItem.Properties[FontWeightProperty].ValueOnInstance != editor.FontWeight)
                            designItem.Properties[FontWeightProperty].SetValue(editor.FontWeight);

                        if (changeGroup != null && _isChangeGroupOpen)
                        {
                            FormatedTextEditor.SetTextBlockTextFromRichTextBlox(this.designItem, editor);
                            changeGroup.Commit();
                            _isChangeGroupOpen = false;
                        }
                        changeGroup = null;
                        this.Visibility = Visibility.Hidden;
                        this.designItem.ReapplyAllExtensions();
                        ((TextBlock)designItem.Component).Visibility = Visibility.Visible;
                        break;
                    case Key.Escape:
                        AbortEditing();
                        break;
                }
            }
            else if (e.Key == Key.Enter)
            {
                editor.Selection.Text += Environment.NewLine;
            }
        }

        public void AbortEditing()
        {
            if (changeGroup != null && _isChangeGroupOpen)
            {
                changeGroup.Abort();
                _isChangeGroupOpen = false;
            }
            this.Visibility = Visibility.Hidden;
        }

        public void StartEditing()
        {
            //if (changeGroup == null)
            //{
            //    changeGroup = designItem.OpenGroup("Change Text");
            //    _isChangeGroupOpen = true;
            //}
            this.Visibility = Visibility.Visible;
        }
    }
}
