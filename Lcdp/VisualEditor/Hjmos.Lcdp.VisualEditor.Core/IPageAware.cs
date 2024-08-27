namespace Hjmos.Lcdp.VisualEditor.Core
{
    /// <summary>
    /// 为页面中涉及的对象提供页面上下文和活动通知
    /// </summary>
    public interface IPageAware
    {
        void OnPageLoaded(PageContext pageContext);
    }
}
