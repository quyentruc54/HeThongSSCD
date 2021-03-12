using NovaAlert.Entities;
using System.Collections.ObjectModel;
using System.Linq;

namespace NovaAlert.Config.ViewModels
{
    public class AlarmListViewModel : ConfigViewModelBase
    {
        INovaAlertConfigService _service = Proxy.CreateProxy();
        bool IsReadOnly { get; set; }
        public byte Type { get; set; }
        ObservableCollection<AlarmItemViewModel> _items;
        public ObservableCollection<AlarmItemViewModel> Items
        {
            get { return _items; }
            set { _items = value; OnPropertyChanged("Items"); }
        }

        bool _showTimeOfPlaying;
        public bool ShowTimeOfPlaying
        {
            get { return _showTimeOfPlaying; }
            set 
            { 
                _showTimeOfPlaying = value;
                OnPropertyChanged("ShowTimeOfPlaying");
                foreach (var item in this.Items)
                    item.ShowTimeOfPlaying = this.ShowTimeOfPlaying;
            }
        }

        public AlarmListViewModel(IClientApp app, byte type, bool showAll = true, bool isReadOnly = false):base(app)
        {
            this.App.AddLog("Khai báo báo hiệu", false);
            IsReadOnly = isReadOnly;
            Type = type;
            Init(showAll);
        }

        private void Init(bool showAll = true)
        {
            Items = new ObservableCollection<AlarmItemViewModel>();
            var allAlarms = _service.GetAlarms();
            var list = allAlarms.Where(a => a.DayType == Type && (showAll == true || a.TimesOfPlaying > 0));
            foreach (var item in list)
            {                 
                Items.Add(new AlarmItemViewModel(item, IsReadOnly));
            }
        }
    }
}
