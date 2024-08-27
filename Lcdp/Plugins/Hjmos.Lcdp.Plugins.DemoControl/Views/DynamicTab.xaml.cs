using Hjmos.Lcdp.Common;
using Hjmos.Lcdp.Plugins.DemoControl.DataFields;
using Hjmos.Lcdp.VisualEditor.Core.Attached;
using Hjmos.Lcdp.VisualEditor.Core.Attributes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Hjmos.Lcdp.Plugins.DemoControl.Views
{
    [Widget(Category = "已废弃", DisplayName = "选项卡")]
    [DataFields(typeof(DynamicTabDataFields))]
    public partial class DynamicTab
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
            DependencyProperty.Register("PageGuid", typeof(string), typeof(DynamicTab), new PropertyMetadata(string.Empty));

        private string[] _eventArray;


        public DynamicTab()
        {
            InitializeComponent();

            WidgetFieldList = new List<string>() { "PageGuid" };

            // 事件传参
            IEventParameters parameters = new EventParameters {
                    { "DependencyProperty", DataFieldsAttached.DataFieldsProperty },
                    { "NewValue", DataFieldsAttached.GetDataFields(this) }
                };

            RaiseAttachedPropertyChanged(parameters);

            this.Loaded += delegate
            {
                if (!PageApi.IsDesignMode)
                {
                    SelectedLayerName = "Layer2";
                    ToggleLayer();
                }
            };

            // 注册附加属性改变事件
            AttachedPropertyChanged += parameters =>
            {
                DependencyProperty dp = parameters.GetValue<DependencyProperty>("DependencyProperty");

                if (dp.Name == "DataFields")
                {
                    var dataFields = parameters.GetValue<DynamicTabDataFields>("NewValue");

                    string strKeys = dataFields.KeyField;
                    string strValues = dataFields.ValueField;
                    string strEvents = dataFields.EventField;

                    if (string.IsNullOrEmpty(strKeys)) return;

                    string[] keyArray = strKeys.Split(',');
                    string[] valueArray = strValues.Split(',');
                    _eventArray = strEvents.Split(',');

                    this.TabList.Clear();
                    for (int i = 0; i < keyArray.Length; i++)
                        this.TabList.Add(new KeyValuePair<string, string>(keyArray[i], valueArray[i]));
                    this.lstTab.ItemsSource = this.TabList;
                    this.lstTab.SelectedIndex = 0;
                }

            };


            MessageReceived += o =>
            {
                string strEvent = o.ToString();

                // 选中事件对应的选项卡
                int index = _eventArray.ToList().IndexOf(strEvent);
                if (index != -1)
                {
                    this.lstTab.SelectedIndex = index;
                }
            };

        }

        public string SelectedLayerName
        {
            get => (string)GetValue(LayerNameProperty);
            set => SetValue(LayerNameProperty, value);
        }

        public static readonly DependencyProperty LayerNameProperty =
            DependencyProperty.Register("SelectedLayerName", typeof(string), typeof(DynamicTab), new PropertyMetadata(null));

        public ObservableCollection<KeyValuePair<string, string>> TabList { get; set; } = new ObservableCollection<KeyValuePair<string, string>>();

        private void lstTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 运行时才触发图层切换操作
            if (!PageApi.IsDesignMode) return;

            if ((sender as ListBox).SelectedItem is not KeyValuePair<string, string> t) return;

            SelectedLayerName = t.Value;

            ToggleLayer();
        }

        /// <summary>
        /// 图层切换
        /// </summary>
        private void ToggleLayer()
        {
            // TODO：如果需要图层切换，设计成用接口切换，这样不用引用RootCanvas等组件库，现在暂时注释掉代码
            //var root = ControlRetrieveHelper.FindVisualParent<RootCanvas>(lstTab);
            //if (root is null) return;

            //foreach (LayerCanvas layer in root.Children)
            //{
            //    if (layer.DisplayName == SelectedLayerName)
            //    {
            //        layer.Visibility = Visibility.Visible;
            //    }
            //    else if ((lstTab.ItemsSource as IEnumerable<KeyValuePair<string, string>>).ToList().Any(x => x.Value == layer.DisplayName))
            //    {
            //        layer.Visibility = Visibility.Hidden;
            //    }
            //}
        }
    }
}
