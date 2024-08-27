using Hjmos.Lcdp.VisualEditor.Core.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup.Primitives;

namespace Hjmos.Lcdp.VisualEditor.Core.Helpers
{
    /// <summary>
    /// 依赖属性操作帮助类
    /// </summary>
    public static class DependencyObjectHelper
    {
        /// <summary>
        /// 获取依赖对象设置过值的属性
        /// </summary>
        public static IEnumerable<DependencyProperty> GetDependencyProperties(DependencyObject element)
        {
            // 序列化的时候过滤不需要的属性
            List<string> filter = new()
            {
                "Tag",
                "CurrentLayer",
                "LayerList",
                "Background",
                "RenderSize",
                "Content",
                "XmlAttributeProperties.XmlnsDictionary",
                "NameScope.NameScope",
                "AutoWireViewModel",
                "DataContext",
                "LayoutTransform"
            };

            // 获取所有标记SerializeToOption为false的属性
            IEnumerable<PropertyDescriptor> descriptors = TypeDescriptor.GetProperties(element, new Attribute[] { new SerializeToOptionAttribute(false) }).OfType<PropertyDescriptor>();

            IEnumerable<DependencyProperty> dpList = MarkupWriter.GetMarkupObjectFor(element).Properties
                .Where(x => x.DependencyProperty != null && !filter.Contains(x.DependencyProperty.Name))
                // 这里使用名称字符串来过滤SerializeToOption为false的属性，如果需要更精确匹配，可以使用这种写法：
                // .Where(x => !descriptors.Any(y => DependencyPropertyDescriptor.FromProperty(y).DependencyProperty == x.DependencyProperty))
                .Where(x => !descriptors.Any(y => y.Name == x.Name))
                .Select(x => x.DependencyProperty);
            return dpList;
        }

        /// <summary>
        /// 获取组件参数配置
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetPropertyDescribers(DependencyObject element)
        {
            // 组件参数配置
            Dictionary<string, string> option = new();

            // 获取依赖属性列表
            IEnumerable<DependencyProperty> props = GetDependencyProperties(element);

            // 获取组件配置
            foreach (DependencyProperty prop in props)
            {

                // 获取属性类型转换器
                TypeConverter typeConverter = TypeDescriptor.GetConverter(prop.PropertyType);

                // 标记是否使用TypeConverter转换字符串（排除CollectionConverter，CollectionConverter中的模型可以不标记ConvertToJson，默认使用Json序列化）
                bool useConverter = typeConverter != null && typeConverter.GetType().Name != "CollectionConverter" && prop.PropertyType.GetCustomAttribute<ConvertToJsonAttribute>() is null;

                // 获取属性值
                string value = useConverter ? typeConverter.ConvertToString(element.GetValue(prop)) : JsonConvert.SerializeObject(element.GetValue(prop));

                if (string.IsNullOrEmpty(value)) continue;

                string defaultValue;

                // 获取属性默认值
                if (useConverter)
                {
                    defaultValue = typeConverter.ConvertToString(prop.DefaultMetadata.DefaultValue);
                }
                else if (prop.DefaultMetadata.DefaultValue is null && element.GetValue(prop) != null)
                {
                    // 获取默认实例
                    object obj = Activator.CreateInstance(element.GetValue(prop).GetType());
                    defaultValue = JsonConvert.SerializeObject(obj);
                }
                else
                {
                    defaultValue = JsonConvert.SerializeObject(prop.DefaultMetadata.DefaultValue);
                }

                // 忽略double默认最大值
                // 考虑让属性面板的数字字段兼容
                if (prop.PropertyType == typeof(double) && defaultValue == "∞" && value == typeConverter.ConvertToString(double.MaxValue))
                    continue;

                // 忽略默认值
                if (!value.Equals(defaultValue))
                    option.Add(prop.Name, value);
            }

            // TODO: 特殊处理Grid的行列
            if (element is Grid grid)
            {
                List<GridLength> cols = new();
                foreach (ColumnDefinition item in grid.ColumnDefinitions)
                {
                    cols.Add(item.Width);
                }

                if (option.ContainsKey("Columns"))
                {
                    option["Columns"] = JsonConvert.SerializeObject(cols);
                }
                else
                {
                    option.Add("Columns", JsonConvert.SerializeObject(cols));
                }


                List<GridLength> rows = new();
                foreach (RowDefinition item in grid.RowDefinitions)
                {
                    rows.Add(item.Height);
                }

                if (option.ContainsKey("Rows"))
                {
                    option["Rows"] = JsonConvert.SerializeObject(rows);
                }
                else
                {
                    option.Add("Rows", JsonConvert.SerializeObject(rows));
                }
            }

            return option;
        }
    }
}