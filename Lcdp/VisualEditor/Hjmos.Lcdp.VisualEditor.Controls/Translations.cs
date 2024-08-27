namespace Hjmos.Lcdp.VisualEditor.Controls
{
    /// <summary>
    /// 描述变换的类
    /// </summary>
    public class Translations
    {
        private static Translations _instance;
        public static Translations Instance
        {
            get => _instance ?? new Translations();
            protected set => _instance = value;
        }

        public virtual string SendToFrontText => "移到最前";

        public virtual string SendForward => "前移";

        public virtual string SendBackward => "后移";

        public virtual string SendToBack => "移到最后";

        public virtual string PressAltText => "按住 \"Alt\" 进入容器";

        public virtual string WrapInCanvas => "Wrap in Canvas";

        public virtual string WrapInGrid => "Wrap in Grid";

        public virtual string WrapInBorder => "Wrap in Border";

        public virtual string WrapInViewbox => "Wrap in Viewbox";

        public virtual string Unwrap => "Unwrap";

        public virtual string FormatedTextEditor => "Formated Text Editor";

        public virtual string ArrangeLeft => "左对齐";

        public virtual string ArrangeHorizontalMiddle => "水平居中";

        public virtual string ArrangeRight => "右对齐";

        public virtual string ArrangeTop => "向上对齐";

        public virtual string ArrangeVerticalMiddle => "垂直居中";

        public virtual string ArrangeBottom => "向下对齐";

        public virtual string EditStyle => "Edit Style";
    }
}
