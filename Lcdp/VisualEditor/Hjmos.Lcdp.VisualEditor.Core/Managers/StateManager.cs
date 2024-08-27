using Hjmos.Lcdp.VisualEditor.Core.Controls;
using Hjmos.Lcdp.VisualEditorServer.Entities;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Data;

namespace Hjmos.Lcdp.VisualEditor.Core.Managers
{
    /// <summary>
    /// 负责维护应用程序的全局状态数据
    /// </summary>
    public sealed class StateManager : BindableBase
    {
        /// <summary>
        /// 插件目录下的所有程序集
        /// </summary>
        public static List<Assembly> PluginList { get; set; } = new List<Assembly>();

        /// <summary>
        /// 命令行参数
        /// </summary>
        public static string[] CommandLineArgs { get; set; }

        /// <summary>
        /// 当前登录的用户
        /// </summary>
        public User CurrentUser { get; set; }

        /// <summary>
        /// 当前设计界面
        /// </summary>
        public DesignSurface CurrentDesignSurface
        {
            get => _currentDesignSurface;
            set
            {
                SetProperty(ref _currentDesignSurface, value);

                // 这里做绑定，是为了方便外面直接绑定当前选中的元素
                // PrimarySelections可能为空，只能绑定到DesignSurface，再提供Path，不能直接绑定Component
                Binding binding = new("DesignContext.Services.Selection.PrimarySelection.Component")
                {
                    Source = _currentDesignSurface
                };
                _currentDesignSurface.SetBinding(DesignSurface.SelectedElementProperty, binding);
            }
        }
        private DesignSurface _currentDesignSurface;

        /// <summary>
        /// 当前设计界面中的页面容器
        /// </summary>
        public PageShell PageShell => CurrentDesignSurface.PageShell;

        /// <summary>
        /// 装饰层
        /// </summary>
        public DesignPanel DesignPanel => CurrentDesignSurface.DesignPanel;
    }
}