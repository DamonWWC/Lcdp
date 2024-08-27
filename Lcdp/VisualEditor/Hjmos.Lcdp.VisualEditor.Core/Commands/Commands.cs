using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Core
{
    public static class Commands
    {
        public static ICommand AlignTopCommand = new RoutedCommand();
        public static ICommand AlignMiddleCommand = new RoutedCommand();
        public static ICommand AlignBottomCommand = new RoutedCommand();
        public static ICommand AlignLeftCommand = new RoutedCommand();
        public static ICommand AlignCenterCommand = new RoutedCommand();
        public static ICommand AlignRightCommand = new RoutedCommand();
        public static ICommand RotateLeftCommand = new RoutedCommand();
        public static ICommand RotateRightCommand = new RoutedCommand();
        public static ICommand StretchToSameWidthCommand = new RoutedCommand();
        public static ICommand StretchToSameHeightCommand = new RoutedCommand();

        static Commands() { }
    }
}
