using Hjmos.Lcdp.VisualEditor.Core.Attributes;
using Hjmos.Lcdp.VisualEditor.Core.Interfaces;
using Hjmos.Lcdp.VisualEditorServer.Entities.DTO;

namespace Hjmos.Lcdp.VisualEditor.Core.BaseClass
{
    /// <summary>
    /// 数据面板上的字段对象的基类
    /// </summary>
    [ConvertToJson]
    [TypeNameMark]
    public class DataFieldsBase : IDataFields
    {
        /// <summary>
        /// 数据源地址
        /// </summary>
        public InterfaceDTO DataSource { get; set; }

        /// <summary>
        /// 数据源维度列表
        /// </summary>
        public string[] DimensionArray { get; set; }

        ///// <summary>
        ///// 保持子类实际的类型
        ///// </summary>
        public string TypeName => this.GetType().FullName;


        /// <summary>
        /// 内存浅拷贝（快速复制内存中的对象，不走构造函数）
        /// </summary>
        /// <returns></returns>
        public object Clone() => MemberwiseClone();
    }
}
