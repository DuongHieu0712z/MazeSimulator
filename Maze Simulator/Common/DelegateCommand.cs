using System;
using System.Windows.Input;

namespace Maze_Simulator.Common
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> execute;

        private readonly Func<object, bool> canExecute;

        public DelegateCommand(Action<object> execute)
        {
            this.execute = execute;
            canExecute = _ => true;
        }

        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
