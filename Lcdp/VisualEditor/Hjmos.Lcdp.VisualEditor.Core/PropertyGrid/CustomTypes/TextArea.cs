namespace Hjmos.Lcdp.VisualEditor.Core.CustomTypes
{
    /// <summary>
    /// 专门针对数据面板多行文本创建的类
    /// </summary>
    public class TextArea
    {

        public TextArea(string value)
        {
            this.Value = value;
        }

        /// <summary>
        /// 内容
        /// </summary>
        public string Value { get; private set; }

    }
}
