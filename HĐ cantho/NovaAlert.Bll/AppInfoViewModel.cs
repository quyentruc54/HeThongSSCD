using System;
using System.Globalization;
using NovaAlert.Common.Mvvm;
using NovaAlert.Common.Utils;
using NovaAlert.Entities;

namespace NovaAlert.Bll
{
    public class AppInfoViewModel: ViewModelBase
    {
        private const string CultureName = "vi-VN";
        protected CultureInfo _cultureInfo = new CultureInfo(CultureName);

        DateTime _curDateTime;
        public DateTime CurDateTime
        {
            get { return _curDateTime; }
            set
            {
                _curDateTime = value;
                OnPropertyChanged("CurDateTime");
                OnPropertyChanged("DayOfWeek");
            }
        }

        public string DayOfWeek
        {
            get
            {
                return _cultureInfo.DateTimeFormat.GetDayName(_curDateTime.DayOfWeek);
            }
        }        

        string _officeName;
        public string OfficeName
        {
            get { return _officeName; }
            set { _officeName = value; OnPropertyChanged("OfficeName"); }
        }

        eWorkingMode _workingMode;
        public eWorkingMode WorkingMode
        {
            get { return _workingMode; }
            set
            {
                _workingMode = value;
                OnPropertyChanged("WorkingMode");
                OnPropertyChanged("Title");
            }
        }

        public string Title
        {
            get { return EnumEx.GetEnumDescription(_workingMode); }
        }

        public AppInfoViewModel()
        {
            this.OfficeName = ((INovaAlertService)WcfProxy.Instance).GetParameterValue(eGlobalParameter.OfficeName); //"SCH - QK7";
            this.CurDateTime = WcfProxy.Instance.GetDateTime(); 
            this.WorkingMode = eWorkingMode.Alert;            
        }
    }
}
