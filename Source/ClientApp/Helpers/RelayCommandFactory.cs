using System;
using System.Windows.Input;

namespace ClientApp.Helpers
{
    public sealed class RelayCommandFactory : ICommandFactory
    {
        public ICommand CreateCommand(Action<object> execute)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            return new RelayCommand<object>(execute);
        }

        public ICommand CreateCommand(Action execute)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            return new RelayCommand<object>(o => execute());
        }

        public ICommand CreateCommand(Action<object> execute, Func<object, bool> canexecute)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            return new RelayCommand<object>(execute, canexecute);
        }

        public ICommand CreateCommand(Action execute, Func<bool> canexecute)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            return new RelayCommand<object>(o => execute(), o => canexecute());
        }

        public ICommand CreateCommand<TArg>(Action<TArg> execute)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            return new RelayCommand<TArg>(execute);
        }

        public ICommand CreateCommand<TArg>(Action<TArg> execute, Func<TArg, bool> canexecute)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            return new RelayCommand<TArg>(execute, canexecute);
        }
    }
}
