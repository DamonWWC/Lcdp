using Hjmos.Lcdp.VisualEditorServer.Entities.DTO;

namespace Hjmos.Lcdp.VisualEditor.Core.Interfaces
{
    /// <summary>
    /// 数据面板上的字段对象的接口
    /// </summary>
    public interface IDataFields
    {
        /// <summary>
        /// 数据源地址
        /// </summary>
        InterfaceDTO DataSource { get; set; }

        /// <summary>
        /// 数据源维度列表
        /// </summary>
        string[] DimensionArray { get; set; }

        /// <summary>
        /// 保持子类实际的类型
        /// </summary>
        string TypeName { get; }

        /// <summary>
        /// 浅拷贝
        /// </summary>
        /// <returns></returns>
        object Clone();
    }
}
