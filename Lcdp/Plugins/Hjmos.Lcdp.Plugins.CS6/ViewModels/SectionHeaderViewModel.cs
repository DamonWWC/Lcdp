using Hjmos.Lcdp.Plugins.CS6.DataFields;
using Hjmos.Lcdp.VisualEditor.Core.ViewModels;
using System.Windows;

namespace Hjmos.Lcdp.Plugins.CS6.ViewModels
{
    public class SectionHeaderViewModel : WidgetViewModelBase
    {
        /// <summary>
        /// 主标题
        /// </summary>
        public string MainTitle
        {
            get => _mainTitle;
            set => SetProperty(ref _mainTitle, value);
        }
        private string _mainTitle = "主标题";

        /// <summary>
        /// 子标题
        /// </summary>
        public string Subtitle
        {
            get => _subtitle;
            set => SetProperty(ref _subtitle, value);
        }
        private string _subtitle = "SUBTITLE";

        public SectionHeaderViewModel()
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

                     // 主标题
                     MainTitle = dataFields.MainTitle;
                     // 子标题
                     Subtitle = dataFields.Subtitle;
                 }
             };
        }
    }
}
