using NovaAlert.Common.Mvvm;
using NovaAlert.Common.Utils;
using NovaAlert.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace NovaAlert.Config.ViewModels
{
    public class ContactItemViewModel : ViewModelBase, NovaAlert.Common.Wpf.IModalDialog
    {
        INovaAlertConfigService _proxy = Proxy.CreateProxy();
        bool _selected;
        public bool Selected
        {
            get { return _selected; }
            set { _selected = value; OnPropertyChanged("Selected"); }
        }
        public Contact Phone { get; set; }

        public string AreaCode { get { return Phone.AreaCode; } set { Phone.AreaCode = value; OnPropertyChanged("AreaCode"); } }

        public string Number { get { return Phone.Number; } set { Phone.Number = value; OnPropertyChanged("Number"); } }
        

        public string UnitName { get { return Phone.UnitName; } set { Phone.UnitName = value; OnPropertyChanged("UnitName"); } }

        public string NameAbbr { get { return Phone.NameAbbr; } set { Phone.NameAbbr = value; OnPropertyChanged("NameAbbr"); } }

        public string Password { get { return Phone.Password; } set { Phone.Password = value; OnPropertyChanged("Password"); } }

        public string TSLAreaCode { get { return Phone.TSLAreaCode; } set { Phone.TSLAreaCode = value; OnPropertyChanged("TSLAreaCode"); } }

        public string TSLNumber { get { return Phone.TSLNumber; } set { Phone.TSLNumber = value; OnPropertyChanged("TSLNumber"); } }

        bool _tslEnable;
        public bool TSLEnabled
        {
            get { return _tslEnable; }
            set
            {
                _tslEnable = value;
                OnPropertyChanged("TSLEnabled");
                if(!_tslEnable)
                {
                    this.TSLAreaCode = null;
                    this.TSLNumber = null;
                }
            }
        }
        public bool ShowTSL { get; set; }

        public string FullNumber
        {
            get { return GeneralUtils.FormatPhoneNumber(this.AreaCode, this.Number); }
        }

        public string FullTSLNumber
        {
            get { return GeneralUtils.FormatPhoneNumber(this.TSLAreaCode, this.TSLNumber); }
        }

        ObservableCollection<string> _nameAbbrList;
        public ObservableCollection<string> NameAbbrList
        {
            get 
            {
                if (_nameAbbrList == null)
                {
                    _nameAbbrList = new ObservableCollection<string>(_proxy.GetUnitNameAbbrs());
                }
                return _nameAbbrList; 
            }
        }

        ObservableCollection<string> _unitNameList;
        public ObservableCollection<string> UnitNameList
        {
            get
            {
                if (_unitNameList == null)
                {
                    _unitNameList = new ObservableCollection<string>(_proxy.GetUnitNames());
                }
                return _unitNameList;
            }
        }
        
        public IEnumerable<Contact> ExistContacts { get; private set; }

        public ContactItemViewModel(Contact p, IEnumerable<Contact> existContacts)
        {
            this.ShowTSL = false;
            this.ExistContacts = existContacts;
            Phone = p;
            _tslEnable = !string.IsNullOrEmpty(this.Phone.TSLNumber);
        }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(UnitName)) throw new Exception("Tên đơn vị không được để trống!");
            if (string.IsNullOrWhiteSpace(NameAbbr)) throw new Exception("Tên hiển thị không được để trống!");
            if (string.IsNullOrWhiteSpace(Number)) throw new Exception("Số điện thoại không được để trống!");

            if (ExistContacts.Any(c => c.PhoneNumberId != this.Phone.PhoneNumberId && c.Number == this.Phone.Number && c.AreaCode == this.Phone.AreaCode))
                throw new Exception("Số điện thoại này đã tồn tại trong danh bạ!");
            
            if (string.IsNullOrEmpty(this.Number))
            {
                var sb = new StringBuilder();
                sb.AppendLine("Vui lòng nhập số điện thoại.");                
                throw new Exception(sb.ToString());
            }

            if(this.TSLEnabled)
            {
                if (string.IsNullOrWhiteSpace(TSLNumber)) throw new Exception("Số điện thoại truyền số liệu không được để trống!");
            }
        }

        public void OnOK()
        {
            Validate();
        }


        public void OnCancel()
        {            
        }
    }
}
