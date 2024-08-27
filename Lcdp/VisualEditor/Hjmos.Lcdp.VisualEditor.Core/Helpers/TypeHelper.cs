using Hjmos.Lcdp.VisualEditor.Core.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Core.Helpers
{
    /// <summary>
    /// 类型转换帮助类
    /// </summary>
    public static class TypeHelper
    {

        /// <summary>
        /// 获取组件依赖属性描述的集合
        /// </summary>
        /// <param name="element">提供属性的组件</param>
        /// <returns></returns>
        public static IEnumerable<DependencyPropertyDescriptor> GetDependencyPropertyDescriptorCollection(FrameworkElement element)
        {
            foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(element, new Attribute[] { new PropertyFilterAttribute(PropertyFilterOptions.All) }))
            {
                DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(pd);

                if (dpd != null && dpd.DependencyProperty != null)
                {
                    yield return dpd;
                }
            }
        }

        /// <summary>
        /// 获取组件属性描述的集合
        /// </summary>
        /// <param name="element">提供属性的组件</param>
        /// <returns></returns>
        public static IEnumerable<PropertyDescriptor> GetPropertyDescriptorCollection(DependencyObject element)
        {
            foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(element, new Attribute[] { new PropertyFilterAttribute(PropertyFilterOptions.All) }))
            {
                yield return pd;
            }
        }

        /// <summary>
        /// 组件依赖属性赋值
        /// </summary>
        /// <param name="element">将要被赋值的组件实例</param>
        /// <param name="option">组件属性值字典</param>
        public static void SetValue(FrameworkElement element, Dictionary<string, string> options)
        {
            IEnumerable<DependencyPropertyDescriptor> dpdCollection = GetDependencyPropertyDescriptorCollection(element);

            // 组件依赖属性赋值
            foreach (var option in options)
            {
                DependencyPropertyDescriptor descriptor = dpdCollection.FirstOrDefault(x => x.DependencyProperty.Name.ToLower() == option.Key.ToLower());

                Type propertyType = descriptor.PropertyType;

                // 获取类型转换器
                TypeConverter typeConverter = TypeDescriptor.GetConverter(propertyType);

                // 标记是否使用TypeConverter转换为对象（排除TyperConverter和CollectionConverter，CollectionConverter中的模型可以不标记ConvertToJson，默认使用Json反序列化）
                bool useConverter = typeConverter != null && !new string[] { "TyperConverter", "CollectionConverter" }.Contains(typeConverter.GetType().Name) && propertyType.GetCustomAttribute<ConvertToJsonAttribute>() is null;

                // 依赖属性的值
                object value;

                if (useConverter)
                {
                    // 使用类型转换器获取依赖属性的值
                    value = typeConverter.ConvertFromString(option.Value);
                }
                else
                {
                    // 获取真实的类型
                    propertyType = GetActualType(propertyType, option.Value);

                    // 没有类型转换器的类型，尝试通过JSON序列化转换
                    value = JsonConvert.DeserializeObject(option.Value, propertyType);
                }

                descriptor.SetValue(element, value);

                continue;
            }
        }

        /// <summary>
        /// 获取真实的子类类型
        /// </summary>
        /// <param name="propertyType"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        private static Type GetActualType(Type propertyType, string json)
        {
            // 是否标记特性TypeNameMarkAttribute
            if (propertyType.GetCustomAttributes<TypeNameMarkAttribute>().Any())
            {
                // 获取子类型FullName字符串
                Type actualType = Type.GetType(JObject.Parse(json)["typeName"].ToString());
                if (actualType != null && propertyType.IsAssignableFrom(actualType))
                {
                    return actualType;
                }
            }

            return propertyType;
        }
    }
}
