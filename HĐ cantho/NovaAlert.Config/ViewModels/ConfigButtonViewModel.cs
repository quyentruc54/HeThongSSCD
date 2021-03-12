using NovaAlert.Common.Mvvm;
using NovaAlert.Entities;
using System;
using System.Collections.ObjectModel;

namespace NovaAlert.Config.ViewModels
{
    public class ConfigButtonViewModel : NovaAlert.Common.Mvvm.ViewModelBase
    {
        
        public RelayCommand ClickCommand { get; set; }

        bool _selected;
        public bool Selected { get { return _selected; } set { _selected = value; OnPropertyChanged("Selected"); } }

        string _name;
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged("Name"); } }

        byte _value;

        public byte Value
        {
            get { return _value; }
            set { _value = value; OnPropertyChanged("Value"); Name = GetName(); }
        }

        protected virtual string GetName()
        {
            return "";
        }

        public ConfigButtonViewModel()
        {
            ClickCommand = new RelayCommand(p => OnClick());
        }


        public EventHandler OnClicked;
        private void OnClick()
        {
            if (OnClicked != null) OnClicked(this, null);
        }
    }

    public class DayButtonViewModel : ConfigButtonViewModel
    {        
        protected override string GetName()
        {
            var str = "";
            var obj = Proxy.CreateProxy().GetEnum("DayOfWeek", Value);
            if (obj != null)
            {
                str = obj.Desc_VN;
            }
            return str;
        }

        public DayButtonViewModel(byte value)
            : base()
        {
            Value = value;
        }
    }

    public class DayTypeButtonViewModel : ConfigButtonViewModel
    {
        INovaAlertConfigService _proxy = Proxy.CreateProxy();
        public ObservableCollection<ConfigButtonViewModel> ButtonList { get; set; }

        protected override string GetName()
        {
            var str = "";
            var obj = _proxy.GetEnum("DayType", Value);
            if (obj != null)
            {
                str = obj.Desc_VN;
            }
            return str;
        }

        public DayTypeButtonViewModel()
            : base()
        {
            Value = 0;
            Init();
        }

        public DayTypeButtonViewModel(byte value):base()
        {
            Value = value;
            Init();
        }

        private void Init()
        {
            ButtonList = new ObservableCollection<ConfigButtonViewModel>();
            var list = _proxy.GetEnums("DayType");
            foreach (var item in list)
            {
                var obj = new ConfigButtonViewModel();
                obj.Value = item.Value;
                obj.Name = item.Desc_VN;
                if (this.Value == obj.Value) obj.Selected = true;
                ButtonList.Add(obj);
                obj.OnClicked += TypeClick;
            }
        }

        private void TypeClick(object sender, EventArgs e)
        {
            foreach (var item in ButtonList)
            {
                item.Selected = false;
            }
            var obj = (sender as ConfigButtonViewModel);
            obj.Selected = true;
            this.Value = obj.Value;
        }

        public void UndoChanges(byte originalValue)
        {
            this.Value = originalValue;
            foreach (var item in ButtonList)
            {
                item.Selected = item.Value == originalValue;
            }
        }
    }
}
