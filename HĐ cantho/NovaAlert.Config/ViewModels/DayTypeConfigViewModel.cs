using NovaAlert.Common.Mvvm;
using NovaAlert.Entities;
using System;
using System.Collections.Generic;

namespace NovaAlert.Config.ViewModels
{
    public class DayTypeConfigViewModel: ConfigViewModelBase
    {
        INovaAlertConfigService _service = Proxy.CreateProxy();
        public List<DayTypeViewModel> DTVMs { get; set; }
        ViewModelBase _itemSelected;
        public ViewModelBase ItemSelected { get { return _itemSelected; } set { _itemSelected = value; OnPropertyChanged("ItemSelected"); } }
        
        public DayTypeConfigViewModel(IClientApp app):base(app)
        {
            this.App.AddLog("Khai báo báo giờ", false);

            DTVMs = new List<DayTypeViewModel>();            
            foreach (var item in _service.GetDayTypes())
            {
                var obj = new DayTypeViewModel(this.App, item);
                obj.OnButtonClicked += ButtonClick;
                DTVMs.Add(obj);
            }            
        }

        protected override void  Cancel()
        {
            ItemSelected = null;
            foreach (var item in DTVMs)
            {
                item.Cancel();
            }

            this.App.AddLog("Hủy khai báo báo giờ", true);
        }

        protected override void Save()
        {
            var list = new List<DayTypeConfig>();
            foreach (var item in DTVMs)
            {
                item.UpdateData();
                if (item.IsChanged)
                {
                    list.Add(item.DayType);
                    item.IsChanged = false;
                }
                
            }
            _service.SaveDayTypes(list);
            this.App.AddLog("Lưu khai báo báo giờ thành công", true);
        }

        private void ButtonClick(object sender, EventArgs e)
        {
            var obj = sender as ConfigButtonViewModel;
            foreach (var item in DTVMs)
            {
                if (item.Day != obj)
                {
                    item.Day.Selected = false;
                }
                if (item.Type != obj)
                {
                    item.Type.Selected = false;
                }
        
            }
            obj.Selected = !obj.Selected;
            if (obj.Selected)
            {
                ItemSelected = obj;
            }
            else
            {
                ItemSelected = null;
            }
        }
    }
}
