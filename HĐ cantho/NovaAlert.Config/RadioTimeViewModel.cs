using NovaAlert.Common.Mvvm;
using NovaAlert.Entities;
using System;

namespace NovaAlert.Config
{
    public class RadioTimeViewModel: ViewModelBase
    {
        #region Properties
        public bool IsReadOnly
        {
            get { return !AllowEdit || !IsEnabled; }
        }

        public bool AllowEdit { get; set; }

        public bool IsChanged { get; set; }
        public RadioTime RadioTimeObj { get; set; }

        public string Name
        {
            get { return string.Format("Lần {0}", this.RadioTimeObj.ListOrder); }
        }

        DateTime? _startTime;
        public DateTime? StartTime
        {
            get { return _startTime; }
            set { _startTime = value; OnPropertyChanged("StartTime"); }
        }

        DateTime? _endTime;

        public DateTime? EndTime
        {
            get { return _endTime; }
            set { _endTime = value; OnPropertyChanged("EndTime"); }
        }

        bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; OnPropertyChanged("IsEnabled"); OnPropertyChanged("IsReadOnly"); }
        }
        #endregion

        #region ctor
        public RadioTimeViewModel(RadioTime rt, bool isReadOnly)
        {
            this.AllowEdit = !isReadOnly;
            this.RadioTimeObj = rt;
            RejectChanges();
        }
        #endregion

        #region Helpers
        public void RejectChanges()
        {
            StartTime = this.RadioTimeObj.StartTime;
            EndTime = this.RadioTimeObj.EndTime;
            this.IsEnabled = this.RadioTimeObj.IsEnabled;
            IsChanged = false;
        }

        public void UpdateData()
        {
            if (RadioTimeObj.StartTime != _startTime)
            {
                RadioTimeObj.StartTime = _startTime;
                IsChanged = true;
            }
            if (RadioTimeObj.EndTime != _endTime)
            {
                RadioTimeObj.EndTime = _endTime;
                IsChanged = true;
            }

            if(this.RadioTimeObj.IsEnabled != _isEnabled)
            {
                this.RadioTimeObj.IsEnabled = _isEnabled;
                IsChanged = true;
            }
        }

        public bool IsValid()
        {
            if (!this.IsEnabled)
            {
                return true;
            }

            if (_startTime.HasValue && _endTime.HasValue)
            {
                return _startTime.Value.TimeOfDay < _endTime.Value.TimeOfDay;
            }

            return false;
        }
        #endregion
    }
}
