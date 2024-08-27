using Hjmos.Lcdp.VisualEditor.Core.Controls;
using Hjmos.Lcdp.VisualEditor.Core.Interfaces;
using Hjmos.Lcdp.VisualEditor.Core.Managers;
using Prism.Ioc;
using Prism.Mvvm;

namespace Hjmos.Lcdp.VisualPlayer.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        private string _title = "Visual Player";

        public string PageGuid
        {
            get => _pageGuid;
            set => SetProperty(ref _pageGuid, value);
        }
        private string _pageGuid = "341bbaca-baeb-4e15-8217-ea5e085aaeed";

        //public IPageParameters PageParameters
        //{
        //    get => _pageParameters;
        //    set => SetProperty(ref _pageParameters, value);
        //}
        //private IPageParameters _pageParameters;

        public MainWindowViewModel(IPageApi pageApi)
        {
            // 标记当前为运行时状态
            pageApi.IsDesignMode = false;


            //// TODO: 临时代码
            //PageParameters = new PageParameters()
            //{
            //    { "eventId", 207544082505728d },
            //    { "planId", 165838527176710d },
            //    { "eventType", EventType.突发事件 },
            //    { "topic", "三号线番禺广场 运力非正常下降（一级）2021-08-09" },
            //    { "lineCode", "03" },
            //    { "stationCode", "0313" }
            //};

            var state = ContainerLocator.Current.Resolve<StateManager>();

            // 命令行传过来的参数 - 页面GUID
            string[] args = StateManager.CommandLineArgs;
            if (args.Length > 0)
            {
                PageGuid = args[0];

                PageShell pageShell = state.CurrentDesignSurface.PageShell;
                //// 清空画布装饰器
                //DesignSurface.DeselectElement();
                pageShell.PageGuid = PageGuid;
            }
        }
    }
}
