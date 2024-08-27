namespace Hjmos.Lcdp.Plugins.NccControl.Models
{
    /// <summary>
    /// 预警监测统计实体类
    /// </summary>
    public class MonitorStatistics
    {
        /// <summary>
        /// 总数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Num { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 占比
        /// </summary>
        public string Ratio { get; set; }
    }
}
