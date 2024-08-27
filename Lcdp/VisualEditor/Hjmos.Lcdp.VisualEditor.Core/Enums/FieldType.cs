using System.ComponentModel;

namespace Hjmos.Lcdp.VisualEditor.Core.Enums
{
    /// <summary>
    /// 属性面板字段可以渲染的表单种类
    /// </summary>
    public enum FieldType
    {
        [Description("文本框")]
        TextBox = 1,
        [Description("复选框")]
        CheckBox = 2,
        [Description("下拉框")]
        ComboBox = 3
    }
}
