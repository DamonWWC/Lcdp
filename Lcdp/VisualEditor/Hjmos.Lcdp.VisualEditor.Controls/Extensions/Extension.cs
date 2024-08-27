using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Controls.Extensions
{
    /// <summary>
    /// 所有扩展的基类。
    /// </summary>
    /// <remarks>
    /// Extensions命名空间中的类设计是为了匹配Cider的设计，如博文中所述:  
    /// https://docs.microsoft.com/en-us/archive/blogs/jnak/cider-item-creation
    /// https://docs.microsoft.com/en-us/archive/blogs/jnak/monthcalendar-adorner-sample
    /// </remarks>
    public abstract class Extension
    {
        /// <summary>
        /// 获取对象的<see cref="DisabledExtensionsProperty"/>附加属性的值。
        /// </summary>
        /// <param name="obj">从其中读取属性值的对象。</param>
        /// <returns>对象的<see cref="DisabledExtensionsProperty"/>属性值。</returns>
        public static string GetDisabledExtensions(DependencyObject obj) => obj != null ? (string)obj.GetValue(DisabledExtensionsProperty) : null;

        /// <summary>
        /// 设置对象的<see cref="DisabledExtensionsProperty"/>附加属性的值。
        /// </summary>
        /// <param name="obj">将附加属性写入的对象。</param>
        /// <param name="value">要设置的值。</param>
        public static void SetDisabledExtensions(DependencyObject obj, string value) => obj.SetValue(DisabledExtensionsProperty, value);

        /// <summary>
        /// 获取或设置一个分号分隔的列表，该列表具有组件视图禁用的扩展名。
        /// </summary>
        public static readonly DependencyProperty DisabledExtensionsProperty =
            DependencyProperty.RegisterAttached("DisabledExtensions", typeof(string), typeof(Extension), new PropertyMetadata(null));


        /// <summary>
        /// 获取对象的<see cref="DisableMouseOverExtensionsProperty"/>附加属性的值。
        /// </summary>
        /// <param name="obj">从其中读取属性值的对象。</param>
        /// <returns>对象的<see cref="DisableMouseOverExtensionsProperty"/>属性值。</returns>
        public static bool GetDisableMouseOverExtensions(DependencyObject obj) => (bool)obj.GetValue(DisableMouseOverExtensionsProperty);

        /// <summary>
        /// 设置对象的<see cref="DisableMouseOverExtensionsProperty"/>附加属性的值。
        /// </summary>
        /// <param name="obj">将附加属性写入的对象。</param>
        /// <param name="value">要设置的值。</param>
        public static void SetDisableMouseOverExtensions(DependencyObject obj, bool value) => obj.SetValue(DisableMouseOverExtensionsProperty, value);

        /// <summary>
        /// 禁用此元素的鼠标移到扩展上
        /// </summary>
        public static readonly DependencyProperty DisableMouseOverExtensionsProperty =
            DependencyProperty.RegisterAttached("DisableMouseOverExtensions", typeof(bool), typeof(Extension), new PropertyMetadata(false));
    }
}
