using Hjmos.Lcdp.Common;
using Hjmos.Lcdp.VisualEditor.Core.Events;
using Hjmos.Lcdp.VisualEditor.Core.ViewModels;
using Hjmos.Lcdp.VisualEditor.IService;
using Hjmos.Lcdp.VisualEditor.Models;
using Hjmos.Lcdp.VisualEditorServer.Entities.Enums;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Unity;

namespace Hjmos.Lcdp.EditorManagement.PageModule.ViewModels
{
    public class ModuleDirectoryViewModel : PageViewModelBase
    {
        #region Property

        /// <summary>
        /// 选中的项
        /// </summary>
        public FileModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value, () =>
                {
                    IsFile = SelectedItem != null && SelectedItem.FileType == FileType.File;
                    Guid = SelectedItem == null || !Guid.TryParse(SelectedItem.Guid, out Guid result) ? Guid.Empty : result;
                });
            }
        }
        private FileModel _selectedItem;

        /// <summary>
        /// 指示选中项是不是文件
        /// </summary>
        public bool IsFile
        {
            get => _isFile;
            set => SetProperty(ref _isFile, value);
        }
        private bool _isFile = false;

        /// <summary>
        /// 选中文件的Guid
        /// </summary>
        public Guid Guid
        {
            get => _guid;
            set => SetProperty(ref _guid, value);
        }
        private Guid _guid;

        /// <summary>
        /// 当前父目录
        /// </summary>
        public FileModel CurrentPath
        {
            get => _currentPath;
            set => SetProperty(ref _currentPath, value);
        }
        private FileModel _currentPath;

        /// <summary>
        /// 指示是否能添加文件
        /// </summary>
        public bool CanAddFile
        {
            get => _canAddFile;
            set => SetProperty(ref _canAddFile, value);
        }
        private bool _canAddFile = true;

        public ObservableCollection<FileModel> DirectoryList { get; set; } = new ObservableCollection<FileModel>();

        #endregion

        #region Command

        /// <summary>
        /// 打开编辑器编辑页面命令
        /// </summary>
        public ICommand OpenCommand { get; private set; }

        /// <summary>
        /// 预览命令
        /// </summary>
        public ICommand PreviewCommand { get; private set; }

        /// <summary>
        /// 窗体加载完毕命令
        /// </summary>
        public ICommand LoadedCommand { get; private set; }

        /// <summary>
        /// 打开文件夹命令
        /// </summary>
        public ICommand OpenDirectoryCommand { get; private set; }

        /// <summary>
        /// 回到主页命令
        /// </summary>
        public ICommand HomeCommand { get; private set; }

        /// <summary>
        /// 打开新增页面窗口命令
        /// </summary>
        public ICommand AddPageCommand { get; private set; }

        /// <summary>
        /// 打开新增目录窗口命令
        /// </summary>
        public ICommand AddDirectoryCommand { get; private set; }

        #endregion

        private readonly IModuleService _moduleService;
        private readonly IDialogService _dialogService;
        private readonly IEventAggregator _ea;

        public ModuleDirectoryViewModel(
            IModuleService moduleService,
            IRegionManager regionManager,
            IUnityContainer unityContainer,
            IEventAggregator ea,
            IDialogService dialogService
        ) : base(regionManager, unityContainer, ea)
        {
            _moduleService = moduleService;
            _dialogService = dialogService;
            _ea = ea;
            PageTitle = "应用管理";

            // 编辑页面命令
            OpenCommand = new DelegateCommand(() =>
            {
                IEventParameters parameters = new EventParameters { { "PageGuid", SelectedItem.Guid.ToString() } };
                // 发送通知切换页面
                _ea.GetEvent<SwitchPageEvent>().Publish(parameters);
            });

            // 预览页面命令
            PreviewCommand = new DelegateCommand(() =>
            {

                // 获取解决方案目录
                string dir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                // 获取编辑器程序路径
                string path = Path.Combine(dir, @"VisualEditor\output\Hjmos.Lcdp.VisualPlayer.exe");

                // 启动编辑器，使用Process.Start有时候调用不起来。需要补充WorkingDirectory
                Process process = new Process();
                process.StartInfo.FileName = path;
                process.StartInfo.WorkingDirectory = Path.GetDirectoryName(path);
                process.StartInfo.Arguments = SelectedItem.Guid.ToString();
                process.Start();

                //等待编辑器启动完毕
                //process.WaitForInputIdle();
            });

            // 窗体加载完毕命令
            LoadedCommand = new DelegateCommand(() => GetModules());

            // 回到主页命令
            HomeCommand = new DelegateCommand(() => GetModules());

            // 打开文件夹命令
            OpenDirectoryCommand = new DelegateCommand<MouseButtonEventArgs>(async e =>
            {
                if (SelectedItem.FileType == FileType.Directory)
                {
                    CurrentPath = SelectedItem;
                    var items = await _moduleService.GetSubItems(SelectedItem.Id);
                    // TODO：这里代码跟GetModules的有重复
                    items.ToList().ForEach(i =>
                    {
                        i.DeleteCommand = new DelegateCommand<FileModel>(async f =>
                        {
                            bool result = await _moduleService.DeleteFileOrDirectory(f);
                            if (result)
                            {
                                // TODO：关闭窗体，刷新父目录
                            }
                        });
                    });

                    DirectoryList.Clear();
                    DirectoryList.AddRange(items);
                    CanAddFile = true;
                }

            });

            // 打开新增页面窗口命令
            AddPageCommand = new DelegateCommand(() =>
            {
                // 窗口传参
                IDialogParameters param = new DialogParameters { { "CurrentPath", CurrentPath } };

                _dialogService.ShowDialog("AddPageDialog", param, d =>
                {
                    // TODO：刷新窗体
                });
            });

            // 打开新增目录窗口命令
            AddDirectoryCommand = new DelegateCommand(() =>
            {
                // 窗口传参
                IDialogParameters param = new DialogParameters { { "CurrentPath", CurrentPath } };

                _dialogService.ShowDialog("AddDirectoryDialog", param, d =>
                {
                    if (d != null && d.Result == ButtonResult.OK)
                    {
                        Refresh();
                    }
                });
            });

        }

        /// <summary>
        /// 加载所有应用
        /// </summary>
        private async void GetModules()
        {
            CurrentPath = null;
            CanAddFile = false;

            var items = await _moduleService.GetModules();
            // TODO：这里代码跟OpenDirectoryCommand的有重复
            items.ToList().ForEach(i =>
            {
                i.DeleteCommand = new DelegateCommand<FileModel>(async f =>
                {
                    bool result = await _moduleService.DeleteFileOrDirectory(f);
                    if (result)
                    {
                        // TODO：关闭窗体，刷新父目录
                    }
                });
            });
            DirectoryList.Clear();
            DirectoryList.AddRange(items);
        }

        public async override void Refresh()
        {
            // TODO：这里代码和OpenDirectoryCommand中是重复的
            var items = await _moduleService.GetSubItems(CurrentPath is null ? 0 : CurrentPath.Id);
            // TODO：这里代码跟GetModules的有重复
            items.ToList().ForEach(i =>
            {
                i.DeleteCommand = new DelegateCommand<FileModel>(async f =>
                {
                    bool result = await _moduleService.DeleteFileOrDirectory(f);
                    if (result)
                    {
                        Refresh();
                    }
                });
            });

            DirectoryList.Clear();
            DirectoryList.AddRange(items);
            CanAddFile = true;
        }

    }
}
