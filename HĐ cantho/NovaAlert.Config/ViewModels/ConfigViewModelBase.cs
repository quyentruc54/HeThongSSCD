using NovaAlert.Common.Mvvm;
using NovaAlert.Entities;

namespace NovaAlert.Config.ViewModels
{
    public class ConfigViewModelBase : ViewModelBase
    {
        public IClientApp App { get; private set; }

        public ConfigViewModelBase(IClientApp app)
        {
            this.App = app;
        }

        RelayCommand _saveCommand;
        public RelayCommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(p => Save());
                }
                return _saveCommand;
            }
        }

        RelayCommand _cancelCommand;
        public RelayCommand CancelCommand 
        {
            get
            {
                if (_cancelCommand == null)
                {
                    _cancelCommand = new RelayCommand(p => Cancel());
                }
                return _cancelCommand;
            }
        }

        protected virtual void Cancel()
        {
            
        }
        protected virtual void Save()
        {
        }
    }
}
