using NovaAlert.Common.Mvvm;
using NovaAlert.Entities;

namespace NovaAlert.Config.ViewModels
{
    public class ChannelItemViewModel : ViewModelBase
    {
        public const string ArmyAreaCode = "069";

        Channel _channel;
        public Channel Channel
        {
            get { return _channel; }
        }
        public int ChannelId { get { return _channel.ChannelId;} }
        public bool IsChanged { get; set; }
        public string Number 
        { 
            get { return _channel.Number; }
            set { if (_channel.Number == value) return; _channel.Number = value; OnPropertyChanged("Number"); IsChanged = true; }
        }
        public string AreaCode 
        {
            get { return _channel.AreaCode; }
            set 
            { 
                if (_channel.AreaCode == value) return; 
                _channel.AreaCode = value; 
                OnPropertyChanged("AreaCode");
                OnPropertyChanged("CanChangeRestricted");

                if (!this.CanChangeRestricted)
                {
                    this.IsRestricted = true;
                }
                IsChanged = true; 
            }
        }
        public bool IsRestricted 
        {
            get { return _channel.IsRestricted; }
            set { if (_channel.IsRestricted == value) return; _channel.IsRestricted = value; OnPropertyChanged("IsRestricted"); IsChanged = true; }
        }
        public bool AutoRecording 
        { 
            get { return _channel.AutoRecording; }
            set { if (_channel.AutoRecording == value) return; _channel.AutoRecording = value; OnPropertyChanged("AutoRecording"); IsChanged = true; } 
        }
        public bool AlertEnabled { get { return _channel.AlertEnabled; }
            set { if (_channel.AlertEnabled == value) return; _channel.AlertEnabled = value; OnPropertyChanged("AlertEnabled"); IsChanged = true; }
        }

        public bool CCPKEnabled
        {
            get { return _channel.CCPKEnabled; }
            set
            {
                if(_channel.CCPKEnabled != value)
                {
                    _channel.CCPKEnabled = value;
                    OnPropertyChanged("CCPKEnabled");
                    IsChanged = true;
                }
            }
        }

        public bool MultiDestEnabled { get { return _channel.MultiDestEnabled; }
            set { if (_channel.MultiDestEnabled == value) return; _channel.MultiDestEnabled = value; OnPropertyChanged("MultiDestEnabled"); IsChanged = true; }
        }

        public bool CanChangeRestricted
        {
            get { return true; }
        }

        public int? HotUnitId
        {
            get { return _channel.HotUnitId; }
            set
            {
                if(_channel.HotUnitId != value)
                {
                    _channel.HotUnitId = value;
                    OnPropertyChanged("HotUnitId");
                    IsChanged = true;
                }
            }
        }

        public ChannelItemViewModel(Channel c)
        {
            _channel = c;
        }
    }
}
