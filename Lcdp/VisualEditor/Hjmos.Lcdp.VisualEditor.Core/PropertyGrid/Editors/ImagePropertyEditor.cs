using HandyControl.Controls;
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Hjmos.Lcdp.VisualEditor.Core
{
    public class ImagePropertyEditor : PropertyEditorBase
    {
        public override FrameworkElement CreateElement(PropertyItem propertyItem)
        {
            //// 保存属性字段关联的组件实例
            //_value = propertyItem.Value;

            var imageSelector = new ImageSelector
            {
                IsEnabled = !propertyItem.IsReadOnly,
                Width = 50,
                Height = 50,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            BindingOperations.SetBinding(this, UriProperty, new Binding(ImageSelector.UriProperty.Name)
            {
                Source = imageSelector,
                Mode = BindingMode.OneWay
            });

            return imageSelector;
        }

        //// 属性字段关联的组件实例
        //private object _value;

        internal static readonly DependencyProperty UriProperty = DependencyProperty.Register(
            "Uri", typeof(Uri), typeof(ImagePropertyEditor), new PropertyMetadata(default(Uri), OnUriChangedCallback));

        private static void OnUriChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImagePropertyEditor editor = d as ImagePropertyEditor;

            //// 组件不在主程序中，所以用放射的方式调用
            //// 获取组件类型
            //Type t = editor._value.GetType();
            ////参数对象      
            //object[] p = new object[1];
            ////产生方法
            //MethodInfo m = t.GetMethod("OnImageChanged");
   
            ////获得参数资料  
            //ParameterInfo[] para = m.GetParameters();
            ////根据参数的名字，拿参数的空值。  
            //p[0] = Type.GetType(para[0].ParameterType.BaseType.FullName).GetProperty("Empty");
            ////调用      
            //m.Invoke(editor._value, p);


            editor.Source = e.NewValue is Uri uri ? BitmapFrame.Create(uri) : null;
        }


        internal Uri Uri
        {
            get => (Uri)GetValue(UriProperty);
            set => SetValue(UriProperty, value);
        }

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            "Source", typeof(ImageSource), typeof(ImagePropertyEditor), new PropertyMetadata(default(ImageSource)));

        public ImageSource Source
        {
            get => (ImageSource)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public override void CreateBinding(PropertyItem propertyItem, DependencyObject element)
            => BindingOperations.SetBinding(this, GetDependencyProperty(),
                new Binding($"({propertyItem.PropertyName})")
                {
                    Source = propertyItem.Value,
                    Mode = GetBindingMode(propertyItem),
                    UpdateSourceTrigger = GetUpdateSourceTrigger(propertyItem),
                    Converter = GetConverter(propertyItem)
                });

        public override DependencyProperty GetDependencyProperty() => SourceProperty;
    }
}
