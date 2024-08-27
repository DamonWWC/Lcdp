using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Core
{

    /// <summary>
    /// 表示设计项属性的集合
    /// </summary>
    public abstract class DesignItemPropertyCollection : IEnumerable<DesignItemProperty>
    {
        /// <summary>
        /// 获取具有指定名称的属性
        /// </summary>
        public DesignItemProperty this[string name] => GetProperty(name);

        /// <summary>
        /// 获取表示指定依赖项属性的设计项属性
        /// 属性不能是附加属性
        /// </summary>
        public DesignItemProperty this[DependencyProperty dp] => GetProperty(dp);

        /// <summary>
        /// 获取具有指定名称的设计项属性
        /// </summary>
        public abstract DesignItemProperty GetProperty(string name);

        /// <summary>
        /// 获取具有指定名称的指定所有者的附加属性
        /// </summary>
        public abstract DesignItemProperty GetAttachedProperty(Type ownerType, string name);

        /// <summary>
        /// 获取表示指定依赖项属性的设计项属性
        /// 属性不能是附加属性。
        /// </summary>
        public DesignItemProperty GetProperty(DependencyProperty dp) => dp is not null ? GetProperty(dp.Name) : throw new ArgumentNullException("dependencyProperty");

        /// <summary>
        /// 获取表示指定的附加依赖项属性的设计项属性。
        /// </summary>
        public DesignItemProperty GetAttachedProperty(DependencyProperty dp) => dp is not null ? GetAttachedProperty(dp.OwnerType, dp.Name) : throw new ArgumentNullException("dependencyProperty");

        /// <summary>
        /// 获取枚举器，以枚举具有非默认值的属性
        /// </summary>
        public abstract IEnumerator<DesignItemProperty> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
