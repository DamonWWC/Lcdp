using Hjmos.Lcdp.VisualEditor.Core.BaseClass;
using Hjmos.Lcdp.VisualEditor.Core.CustomTypes;

namespace Hjmos.Lcdp.Plugins.DemoControl.DataFields
{
    public class BasicColumnDataFields : DataFieldsBase
    {
        /// <summary>
        /// 序列字段
        /// </summary>
        public Dimension SeriesField { get; set; }

        /// <summary>
        /// X轴标签字段
        /// </summary>
        public Dimension LabelField { get; set; }

        /// <summary>
        /// 值字段
        /// </summary>
        public Dimension ValueField { get; set; }

        /// <summary>
        /// 接口地址
        /// </summary>
        public TextArea Address { get; set; }

        public string TextField { get; set; }

    }
}
