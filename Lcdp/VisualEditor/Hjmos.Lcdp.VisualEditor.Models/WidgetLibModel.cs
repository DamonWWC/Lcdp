using Prism.Mvvm;
using System;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Models
{
    /// <summary>
    /// 表示要上传的组件库DLL
    /// </summary>
    public class WidgetLibModel : BindableBase
    {
        /// <summary>
        /// 序号
        /// </summary>
        public string Index { get; set; }

        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 类库名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 上传时间
        /// </summary>
        public DateTime UploadTime { get; set; }

        /// <summary>
        /// 组件库文件路径
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// 上传状态
        /// </summary>
        public string State
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }
        private string _state;

        /// <summary>
        /// 配置组件库
        /// </summary>
        public ICommand ConfigCommand { get; set; }

        /// <summary>
        /// 删除组件库
        /// </summary>
        public ICommand DeleteCommand { get; set; }
    }
}
