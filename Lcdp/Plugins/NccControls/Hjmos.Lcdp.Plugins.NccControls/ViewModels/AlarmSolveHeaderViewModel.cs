using Hjmos.Lcdp.VisualEditor.Core.Helpers;
using Prism.Mvvm;
using System.Windows.Media;

namespace Hjmos.Lcdp.Plugins.NccControls.ViewModels
{
    public class AlarmSolveHeaderViewModel : BindableBase
    {
        /// <summary>
        /// 主题
        /// </summary>
        public string Topic
        {
            get => _topic;
            set => SetProperty(ref _topic, value);
        }
        private string _topic;

        /// <summary>
        /// 事件类型
        /// </summary>
        public string Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }
        private string _type;

        /// <summary>
        /// 类型颜色
        /// </summary>
        public Brush TypeBackground
        {
            get => _typeBackground;
            set => SetProperty(ref _typeBackground, value);
        }
        private Brush _typeBackground;

        public AlarmSolveHeaderViewModel() { }
    }
}
