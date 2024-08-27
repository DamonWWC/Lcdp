using System.Collections.Generic;

namespace Hjmos.Lcdp.VisualEditor.Core.CustomTypes
{
    /// <summary>
    /// 专门针对数据面板维度字段创建的类，可以多选维度
    /// </summary>
    public class DimensionArray
    {

        public DimensionArray(List<string> values) => this.Values = values;

        /// <summary>
        /// 维度数组
        /// </summary>
        public List<string> Values { get; set; }
    }
}
