using Prism.Ioc;
using System;

namespace Hjmos.Lcdp.Helpers
{
    /// <summary>
    /// 插件注册类型到宿主
    /// </summary>
    public class RegisterTypesHelper
    {
        /// <summary>
        /// 懒汉单例
        /// </summary>
        private static readonly Lazy<RegisterTypesHelper> lazy = new Lazy<RegisterTypesHelper>(() => new RegisterTypesHelper());

        /// <summary>
        /// 获取RegisterTypesHelper的实例
        /// </summary>
        public static RegisterTypesHelper Instance => lazy.Value;

        public IContainerRegistry ContainerRegistry { get; set; }

        public void RegisterTypes(Action<IContainerRegistry> action) => action(ContainerRegistry);
    }
}
