using System;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    /// <summary>
    /// 将扩展应用于所选组件。
    /// </summary>
    public class SelectionExtensionServer : DefaultExtensionServer
    {
        /// <summary>
        /// 在扩展服务器初始化并设置了Context属性之后调用。
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();
            Services.Selection.SelectionChanged += OnSelectionChanged;
        }

        private void OnSelectionChanged(object sender, DesignItemCollectionEventArgs e) => ReapplyExtensions(e.Items);

        /// <summary>
        /// 获取项是否被选中
        /// </summary>
        public override bool ShouldApplyExtensions(DesignItem extendedItem) => Services.Selection.IsComponentSelected(extendedItem);
    }

    /// <summary>
    /// 将扩展应用于所选组件，但不应用于主选择。
    /// </summary>
    public class SecondarySelectionExtensionServer : SelectionExtensionServer
    {
        /// <summary>
        /// 获取项是否在二级选择中。
        /// </summary>
        public override bool ShouldApplyExtensions(DesignItem extendedItem) => base.ShouldApplyExtensions(extendedItem) && Services.Selection.PrimarySelection != extendedItem;
    }

    /// <summary>
    /// 对主选择应用扩展。
    /// </summary>
    public class PrimarySelectionExtensionServer : DefaultExtensionServer
    {
        private DesignItem oldPrimarySelection;

        /// <summary>
        /// 在扩展服务初始化并设置了Context属性之后调用。
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();
            this.Services.Selection.PrimarySelectionChanged += OnPrimarySelectionChanged;
        }

        /// <summary>
        /// 主选择改变事件
        /// </summary>
        private void OnPrimarySelectionChanged(object sender, EventArgs e)
        {
            DesignItem newPrimarySelection = this.Services.Selection.PrimarySelection;

            // 注释ReapplyExtensions后，组件就没有装饰层了，组件从这个方法开始附加装饰层
            if (oldPrimarySelection != newPrimarySelection)
            {
                if (oldPrimarySelection == null)
                {
                    ReapplyExtensions(new DesignItem[] { newPrimarySelection });
                }
                else if (newPrimarySelection == null)
                {
                    ReapplyExtensions(new DesignItem[] { oldPrimarySelection });
                }
                else
                {
                    ReapplyExtensions(new DesignItem[] { oldPrimarySelection, newPrimarySelection });
                }
                oldPrimarySelection = newPrimarySelection;
            }
        }

        /// <summary>
        /// 获取项是否是主选择项
        /// </summary>
        public override bool ShouldApplyExtensions(DesignItem extendedItem) => Services.Selection.PrimarySelection == extendedItem;
    }

    /// <summary>
    /// 仅当多个项被选中时，应用一个扩展到主选择
    /// </summary>
    public class PrimarySelectionButOnlyWhenMultipleSelectedExtensionServer : PrimarySelectionExtensionServer
    {
        /// <summary>
        /// 在扩展服务器初始化并设置了Context属性之后调用。
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();
            this.Services.Selection.SelectionChanged += OnSelectionChanged;
        }

        private void OnSelectionChanged(object sender, EventArgs e) => ReapplyExtensions(this.Services.Selection.SelectedItems);

        /// <inheritdoc/>
        public override bool ShouldBeReApplied() => true;

        /// <summary>
        /// 获取项是否在二级选择中。
        /// </summary>
        public override bool ShouldApplyExtensions(DesignItem extendedItem) => Services.Selection.PrimarySelection == extendedItem && Services.Selection.SelectionCount > 1;
    }

    /// <summary>
    /// 仅当多个项被选中时应用扩展!
    /// </summary>
    public class MultipleSelectedExtensionServer : DefaultExtensionServer
    {
        /// <summary>
        /// 在扩展服务器初始化并设置了Context属性之后调用。
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();
            this.Services.Selection.SelectionChanged += OnSelectionChanged;
        }

        private void OnSelectionChanged(object sender, EventArgs e) => ReapplyExtensions(this.Services.Selection.SelectedItems);

        /// <summary>
        /// 获取项是否在二级选择中。
        /// </summary>
        public override bool ShouldApplyExtensions(DesignItem extendedItem) => Services.Selection.SelectionCount > 1;
    }

    /// <summary>
    /// 如果“仅选中一项”，则对主选择应用扩展。
    /// </summary>
    public class OnlyOneItemSelectedExtensionServer : PrimarySelectionExtensionServer
    {
        /// <summary>
        /// 在扩展服务器初始化并设置了Context属性之后调用。
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();
            this.Services.Selection.SelectionChanged += OnSelectionChanged;
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            if (this.Services.Selection.SelectedItems.Count > 1)
                ReapplyExtensions(this.Services.Selection.SelectedItems);
        }

        /// <summary>
        /// 获取项是否是主选择项。
        /// </summary>
        public override bool ShouldApplyExtensions(DesignItem extendedItem) => Services.Selection.PrimarySelection == extendedItem && Services.Selection.SelectionCount < 2;
    }

    /// <summary>
    /// 将扩展应用于主选择的父对象。
    /// </summary>
    public class PrimarySelectionParentExtensionServer : DefaultExtensionServer
    {
        /// <summary>
        /// 在扩展服务器初始化并设置了Context属性之后调用。
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();
            this.Services.Selection.PrimarySelectionChanged += OnPrimarySelectionChanged;
        }

        private DesignItem primarySelection;
        private DesignItem primarySelectionParent;

        private void OnPrimarySelectionChanged(object sender, EventArgs e)
        {
            DesignItem newPrimarySelection = this.Services.Selection.PrimarySelection;
            if (primarySelection != newPrimarySelection)
            {
                if (primarySelection != null)
                {
                    primarySelection.ParentChanged -= OnParentChanged;
                }
                if (newPrimarySelection != null)
                {
                    newPrimarySelection.ParentChanged += OnParentChanged;
                }
                primarySelection = newPrimarySelection;
                OnParentChanged(sender, e);
            }
        }

        private void OnParentChanged(object sender, EventArgs e)
        {
            DesignItem newPrimarySelectionParent = primarySelection != null ? primarySelection.Parent : null;

            if (primarySelectionParent != newPrimarySelectionParent)
            {
                DesignItem oldPrimarySelectionParent = primarySelectionParent;
                primarySelectionParent = newPrimarySelectionParent;
                ReapplyExtensions(new DesignItem[] { oldPrimarySelectionParent, newPrimarySelectionParent });
            }
        }

        /// <summary>
        /// 获取项是否是主选择项。
        /// </summary>
        public override bool ShouldApplyExtensions(DesignItem extendedItem) => primarySelectionParent == extendedItem;
    }
}
