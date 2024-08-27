using Hjmos.Lcdp.VisualEditor.Core;
using Hjmos.Lcdp.VisualEditor.Core.Controls;
using Hjmos.Lcdp.VisualEditor.Core.Entities;
using Hjmos.Lcdp.VisualEditor.Core.Events;
using Hjmos.Lcdp.VisualEditor.Core.Interfaces;
using Hjmos.Lcdp.VisualEditor.Core.ViewModels;
using Hjmos.Lcdp.VisualEditor.IService;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.MainModule.ViewModels
{
    public class ToolbarViewModel : ViewModelBase
    {
        #region Command

        /// <summary>
        /// 预览命令
        /// </summary>
        public ICommand PreviewPageCommand { get; private set; }

        /// <summary>
        /// 保存页面命令
        /// </summary>
        public ICommand SavePageCommand => new DelegateCommand(OnSavePage);

        /// <summary>
        /// 加载页面命令
        /// </summary>
        public ICommand LoadPageCommand => new DelegateCommand(() => OnLoadPage("341bbaca-baeb-4e15-8217-ea5e085aaeed"));

        #endregion

        private readonly IPageService _pageService;

        public ToolbarViewModel(IPageService pageService, IEventAggregator ea)
        {
            _pageService = pageService;

            // 显示编辑器
            ea.GetEvent<SwitchPageEvent>().Subscribe(p =>
            {
                if (!p.ContainsKey("PageGuid")) return;

                OnLoadPage(p.GetValue<string>("PageGuid"));
            });


            // 预览页面命令
            PreviewPageCommand = new DelegateCommand(() =>
            {
                // 获取解决方案目录
                string dir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                // 获取播放器程序路径
                string path = Path.Combine(dir, @"VisualEditor\output\Hjmos.Lcdp.VisualPlayer.exe");

                // 启动播放器，使用Process.Start有时候调用不起来。需要补充WorkingDirectory
                Process process = new();
                process.StartInfo.FileName = path;
                process.StartInfo.WorkingDirectory = Path.GetDirectoryName(path);
                process.StartInfo.Arguments = State.PageShell.PageGuid;
                process.Start();

                //等待播放器启动完毕
                //process.WaitForInputIdle();

            });
        }

        /// <summary>
        /// 保存页面事件
        /// </summary>
        private async void OnSavePage()
        {
            // 页面根节点(GetNode方法会返回所有子节点)
            RootNode root = (State.PageShell.RootElement as IRoot).GetNode() as RootNode;

            if (string.IsNullOrEmpty(State.PageShell.PageGuid) || State.PageShell.RawData is null)
            {
                MessageBox.Show("请先加载页面");
                return;
            }

            //序列化为JSON
            State.PageShell.RawData.Content = JsonConvert.SerializeObject(root);

            //保存数据
            bool result = await _pageService.Save(State.PageShell.RawData);

            if (result)
                MessageBox.Show("保存成功");
            else
                MessageBox.Show("保存失败");

        }

        /// <summary>
        /// 加载页面
        /// </summary>
        /// <param name="PageGuid">页面Guid</param>
        private void OnLoadPage(string PageGuid)
        {
            //// 清空画布装饰器
            //DesignSurface.DeselectElement();
            State.PageShell.PageGuid = PageGuid;
        }
    }
}
