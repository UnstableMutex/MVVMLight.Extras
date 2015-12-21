using System;
using System.Diagnostics;
using System.Windows.Input;

namespace MVVMLight.Extras.Commands
{
    public class MyRelayCommand : ICommand
    {
        #region Fields

        private readonly Func<bool> _canExecute;
        private readonly Action _execute;

        #endregion // Fields

        #region Constructors

        public MyRelayCommand(Action execute)
            : this(execute, null)
        {
        }

        public MyRelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion // Constructors

        #region ICommand Members

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _execute();
        }

        #endregion // ICommand Members
    }
}