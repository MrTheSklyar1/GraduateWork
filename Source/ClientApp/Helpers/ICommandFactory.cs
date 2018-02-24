using System;
using System.Windows.Input;

namespace ClientApp.Helpers
{
    public interface ICommandFactory
    {
        ICommand CreateCommand(Action execute);
        ICommand CreateCommand(Action execute, Func<bool> canexecute);
        ICommand CreateCommand(Action<object> execute, Func<object, bool> canexecute);
        ICommand CreateCommand<TArg>(Action<TArg> execute);
        ICommand CreateCommand<TArg>(Action<TArg> execute, Func<TArg, bool> canexecute);
    }
}
