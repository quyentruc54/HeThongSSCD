using System.Windows;
using NovaAlert.Common.Mvvm;

namespace NovaAlert.Bll
{
    public class LoginViewModel: ViewModelBase
    {
        string _userName;
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; OnPropertyChanged("UserName"); }
        }

        string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; OnPropertyChanged("Password"); }
        }

        bool _visible;
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; OnPropertyChanged("Visible"); }
        }

        public RelayCommand LoginCommand { get; private set; }
        public RelayCommand ExitCommand { get; private set; }

        public LoginViewModel()
        {
            this.LoginCommand = new RelayCommand(p => DoLogin());
            this.ExitCommand = new RelayCommand(p => Application.Current.Shutdown());
        }

        void DoLogin()
        {
            this.Visible = false;
        }
    }
}
