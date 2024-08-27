using Hjmos.Lcdp.VisualEditor.Core.BaseClass;
using Hjmos.Lcdp.VisualEditor.Core.CustomTypes;

namespace Hjmos.Lcdp.Plugins.NccControl.DataFields
{
    public class PieStatisticsDataFields : DataFieldsBase
    {
        /// <summary>
        /// 接口地址
        /// </summary>
        public TextArea Address { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
    }
}
