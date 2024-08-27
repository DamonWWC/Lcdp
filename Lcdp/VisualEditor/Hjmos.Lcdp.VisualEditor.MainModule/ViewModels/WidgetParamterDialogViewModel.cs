using Hjmos.Lcdp.Common;
using Hjmos.Lcdp.VisualEditor.Core.Attached;
using Hjmos.Lcdp.VisualEditor.Core.Entities;
using Hjmos.Lcdp.VisualEditor.Core.Events;
using Hjmos.Lcdp.VisualEditor.Core.Interfaces;
using Hjmos.Lcdp.VisualEditor.Core.ViewModels;
using Hjmos.Lcdp.VisualEditor.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.MainModule.ViewModels
{
    public class WidgetParamterDialogViewModel : ViewModelBase, IDialogAware
    {
        #region Property

        public string Title => "选择数据源";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed() { }

        /// <summary>
        /// 组件字段
        /// </summary>
        public List<string> WidgetFieldList
        {
            get => _widgetFieldList;
            set => SetProperty(ref _widgetFieldList, value);
        }
        private List<string> _widgetFieldList;


        /// <summary>
        /// 选中的组件
        /// </summary>
        public UIElement SelectedElement
        {
            get => _selectedElement;
            set => SetProperty(ref _selectedElement, value);
        }
        private UIElement _selectedElement;

        /// <summary>
        /// 选中的字段
        /// </summary>
        public object SelectedField
        {
            get => _selectedField;
            set => SetProperty(ref _selectedField, value);
        }
        private object _selectedField;

        /// <summary>
        /// 选中的参数
        /// </summary>
        public ParameterModel SelectedParamter
        {
            get => _selectedParamter;
            set => SetProperty(ref _selectedParamter, value);
        }
        private ParameterModel _selectedParamter;

        /// <summary>
        /// 参数映射列表
        /// </summary>
        private ObservableCollection<ParameterMapping> _mapping;

        /// <summary>
        /// 变量列表
        /// </summary>
        public ObservableCollection<ParameterModel> ParameterList
        {
            get => _parameterList ??= PageApi.ParameterList;
            set => SetProperty(ref _parameterList, value);
        }
        private ObservableCollection<ParameterModel> _parameterList;

        #endregion

        /// <summary>
        /// 添加命令
        /// </summary>
        public ICommand AddCommand { get; private set; }

        /// <summary>
        /// 删除命令
        /// </summary>
        public ICommand DeleteCommand { get; private set; }

        private IDialogParameters _parameters;

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _parameters = parameters;

            if (_parameters.ContainsKey("WidgetFieldList"))
            {
                WidgetFieldList = _parameters.GetValue<List<string>>("WidgetFieldList");
            }
            if (_parameters.ContainsKey("SelectedElement"))
            {
                SelectedElement = _parameters.GetValue<UIElement>("SelectedElement");
                _mapping = ParameterMappingAttached.GetParameterMapping(SelectedElement);

                if (_mapping is null) return;

                foreach (ParameterMapping item in _mapping)
                {
                    item.DeleteCommand = new DelegateCommand<ParameterMapping>(RemoveParameterMapping);
                }
            }
        }

        public WidgetParamterDialogViewModel(IEventAggregator ea)
        {
            // 添加命令
            AddCommand = new DelegateCommand(() =>
            {
                _mapping.Add(new ParameterMapping() { WidgetField = SelectedField, Parameter = SelectedParamter, DeleteCommand = new DelegateCommand<ParameterMapping>(RemoveParameterMapping) });


                IWidget widget = SelectedElement as IWidget;

                // TODO：此处代码和WidgetBase类似，考虑是否合并
                ea.GetEvent<VariableEvent>().Subscribe(
                    p =>
                    {
                        // 全局变量改变后，给属性赋值
                        string variableName = p.GetValue<string>("VariableName");
                        string newValue = p.GetValue<string>("NewValue");

                        // TODO：绑定属性的变量加一个Attribute
                        PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(SelectedElement, new Attribute[] { new PropertyFilterAttribute(PropertyFilterOptions.All) }).OfType<PropertyDescriptor>().FirstOrDefault(x => x.Name == variableName);
                        DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(propertyDescriptor);

                        if (dpd != null && dpd.DependencyProperty != null)
                        {
                            SelectedElement.SetValue(dpd.DependencyProperty, newValue);
                        }
                    },
                    ThreadOption.PublisherThread, false,
                    // 过滤条件
                    EventFilter
                );
            });
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
            return ParameterMappingAttached.GetParameterMapping(SelectedElement).Any(x => x.Parameter.Name == parameters.GetValue<string>("VariableName"))
                && parameters.GetValue<string>("WidgetGuid") != (SelectedElement as IWidget).Guid.ToString();// 不响应组件自身触发的改变
        }

        /// <summary>
        /// 删除参数映射关系
        /// </summary>
        /// <param name="item"></param>
        private void RemoveParameterMapping(ParameterMapping item) => _mapping.Remove(item);
    }
}
