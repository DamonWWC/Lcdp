using Hjmos.Lcdp.VisualEditor.Core.Attributes;
using Hjmos.Lcdp.VisualEditor.Core.Managers;
using Prism.Ioc;

namespace Hjmos.Lcdp.Plugins.NccControls.Views
{
    [Widget(Category = "预警处置", DisplayName = "❌预警处置线网图", DefaultWidth = 1280d, DefaultHeight = 768d)]
    public partial class AlarmSolveLineNetView
    {
        public AlarmSolveLineNetView()
        {
            InitializeComponent();

            var state = ContainerLocator.Current.Resolve<StateManager>();
            // TODO: Services考虑放到PageAPI或者其他地方
            // TODO: 检索有附加属性IsPart的组件进行注册功能
            // 给子部件注册设计时功能
            if (PageApi.IsDesignMode)
            {
                state.CurrentDesignSurface.DesignContext.Services.Component.RegisterComponentForDesigner(header);
                state.CurrentDesignSurface.DesignContext.Services.Component.RegisterComponentForDesigner(grid);
                state.CurrentDesignSurface.DesignContext.Services.Component.RegisterComponentForDesigner(switcher);
            }

            // TODO: 注册名字供UnitNode.GetElement方法中遍历子部件用，因为当时UnitNode对应的组件还没有加到VisualTree，无法使用VisualTreeHelper检索，后续封装一下或找更好的方式代替
            this.RegisterName(header.Name, header);
            this.RegisterName(grid.Name, grid);
            this.RegisterName(switcher.Name, switcher);
        }
    }
}
