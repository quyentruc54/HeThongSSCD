using System;
using NovaAlert.Entities;
using System.ComponentModel;

namespace NovaAlert.Service
{
    public class Client: BaseEntity, INotifyPropertyChanged
    {
        public eClientType Type { get; set; }        
        public INovaAlertServiceCallback Callback { get; set; }

        public DateTime SubscribeTime { get; private set; }

        DateTime? _lastAction;
        public DateTime? LastAction
        {
            get { return _lastAction; }
            set { _lastAction = value; OnPropertyChanged("LastAction"); }
        }

        bool _isDisconnected;
        public bool IsDisconnected
        {
            get { return _isDisconnected; }
            set { _isDisconnected = value; OnPropertyChanged("IsDisconnected"); }
        }

        eAlertSoundStatus _alertSoundStatus;
        public eAlertSoundStatus AlertSoundStatus
        {
            get { return _alertSoundStatus; }
            set { _alertSoundStatus = value; OnPropertyChanged("AlertSoundStatus"); }
        }

        public Client(int id, string name = null):base(id, name)
        {
            SubscribeTime = DateTime.Now;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
