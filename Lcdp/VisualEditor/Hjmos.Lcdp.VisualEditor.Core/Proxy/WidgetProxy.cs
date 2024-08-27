using Hjmos.Lcdp.Common;
using Hjmos.Lcdp.ILoger;
using Hjmos.Lcdp.VisualEditor.Core.Interfaces;
using Prism.Ioc;
using System;
using System.Collections.Generic;

namespace Hjmos.Lcdp.VisualEditor.Core.Proxy
{
    /// <summary>
    /// 组件代理类
    /// </summary>
    public class WidgetProxy : IWidget
    {
        private readonly IWidget _widget;

        private readonly ILogHelper _logHelper;

        /// <summary>
        /// 平台页面API对象
        /// </summary>
        public IPageApi PageApi
        {
            get => _widget.PageApi;
            set => _widget.PageApi = value;
        }

        /// <summary>
        /// 平台组件API对象
        /// </summary>
        public IWidgetApi WidgetApi
        {
            get => _widget.WidgetApi;
            set => _widget.WidgetApi = value;
        }


        public WidgetProxy(IWidget widget)
        {
            _widget = widget;

            _logHelper = ContainerLocator.Current.Resolve<ILogHelper>();
        }

        /// <summary>
        /// 用来关联全局参数的组件字段列表
        /// TODO：全局参数后续改用其他方式
        /// </summary>
        public List<string> WidgetFieldList
        {
            get => _widget.WidgetFieldList;
            set => _widget.WidgetFieldList = value;
        }

        public Guid Guid
        {
            get => _widget.Guid;
            set => _widget.Guid = value;
        }

        /// <summary>
        /// 附加属性改变事件
        /// </summary>
        public event Action<IEventParameters> AttachedPropertyChanged
        {
            add => _widget.AttachedPropertyChanged += value;
            remove => _widget.AttachedPropertyChanged -= value;
        }

        /// <summary>
        /// 加载完成事件
        /// </summary>
        public event Action WidgetLoaded
        {
            add => _widget.WidgetLoaded += value;
            remove => _widget.WidgetLoaded -= value;
        }

        /// <summary>
        /// 触发附加属性改变事件
        /// </summary>
        /// <param name="parameters"></param>
        public void RaiseAttachedPropertyChanged(IEventParameters parameters) => _widget.RaiseAttachedPropertyChanged(parameters);

        /// <summary>
        /// 接收消息事件
        /// </summary>
        public event Action<object> MessageReceived
        {
            add => _widget.MessageReceived += value;
            remove => _widget.MessageReceived -= value;
        }

        /// <summary>
        /// 获取组件节点
        /// </summary>
        public IUnitNode GetNode()
        {
            return _widget.GetNode();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public IUnit Init()
        {
            _logHelper.Debug(_widget, $"组件初始化开始");

            try
            {
                _widget.Init();
            }
            catch (Exception ex)
            {
                _logHelper.Error(_widget, $"初始化时发生异常", ex);
            }

            _logHelper.Debug(_widget, $"组件初始化结束");

            return this;

        }

        /// <summary>
        /// 设置组件参数
        /// </summary>
        /// <param name="node">页面节点</param>
        public void SetOption(IUnitNode node)
        {
            _logHelper.Debug(_widget, $"组件设置参数开始");

            try
            {
                _widget.SetOption(node);
            }
            catch (Exception ex)
            {
                _logHelper.Error(_widget, $"设置参数时发生异常", ex);
                throw ex;
            }

            _logHelper.Debug(_widget, $"组件设置参数结束");

        }

        /// <summary>
        /// 渲染数据
        /// </summary>
        public void Render()
        {
            _widget.Render();
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        public void SetData()
        {
            _widget.SetData();
        }

        public void InitParameterBinding()
        {
            _widget.InitParameterBinding();
        }
    }
}
