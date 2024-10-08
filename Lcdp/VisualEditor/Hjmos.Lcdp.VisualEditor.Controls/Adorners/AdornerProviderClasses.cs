﻿using Hjmos.Lcdp.VisualEditor.Controls.Extensions;

namespace Hjmos.Lcdp.VisualEditor.Controls.Adorners
{
    // 从AdornerProvider派生的一些类来指定某个ExtensionServer。

    /// <summary>
    /// 一种永久附加的装饰扩展。
    /// </summary>
    [ExtensionServer(typeof(DefaultExtensionServer.PermanentWithDesignPanel))]
    public abstract class PermanentAdornerProvider : AdornerProvider
    {

    }

    /// <summary>
    /// An adorner extension that is attached to selected components.
    /// 附加到选定组件的装饰器扩展。
    /// </summary>
    [ExtensionServer(typeof(SelectionExtensionServer))]
    public abstract class SelectionAdornerProvider : AdornerProvider
    {

    }

    /// <summary>
    /// An adorner extension that is attached to the primary selection.
    /// 附加到主选择的装饰器扩展。
    /// 继承自这个类的装饰器，意味着仅在其装饰的控件是主要选择时才会被激活。
    /// </summary>
    [ExtensionServer(typeof(PrimarySelectionExtensionServer))]
    public abstract class PrimarySelectionAdornerProvider : AdornerProvider
    {

    }

    /// <summary>
    /// An adorner extension that is attached to the secondary selection.
    /// 附加在第二选择上的装饰扩展。
    /// </summary>
    [ExtensionServer(typeof(SecondarySelectionExtensionServer))]
    public abstract class SecondarySelectionAdornerProvider : AdornerProvider
    {

    }
}
