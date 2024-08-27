using LiveCharts;
using LiveCharts.Configurations;
using System;

namespace Hjmos.Lcdp.Plugins.CS6.Models
{
    /// <summary>
    /// 为了方便LiveChart图表处理实时数据
    /// </summary>
    public class MeasureModel
    {
        public DateTime DateTime { get; set; }
        public double Value { get; set; }

        static MeasureModel()
        {
            var mapper = Mappers.Xy<MeasureModel>()
                .X(model => model.DateTime.Ticks)  // 使用DateTime.Ticks作为X
                .Y(model => model.Value);          // 使用Value属性作为Y

            // 全局保存映射.
            Charting.For<MeasureModel>(mapper);
        }
    }
}
