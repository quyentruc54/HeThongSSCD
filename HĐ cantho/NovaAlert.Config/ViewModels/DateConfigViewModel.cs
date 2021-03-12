using NovaAlert.Entities;
using System;

namespace NovaAlert.Config.ViewModels
{
    public class DateConfigViewModel : ConfigViewModelBase
    {
        DateTime _systemTime;
        public DateTime SystemTime 
        {
            get { return _systemTime;}
            set { _systemTime = value; OnPropertyChanged("SystemTime"); } 
        }

        public DateConfigViewModel(IClientApp app):base(app)
        {
            this.App.AddLog("Khai báo thời gian", false);
            SystemTime = DateTime.Now;
        }

        protected override void Save()
        {
            App.Service.UpdateSystemDateTime(SystemTime);
            //ConfigServices.RaiseDateTimeChangedEvent(SystemTime);
            System.Threading.Thread.Sleep(100);
            this.App.AddLog("Lưu khai báo thời gian thành công", true);
        }

        protected override void Cancel()
        {
            this.SystemTime = DateTime.Now;
            this.App.AddLog("Hủy khai báo thời gian", true);
        }
    }
}
