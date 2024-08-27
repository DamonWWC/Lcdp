using System;
using System.Collections.Generic;

namespace Hjmos.Lcdp.VisualEditor.Core
{
    internal static class SharedInstances
    {
        internal static readonly object BoxedTrue = true;
        internal static readonly object BoxedFalse = false;
        internal static readonly object BoxedDouble1 = 1.0;
        internal static readonly object BoxedDouble0 = 0.0;
        internal static readonly object[] EmptyObjectArray = new object[0];
        internal static readonly DesignItem[] EmptyDesignItemArray = new DesignItem[0];

        internal static object Box(bool value) => value ? BoxedTrue : BoxedFalse;
    }

    /// <summary>
    /// 泛型缓存，把枚举的值装箱
    /// </summary>
    /// <typeparam name="T">枚举类型</typeparam>
    internal static class SharedInstances<T> where T : Enum
    {
        private static readonly Dictionary<T, object> _boxedEnumValues;

        static SharedInstances()
        {
            _boxedEnumValues = new Dictionary<T, object>();
            foreach (object value in Enum.GetValues(typeof(T)))
            {
                _boxedEnumValues.Add((T)value, value);
            }
        }

        internal static object Box(T value) => _boxedEnumValues[value];
    }
}
