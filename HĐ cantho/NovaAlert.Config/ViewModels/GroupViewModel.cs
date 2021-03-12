using NovaAlert.Common;
using NovaAlert.Common.Mvvm;
using NovaAlert.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace NovaAlert.Config.ViewModels
{
    public class GroupViewModel : ViewModelBase
    {
        public IClientApp App { get; private set; }
        public RelayCommand DeleteItemCommand { get; set; }
        public RelayCommand AddItemCommand { get; set; }
        public RelayCommand UpCommand { get; set; }
        public RelayCommand DownCommand { get; set; }
        public RelayCommand TopCommand { get; set; }
        public RelayCommand BottomCommand { get; set; }

        private ContactGroup _groupObj;
        public ContactGroup GroupObj 
        {
            get { return _groupObj; } 
            set
            {
                _groupObj = value;
                OnPropertyChanged("Name");
            }
        }

        bool _refreshSortTrigger;
        public bool RefreshSortTrigger
        {
            get { return _refreshSortTrigger; }
            set { _refreshSortTrigger = value; OnPropertyChanged("RefreshSortTrigger"); }
        }
        
        public string Name
        {
            get { return _groupObj.Name; }
            set { _groupObj.Name = value; OnPropertyChanged("Name"); }
        }


        GroupItemViewModel _itemSelected;
        public GroupItemViewModel ItemSelected { get { return _itemSelected; } set { _itemSelected = value; OnPropertyChanged("ItemSelected"); } }

        public ObservableCollection<GroupItemViewModel> Details { get; set; }
        
        public GroupViewModel(IClientApp app, ContactGroup g)
        {
            this.App = app;                     
            Details = new ObservableCollection<GroupItemViewModel>();

            LoadContactGroup(g);

            Details.CollectionChanged += Details_CollectionChanged;
            DeleteItemCommand = new RelayCommand(p => DeleteItem(), p => ItemSelected != null);
            AddItemCommand = new RelayCommand(p => AddItem());
            UpCommand = new RelayCommand(p => Up(), p => ItemSelected != null);
            DownCommand = new RelayCommand(p => Down(), p => ItemSelected != null);
            TopCommand = new RelayCommand(p => Top(), p => ItemSelected != null);
            BottomCommand = new RelayCommand(p => Bottom(), p => ItemSelected != null);
            DeleteList = new List<GroupItemViewModel>();
        }

        public void LoadContactGroup(ContactGroup g)
        {
            GroupObj = g;            
            if(this.Details == null) Details = new ObservableCollection<GroupItemViewModel>();
            else this.Details.Clear();

            if (g.Contacts != null)
            {
                foreach (var item in g.Contacts)
                {
                    Details.Add(new GroupItemViewModel(item));
                }
            }
            else
            {
                g.Contacts = new List<ContactInGroup>();
            }
        }

        private void AddItem()
        {
            var vm = ConfigServices.ShowContact(this.App, null, true);
            if (vm == null) return;
            var list = vm.GetSelectedItems();
            var dupList = new List<ContactItemViewModel>();

            foreach(var c in list)            
            {
                if(this.Details.Any(d => d.UnitName == c.UnitName && d.PhoneNumber == c.Phone.Number))
                {
                    dupList.Add(c);                    
                }
                else
                {
                    var obj = new ContactInGroup();
                    obj.Contact = c.Phone;
                    if (Details.Count > 0) obj.ListOrder = Details.Max(d => d.ListOrder) + 1;
                    else obj.ListOrder = 1;
                    GroupObj.Contacts.Add(obj);
                    var ivm = new GroupItemViewModel(obj);
                    ivm.PhoneNumber = c.Number;
                    ivm.UnitName = c.UnitName;
                    Details.Add(ivm);
                }
            }

            if(dupList.Count > 0)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Các đơn vị bị trùng, không thêm vào:");
                foreach (var item in dupList)
                    sb.AppendLine(string.Format("Đơn vị: {0}, số điện thoại: {1}", item.UnitName, item.Phone.Number));
                GetService<IMessageBoxService>().ShowError(sb.ToString());
            }
        }

        public List<GroupItemViewModel> DeleteList { get; set; }
        private void DeleteItem()
        {
            var list = GetSelectedItems();
            foreach(var item in list)
            {
                item.Item.IsDeleted = true;
                Details.Remove(item);
                DeleteList.Add(item);
            }

            // Do re-oder
            for (int i = 1; i <= Details.Count; i++)
            {
                var item = this.Details.Where(d => d.ListOrder == i).FirstOrDefault();
                if(item == null)
                {
                    var min = this.Details.Where(d => d.ListOrder > i).Select(d => d.ListOrder - i).Min();
                    item = this.Details.Where(d => d.ListOrder - i == min).FirstOrDefault();
                    if (item != null) item.ListOrder = i;
                }
            }
            
            RefreshSort();
        }

        void Details_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:                    
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:                    
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }

        private void Up()
        {
            if (ItemSelected.ListOrder == 1) return;
            ItemSelected.ListOrder -= 1;
            var obj = Details.Where(d => d != ItemSelected && d.ListOrder == ItemSelected.ListOrder).FirstOrDefault();
            if (obj != null)
            {
                obj.ListOrder += 1;
            }

            RefreshSort();
            
        }
        private void Down()
        {
            if (ItemSelected.ListOrder == Details.Max(d=>d.ListOrder)) return;
            ItemSelected.ListOrder += 1;
            var obj = Details.Where(d => d != ItemSelected && d.ListOrder == ItemSelected.ListOrder).FirstOrDefault();
            if (obj != null)
            {
                obj.ListOrder -= 1;
            }

            RefreshSort();
        }
        private void Top()
        {
            foreach (var item in Details.OrderBy(d=>d.ListOrder))
            {
                if (item.ListOrder < ItemSelected.ListOrder)
                {
                    item.ListOrder += 1;
                }
                else
                {
                    break;
                }
            }
            ItemSelected.ListOrder = 1;

            RefreshSort();
        }
        private void Bottom()
        {
            var max = Details.Max(d => d.ListOrder);
            foreach (var item in Details.OrderByDescending(d => d.ListOrder))
            {
                if (item.ListOrder > ItemSelected.ListOrder)
                {
                    item.ListOrder -= 1;
                }
                else
                {
                    break;
                }
            }
            ItemSelected.ListOrder = max;

            RefreshSort();
        }

        void RefreshSort()
        {
            this.RefreshSortTrigger = false;
            this.RefreshSortTrigger = true;
        }

        public void RefeshDetails()
        {
            if (this.Details == null) return;
            var items = this.Details.ToList();
            this.Details.Clear();
            foreach (var item in items)
            {
                this.Details.Add(item);
            }
        }

        List<GroupItemViewModel> GetSelectedItems()
        {
            var list = this.Details.Where(d => d.Selected).ToList();
            if (list.Count == 0 && this.ItemSelected != null)
            {
                list.Add(this.ItemSelected);
            }
            return list;
        }
    }
}
