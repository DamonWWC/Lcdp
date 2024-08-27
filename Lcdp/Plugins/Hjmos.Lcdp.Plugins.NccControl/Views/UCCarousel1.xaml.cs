using Hjmos.Lcdp.Plugins.NccControl.ViewModels;
using Hjmos.Lcdp.VisualEditor.Core.Attributes;
using Hjmos.Lcdp.VisualEditor.Core.Helpers;
using System;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Xml;

namespace Hjmos.Lcdp.Plugins.NccControl.Views
{
    [Widget(Category = "应急指挥独立版", DisplayName = "轮播图", DefaultWidth = 360d, DefaultHeight = 278d)]
    public partial class UCCarousel1
    {
        public UCCarousel1()
        {
            InitializeComponent();

            var vm = DataContext as UCCarousel1ViewModel;

            AttachedPropertyChanged += parameters =>
            {
                // 获取变化的附加属性
                DependencyProperty dp = parameters.GetValue<DependencyProperty>("DependencyProperty");
                if (dp.Name == "CustomXaml")
                {
                    string dataTemplateString = parameters.GetValue<string>("NewValue");

                    // 默认的无数据模板
                    if (string.IsNullOrEmpty(dataTemplateString))
                    {
                        carousel1.ItemTemplate = this.Resources["NoDataTemplate"] as DataTemplate;
                        return;
                    }

                    try
                    {
                        // 加载用户自定义模板
                        StringReader stringReader = new(dataTemplateString);
                        XmlReader xmlReader = XmlReader.Create(stringReader);
                        DataTemplate dataTemplate = XamlReader.Load(xmlReader) as DataTemplate;
                        carousel1.ItemTemplate = dataTemplate;
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("CustomXaml字符串有误");
                        return;
                    }
                }
                else if (dp.Name == "LoadedCode")
                {
                    string strLoadedCode = parameters.GetValue<string>("NewValue");

                    if (string.IsNullOrEmpty(strLoadedCode)) return;

                    try
                    {
                        // 执行用户自定义代码
                        EvalCodeHelper.Eval(strLoadedCode, this);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("LoadedCode字符串有误");
                        return;
                    }
                }
            };

        }
    }
}
