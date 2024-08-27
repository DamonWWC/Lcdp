using Hjmos.Lcdp.VisualEditor.Core.ItemExtensions;
using Hjmos.Lcdp.VisualEditor.Core.Services;
using System.Diagnostics;
using System.Xml;

namespace Hjmos.Lcdp.VisualEditor.Core
{
    /// <summary>
    /// 设计器上下文
    /// </summary>
    public abstract class DesignContext
    {

        /// <summary>
        /// 创建一个新的DesignContext实例。
        /// </summary>
        protected DesignContext() => Services.AddService(typeof(ExtensionManager), new ExtensionManager(this));

        public ServiceContainer Services { [DebuggerStepThrough] get; } = new();

        /// <summary>获取根设计项</summary>
        public abstract DesignItem RootItem { get; }

        /// <summary>将设计的元素保存为XML</summary>
        public abstract void Save(XmlWriter writer);

        ///// <summary>
        ///// 打开一个用于批处理多个更改的新更改组。
        ///// ChangeGroups作为事务工作，用于支持撤消/重做系统。
        ///// </summary>
        //public abstract ChangeGroup OpenGroup(string changeGroupTitle, ICollection<DesignItem> affectedItems);
    }
}
