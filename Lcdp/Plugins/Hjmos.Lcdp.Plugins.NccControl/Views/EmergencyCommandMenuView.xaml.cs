using Hjmos.Lcdp.Plugins.NccControl.ViewModels;
using Hjmos.Lcdp.VisualEditor.Core.Attributes;
using Hjmos.Lcdp.VisualEditor.Core.Events;
using Hjmos.Lcdp.VisualEditor.Core.Helpers;
using Hjmos.Lcdp.VisualEditor.Core.Interfaces;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Hjmos.Lcdp.Plugins.NccControl.Views
{
    [Widget(Category = "应急指挥独立版", DisplayName = "NCC菜单", DefaultWidth = 82d, DefaultHeight = 72d)]

    public partial class EmergencyCommandMenuView
    {
        /// <summary>
        /// 接收外部全局参数
        /// </summary>
        public string PageGuid
        {
            get => (string)GetValue(PageGuidProperty);
            set => SetValue(PageGuidProperty, value);
        }

        public static readonly DependencyProperty PageGuidProperty =
            DependencyProperty.Register("PageGuid", typeof(string), typeof(EmergencyCommandMenuView), new PropertyMetadata(string.Empty, (d, e) =>
            {
                // TODO: 这里第一个参数，不是组件的属性名，是变量名字符串
                // TODO: 属性改变事件，统一自动写，不用每个属性写
                (d as EmergencyCommandMenuView).PageApi.SetParameter("PageGuid", e.NewValue.ToString(), (d as IWidget).Guid.ToString());
            }));


        public EmergencyCommandMenuView()
        {
            InitializeComponent();

            // TODO: 设置实时回馈状态到控制器
            this.Loaded += (s, e) =>
            {
                // 发送加载完毕，那个菜单被选中
                _ea.GetEvent<MessageCenterEvent>().Publish("MenuLoaded");
            };

            EmergencyCommandMenuViewModel vm = DataContext as EmergencyCommandMenuViewModel;

            // 传递Guid给ViewModel，接收事件的时候用来定位组件
            // TODO: 考虑替代的方式
            vm.Guid = this.Guid;

            // TODO: 定义对外提供的参数列表，之后改成在附加属性上打特性
            WidgetFieldList = new List<string>() { "PageGuid" };

            MessageReceived += (o) =>
            {
                if (!string.IsNullOrEmpty(o as string))
                {
                    string msg = o as string;
                    if (msg == "jcyj") // 监测预警
                    {
                        PageGuid = vm.MainMenus.FirstOrDefault(x => x.Name == "监测预警")?.Guid;
                    }
                    else if (msg == "yjzy") // 应急资源
                    {
                        PageGuid = vm.MainMenus.FirstOrDefault(x => x.Name == "应急资源")?.Guid;
                    }
                }
            };

            AttachedPropertyChanged += parameters =>
            {
                // 获取变化的附加属性
                DependencyProperty dp = parameters.GetValue<DependencyProperty>("DependencyProperty");
                if (dp.Name == "LoadedCode")
                {
                    string strLoadedCode = parameters.GetValue<string>("NewValue");

                    if (string.IsNullOrEmpty(strLoadedCode)) return;

                    try
                    {
                        EvalCodeHelper.Eval(strLoadedCode, this);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("LoadedCode字符串有误");
                        return;
                    }
                }
            };

            // 订阅消息中心事件
            _ea.GetEvent<CallBehindCodeEvent>().Subscribe(p =>
            {
                if (!p.ContainsKey("Guid") || !p.ContainsKey("Type") || !p.ContainsKey("PageGuid")) return;
                if (p.GetValue<Type>("Type") != typeof(EmergencyCommandMenuViewModel)) return;

                PageGuid = p.GetValue<string>("PageGuid");
            }, ThreadOption.UIThread);
        }
    }
}