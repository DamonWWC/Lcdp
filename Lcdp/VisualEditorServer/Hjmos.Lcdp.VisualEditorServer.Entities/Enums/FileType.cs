using System.ComponentModel;

namespace Hjmos.Lcdp.VisualEditorServer.Entities.Enums
{
    /// <summary>
    /// 文件类型
    /// </summary>
    public enum FileType
    {
        [Description("目录")]
        Directory = 1,
        [Description("文件")]
        File = 2,
    }
}
