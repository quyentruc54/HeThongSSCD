using NovaAlert.Entities;
using System.Collections.Generic;

namespace NovaAlert.Config.ViewModels
{
    public class DisplayDataViewModel: ConfigViewModelBase
    {
        INovaAlertConfigService _service = Proxy.CreateProxy();
        public List<Contact> Contacts { get; private set; }
        public List<DisplayData> DisplayDataList { get; private set; }


        public DisplayDataViewModel(IClientApp app):base(app)
        {
            this.App.AddLog("Khai báo hiển thị kết quả báo động", false);
            this.Contacts = _service.GetAllContacts();
            LoadData();
        }

        private void LoadData()
        {
            this.DisplayDataList = _service.GetDisplayData();
            OnPropertyChanged("DisplayDataList");
        }

        protected override void Save()
        {
            _service.SaveDisplayData(this.DisplayDataList);
            this.App.AddLog("Lưu khai báo hiển thị kết quả báo động thành công", true);
        }

        protected override void Cancel()
        {
            LoadData();
            this.App.AddLog("Hủy khai báo hiển thị kết quả báo động", true);
        }
    }
}
