using Hjmos.Lcdp.Plugins.NccControl.DataFields;
using Hjmos.Lcdp.Plugins.NccControl.ViewModels;
using Hjmos.Lcdp.VisualEditor.Core.Attributes;
using Hjmos.Lcdp.VisualEditor.Core.Helpers;
using System;
using System.Windows;

namespace Hjmos.Lcdp.Plugins.NccControl.Views
{

    [Widget(Category = "应急指挥独立版", DisplayName = "饼图统计", DefaultWidth = 360d, DefaultHeight = 278d)]
    [DataFields(typeof(PieStatisticsDataFields))]
    public partial class PieStatisticsView
    {
        public PieStatisticsView()
        {
            InitializeComponent();

            AttachedPropertyChanged += parameters =>
            {
                // 获取变化的附加属性
                DependencyProperty dp = parameters.GetValue<DependencyProperty>("DependencyProperty");
                if (dp.Name == "LoadedCode")
                {
                    string strLoadedCode = parameters.GetValue<string>("NewValue");

                    if (string.IsNullOrEmpty(strLoadedCode)) return;

                    try
                    {
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
