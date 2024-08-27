using System.ComponentModel;

namespace Hjmos.Lcdp.VisualEditor.Core.Enums
{
    /// <summary>
    /// 节点类型
    /// </summary>
    public enum NodeType
    {
        [Description("组件")]
        Widget = 1,
        [Description("图层")]
        Layer = 2,
        [Description("分区")]
        Region = 3,
        [Description("页面根节点")]
        Root = 4,
        [Description("页面容器")]
        PageContainer = 5
    }
}
