using Hjmos.Lcdp.VisualEditor.Core.Attached;
using Hjmos.Lcdp.VisualEditor.Core.Attributes;
using Hjmos.Lcdp.VisualEditor.Core.BaseClass;
using Hjmos.Lcdp.VisualEditor.Core.Entities;
using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Hjmos.Lcdp.VisualEditor.Core.Helpers
{
    public class WidgetPropertyHelper
    {
        // 组件添加一些常用属性（为了属性在属性面板显示）
        public static void InitGeneralProperty(FrameworkElement element)
        {
            Panel.SetZIndex(element, 0); // 层叠
            element.Margin = new Thickness(0); // 边距
            element.Opacity = 1; // 透明度
            element.MaxHeight = element.MaxWidth = double.MaxValue; // 最大宽高
            element.MinHeight = element.MinWidth = 0d; // 最小宽高
            element.HorizontalAlignment = HorizontalAlignment.Stretch; // 水平对齐
            element.VerticalAlignment = VerticalAlignment.Stretch; // 垂直对齐
            AdaptiveAttached.SetAdaptive(element, false); // 是否自适应布局容器
            ParameterMappingAttached.SetParameterMapping(element, new ObservableCollection<ParameterMapping>());// 参数映射
            JsonAttached.SetJson(element, string.Empty);// 静态JSON
            CustomXamlAttached.SetCustomXaml(element, string.Empty);// 自定义XAML
            LoadedCodeAttached.SetLoadedCode(element, string.Empty);// 自定义加载完毕代码

            // 获取组件上的特性
            DataFieldsAttribute att = element.GetType().GetCustomAttribute<DataFieldsAttribute>();
            if (att != null)
            {
                DataFieldsBase dataFiled = Activator.CreateInstance(att.Type) as DataFieldsBase;
                // 设置数据面板字段实体
                DataFieldsAttached.SetDataFields(element, dataFiled);
            }

            // TODO:新增一个特性，自动给需要的依赖属性SetValue

        }
    }
}
