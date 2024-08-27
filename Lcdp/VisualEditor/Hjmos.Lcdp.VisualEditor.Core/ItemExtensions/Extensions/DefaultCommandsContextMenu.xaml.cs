namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    public partial class DefaultCommandsContextMenu
    {
        private readonly DesignItem _designItem;

        public DefaultCommandsContextMenu() => this.InitializeComponent();

        public DefaultCommandsContextMenu(DesignItem designItem)
        {
            _designItem = designItem;

            this.InitializeComponent();
        }

    }
}
