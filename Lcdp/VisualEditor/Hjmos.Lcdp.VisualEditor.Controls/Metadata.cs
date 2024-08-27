using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Controls
{
    /// <summary>
    /// 包含检索元数据的帮助器方法。
    /// </summary>
    public static class Metadata
    {
        private class NamedValue
        {
            public string Name { get; set; }

            public object Value { get; set; }
        }

        /// <summary>
        /// 获取依赖项属性的全名 (OwnerType.FullName + "." + Name)
        /// </summary>
        public static string GetFullName(this DependencyProperty p) => p.OwnerType.FullName + "." + p.Name;

        // TODO: do we really want to store these values in a static dictionary? 我们真的希望将这些值存储在静态字典中吗?
        // Why not per-design context (as a service?) 为什么不是每个设计上下文(作为服务?)
        private static readonly Dictionary<Type, List<object>> standardValues = new();
        private static readonly Dictionary<DependencyProperty, object[]> standardValuesForDependencyPropertys = new();
        private static readonly Dictionary<Type, List<NamedValue>> standardNamedValues = new();
        private static readonly Dictionary<Type, Dictionary<DependencyProperty, object>> standardPropertyValues = new();

        /// <summary>
        /// 使用类型<paramref name="valuesContainer"/>的公共静态属性为<paramref name="type"/>注册一组标准值
        /// </summary>
        /// <example>Metadata.AddStandardValues(typeof(Brush), typeof(Brushes));</example>
        public static void AddStandardValues(Type type, Type valuesContainer)
        {
            AddStandardValues(type, valuesContainer.GetProperties(BindingFlags.Public | BindingFlags.Static).Select(p => p.GetValue(null, null)));
        }

        /// <summary>
        /// 使用<paramref name="values"/>为<paramref name="dependencyProperty"/>注册一组标准值。  
        /// </summary>
        /// <example>Metadata.AddStandardValues(typeof(Brush), typeof(Brushes));</example>
        public static void AddStandardValues(DependencyProperty dependencyProperty, params object[] values)
        {
            lock (standardValuesForDependencyPropertys)
            {
                standardValuesForDependencyPropertys[dependencyProperty] = values;
            }
        }

        /// <summary>
        /// 使用类型<paramref name="valuesContainer"/>的公共静态属性为<paramref name="type"/>注册一组标准值
        /// </summary>
        /// <example>Metadata.AddDoubleNamedStandardValues(typeof(Brush), typeof(Brushes));</example>
        public static void AddDoubleNamedStandardValues(Type type, Type valuesContainer)
        {

            IEnumerable<NamedValue> pList = valuesContainer.GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Select(p => new NamedValue { Name = p.Name, Value = p.GetValue(null, null) });

            lock (standardNamedValues)
            {
                if (!standardNamedValues.TryGetValue(type, out List<NamedValue> list))
                {
                    list = new List<NamedValue>();
                    standardNamedValues[type] = list;
                }
                foreach (NamedValue v in pList)
                {
                    list.Add(v);
                }
            }
        }

        /// <summary>
        /// Registers a set of standard <paramref name="values"/> for a <paramref name="type"/>.
        /// </summary>
        /// <remarks>You can call this method multiple times to add additional standard values.</remarks>
        public static void AddStandardValues<T>(Type type, IEnumerable<T> values)
        {
            List<object> list;
            lock (standardValues)
            {
                if (!standardValues.TryGetValue(type, out list))
                {
                    list = new List<object>();
                    standardValues[type] = list;
                }
                foreach (var v in values)
                {
                    list.Add(v);
                }
            }
        }

        /// <summary>
        /// 检索指定<paramref name="type"/>的标准值。
        /// </summary>
        public static IEnumerable GetNamedStandardValues(Type type)
        {
            lock (standardNamedValues)
            {
                if (standardNamedValues.TryGetValue(type, out List<NamedValue> values))
                {
                    return values;
                }
            }
            return null;
        }

        /// <summary>
        /// 检索指定<paramref name="type"/>的标准值。
        /// </summary>
        public static IEnumerable GetStandardValues(Type type)
        {
            Type baseT = Nullable.GetUnderlyingType(type);

            if (type.IsEnum)
            {
                return Enum.GetValues(type);
            }

            if (baseT != null && baseT.IsEnum)
            {
                return Enum.GetValues(baseT);
            }

            lock (standardValues)
            {
                if (standardValues.TryGetValue(type, out List<object> values))
                {
                    return values;
                }
            }
            return null;
        }

        /// <summary>
        /// 检索指定<paramref name="dependencyProperty"/>的标准值
        /// </summary>
        public static IEnumerable GetStandardValues(DependencyProperty dependencyProperty)
        {
            lock (standardValuesForDependencyPropertys)
            {
                if (standardValuesForDependencyPropertys.TryGetValue(dependencyProperty, out object[] values))
                {
                    return values;
                }
            }
            return null;
        }

        #region MyRegion 这些本来就注释掉的
        //static Dictionary<string, string> categories = new Dictionary<string, string>();

        //public static void AddCategory(DependencyProperty p, string category)
        //{
        //    lock (categories) {
        //        categories[p.GetFullName()] = category;
        //    }
        //}

        //public static void AddCategory(Type type, string property, string category)
        //{
        //    lock (categories) {
        //        categories[type + "." + property] = category;
        //    }
        //}

        //public static string GetCategory(DesignItemProperty p)
        //{
        //    string result;
        //    lock (categories) {
        //        if (categories.TryGetValue(p.DependencyFullName, out result)) {
        //            return result;
        //        }
        //    }
        //    return p.Category;
        //}

        //static HashSet<string> advancedProperties = new HashSet<string>();

        //public static void AddAdvancedProperty(DependencyProperty p)
        //{
        //    lock (advancedProperties) {
        //        advancedProperties.Add(p.GetFullName());
        //    }
        //}

        //public static void AddAdvancedProperty(Type type, string member)
        //{
        //    lock (advancedProperties) {
        //        advancedProperties.Add(type.FullName + "." + member);
        //    }
        //}

        //public static bool IsAdvanced(DesignItemProperty p)
        //{
        //    lock (advancedProperties) {
        //        if (advancedProperties.Contains(p.DependencyFullName)) {
        //            return true;
        //        }
        //    }
        //    return p.IsAdvanced;
        //} 
        #endregion

        static readonly HashSet<string> hiddenProperties = new();

        /// <summary>
        /// 隐藏指定的属性(将其标记为不可浏览)。
        /// </summary>
        public static void HideProperty(DependencyProperty p)
        {
            lock (hiddenProperties)
            {
                hiddenProperties.Add(p.GetFullName());
            }
        }

        /// <summary>
        /// 隐藏指定的属性(将其标记为不可浏览)。
        /// </summary>
        public static void HideProperty(Type type, string member)
        {
            lock (hiddenProperties)
            {
                hiddenProperties.Add(type.FullName + "." + member);
            }
        }

        /// <summary>
        /// 获取指定的属性是否可浏览(应该在属性网格中可见)。
        /// </summary>
        public static bool IsBrowsable(DesignItemProperty p)
        {
            lock (hiddenProperties)
            {
                if (hiddenProperties.Contains(p.DependencyFullName))
                {
                    return false;
                }
            }
            return true;
        }

        //public static string[] CategoryOrder { get; set; }

        private static readonly HashSet<string> popularProperties = new();

        /// <summary>
        /// 注册一个常用的属性(首先显示在属性网格中)。
        /// </summary>
        public static void AddPopularProperty(DependencyProperty p)
        {
            lock (popularProperties)
            {
                popularProperties.Add(p.GetFullName());
            }
        }

        /// <summary>
        /// 注册一个常用的属性(首先显示在属性网格中)。
        /// </summary>
        public static void AddPopularProperty(Type type, string member)
        {
            lock (popularProperties)
            {
                popularProperties.Add(type.FullName + "." + member);
            }
        }

        /// <summary>
        /// 获取指定的属性是否已注册为常用属性。
        /// </summary>
        public static bool IsPopularProperty(DesignItemProperty p)
        {
            lock (popularProperties)
            {
                if (popularProperties.Contains(p.DependencyFullName))
                {
                    return true;
                }
            }
            return false;
        }

        private static readonly HashSet<Type> popularControls = new();

        /// <summary>
        /// 注册一个常用控件(在默认工具箱中可见)。
        /// </summary>
        public static void AddPopularControl(Type t)
        {
            lock (popularControls)
            {
                popularControls.Add(t);
            }
        }

        /// <summary>
        /// 获取常用控件列表。
        /// </summary>
        public static IEnumerable<Type> GetPopularControls()
        {
            lock (popularControls)
            {
                return popularControls.ToArray();
            }
        }

        /// <summary>
        /// 获取指定的控件是否已注册为常用控件。
        /// </summary>
        public static bool IsPopularControl(Type t)
        {
            lock (popularControls)
            {
                return popularControls.Contains(t);
            }
        }

        static readonly Dictionary<string, NumberRange> ranges = new();

        /// <summary>
        /// 为属性注册值的范围
        /// </summary>
        public static void AddValueRange(DependencyProperty p, double min, double max)
        {
            lock (ranges)
            {
                ranges[p.GetFullName()] = new NumberRange() { Min = min, Max = max };
            }
        }

        /// <summary>
        /// 获取属性的注册值范围，如果没有注册范围，则为null
        /// </summary>
        public static NumberRange GetValueRange(DesignItemProperty p)
        {
            lock (ranges)
            {
                if (ranges.TryGetValue(p.DependencyFullName, out NumberRange r))
                {
                    return r;
                }
            }
            return null;
        }

        private static readonly HashSet<Type> placementDisabled = new();

        /// <summary>
        /// 禁用该类型的默认放置行为(设置ContentProperty)
        /// </summary>
        public static void DisablePlacement(Type type)
        {
            lock (placementDisabled)
            {
                placementDisabled.Add(type);
            }
        }

        /// <summary>
        /// 获取该类型是否禁用了默认放置行为(设置ContentProperty)。
        /// </summary>
        public static bool IsPlacementDisabled(Type type)
        {
            lock (placementDisabled)
            {
                return placementDisabled.Contains(type);
            }
        }

        private static readonly Dictionary<Type, Size> defaultSizes = new();

        /// <summary>
        /// 为指定类型的新控件注册默认大小。
        /// </summary>
        public static void AddDefaultSize(Type t, Size s)
        {
            lock (defaultSizes)
            {
                defaultSizes[t] = s;
            }
        }

        /// <summary>
        /// 获取指定类型的新控件的默认大小,如果没有注册默认大小，则返回new Size(double.NaN, double.NaN)。
        /// </summary>
        public static Size? GetDefaultSize(Type t, bool checkBasetype = true)
        {
            lock (defaultSizes)
            {
                while (t != null)
                {
                    if (defaultSizes.TryGetValue(t, out Size s))
                    {
                        return s;
                    }
                    t = checkBasetype ? t.BaseType : null;
                }
            }
            return null;
        }

        /// <summary>
        /// 注册一个应该使用的默认属性值
        /// </summary>
        public static void AddDefaultPropertyValue(Type t, DependencyProperty p, object value)
        {
            lock (standardPropertyValues)
            {
                if (!standardPropertyValues.ContainsKey(t))
                    standardPropertyValues.Add(t, new Dictionary<DependencyProperty, object>());

                standardPropertyValues[t][p] = value;
            }
        }

        /// <summary>
        /// 获取类型的默认属性值
        /// </summary>
        public static Dictionary<DependencyProperty, object> GetDefaultPropertyValues(Type t)
        {
            lock (standardPropertyValues)
            {
                if (standardPropertyValues.ContainsKey(t))
                    return standardPropertyValues[t];

                return null;
            }
        }
    }
}
