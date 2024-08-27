using Hjmos.Lcdp.VisualEditor.Core.Attributes;
using Hjmos.Lcdp.VisualEditor.Models;
using Newtonsoft.Json;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Core.Entities
{
    /// <summary>
    /// 组件字段和全局参数映射关系
    /// </summary>
    [ConvertToJson]
    public class ParameterMapping
    {
        /// <summary>
        /// 组件字段
        /// </summary>
        public object WidgetField { get; set; }

        /// <summary>
        /// 全局参数
        /// </summary>
        public ParameterModel Parameter { get; set; }

        /// <summary>
        /// 删除命令
        /// </summary>
        [JsonIgnore]
        public ICommand DeleteCommand { get; set; }

    }
}
