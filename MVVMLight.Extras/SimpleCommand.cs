public  class SimpleCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canexecute;

        public SimpleCommand(Action execute, Func<bool> canexecute)
        {
            _execute = execute;
            _canexecute = canexecute;
        }

        public SimpleCommand(Action execute)
            : this(execute, () => true)
        {

        }
        public bool CanExecute(object parameter)
        {
            return _canexecute();
        }

        public void Execute(object parameter)
        {
            _execute();
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }
    }
