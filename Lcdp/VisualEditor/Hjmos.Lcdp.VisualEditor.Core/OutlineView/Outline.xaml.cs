using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Core.OutlineView
{
    public partial class Outline
    {
        public Outline()
        {
            this.InitializeComponent();

            //this.AddCommandHandler(ApplicationCommands.Undo,
            //    () => ((DesignPanel)Root.DesignItem.Services.DesignPanel).DesignSurface.Undo(),
            //    () => Root == null ? false : ((DesignPanel)Root.DesignItem.Services.DesignPanel).DesignSurface.CanUndo());
            //this.AddCommandHandler(ApplicationCommands.Redo,
            //    () => ((DesignPanel)Root.DesignItem.Services.DesignPanel).DesignSurface.Redo(),
            //    () => Root == null ? false : ((DesignPanel)Root.DesignItem.Services.DesignPanel).DesignSurface.CanRedo());
            //this.AddCommandHandler(ApplicationCommands.Copy,
            //    () => ((DesignPanel)Root.DesignItem.Services.DesignPanel).DesignSurface.Copy(),
            //    () => Root == null ? false : ((DesignPanel)Root.DesignItem.Services.DesignPanel).DesignSurface.CanCopyOrCut());
            //this.AddCommandHandler(ApplicationCommands.Cut,
            //    () => ((DesignPanel)Root.DesignItem.Services.DesignPanel).DesignSurface.Cut(),
            //    () => Root == null ? false : ((DesignPanel)Root.DesignItem.Services.DesignPanel).DesignSurface.CanCopyOrCut());
            //this.AddCommandHandler(ApplicationCommands.Delete,
            //    () => ((DesignPanel)Root.DesignItem.Services.DesignPanel).DesignSurface.Delete(),
            //    () => Root == null ? false : ((DesignPanel)Root.DesignItem.Services.DesignPanel).DesignSurface.CanDelete());
            //this.AddCommandHandler(ApplicationCommands.Paste,
            //    () => ((DesignPanel)Root.DesignItem.Services.DesignPanel).DesignSurface.Paste(),
            //    () => Root == null ? false : ((DesignPanel)Root.DesignItem.Services.DesignPanel).DesignSurface.CanPaste());
            //this.AddCommandHandler(ApplicationCommands.SelectAll,
            //    () => ((DesignPanel)Root.DesignItem.Services.DesignPanel).DesignSurface.SelectAll(),
            //    () => Root == null ? false : ((DesignPanel)Root.DesignItem.Services.DesignPanel).DesignSurface.CanSelectAll());
        }

        public static readonly DependencyProperty RootProperty =
            DependencyProperty.Register("Root", typeof(IOutlineNode), typeof(Outline));

        public IOutlineNode Root
        {
            get => (IOutlineNode)GetValue(RootProperty);
            set => SetValue(RootProperty, value);
        }

        public object OutlineContent => this;
    }
}
