namespace Hjmos.Lcdp.VisualEditor.Core.CustomTypes
{
    /// <summary>
    /// 专门针对数据面板维度字段创建的类
    /// </summary>
    public class Dimension
    {

        public Dimension(string value) => this.Value = value;

        /// <summary>
        /// 维度值
        /// </summary>
        public string Value { get; private set; }

    }
}
