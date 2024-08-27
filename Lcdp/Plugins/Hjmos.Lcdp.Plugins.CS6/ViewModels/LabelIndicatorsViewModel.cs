using Hjmos.Lcdp.Plugins.CS6.DataFields;
using Hjmos.Lcdp.Plugins.CS6.Models;
using Hjmos.Lcdp.VisualEditor.Core.ViewModels;
using System.Collections.Generic;
using System.Windows;

namespace Hjmos.Lcdp.Plugins.CS6.ViewModels
{
    public class LabelIndicatorsViewModel : WidgetViewModelBase
    {

        /// <summary>
        /// 菜单的集合
        /// </summary>
        public List<LabelIndicatorModel> LabelIndicatorList
        {
            get => _labelIndicatorList;
            set => SetProperty(ref _labelIndicatorList, value);
        }
        private List<LabelIndicatorModel> _labelIndicatorList = new()
        {
            new LabelIndicatorModel() { Icon="\ue7fa", Title="安全运营天数", Value="1342" },
            new LabelIndicatorModel() { Icon="\ue7fa", Title="安全运营里程", Value="543100" },
            new LabelIndicatorModel() { Icon="\ue7fa", Title="安全运营车次", Value="2789" },
            new LabelIndicatorModel() { Icon="\ue7fa", Title="安全运营客流", Value="790800" },
            new LabelIndicatorModel() { Icon="\ue7fa", Title="安全指数", Value="100" },
            new LabelIndicatorModel() { Icon="\ue7fa", Title="设备故障", Value="316" }
        };

        public LabelIndicatorsViewModel()
        {
            // 数据字段改变，刷新组件内容
            AttachedPropertyChanged += parameters =>
             {
                 // 获取变化的附加属性
                 DependencyProperty dp = parameters.GetValue<DependencyProperty>("DependencyProperty");

                 if (dp.Name == "DataFields")
                 {
                     SectionHeaderDataFields dataFields = parameters.GetValue<SectionHeaderDataFields>("NewValue");

                     if (dataFields is null) return;


                     //LabelIndicatorList
                     //// 主标题
                     //MainTitle = dataFields.MainTitle;
                     //// 子标题
                     //Subtitle = dataFields.Subtitle;
                 }
             };
        }
    }
}
