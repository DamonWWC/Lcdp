using Prism.Mvvm;

namespace Hjmos.Lcdp.Plugins.CS6.Models
{
    public class LabelIndicatorModel : BindableBase
    {
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon
        {
            get => _icon;
            set => SetProperty(ref _icon, value);
        }
        private string _icon;

        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        private string _title;

        /// <summary>
        /// 值
        /// </summary>
        public string Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }
        private string _value;
    }
}
