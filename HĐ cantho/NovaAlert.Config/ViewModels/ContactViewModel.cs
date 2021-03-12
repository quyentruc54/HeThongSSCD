using NovaAlert.Common;
using NovaAlert.Common.Mvvm;
using NovaAlert.Common.Wpf;
using NovaAlert.Config.Views;
using NovaAlert.Entities;
using NovaAlert.Entities.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace NovaAlert.Config.ViewModels
{
    public class ContactViewModel : ConfigViewModelBase
    {
        public RelayCommand AddContactCommand { get; set; }
        public RelayCommand EditContactCommand { get; set; }
        public RelayCommand DeleteContactCommand { get; set; }
        public RelayCommand MakeCallCommand { get; private set; }

        string _conditions;
        public string Conditions
        {
            get { return _conditions; }
            set { _conditions = value; OnPropertyChanged("Conditions"); ItemsView.Refresh(); }
        }

        ObservableCollection<ContactItemViewModel> _items;
        public ObservableCollection<ContactItemViewModel> Items
        {
            get { return _items; }
            set { _items = value; OnPropertyChanged("Items"); }
        }

        public ICollectionView ItemsView { get { return CollectionViewSource.GetDefaultView(Items); } }

        ContactItemViewModel _itemSelected;

        public ContactItemViewModel ItemSelected
        {
            get { return _itemSelected; }
            set { _itemSelected = value; OnPropertyChanged("ItemSelected"); }
        }

        public IEnumerable<HostPhoneViewModel> Channels { get; private set; }        

        HostPhoneViewModel _selectedChannel;
        public HostPhoneViewModel SelectedChannel
        {
            get { return _selectedChannel; }
            set { _selectedChannel = value; OnPropertyChanged("SelectedChannel"); }
        }

        public bool IsShowModal { get; set; }

        public bool ShowMakeCall { get { return this.Channels != null && this.Channels.Count() > 0; } }

        public bool AllowMultiSelect { get; set; }

        public event EventHandler MakeCallRequestHandler;
        public Action<HostPhoneViewModel, string> CallFunc { get; set; }
        public bool ShowTSL { get; set; }

        public ContactViewModel(IClientApp app, bool isModal = false, IEnumerable<HostPhoneViewModel> channels = null):base(app)
        {            
            this.AllowMultiSelect = false;
            this.Channels = channels;
            IsShowModal = isModal;
            
            if (IsShowModal) this.App.AddLog("Xem danh bạ", false);
            else this.App.AddLog("Khai báo danh bạ", false);

            AddContactCommand = new RelayCommand(p => AddContact());
            EditContactCommand = new RelayCommand(p => EditContact(), p => ItemSelected != null);
            DeleteContactCommand = new RelayCommand(p => OnDelete(), p => ItemSelected != null);
            Items = new ObservableCollection<ContactItemViewModel>();
            Init();
            ItemsView.Filter = new Predicate<object>(Filter);

            this.MakeCallCommand = new RelayCommand(p => OnMakeCall(), p => this.SelectedChannel != null);
        }

        public bool Filter(object obj)
        {
            if (string.IsNullOrWhiteSpace(Conditions)) return true;
            var p = obj as ContactItemViewModel;
            if (p != null)
                if ((p.NameAbbr != null && p.NameAbbr.IndexOf(Conditions, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    || p.Number.IndexOf(Conditions, StringComparison.CurrentCultureIgnoreCase) >= 0
                    || (p.UnitName != null && p.UnitName.IndexOf(Conditions, StringComparison.CurrentCultureIgnoreCase) >= 0)) return true;
            return false;
        }

        private void Init(bool isReload = false)
        {
            AddList = new List<object>();
            LoadData();
            Items.CollectionChanged += Items_CollectionChanged;
        }

        List<object> AddList;
        void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        AddList.Add(item);
                    }
                    _conditions = "";
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        if (AddList.Contains(item))
                        {
                            AddList.Remove(item);
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }

        private void LoadData()
        {
            Items.Clear();            
            var service = Proxy.CreateProxy();

            this.ShowTSL = string.Compare(service.GetParameterValue(eGlobalParameter.ShowTSL), "true", true) == 0;
            OnPropertyChanged("ShowTSL");

            var contactList = service.GetAllContacts();
            foreach (var item in contactList)
            {
                Items.Add(new ContactItemViewModel(item, this.Items.Select(it => it.Phone)));
            }
            
            ItemsView.Refresh();
        }

        private void AddContact()
        {
            var vm = new ContactItemViewModel(new Contact(), this.Items.Select(it => it.Phone));
            vm.ShowTSL = this.ShowTSL;
            var v = new ContactView();
            v.DataContext = vm;
            if (ModalDialog.ShowControl(v, "Thêm danh bạ") == true)
            {
                Conditions = "";
                var service = Proxy.CreateProxy();
                service.SaveContact(vm.Phone);
                Items.Add(vm);
                this.App.AddLog(string.Format("Thêm số điện thoại {0} {1}", vm.Phone.AreaCode, vm.Phone.Number), true);
            }
        }

        private void EditContact()
        {            
            var v = new ContactView();            
            v.DataContext = ItemSelected;
            ItemSelected.ShowTSL = this.ShowTSL;

            var oldContact = new Contact()
            {
                AreaCode = ItemSelected.Phone.AreaCode,
                Number = ItemSelected.Phone.Number,
                NameAbbr = ItemSelected.Phone.NameAbbr,
                UnitName = ItemSelected.Phone.UnitName,
                Password = ItemSelected.Password,
                PhoneNumberId = ItemSelected.Phone.PhoneNumberId,
                TSLAreaCode = ItemSelected.TSLAreaCode,
                TSLNumber = ItemSelected.TSLNumber
            };

            this.App.AddLog(string.Format("Thay đổi số điện thoại {0} {1}", oldContact.AreaCode, oldContact.Number), true);

            if (ModalDialog.ShowControl(v, "Sửa danh bạ") == true)
            {
                var service = Proxy.CreateProxy();
                service.SaveContact(ItemSelected.Phone);                
                this.App.AddLog(string.Format("Lưu thay đổi số điện thoại {0} {1}", ItemSelected.Phone.AreaCode, ItemSelected.Phone.Number), true);
            }
            else
            {
                ItemSelected.Phone = oldContact;
                this.App.AddLog(string.Format("Hủy thay đổi số điện thoại {0} {1}", oldContact.AreaCode, oldContact.Number), true);
            }

            ItemSelected.Refesh();
        }

        void OnDelete()
        {
            var msg = string.Format("Bạn có chắc chắn xóa số điện thoại {0} {1} này không ?", ItemSelected.AreaCode, ItemSelected.Number);
            if(GetService<IMessageBoxService>().AskYesNo(msg) == System.Windows.MessageBoxResult.Yes)
            {
                Proxy.CreateProxy().DeleteContact(ItemSelected.Phone.PhoneNumberId);
                this.Items.Remove(ItemSelected);

                this.App.AddLog(string.Format("Xóa số điện thoại {0} {1}", ItemSelected.AreaCode, ItemSelected.Number), true);                
            }
        }

        protected override void Save()
        {
            foreach (var item in AddList)
            {
                var obj = item as ContactItemViewModel;
                obj.Validate();                
            }
            AddList.Clear();
        }

        protected override void Cancel()
        {
            Init(true);
        }

        public List<ContactItemViewModel> GetSelectedItems()
        {
            var list = new List<ContactItemViewModel>();
            if (!this.AllowMultiSelect && this.ItemSelected != null) list.Add(this.ItemSelected);
            else
            {
                list.AddRange(this.Items.Where(it => it.Selected));
                if (list.Count == 0 && this.ItemSelected != null)
                {
                    list.Add(this.ItemSelected);
                }
            }
            return list;
        }

        void OnMakeCall()
        {
            if (MakeCallRequestHandler != null) MakeCallRequestHandler(this, EventArgs.Empty);
            if (CallFunc != null)
            {
                CallFunc(this.SelectedChannel, this.ItemSelected.Number);
            }
        }
    }
}
