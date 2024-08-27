using Hjmos.Lcdp.VisualEditor.IService;
using Hjmos.Lcdp.VisualEditor.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

namespace Hjmos.Lcdp.EditorManagement.PageModule.ViewModels
{
    public class AddLibDialogViewModel : BindableBase, IDialogAware
    {
        #region Property

        public string Title => "上传组件";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed() { }

        /// <summary>
        /// 上传的组件列表
        /// </summary>
        public ObservableCollection<WidgetLibModel> Libs { get; set; } = new ObservableCollection<WidgetLibModel>();

        #endregion

        #region Command

        /// <summary>
        /// 选择文件命令
        /// </summary>
        public ICommand SelectFileCommand { get; private set; }

        /// <summary>
        /// 上传文件命令
        /// </summary>
        public ICommand UploadCommand { get; private set; }

        #endregion

        IDialogParameters _parameters;

        public void OnDialogOpened(IDialogParameters parameters) => _parameters = parameters;

        public AddLibDialogViewModel(IWidgetService widgetLibService)
        {
            // 选择要上传的文件
            SelectFileCommand = new DelegateCommand(new Action(() =>
            {
                Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
                dialog.Multiselect = true;
                if (dialog.ShowDialog() == true)
                {
                    Libs.Clear();
                    if (dialog.FileNames != null && dialog.FileNames.Length > 0)
                    {
                        for (int i = 0; i < dialog.FileNames.Length; i++)
                        {
                            Libs.Add(new WidgetLibModel
                            {
                                Index = (i + 1).ToString("00"),
                                FullPath = dialog.FileNames[i],
                                Name = new FileInfo(dialog.FileNames[i]).Name,
                                State = "待上传"
                            });
                        }
                    }
                }
            }));

            // 文件上传命令
            UploadCommand = new DelegateCommand(new Action(async () =>
            {
                if (this.Libs.Count > 0)
                {
                    foreach (WidgetLibModel item in this.Libs)
                    {
                        if (item.State != "完成")
                        {
                            bool state = await widgetLibService.UploadLib(item.Name, item.FullPath);
                            if (state)
                                item.State = "完成";
                        }
                    };
                    // TODO：通知主窗口刷新
                }
            }));
        }

    }
}
