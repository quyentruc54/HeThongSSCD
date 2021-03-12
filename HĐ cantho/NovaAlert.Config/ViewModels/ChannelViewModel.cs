using NovaAlert.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NovaAlert.Config.ViewModels
{
    public class ChannelViewModel : ConfigViewModelBase
    {        
        public bool ShowCCPK { get; private set; }

        public bool ShowTSL { get; private set; }

        ObservableCollection<ChannelItemViewModel> _channels;
        public ObservableCollection<ChannelItemViewModel> Channels
        {
            get { return _channels; }
            set { _channels = value; }
        }

        public List<KeyValuePair<int?, string>> AllContacts { get; set; }

        public ChannelViewModel(IClientApp app):base(app)
        {
            this.App.AddLog("Khai báo danh sách kênh", false);
            Channels = new ObservableCollection<ChannelItemViewModel>();
            LoadData();
        }

        private void LoadData()
        {
            Channels.Clear();

            var service = Proxy.CreateProxy();
            this.ShowCCPK = string.Compare(service.GetParameterValue(eGlobalParameter.Menu_CCPK_Visible), "true", true) == 0;
            OnPropertyChanged("ShowCCPK");

            var list = service.GetAllChannels();
            foreach (var c in list)
            {
                var cvm = new ChannelItemViewModel(c);
                cvm.PropertyChanged += ChannelVM_PropertyChanged;
                Channels.Add(cvm);
            }
            
            this.AllContacts = new List<KeyValuePair<int?, string>>();
            this.AllContacts.Add(new KeyValuePair<int?, string>(null, string.Empty));
            foreach(var item in service.GetAllContacts())
            {
                this.AllContacts.Add(new KeyValuePair<int?, string>(item.PhoneNumberId, string.Format("{0}-{1}{2}", item.UnitName, item.AreaCode, item.Number)));
            }
        }

        void ChannelVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        { 
        }

        protected override void Save()
        {
            var hotChannels = this.Channels.Where(c => c.HotUnitId.HasValue).ToList();
            if (hotChannels.GroupBy(c => c.HotUnitId.Value).Any(g => g.Count() > 1))
            {
                throw new InvalidOperationException("Khai báo trùng đơn vị nóng.");
            }

            var list = new List<Channel>();
            foreach (var item in Channels)
            {
                if(item.IsChanged) 
                {
                    if (string.IsNullOrEmpty(item.Number))
                    {
                        throw new InvalidOperationException("Vui lòng nhập số điện thoại cho kênh " + item.ChannelId.ToString());
                    }
                    list.Add(item.Channel);
                    item.IsChanged = false;
                }                 
            }

            var service = Proxy.CreateProxy(); 
            service.SaveChannels(list);
            this.App.AddLog("Lưu thay đổi danh sách kênh thành công", true);
        }

        protected override void Cancel()
        {
            LoadData();
            this.App.AddLog("Hủy thay đổi danh sách kênh", true);
        }
    }
}
