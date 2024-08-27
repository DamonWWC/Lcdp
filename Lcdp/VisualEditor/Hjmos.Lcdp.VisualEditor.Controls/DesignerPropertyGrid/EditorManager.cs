using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Hjmos.Lcdp.VisualEditor.Controls.DesignerPropertyGrid
{
    /// <summary>
    /// Manages registered type and property editors.
    /// 管理已注册的类型和属性编辑器。
    /// </summary>
    public static class EditorManager
    {
        // 属性返回类型 => 编辑器类型
        static readonly Dictionary<Type, Type> typeEditors = new();

        // 属性全名 => 编辑器类型
        static readonly Dictionary<string, Type> propertyEditors = new();

        static Type defaultComboboxEditor;

        static Type defaultTextboxEditor;

        /// <summary>
        /// 为指定的属性<paramref name="property"/>创建属性编辑器
        /// </summary>
        public static FrameworkElement CreateEditor(DesignItemProperty property)
        {
            if (!propertyEditors.TryGetValue(property.FullName, out Type editorType))
            {
                var type = property.ReturnType;
                while (type != null)
                {
                    if (typeEditors.TryGetValue(type, out editorType))
                    {
                        break;
                    }
                    type = type.BaseType;
                }

                foreach (var t in typeEditors)
                {
                    if (t.Key.IsAssignableFrom(property.ReturnType))
                    {
                        return (FrameworkElement)Activator.CreateInstance(t.Value);
                    }
                }

                if (editorType == null)
                {
                    IEnumerable standardValues = null;

                    if (property.DependencyProperty != null)
                    {
                        standardValues = Metadata.GetStandardValues(property.DependencyProperty);
                    }
                    if (standardValues == null)
                    {
                        standardValues = Metadata.GetStandardValues(property.ReturnType);
                    }

                    if (standardValues != null)
                    {
                        var itemsControl = (ItemsControl)Activator.CreateInstance(defaultComboboxEditor);
                        itemsControl.ItemsSource = standardValues;
                        if (Nullable.GetUnderlyingType(property.ReturnType) != null)
                        {
                            itemsControl.GetType().GetProperty("IsNullable").SetValue(itemsControl, true, null); //In this Class we don't know the Nullable Combo Box
                        }
                        return itemsControl;
                    }

                    var namedStandardValues = Metadata.GetNamedStandardValues(property.ReturnType);
                    if (namedStandardValues != null)
                    {
                        var itemsControl = (ItemsControl)Activator.CreateInstance(defaultComboboxEditor);
                        itemsControl.ItemsSource = namedStandardValues;
                        itemsControl.DisplayMemberPath = "Name";
                        ((Selector)itemsControl).SelectedValuePath = "Value";
                        if (Nullable.GetUnderlyingType(property.ReturnType) != null)
                        {
                            itemsControl.GetType().GetProperty("IsNullable").SetValue(itemsControl, true, null); //In this Class we don't know the Nullable Combo Box
                        }
                        return itemsControl;
                    }
                    return (FrameworkElement)Activator.CreateInstance(defaultTextboxEditor);
                    return (FrameworkElement)Activator.CreateInstance(defaultTextboxEditor);
                }
            }
            return (FrameworkElement)Activator.CreateInstance(editorType);
        }

        /// <summary>
        /// 注册文本框编辑器。
        /// </summary>
        public static void SetDefaultTextBoxEditorType(Type type) => defaultTextboxEditor = type;

        /// <summary>
        /// 注册组合框编辑器。
        /// </summary>
        public static void SetDefaultComboBoxEditorType(Type type) => defaultComboboxEditor = type;

        /// <summary>
        /// 注册在指定程序集中定义的属性编辑器。
        /// </summary>
        public static void RegisterAssembly(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            foreach (Type type in assembly.GetExportedTypes())
            {
                foreach (TypeEditorAttribute editorAttribute in type.GetCustomAttributes(typeof(TypeEditorAttribute), false))
                {
                    CheckValidEditor(type);
                    typeEditors[editorAttribute.SupportedPropertyType] = type;
                }
                foreach (PropertyEditorAttribute editorAttribute in type.GetCustomAttributes(typeof(PropertyEditorAttribute), false))
                {
                    CheckValidEditor(type);
                    string propertyName = editorAttribute.PropertyDeclaringType.FullName + "." + editorAttribute.PropertyName;
                    propertyEditors[propertyName] = type;
                }
            }
        }

        static void CheckValidEditor(Type type)
        {
            if (!typeof(FrameworkElement).IsAssignableFrom(type))
            {
                throw new DesignerException("Editor types must derive from FrameworkElement!");
            }
        }
    }
}
