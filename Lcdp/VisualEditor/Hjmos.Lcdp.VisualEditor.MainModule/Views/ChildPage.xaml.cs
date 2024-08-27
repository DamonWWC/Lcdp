using Hjmos.Lcdp.VisualEditor.Core.Attributes;
using Hjmos.Lcdp.VisualEditor.Core.DataFields;
using Hjmos.Lcdp.VisualEditor.Core.Interfaces;
using Hjmos.Lcdp.VisualEditor.MainModule.ViewModels;
using Hjmos.Lcdp.VisualEditorServer.Entities;
using System.Collections.Generic;
using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.MainModule.Views
{
    [Widget(Category = "布局组件", DisplayName = "子页面")]
    [DataFields(typeof(ChildPageDataFields))]
    public partial class ChildPage : IPage
    {
        /// <summary>
        /// 页面Guid
        /// </summary>
        public string PageGuid
        {
            get => (string)GetValue(PageGuidProperty);
            set => SetValue(PageGuidProperty, value);
        }

        public static readonly DependencyProperty PageGuidProperty =
            DependencyProperty.Register("PageGuid", typeof(string), typeof(ChildPage), new PropertyMetadata(string.Empty, (d, e) =>
            {
                if (e.NewValue == null) return;
                (d as ChildPage).root.PageGuid = e.NewValue.ToString();
            }));

        public ChildPage()
        {
            InitializeComponent();

            WidgetFieldList = new List<string>() { "PageGuid" };

            // 注册附加属性改变事件
            AttachedPropertyChanged += parameters =>
            {
                ChildPageViewModel vm = this.DataContext as ChildPageViewModel;
                vm.RaiseAttachedPropertyChanged(parameters);
            };

            MessageReceived += o =>
            {
                if (o.GetType().GetProperty("Type") is null) return;
                if (o.GetType().GetProperty("Type").GetValue(o).ToString() != "menu") return;
                //if (o.GetType().GetProperty("PanelGuid").GetValue(o).ToString() != this.Guid.ToString()) return;

                root.PageGuid = o.GetType().GetProperty("ContentGuid").GetValue(o).ToString();
            };
        }
    }
}