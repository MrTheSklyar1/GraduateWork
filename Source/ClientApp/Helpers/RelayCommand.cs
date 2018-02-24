using System;
using System.Windows.Input;

namespace ClientApp.Helpers
{
    public class RelayCommand<TArg> : ICommand
    {
        #region Constructors

        public RelayCommand(Action<TArg> execute)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            _execute = execute;
        }

        public RelayCommand(Action<TArg> execute, Func<TArg, bool> canexecute)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            if (canexecute == null)
                throw new ArgumentNullException(nameof(canexecute));

            _execute = execute;
            _canexecute = canexecute;
        }

        #endregion

        #region ICommand

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
             => _canexecute == null || _canexecute((TArg)parameter);

        public void Execute(object parameter)
            => _execute((TArg)parameter);

        #endregion

        #region Fields

        private readonly Action<TArg> _execute;
        private readonly Func<TArg, bool> _canexecute;

        #endregion
    }
}
