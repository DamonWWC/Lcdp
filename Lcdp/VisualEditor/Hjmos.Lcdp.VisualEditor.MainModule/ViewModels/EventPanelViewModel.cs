using Hjmos.Lcdp.VisualEditor.Core.Attached;
using Hjmos.Lcdp.VisualEditor.Core.ViewModels;
using Prism.Commands;
using Prism.Services.Dialogs;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.MainModule.ViewModels
{
    public class EventPanelViewModel : ViewModelBase
    {
        /// <summary>
        /// 打开加载完毕事件窗口命令
        /// </summary>
        public ICommand LoadedCommand { get; private set; }

        public EventPanelViewModel(IDialogService dialogService)
        {
            // 打开JSON窗口命令
            LoadedCommand = new DelegateCommand(() =>
            {
                // 窗口传参
                IDialogParameters param = new DialogParameters { { "LoadedCode", LoadedCodeAttached.GetLoadedCode(State.CurrentDesignSurface.SelectedElement) } };

                dialogService.ShowDialog("LoadedCodeDialog", param, d =>
                {
                    if (d != null && d.Result == ButtonResult.OK)
                    {
                        string loadedCode = d.Parameters.GetValue<string>("LoadedCode");

                        LoadedCodeAttached.SetLoadedCode(State.CurrentDesignSurface.SelectedElement, loadedCode);
                    }
                });
            });
        }
    }
}