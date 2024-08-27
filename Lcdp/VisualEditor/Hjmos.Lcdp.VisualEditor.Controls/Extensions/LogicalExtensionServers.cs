namespace Hjmos.Lcdp.VisualEditor.Controls.Extensions
{
    /// <summary>
    /// 使用逻辑OR组合两个扩展服务
    /// </summary>
    public sealed class LogicalOrExtensionServer<A, B> : DefaultExtensionServer where A : ExtensionServer where B : ExtensionServer
    {
        private ExtensionServer _a;
        private ExtensionServer _b;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            _a = Context.Services.ExtensionManager.GetExtensionServer(new ExtensionServerAttribute(typeof(A)));
            _b = Context.Services.ExtensionManager.GetExtensionServer(new ExtensionServerAttribute(typeof(B)));
            _a.ShouldApplyExtensionsInvalidated += OnShouldApplyExtensionsInvalidated;
            _b.ShouldApplyExtensionsInvalidated += OnShouldApplyExtensionsInvalidated;
        }

        private void OnShouldApplyExtensionsInvalidated(object sender, DesignItemCollectionEventArgs e) => ReapplyExtensions(e.Items);

        public override bool ShouldApplyExtensions(DesignItem extendedItem) => _a.ShouldApplyExtensions(extendedItem) || _b.ShouldApplyExtensions(extendedItem);
    }

    /// <summary>
    /// 使用逻辑AND组合两个扩展服务
    /// </summary>
    public sealed class LogicalAndExtensionServer<A, B> : DefaultExtensionServer where A : ExtensionServer where B : ExtensionServer
    {
        private ExtensionServer _a;
        private ExtensionServer _b;

        /// <summary/>
        protected override void OnInitialized()
        {
            base.OnInitialized();
            _a = Context.Services.ExtensionManager.GetExtensionServer(new ExtensionServerAttribute(typeof(A)));
            _b = Context.Services.ExtensionManager.GetExtensionServer(new ExtensionServerAttribute(typeof(B)));
            _a.ShouldApplyExtensionsInvalidated += OnShouldApplyExtensionsInvalidated;
            _b.ShouldApplyExtensionsInvalidated += OnShouldApplyExtensionsInvalidated;
        }

        private void OnShouldApplyExtensionsInvalidated(object sender, DesignItemCollectionEventArgs e) => ReapplyExtensions(e.Items);

        public override bool ShouldApplyExtensions(DesignItem extendedItem) => _a.ShouldApplyExtensions(extendedItem) && _b.ShouldApplyExtensions(extendedItem);
    }
}
