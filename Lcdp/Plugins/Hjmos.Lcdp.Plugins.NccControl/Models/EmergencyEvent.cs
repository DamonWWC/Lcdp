using Prism.Mvvm;
using System;

namespace Hjmos.Lcdp.Plugins.NccControl.Models
{
    /// <summary>
    /// 突发事件实体
    /// </summary>
    public class EmergencyEvent : BindableBase
    {

        /// <summary>
        /// 主题（后端自动生成）
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// 发生时间
        /// </summary>
        public DateTime OccurTime { get; set; }

        /// <summary>
        /// 持续时长
        /// </summary>
        public Tuple<int, int> TimeSpan { get; set; }


        /// <summary>
        /// 背景色
        /// </summary>
        public Tuple<string, string> Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }
        private Tuple<string, string> _color;

    }
}
