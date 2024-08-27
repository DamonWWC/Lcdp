using Prism.Mvvm;

namespace Hjmos.Lcdp.Plugins.NccControl.Models
{
    public class EmergencyCommandMenuModel : BindableBase
    {
        public string Name { get; set; }

        public string Guid { get; set; }

        public bool IsEnabled { get; set; }

        public string Icon { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
        private bool _isSelected;
    }
}
