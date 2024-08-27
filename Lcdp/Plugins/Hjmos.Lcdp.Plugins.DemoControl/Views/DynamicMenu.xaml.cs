using Hjmos.Lcdp.Common;
using Hjmos.Lcdp.Plugins.DemoControl.DataFields;
using Hjmos.Lcdp.VisualEditor.Core.Attached;
using Hjmos.Lcdp.VisualEditor.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Hjmos.Lcdp.Plugins.DemoControl.Views
{
    [Widget(Category = "已废弃", DisplayName = "❌菜单")]
    [DataFields(typeof(DynamicMenuDataFields))]
    public partial class DynamicMenu
    {

        /// <summary>
        /// 接收外部全局参数
        /// </summary>
        public string PageGuid
        {
            get => (string)GetValue(PageGuidProperty);
            set => SetValue(PageGuidProperty, value);
        }

        public static readonly DependencyProperty PageGuidProperty =
            DependencyProperty.Register("PageGuid", typeof(string), typeof(DynamicMenu), new PropertyMetadata(string.Empty));

        public DynamicMenu()
        {
            InitializeComponent();

            WidgetFieldList = new List<string>() { "PageGuid" };


            Loaded += delegate { };

            // 注册附加属性改变事件
            AttachedPropertyChanged += parameters =>
            {

                DependencyProperty dp = parameters.GetValue<DependencyProperty>("DependencyProperty");

                if (dp.Name == "DataFields")
                {
                    var dataFields = parameters.GetValue<DynamicMenuDataFields>("NewValue");

                    string strKeys = dataFields.KeyField;
                    string strValues = dataFields.ValueField?.Value ?? string.Empty;

                    if (string.IsNullOrEmpty(strKeys)) return;

                    string[] keyArray = strKeys.Split(',');
                    string[] valueArray = strValues.Split(',');

                    this.TabList.Clear();
                    for (int i = 0; i < keyArray.Length; i++)
                    {
                        string value = string.Empty;
                        if (valueArray.Length > i)
                        {
                            value = valueArray[i];
                        }
                        this.TabList.Add(new KeyValuePair<string, string>(keyArray[i], value));
                    }

                    this.lstTab.ItemsSource = this.TabList;
                    this.lstTab.SelectedIndex = 0;
                }


            };

            // 事件传参
            IEventParameters parameters = new EventParameters {
                { "DependencyProperty", DataFieldsAttached.DataFieldsProperty },
                { "NewValue", DataFieldsAttached.GetDataFields(this) }
            };

            RaiseAttachedPropertyChanged(parameters);
        }


        public ObservableCollection<KeyValuePair<string, string>> TabList { get; set; } = new ObservableCollection<KeyValuePair<string, string>>();

        private void lstTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 运行时才触发图层切换操作
            if (!PageApi.IsDesignMode) return;

            if ((sender as ListBox).SelectedItem is not KeyValuePair<string, string> t) return;

            if (!Guid.TryParse(t.Value, out Guid result)) return;

            PageGuid = t.Value;
        }
    }
}
