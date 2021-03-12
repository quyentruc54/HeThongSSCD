using System;
using NovaAlert.Common;
using NovaAlert.Common.Mvvm;
using NovaAlert.Entities;

namespace NovaAlert.Config.ViewModels
{
    public class ConfigViewModel : NovaAlert.Common.Mvvm.ViewModelBase
    {
        public IClientApp App { get; private set; }
        public RelayCommand AbortCommand { get; private set; }
        public RelayCommand ExitCommand { get; private set; }
        public ConfigNavBarViewModel NavBar { get; set; }
        
        public event EventHandler OnAbortHandler;
        public ConfigViewModel(IClientApp app)
        {
            this.App = app;
            NavBar = new ConfigNavBarViewModel(this.App);
            this.AbortCommand = new RelayCommand(p => OnAbort());
            this.ExitCommand = new RelayCommand(p => DoExit());
        }

        private void OnAbort()
        {
            this.NavBar.SetSelectView(eConfigView.None);
            if (OnAbortHandler != null)
            {
                OnAbortHandler(this, EventArgs.Empty);
            }
        }

        void DoExit()
        {
            const string msg = "Bạn có chắc chắn thoát khỏi chương trình không ?";
            if (ServiceContainer.Instance.GetService<IMessageBoxService>().AskYesNo(msg) == System.Windows.MessageBoxResult.Yes)
            {
                System.Windows.Application.Current.Shutdown();
            }
        }
    }
}
