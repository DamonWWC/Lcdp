using Hjmos.Lcdp.Common;
using Hjmos.Lcdp.Plugins.NccControl.Models;
using Hjmos.Lcdp.VisualEditor.Core.Events;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Hjmos.Lcdp.Plugins.NccControl.ViewModels
{
    public class EmergencyCommandMenuViewModel : ViewModelBase
    {
        #region Property

        /// <summary>
        /// 高度
        /// </summary>
        public double Width
        {
            get => _width;
            set => SetProperty(ref _width, value);
        }
        private double _width = 82d;

        /// <summary>
        /// 宽度
        /// </summary>
        public double Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }
        private double _height = 72d;

        /// <summary>
        /// 组件Guid
        /// </summary>
        public Guid Guid
        {
            get => _guid;
            set => SetProperty(ref _guid, value);
        }
        private Guid _guid;

        /// <summary>
        /// 当前选中的菜单
        /// </summary>
        public EmergencyCommandMenuModel CurrentSelectedMenu
        {
            get => _currentSelectedMenu;
            set => SetProperty(ref _currentSelectedMenu, value);
        }
        private EmergencyCommandMenuModel _currentSelectedMenu;

        /// <summary>
        /// 菜单的集合
        /// </summary>
        public List<EmergencyCommandMenuModel> MainMenus
        {
            get => _mainMenus;
            set => SetProperty(ref _mainMenus, value);
        }
        private List<EmergencyCommandMenuModel> _mainMenus;

        #endregion

        private readonly IEventAggregator _ea;

        public EmergencyCommandMenuViewModel(IEventAggregator ea)
        {
            _ea = ea;

            AttachedPropertyChanged += parameters =>
            {
                // 获取变化的附加属性
                DependencyProperty dp = parameters.GetValue<DependencyProperty>("DependencyProperty");
                if (dp.Name == "Json")
                {
                    object newValue = parameters.GetValue<object>("NewValue");

                    if (string.IsNullOrEmpty(newValue.ToString()))
                    {
                        return;
                    }

                    try
                    {
                        // JSON字符串转换成模型
                        MainMenus = JsonConvert.DeserializeObject<List<EmergencyCommandMenuModel>>(newValue.ToString());
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("JSON字符串有误");
                        return;
                    }

                    EmergencyCommandMenuModel selectedMenu = MainMenus.FirstOrDefault(x => x.IsSelected);

                    if (selectedMenu != null)
                    {
                        // 初始化选中项
                        SelectedMenuChanged(selectedMenu);
                    }
                 

                    Width = MainMenus.Count() * 82d;
                }
                else if (dp.Name == "ParameterMapping")
                {
                    // TODO：初始加载好像不走这边？
                    var a = parameters;
                }
            };
        }

        /// <summary>
        /// 选中菜单按钮的触发
        /// </summary>
        public ICommand SelectedMenuCommand => new DelegateCommand<EmergencyCommandMenuModel>(model => SelectedMenuChanged(model));

        /// <summary>
        /// 选中菜单改变逻辑
        /// </summary>
        /// <param name="model"></param>
        private void SelectedMenuChanged(EmergencyCommandMenuModel model)
        {
            CurrentSelectedMenu = model;
            MainMenus.ForEach(x => x.IsSelected = x.Guid == model.Guid);

            // 事件传参
            IEventParameters parameters = new EventParameters {
                { "Type", typeof(EmergencyCommandMenuViewModel) },
                { "Guid", Guid },
                { "PageGuid", CurrentSelectedMenu.Guid }
            };
            _ea.GetEvent<CallBehindCodeEvent>().Publish(parameters);
        }
    }
}
