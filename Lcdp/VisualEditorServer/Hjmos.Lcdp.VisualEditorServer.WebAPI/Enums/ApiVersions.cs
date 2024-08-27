using System.ComponentModel;

namespace Hjmos.Lcdp.VisualEditorServer.WebAPI.Enums
{
    /// <summary>
    /// OpenAPI版本管理枚举类
    /// </summary>
    public enum ApiVersions
    {
        [Description("WebAPI Version 01")]
        v1 = 1,
        [Description("WebAPI Version 02")]
        v2 = 2,
        [Description("WebAPI Version 03")]
        v3 = 3
    }
}
