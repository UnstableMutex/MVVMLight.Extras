    public abstract class ApproveVMB : ViewModelBase
    {
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected ApproveVMB()
        {
            ApproveCommand = new RelayCommand(Approve,CanApprove);
        }
        public ICommand ApproveCommand { get; private set; }
        protected abstract void Approve();
        protected virtual bool CanApprove()
        {
            return true;
        }
    }
