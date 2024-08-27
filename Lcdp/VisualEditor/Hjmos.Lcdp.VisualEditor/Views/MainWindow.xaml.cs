using HandyControl.Controls;
using System;

namespace Hjmos.Lcdp.VisualEditor.Views
{
    public partial class MainWindow
    {
        public MainWindow() => InitializeComponent();

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            // Chrome区域内容
            NonClientAreaContent = new NonClientAreaContent();

            // 禁用Alt+F4关闭窗口
            WindowAttach.SetIgnoreAltF4(this, true);
        }
    }
}