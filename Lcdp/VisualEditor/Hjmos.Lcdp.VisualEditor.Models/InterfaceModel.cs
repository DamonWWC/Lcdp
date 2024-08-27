using Prism.Mvvm;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Models
{
    /// <summary>
    /// 数据源接口
    /// </summary>
    public class InterfaceModel : BindableBase
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
        /// 接口名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 编辑接口
        /// </summary>
        public ICommand ModifyCommand { get; set; }

        /// <summary>
        /// 删除接口
        /// </summary>
        public ICommand DeleteCommand { get; set; }
    }
}
