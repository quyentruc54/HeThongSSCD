using NovaAlert.Common.Mvvm;
using NovaAlert.Common.Wpf;
using NovaAlert.Common.Utils;
using NovaAlert.Config.Views;
using NovaAlert.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NovaAlert.Common;

namespace NovaAlert.Config.ViewModels
{
    public class GroupListViewModel : ConfigViewModelBase
    {
        public RelayCommand AddCommand { get; set; }
        public RelayCommand ModifyCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        GroupViewModel _itemSelected;
        public GroupViewModel ItemSelected { get { return _itemSelected; } set { _itemSelected = value; OnPropertyChanged("ItemSelected"); } }
        
        public ObservableCollection<GroupViewModel> Groups { get; set; }
        
        public List<Contact> PhoneNumbers {get;set;}

        INovaAlertConfigService _service = Proxy.CreateProxy();

        public bool ShowButtons { get; set; }

        public GroupListViewModel(IClientApp app): base(app)
        {            
            this.ShowButtons = true;
            PhoneNumbers = _service.GetAllContacts();
            
            AddCommand = new RelayCommand(p => Add());
            ModifyCommand = new RelayCommand(p => Modify(), p => ItemSelected != null);
            DeleteCommand = new RelayCommand(p => Delete(), p => ItemSelected != null);

            LoadGroups();
        }

        private void LoadGroups()
        {
            if (this.Groups == null) this.Groups = new ObservableCollection<GroupViewModel>();
            else this.Groups.Clear();

            var list = _service.GetAllContactGroups();
            foreach (var item in list)
            {
                Groups.Add(new GroupViewModel(this.App, item));
            }
        }

        private void Delete()
        {
            if (GetService<IMessageBoxService>().AskYesNo("Bạn có chắc chắn xóa nhóm đang chọn không ?") == System.Windows.MessageBoxResult.Yes)
            {
                string groupName = this.ItemSelected.Name;
                this.ItemSelected.GroupObj.IsDeleted = true;
                _service.SaveContactGroup(ItemSelected.GroupObj);
                Groups.Remove(ItemSelected);                
                this.App.AddLog(string.Format("Xóa nhóm {0}", groupName), true);
            }
        }

        private void Modify()
        {
            var v = new GroupView();
            
            v.DataContext = ItemSelected;
            this.App.AddLog(string.Format("Thay đổi nhóm {0}", this.ItemSelected.Name), true);

            var cg = this.ItemSelected.GroupObj.CloneJson();
            if (ModalDialog.ShowControl(v, "Sửa nhóm") == true)
            {
                _service.SaveContactGroup(ItemSelected.GroupObj);
                this.App.AddLog("Lưu thay đổi nhóm thành công", true);
                this.ItemSelected.RefeshDetails();
            }
            else
            {
                ItemSelected.LoadContactGroup(cg);
                this.App.AddLog("Hủy thay đổi nhóm", true);
            }
        }

        private void Add()
        {
            var obj = new ContactGroup();
            var vm = new GroupViewModel(this.App, obj);
            var v = new GroupView();
            v.DataContext = vm;
            if (ModalDialog.ShowControl(v, "Thêm nhóm") == true)
            {
                _service.SaveContactGroup(obj);
                this.App.AddLog(string.Format("Thêm nhóm {0}", vm.Name), false);
                LoadGroups();
            }
        }
    }
}
