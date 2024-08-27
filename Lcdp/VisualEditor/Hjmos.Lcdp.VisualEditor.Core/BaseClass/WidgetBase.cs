using Hjmos.Lcdp.Common;
using Hjmos.Lcdp.VisualEditor.Core.Attached;
using Hjmos.Lcdp.VisualEditor.Core.Entities;
using Hjmos.Lcdp.VisualEditor.Core.Enums;
using Hjmos.Lcdp.VisualEditor.Core.Events;
using Hjmos.Lcdp.VisualEditor.Core.Helpers;
using Hjmos.Lcdp.VisualEditor.Core.Interfaces;
using Hjmos.Lcdp.VisualEditor.Core.ViewModels;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Hjmos.Lcdp.VisualEditor.Core.BaseClass
{
    /// <summary>
    /// 组件基类（UserControl）
    /// </summary>
    public abstract class WidgetBase : UserControl, IWidget
    {
        public Guid Guid { get; set; }

        protected IEventAggregator _ea;

        /// <summary>
        /// 页面API对象
        /// </summary>
        public IPageApi PageApi { get; set; }

        /// <summary>
        /// 平台组件API对象
        /// </summary>
        public IWidgetApi WidgetApi { get; set; }

        /// <summary>
        /// 用来关联全局参数的组件字段列表
        /// TODO: 全局参数后续改用其他方式
        /// </summary>
        public List<string> WidgetFieldList { get; set; }

        public WidgetBase()
        {
            // PS：如果控件构造函数中没有写InitializeComponent()，这个事件不会被执行，会导致下方WidgetApi为null后报错
            this.Initialized += delegate
            {
                // 订阅消息中心事件
                _ea = ContainerLocator.Current.Resolve<IEventAggregator>();
                _ea.GetEvent<MessageCenterEvent>().Subscribe(o => MessageReceived?.Invoke(o), ThreadOption.UIThread);

                // 平台API
                PageApi = ContainerLocator.Current.Resolve<IPageApi>();
                WidgetApi = ContainerLocator.Current.Resolve<IWidgetApi>();
            };

            // 注册附加属性改变事件
            this.DataContextChanged += delegate { AttachedPropertyChanged += parameters => (DataContext as WidgetViewModelBase)?.RaiseAttachedPropertyChanged(parameters); };

            //this.Loaded+=delegate { WidgetLoaded?.Invoke(); };

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
        public IUnit Init()
        {
            Debug.Assert(WidgetApi is not null, "WidgetApi为null，请检查控件构造函数中有没有调用InitializeComponent函数");

            WidgetApi.BaseInit(this);

            WidgetLoaded?.Invoke();

            return this;
        }

        /// <summary>
        /// 加载完成事件
        /// </summary>
        public event Action WidgetLoaded;

        /// <summary>
        /// 初始化全局变量绑定关系
        /// </summary>
        public void InitParameterBinding()
        {
            // 属性和变量对应关系
            ObservableCollection<ParameterMapping> parameterMappingList = ParameterMappingAttached.GetParameterMapping(this);

            if (parameterMappingList != null && parameterMappingList.Count > 0)
            {
                _ea.GetEvent<VariableEvent>().Subscribe(
                    p =>
                    {
                        // 全局变量改变后，给属性赋值
                        string variableName = p.GetValue<string>("VariableName");
                        string newValue = p.GetValue<string>("NewValue");

                        // TODO：绑定属性的变量加一个Attribute
                        PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(this, new Attribute[] { new PropertyFilterAttribute(PropertyFilterOptions.All) }).OfType<PropertyDescriptor>().FirstOrDefault(x => x.Name == variableName);
                        DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(propertyDescriptor);

                        if (dpd != null && dpd.DependencyProperty != null)
                        {
                            this.SetValue(dpd.DependencyProperty, newValue);
                        }
                    },
                    ThreadOption.PublisherThread, false,
                    // 过滤条件
                    EventFilter
                );
            }
        }

        /// <summary>
        /// 事件过滤条件（lambda表达式无法访问外部变量，单独定义一个方法）
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public bool EventFilter(IEventParameters parameters)
        {
            // 判断是不是组件需要关注的属性
            // TODO：除了属性绑定，还要考虑其他的绑定，如地址参数
            return ParameterMappingAttached.GetParameterMapping(this).Any(x => x.Parameter.Name == parameters.GetValue<string>("VariableName"))
                && parameters.GetValue<string>("WidgetGuid") != this.Guid.ToString();// 不响应组件自身触发的改变
        }

        /// <summary>
        /// 设置组件参数
        /// </summary>
        /// <param name="node">页面节点</param>
        public void SetOption(IUnitNode node)
        {
            WidgetApi.SetOption(this, node);

            // 设置组件参数后初始化绑定关系
            InitParameterBinding();
        }

        /// <summary>
        /// 获取组件节点
        /// </summary>
        public IUnitNode GetNode()
        {
            IUnitNode node = new UnitNode
            {
                Part = new List<IUnitNode>(),
                NodeType = NodeType.Widget,
                ElementType = this.GetType().FullName,
                Guid = this.Guid == Guid.Empty ? Guid.NewGuid() : this.Guid,
                // 获取参数配置
                Options = DependencyObjectHelper.GetPropertyDescribers(this)
            };

            IEnumerable<IWidget> parts = this.TryFindAllChildWidget<IWidget>();

            // TODO: 测试获取组件的子部件
            // TODO: 子部件是否合适也包括普通控件，不一定要求实现IWidget
            foreach (IWidget part in parts)
            {
                // 如果是子部件
                if (PartAttached.GetIsPart(part as DependencyObject))
                {
                    IUnitNode partNode = part.GetNode();
                    // TODO: 改成给GetNode传参，不用获取Child
                    partNode.Child = null;
                    // TODO: 是否有必要限制必须有name属性
                    if (partNode != null && !partNode.Options.ContainsKey("name"))
                    {
                        node.Part.Add(partNode);
                    }
                }
            }

            return node;
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        public void SetData() => throw new NotImplementedException();

        /// <summary>
        /// 渲染数据
        /// </summary>
        public void Render() => throw new NotImplementedException();
    }
}
