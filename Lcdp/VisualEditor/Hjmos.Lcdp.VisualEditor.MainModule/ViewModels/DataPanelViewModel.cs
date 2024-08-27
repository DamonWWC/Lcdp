using Hjmos.Lcdp.Common;
using Hjmos.Lcdp.Toolkits;
using Hjmos.Lcdp.VisualEditor.Core.Attached;
using Hjmos.Lcdp.VisualEditor.Core.BaseClass;
using Hjmos.Lcdp.VisualEditor.Core.Helpers;
using Hjmos.Lcdp.VisualEditor.Core.Interfaces;
using Hjmos.Lcdp.VisualEditor.Core.ViewModels;
using Hjmos.Lcdp.VisualEditorServer.Entities.Core;
using Hjmos.Lcdp.VisualEditorServer.Entities.DTO;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Services.Dialogs;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.MainModule.ViewModels
{
    public class DataPanelViewModel : ViewModelBase
    {
        /// <summary>
        /// 打开数据源窗口命令
        /// </summary>
        public ICommand DataSourceCommand { get; private set; }

        /// <summary>
        /// 打开JSON窗口命令
        /// </summary>
        public ICommand JsonCommand { get; private set; }

        /// <summary>
        /// 打开自定义XAML窗口命令
        /// </summary>
        public ICommand CustomXamlCommand { get; private set; }


        /// <summary>
        /// 打开参数设置窗口命令
        /// </summary>
        public ICommand ParameterSettingCommand { get; private set; }

        /// <summary>
        /// 保存命令
        /// </summary>
        public ICommand SaveCommand { get; private set; }

        /// <summary>
        /// 选中的组件
        /// </summary>
        public UIElement SelectedElement => State.CurrentDesignSurface.SelectedElement;

        // 接口返回结果
        private object _result;

        public DataPanelViewModel(IDialogService dialogService)
        {
            // 打开数据源窗口命令
            DataSourceCommand = new DelegateCommand(() =>
            {
                dialogService.ShowDialog("SelectDataSourceDialog", null, async d =>
                {
                    if (d != null && d.Result == ButtonResult.OK)
                    {
                        // 选中组件的数据字段模型
                        DataFieldsBase dataFields = DataFieldsAttached.GetDataFields(SelectedElement);

                        // 获取选中的接口信息
                        InterfaceDTO i = d.Parameters.GetValue<InterfaceDTO>("DataSource");

                        dataFields.DataSource = i;

                        // 从接口获取数据（TODO：后续可以改成用接口ID来获取数据，接口后台可以改变）
                        _result = (await WebApiHelper.GetAsync<Result<object>>(i.Address)).Data;

                        // 获取标准二维表的所有维度
                        dataFields.DimensionArray = DimensionJsonHelper.GetDimensions(JArray.FromObject(_result));

                        // 触发属性改变，刷新数据面板表单
                        DataFieldsAttached.SetDataFields(SelectedElement, dataFields.Clone() as DataFieldsBase);
                    }
                });
            });

            // 打开JSON窗口命令
            JsonCommand = new DelegateCommand(() =>
            {
                // 窗口传参
                IDialogParameters param = new DialogParameters { { "Json", JsonAttached.GetJson(SelectedElement) } };

                dialogService.ShowDialog("JsonDataSourceDialog", param, d =>
                {
                    if (d != null && d.Result == ButtonResult.OK)
                    {
                        string json = d.Parameters.GetValue<string>("Json");

                        JsonAttached.SetJson(SelectedElement, json);
                    }
                });
            });

            // 打开自定义XAML窗口命令
            CustomXamlCommand = new DelegateCommand(() =>
            {
                // 窗口传参
                IDialogParameters param = new DialogParameters { { "CustomXaml", CustomXamlAttached.GetCustomXaml(SelectedElement) } };

                dialogService.ShowDialog("CustomXamlDialog", param, d =>
                {
                    if (d != null && d.Result == ButtonResult.OK)
                    {
                        string customXaml = d.Parameters.GetValue<string>("CustomXaml");

                        CustomXamlAttached.SetCustomXaml(SelectedElement, customXaml);
                    }
                });
            });


            // 打开参数设置窗口命令
            ParameterSettingCommand = new DelegateCommand(() =>
            {
                List<string> list = (SelectedElement as IWidget).WidgetFieldList;

                // 窗口传参
                IDialogParameters param = new DialogParameters { { "WidgetFieldList", list }, { "SelectedElement", SelectedElement } };

                dialogService.ShowDialog("WidgetParamterDialog", param, d =>
                {
                    //if (d != null && d.Result == ButtonResult.OK)
                    //{
                    //}
                });
            });

            // 保存命令
            SaveCommand = new DelegateCommand(() =>
            {
                if (SelectedElement is not IWidget widget) return;

                // 事件传参
                IEventParameters parameters = new EventParameters {
                    { "DependencyProperty", DataFieldsAttached.DataFieldsProperty },
                    { "NewValue", DataFieldsAttached.GetDataFields(SelectedElement) },
                    { "Result", _result }
                };

                widget.RaiseAttachedPropertyChanged(parameters);
            });
        }
    }
}
