namespace Hjmos.Lcdp.VisualEditor.Controls.Extensions2
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
