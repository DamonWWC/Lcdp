using Hjmos.Lcdp.Plugins.NccControl.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Hjmos.Lcdp.Plugins.NccControl.ViewModels
{
    public class UCCarousel1ViewModel : ViewModelBase
    {

        /// <summary>
        /// 突发事件信息
        /// </summary>
        public List<EmergencyEvent> EmergencyInfos
        {
            get => _emergencyInfos;
            set => SetProperty(ref _emergencyInfos, value);
        }
        private List<EmergencyEvent> _emergencyInfos = new();

        public UCCarousel1ViewModel()
        {
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
                        EmergencyInfos = JsonConvert.DeserializeObject<List<EmergencyEvent>>(newValue.ToString());
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("JSON字符串有误");
                        return;
                    }
                }
            };
        }
    }
}
