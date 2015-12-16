  /// <summary>
    /// A command whose sole purpose is to 
    /// relay its functionality to other
    /// objects by invoking delegates. The
    /// default return value for the CanExecute
    /// method is 'true'.
    /// </summary>
    public class AskOpenCommand : ICommand
    {
        #region Constructors

        /// <summary>
        /// Creates a new command that can always execute.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        ///  <param name="ext">extention with no dot</param>
        public AskOpenCommand(Action<string> execute, string ext)
            : this(execute, null, ext)
        {
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        ///  <param name="ext">extention with no dot</param>
        public AskOpenCommand(Action<string> execute, Func<bool> canExecute, string ext)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute;
            _ext = ext;
        }

        #endregion // Constructors

        #region ICommand Members

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute();
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                if (_canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        public void Execute(object parameter)
        {
            FileDialog d = new OpenFileDialog();
            d.Filter = string.Format("{0} Files|*.{0}", _ext);
            var res = d.ShowDialog();

            if (res.HasValue && res.Value)
            {
                _execute(d.FileName);
            }
        }

        #endregion // ICommand Members

        #region Fields

        readonly Action<string> _execute;
        readonly Func<bool> _canExecute;
        private string _ext;

        #endregion // Fields
    }
