using Hjmos.Lcdp.VisualEditor.Controls.DataFields;
using Hjmos.Lcdp.VisualEditor.Core.Managers;
using Hjmos.Lcdp.VisualEditor.Core.ViewModels;
using Hjmos.Lcdp.VisualEditor.Models;
using System;
using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Controls.ViewModels
{
    public class ChildPageViewModel : WidgetViewModelBase
    {
        private string _PageGuid = string.Empty;
        public string PageGuid
        {
            get => _PageGuid;
            set => SetProperty(ref _PageGuid, value);
        }

        public ChildPageViewModel()
        {
            AttachedPropertyChanged += parameters =>
            {
                DependencyProperty dp = parameters.GetValue<DependencyProperty>("DependencyProperty");

                if (dp.Name == "DataFields")
                {
                    ChildPageDataFields dataFields = parameters.GetValue<ChildPageDataFields>("NewValue");

                    string guid = dataFields?.PageGuid;
                    if (guid == null) return;

                    //if (guid.Contains("@"))
                    //{
                    //    // 获取全局变量的值
                    //    ParameterModel parameter = StateManager.ParameterList.FirstOrDefault(x => $"@{x.Name}" == guid);

                    //    if (parameter.Value == null) return;

                    //    guid = StateManager.ParameterList.FirstOrDefault(x => $"@{x.Name}" == guid).Value.ToString();
                    //}

                    if (!Guid.TryParse(guid, out Guid result)) return;

                    PageGuid = result.ToString();
                }
            };
        }
    }
}
