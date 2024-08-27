using Hjmos.Lcdp.Enums;
using System.Windows;
using System.Windows.Data;

namespace Hjmos.Lcdp.Converters
{
    /// <summary>
    /// 可见性转换器的基类
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public abstract class BaseVisibilityConverter
    {
        /// <summary>
        /// 配置False值和不可见状态的值的转换关系，默认Collapsed
        /// </summary>
        protected Visibility FalseVisibilityValue { get; set; } = Visibility.Collapsed;

        /// <summary>
        /// 获取或设置不可见状态的值
        /// </summary>
        public FalseVisibility FalseVisibilityState
        {
            get => FalseVisibilityValue == Visibility.Collapsed ? FalseVisibility.Collapsed : FalseVisibility.Hidden;
            set => FalseVisibilityValue = value == FalseVisibility.Collapsed ? Visibility.Collapsed : Visibility.Hidden;
        }

        /// <summary>
        /// 是否反向转换
        /// </summary>
        public bool IsInverted { get; set; }
    }
}