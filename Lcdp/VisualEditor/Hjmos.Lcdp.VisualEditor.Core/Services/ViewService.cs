namespace Hjmos.Lcdp.VisualEditor.Core.Services
{
    internal sealed class DefaultViewService : ViewService
    {
        private readonly DesignContext context;

        public DefaultViewService(DesignContext context) => this.context = context;

        public override DesignItem GetModel(System.Windows.DependencyObject view)
        {
            // In the WPF designer, we do not support having a different view for a component
            // 在WPF设计器中，我们不支持对组件使用不同的视图
            return context.Services.Component.GetDesignItem(view);
        }
    }
}
