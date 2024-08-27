using Prism.Mvvm;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Models
{
    public class WidgetModel : BindableBase
    {
        /// <summary>
        /// 序号
        /// </summary>
        public string Index { get; set; }

        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 类别
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 是否显示为样例
        /// </summary>
        public int RenderAsSample { get; set; }

        /// <summary>
        /// 配置组件
        /// </summary>
        public ICommand ConfigCommand { get; set; }

        /// <summary>
        /// 删除组件
        /// </summary>
        public ICommand DeleteCommand { get; set; }
    }
}
