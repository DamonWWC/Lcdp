using System;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Core.DesignerControls
{
    /// <summary>
    /// A command that invokes a delegate.
    /// The command parameter must be of type T.
    /// </summary>
    public class RelayCommand<T> : ICommand
    {
        private readonly Predicate<T> canExecute;
        private readonly Action<T> execute;

        public RelayCommand(Action<T> execute) => this.execute = execute ?? throw new ArgumentNullException("execute");

        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            this.execute = execute ?? throw new ArgumentNullException("execute");
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                if (canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        public bool CanExecute(object parameter) => (parameter is null or T) && (canExecute == null || canExecute((T)parameter));

        public void Execute(object parameter) => execute((T)parameter);
    }

    /// <summary>
    /// A command that invokes a delegate.
    /// This class does not provide the command parameter to the delegate -
    /// if you need that, use the generic version of this class instead.
    /// 
    /// 调用委托的命令。
    /// 这个类没有向委托提供命令参数-如果需要，请使用这个类的泛型版本。
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Func<bool> canExecute;
        private readonly Action execute;

        public RelayCommand(Action execute) => this.execute = execute ?? throw new ArgumentNullException("execute");

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            this.execute = execute ?? throw new ArgumentNullException("execute");
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                if (canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        public bool CanExecute(object parameter) => canExecute == null || canExecute();

        public void Execute(object parameter) => execute();
    }
}
