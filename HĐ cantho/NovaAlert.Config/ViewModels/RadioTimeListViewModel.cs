using NovaAlert.Common.Mvvm;
using NovaAlert.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace NovaAlert.Config.ViewModels
{
    public class RadioTimeListViewModel : ConfigViewModelBase
    {
        INovaAlertConfigService _service = Proxy.CreateProxy();
        bool IsReadOnly { get; set; }
        public byte Type { get; set; }
        ObservableCollection<RadioTimeViewModel> _items;
        public ObservableCollection<RadioTimeViewModel> Items
        {
            get { return _items; }
            set { _items = value; OnPropertyChanged("Items"); }
        }

        public RadioTimeListViewModel(IClientApp app, byte type, bool showAll = true, bool isReadOnly = false):base(app)
        {
            this.App.AddLog("Khai báo báo hiệu", false);
            IsReadOnly = isReadOnly;
            Type = type;
            Init(showAll);
        }

        private void Init(bool showAll = true)
        {
            Items = new ObservableCollection<RadioTimeViewModel>();
            var list = _service.GetRadioTimes(this.Type);            
            foreach (var item in list)
            {                  
                Items.Add(new RadioTimeViewModel(item, IsReadOnly));
            }
        }
    }
}
