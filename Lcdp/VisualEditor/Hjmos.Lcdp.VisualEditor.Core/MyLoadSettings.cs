using System;
using System.Collections.Generic;
using System.Reflection;

namespace Hjmos.Lcdp.VisualEditor.Core
{
    /// <summary>
    /// 用于加载初始化上下文的设置
    /// </summary>
    public sealed class MyLoadSettings
    {
        // 设计器程序集
        public readonly ICollection<Assembly> DesignerAssemblies = new List<Assembly>();

        // 自定义服务注册的委托
        public readonly List<Action<MyDesignContext>> CustomServiceRegisterFunctions = new();

        public string CurrentProjectAssemblyName { get; set; }

        public MyLoadSettings() => DesignerAssemblies.Add(typeof(MyDesignContext).Assembly);
    }
}
