using NovaAlert.Common;
using NovaAlert.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace NovaAlert.Config.ViewModels
{
    public class DayTypeAlarmViewModel : ConfigViewModelBase
    {
        AlarmListViewModel _alarmVM;

        public AlarmListViewModel AlarmVM
        {
            get { return _alarmVM; }
            set { _alarmVM = value; OnPropertyChanged("AlarmVM"); }
        }

        RadioTimeListViewModel _radioTimeListVM;
        public RadioTimeListViewModel RadioTimeListVM
        {
            get { return _radioTimeListVM; }
            set { _radioTimeListVM = value; OnPropertyChanged("RadioTimeListVM"); }
        }

        public ObservableCollection<ConfigButtonViewModel> ButtonList { get; set; }

        bool _isAlarmOnSwitch;
        public bool IsAlarmOnSwitch
        {
            get { return _isAlarmOnSwitch; }
            set 
            { 
                _isAlarmOnSwitch = value;
                OnPropertyChanged("IsAlarmOnSwitch");
                Proxy.CreateProxy().SetAlarmConfig(_isAlarmOnSwitch);
                if (this.AlarmVM != null)
                {
                    this.AlarmVM.ShowTimeOfPlaying = !_isAlarmOnSwitch;
                }
            }
        }
        public DayTypeAlarmViewModel(IClientApp app):base(app)
        {
            this.App.AddLog("Khai báo âm hiệu", false);
            ButtonList = new ObservableCollection<ConfigButtonViewModel>();
            var service = Proxy.CreateProxy();
            var list = service.GetEnums("DayType");
            foreach (var item in list)
            {
                var obj = new ConfigButtonViewModel();
                obj.Value = item.Value;
                obj.Name = item.Desc_VN;                
                ButtonList.Add(obj);
                obj.OnClicked += TypeClick;
            }

            _isAlarmOnSwitch = service.IsAlartOnSwitch();
        }

        private void TypeClick(object sender, EventArgs e)
        {
            foreach (var item in ButtonList)
            {
                item.Selected = false;
            }
            var obj = (sender as ConfigButtonViewModel);
            obj.Selected = true;
            AlarmVM = new AlarmListViewModel(this.App, obj.Value);
            AlarmVM.ShowTimeOfPlaying = !_isAlarmOnSwitch;

            this.RadioTimeListVM = new RadioTimeListViewModel(this.App, obj.Value);            
        }
        
        protected override void Save()
        {
            if (this.AlarmVM == null || !DoValidate())
            {
                return;
            }
            
            var service = Proxy.CreateProxy();
            var list = new List<Alarm>(); 
            foreach (var item in AlarmVM.Items)
            {
                item.UpdateData();
                if(item.IsChanged)
                {
                    list.Add(item.AlarmObj);
                    item.IsChanged = false;
                }
            }
            if (list.Count > 0)
            {
                service.SaveAlarms(list);
            }

            var rtList = new List<RadioTime>();
            foreach(var item in this.RadioTimeListVM.Items)
            {
                item.UpdateData();
                if(item.IsChanged)
                {
                    rtList.Add(item.RadioTimeObj);
                    item.IsChanged = false;
                }
            }
            if (rtList.Count > 0)
            {
                service.SaveRadioTimes(rtList);
            }

            this.App.AddLog("Lưu khai báo âm hiệu thành công", true);
        }

        bool DoValidate()
        {
            var radioTimes = this.RadioTimeListVM.Items.Where(it => it.IsValid() == false);
            if (radioTimes.Any())
            {
                var sb = new StringBuilder();
                sb.AppendLine("Mốc thời gian không hợp lệ:");
                foreach (var item in radioTimes)
                {
                    sb.AppendLine("    - " + item.Name);
                }

                GetService<IMessageBoxService>().ShowError(sb.ToString());
                return false;
            }

            return true;
        }

        protected override void Cancel()
        {
            if (AlarmVM == null) return;
            foreach (var item in AlarmVM.Items)
            {
                item.RejectChanges();
            }

            foreach(var item in this.RadioTimeListVM.Items)
            {
                item.RejectChanges();
            }

            this.App.AddLog("Hủy khai báo âm hiệu", true);
        }
    }
}
