using System;
using System.ComponentModel;

namespace Hjmos.Lcdp.VisualEditor.Controls.Attached
{
    /// <summary>
    /// 可以绑定的静态属性
    /// </summary>
    public class StaticProperties
    {
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        #region 画布缩放比例

        private static double zoom = 100d;
        public static double Zoom
        {
            get => zoom;
            set { zoom = value; StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Zoom))); }
        }

        #endregion
    }
}
