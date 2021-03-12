using System;
using System.Windows;
using NovaAlert.Common.Mvvm;
using NovaAlert.Config.ViewModels;
using NovaAlert.Entities;
using NovaAlert.Common.Utils;

namespace NovaAlert.Bll
{
    public class MenuViewModel: ViewModelBase
    {
        #region Commands
        public RelayCommand ExitCommand { get; private set; }
        public RelayCommand MenuCommand { get; private set; }
        public RelayCommand ContactCommand { get; private set; }
        public RelayCommand AlertCommand { get; private set; }
        public RelayCommand MultiDestCommand { get; private set; }
        public RelayCommand AlarmCommand { get; private set; }
        public RelayCommand LoginCommand { get; private set; }
        #endregion

        #region Members & Properties

        bool _isInConfigMode = false;
        public bool IsInConfigMode
        {
            get { return _isInConfigMode; }
            set 
            { 
                _isInConfigMode = value; 
                OnPropertyChanged("IsInConfigMode");
                if (_isInConfigMode == false)
                {
                    Config.NavBar.Reset();
                }
            }
        }

        ConfigViewModel _config;
        public ConfigViewModel Config
        {
            get
            {
                if (_config == null)
                {
                    _config = new ConfigViewModel(this.App);
                    _config.OnAbortHandler += Config_OnAbortHandler;
                }
                return _config;
            }
        }

        LoginViewModel _login;
        public LoginViewModel Login
        {
            get { return _login; }
            set { _login = value; OnPropertyChanged("Login"); }
        }

        public ClientAppViewModel App { get; private set; }

        #endregion

        public MenuViewModel(ClientAppViewModel app)
        {
            this.App = app;
            InitCommand();
            _login = new LoginViewModel();            
        }

        protected virtual void InitCommand()
        {
            this.ExitCommand = new RelayCommand(p => Application.Current.Shutdown());
            this.LoginCommand = new RelayCommand(p => Login.Visible = true);
            this.MenuCommand = new RelayCommand(p => ShowConfig(), p => CanShowConfig());
            this.AlertCommand = new RelayCommand(p => SwitchAppMode(eWorkingMode.Alert));
            this.MultiDestCommand = new RelayCommand(p => SwitchAppMode(eWorkingMode.MultiDest));
            this.AlarmCommand = new RelayCommand(p => SwitchAppMode(eWorkingMode.Alarm));
            this.ContactCommand = new RelayCommand(p => ContactView(), p => CanShowContact());
        }

        private void ContactView()
        {
            var ct = NovaAlert.Config.ConfigServices.ShowContact(this.App, this.App.Channels.Items, false, null);
            if(ct != null && ct.ItemSelected != null)
            {                
                System.Windows.Application.Current.Dispatcher.Invoke(
                    new Action<Contact>(this.App.Units.SetSelectedContact),
                    System.Windows.Threading.DispatcherPriority.Background, ct.ItemSelected.Phone);
            }
        }

        private void Config_OnAbortHandler(object sender, EventArgs e)
        {
            this.IsInConfigMode = false;
            this.App.Refesh();
        }

        void SwitchAppMode(eWorkingMode mode)
        {
            if (App != null)
            {
                App.AddLog(string.Format("Chọn chế độ: {0}", EnumEx.GetEnumDescription(mode)));
                App.WorkingMode = mode;
            }
        }

        void ShowConfig()
        {
            this.App.AddLog("Khai báo");
            IsInConfigMode = true;
        }
        bool CanShowConfig()
        {
            return this.App != null && this.App.MainController != null && !this.App.MainController.HasConnection;            
        }

        bool CanShowContact()
        {
            return this.App != null && this.App.MainController != null &&
                (this.App.MainController.HasConnection == false || this.App.MainController.IsHolding)
                && this.App.Units != null && !this.App.Units.IsTempUnitsConnected();            
        }
    }
}
