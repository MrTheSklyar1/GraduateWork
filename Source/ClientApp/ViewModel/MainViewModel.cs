using ClientApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.ViewModel
{
    public sealed class MainViewModel : ViewModelBase
    {
        #region Constructors

        public MainViewModel(ICommandFactory commandFactory)
        {
            if (commandFactory == null)
                return;


            _commandFactory = commandFactory;
        }

        #endregion

        #region Ovveride UpdateValue

        protected override void UpdateValue<TArg>(TArg value, ref TArg storage, [CallerMemberName] string propertyName = null)
        {
            base.UpdateValue(value, ref storage, propertyName);
        }

        #endregion


        #region Fields

        private readonly ICommandFactory _commandFactory;

        #endregion
    }
}
