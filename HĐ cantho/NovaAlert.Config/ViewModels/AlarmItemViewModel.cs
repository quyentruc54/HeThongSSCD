using NovaAlert.Common.Mvvm;
using NovaAlert.Entities;
using System;

namespace NovaAlert.Config.ViewModels
{
    public class AlarmItemViewModel : ViewModelBase
    {
        public bool IsReadOnly
        {
            get { return !AllowEdit || !IsEnabled; }
        }
        
        public bool AllowEdit { get; set; }

        public bool IsChanged { get; set; }
        public Alarm AlarmObj { get; set; }

        public string Name { get { return AlarmObj.Name;} }

        DateTime _time;
        public DateTime Time { get { return _time; } set { _time = value; OnPropertyChanged("Time"); } }

        byte _timesOfPlaying;
        public byte TimesOfPlaying { get { return _timesOfPlaying; } set { _timesOfPlaying = value; OnPropertyChanged("TimesOfPlaying"); } }

        bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; OnPropertyChanged("IsEnabled"); OnPropertyChanged("IsReadOnly"); }
        }

        bool _showTimeOfPlaying;
        public bool ShowTimeOfPlaying
        {
            get { return _showTimeOfPlaying; }
            set { _showTimeOfPlaying = value; OnPropertyChanged("ShowTimeOfPlaying"); }
        }

        public AlarmItemViewModel(Alarm alarm, bool isReadOnly)
        {
            _showTimeOfPlaying = true;
            this.AllowEdit = !isReadOnly;
            AlarmObj = alarm;
            RejectChanges();
        }

        public void RejectChanges()
        {
            TimesOfPlaying = AlarmObj.TimesOfPlaying;
            Time = AlarmObj.Time;
            this.IsEnabled = AlarmObj.IsEnabled;
        }

        public void UpdateData()
        {
            if(AlarmObj.Time != _time)
            {
                AlarmObj.Time = _time;
                IsChanged = true;
            }

            if (AlarmObj.TimesOfPlaying != _timesOfPlaying)
            {
                AlarmObj.TimesOfPlaying = _timesOfPlaying;
                IsChanged = true;
            }

            if (this.AlarmObj.IsEnabled != _isEnabled)
            {
                this.AlarmObj.IsEnabled = _isEnabled;
                IsChanged = true;
            }
        }
    }
}
