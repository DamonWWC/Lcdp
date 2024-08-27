using Newtonsoft.Json;
using Prism.Mvvm;
using System.ComponentModel;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Models
{
    public class ParameterModel : BindableBase
    {
        /// <summary>
        /// 序号
        /// </summary>
        [JsonIgnore]
        public string Index
        {
            get => _index;
            set => SetProperty(ref _index, value);
        }
        private string _index;


        /// <summary>
        /// Id
        /// </summary>
        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }
        private int _id;

        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        private string _name;

        /// <summary>
        /// 使用范围
        /// </summary>
        public ParameterRange Range
        {
            get => _range;
            set => SetProperty(ref _range, value);
        }
        private ParameterRange _range;

        /// <summary>
        /// 值
        /// TODO：这里在Value不能加[JsonIgnore]，页面不用这个参数，但是WebAPI需要
        /// </summary>
        public string Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }
        private string _value;

        /// <summary>
        /// 应用ID
        /// </summary>
        public int AppId
        {
            get => _appid;
            set => SetProperty(ref _appid, value);
        }
        private int _appid;

        /// <summary>
        /// 编辑命令
        /// </summary>
        [JsonIgnore]
        public ICommand ModifyCommand { get; set; }

        /// <summary>
        /// 删除命令
        /// </summary>
        [JsonIgnore]
        public ICommand DeleteCommand { get; set; }
    }

    public enum ParameterRange
    {
        [Description("应用级别")]
        App = 1,
        [Description("页面级别")]
        Page = 2
    }
}
