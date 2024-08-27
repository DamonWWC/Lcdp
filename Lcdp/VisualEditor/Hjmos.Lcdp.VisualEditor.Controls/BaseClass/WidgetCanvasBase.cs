using Hjmos.Lcdp.Common;
using Hjmos.Lcdp.VisualEditor.Controls.Entities;
using Hjmos.Lcdp.VisualEditor.Core.Enums;
using Hjmos.Lcdp.VisualEditor.Core.Helpers;
using Hjmos.Lcdp.VisualEditor.Core.Interface;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Hjmos.Lcdp.VisualEditor.Controls.BaseClass
{
    /// <summary>
    /// 自定义画布基类（CustomControl）
    /// </summary>
    public abstract class WidgetCanvasBase : Canvas, IWidget
    {
        /// <summary>
        /// 用来关联全局参数的组件字段列表
        /// TODO：全局参数后续改用其他方式
        /// </summary>
        public List<string> WidgetFieldList { get; set; }

        public Guid Guid { get; set; }

        /// <summary>
        /// 平台页面API对象
        /// </summary>
        public IPageApi PageApi { get; set; }

        /// <summary>
        /// 平台组件API对象
        /// </summary>
        public IWidgetApi WidgetApi { get; set; }

        public WidgetCanvasBase()
        {
            PageApi = ContainerLocator.Current.Resolve<IPageApi>();
            WidgetApi = ContainerLocator.Current.Resolve<IWidgetApi>();
        }

        /// <summary>
        /// 接收消息事件
        /// </summary>
        public event Action<object> MessageReceived;

        /// <summary>
        /// 附加属性改变事件
        /// </summary>
        public event Action<IEventParameters> AttachedPropertyChanged;

        /// <summary>
        /// 触发附加属性改变事件
        /// </summary>
        /// <param name="parameters"></param>
        public void RaiseAttachedPropertyChanged(IEventParameters parameters) => AttachedPropertyChanged?.Invoke(parameters);


        /// <summary>
        /// 初始化
        /// </summary>
        public virtual IUnit Init()
        {
            WidgetApi.BaseInit(this);

            return this;
        }

        /// <summary>
        /// 设置组件参数
        /// </summary>
        /// <param name="node">页面节点</param>
        public virtual void SetOption(IUnitNode node)
        {
            WidgetApi.SetOption(this, node);
        }

        /// <summary>
        /// 获取组件节点
        /// </summary>
        public virtual IUnitNode GetNode()
        {
            IUnitNode node = new UnitNode()
            {
                NodeType = NodeType.Widget,
                ElementType = this.GetType().FullName,
                Guid = this.Guid == Guid.Empty ? Guid.NewGuid() : this.Guid
            };

            // 获取参数配置
            node.Options = DependencyObjectHelper.GetPropertyDescribers(this);

            return node;
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        public virtual void SetData()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 渲染数据
        /// </summary>
        public virtual void Render()
        {
            throw new NotImplementedException();
        }

        public void InitParameterBinding()
        {
            throw new NotImplementedException();
        }
    }
}
