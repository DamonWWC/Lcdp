using Prism.Services.Dialogs;
using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Core.Dialogs
{
    /// <summary>
    /// 弹出窗父窗口
    /// </summary>
    public partial class DialogWindow : Window, IDialogWindow
    {
        public DialogWindow() => InitializeComponent();

        public IDialogResult Result { get; set; }
    }
}
