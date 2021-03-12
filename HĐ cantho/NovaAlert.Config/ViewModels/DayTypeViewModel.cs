using NovaAlert.Entities;
using System;

namespace NovaAlert.Config.ViewModels
{
    public class DayTypeViewModel: ConfigViewModelBase
    {
        public EventHandler OnButtonClicked;
        public bool IsChanged { get; set; }
        public DayTypeConfig DayType { get; private set; }

        public DayButtonViewModel Day { get; set; }

        public DayTypeButtonViewModel Type { get; set; }
        AlarmListViewModel _alarmVM;
        public AlarmListViewModel AlarmVM
        {
            get 
            {
                if (_alarmVM == null)
                {
                    _alarmVM = new AlarmListViewModel(this.App, Type.Value, false, true);
                }
                return _alarmVM; 
            }
        }

        public DayTypeViewModel(IClientApp app, DayTypeConfig day): base(app)
        {
            DayType = day;
            Day = new DayButtonViewModel(day.DayOfWeek);            
            Type = new DayTypeButtonViewModel(day.DayType);            
            Day.OnClicked += OnButtonClick;
            Type.OnClicked += OnButtonClick;
        }

        public void UpdateData()
        {
            if (DayType.DayType != Type.Value)
            {
                DayType.DayType = Type.Value;
                IsChanged = true;
            }
        }

        public void Cancel()
        {
            Type.UndoChanges( DayType.DayType);
        }

        void OnButtonClick(object sender, EventArgs e)
        {
            if (OnButtonClicked != null) OnButtonClicked(sender, e);
        }
    }
}
