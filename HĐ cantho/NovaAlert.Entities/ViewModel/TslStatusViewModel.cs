using NovaAlert.Common.Mvvm;

namespace NovaAlert.Entities.ViewModel
{
    public class TslStatusViewModel: ViewModelBase
    {
        TslStatus _status;
        
        public eTslStatus Status
        {
            get { return _status.Status; }
            set
            {
                if(_status.Status != value)
                {
                    _status.Status = value;
                    OnPropertyChanged("Status");
                }
            }
        }

        public TslStatusViewModel(TslStatus status)
        {
            _status = status;
        }
    }
}
