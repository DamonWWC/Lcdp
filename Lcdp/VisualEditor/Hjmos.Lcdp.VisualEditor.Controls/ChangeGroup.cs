using System;

namespace Hjmos.Lcdp.VisualEditor.Controls
{
    /// <summary>
    /// 设计面板上选中控件更改组的基类。
    /// </summary>
    public abstract class ChangeGroup : IDisposable
    {

        /// <summary>获取/设置更改组的标题</summary>
        public string Title { get; set; }

        /// <summary>提交更改组</summary>
        public abstract void Commit();

        /// <summary>终止变更组</summary>
        public abstract void Abort();

        /// <summary>在释放更改组时调用。如果变更组还没有提交，则中止变更组</summary>
        protected abstract void Dispose();

        /// <summary>指向重写的销毁方法</summary>
        void IDisposable.Dispose() => Dispose();
    }
}
