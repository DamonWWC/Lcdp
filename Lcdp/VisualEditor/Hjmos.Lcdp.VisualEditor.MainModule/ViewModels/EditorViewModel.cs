using Hjmos.Lcdp.VisualEditor.Core.Interfaces;
using Hjmos.Lcdp.VisualEditor.Core.Managers;
using Hjmos.Lcdp.VisualEditor.Core.ViewModels;
using Prism.Commands;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.MainModule.ViewModels
{
    public class EditorViewModel : ViewModelBase
    {
        /// <summary>
        /// 窗体加载完成命令
        /// </summary>
        public ICommand WinLoadedCommand { get; private set; }

        public EditorViewModel(IPageApi pageApi)
        {
            // 标记当前为设计时状态
            pageApi.IsDesignMode = true;

            // 窗体加载完成命令
            WinLoadedCommand = new DelegateCommand(() =>
            {
                // 命令行传过来的参数 - 页面GUID
                string[] args = StateManager.CommandLineArgs;
                if (args.Length > 0)
                {
                    State.PageShell.PageGuid = args[0];
                }
            });
        }
    }
}