using System;
using System.Collections.Generic;

namespace Hjmos.Lcdp.VisualEditor.Controls
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

    internal static class SharedInstances<T> where T : struct, IConvertible
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
