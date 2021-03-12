using NovaAlert.Common.Mvvm;

namespace NovaAlert.Common.Wpf
{
    public class MsgBoxWithTimeoutViewModel: DialogViewModelBase
    {
        string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; OnPropertyChanged("Title"); }
        }

        string _message;
        public string Message
        {
            get { return _message; }
            set { _message = value; OnPropertyChanged("Message"); }
        }

        System.Threading.Timer _timeoutTimer;
        public int Timeout { get; set; }
        int _counter = 0;

        public MsgBoxWithTimeoutViewModel(string msg, string title = "Thông báo", int timeout = 5)
        {
            _title = title;
            _message = msg;
            this.Timeout = timeout;

            _timeoutTimer = new System.Threading.Timer(OnTimerElapsed, null, 1000, 1000);
        }

        void OnTimerElapsed(object state)
        {
            _counter++;
            if(_counter > this.Timeout)
            {                
                _timeoutTimer.Dispose();
                this.DialogResult = false;
            }
            else
            {
                this.Title = string.Format("Thông báo sẽ tự động tắt trong {0} giây", this.Timeout - _counter);
            }
        }
    }
}
