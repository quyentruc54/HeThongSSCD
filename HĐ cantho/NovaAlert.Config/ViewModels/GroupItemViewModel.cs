using NovaAlert.Common.Mvvm;
using NovaAlert.Entities;

namespace NovaAlert.Config.ViewModels
{
    public class GroupItemViewModel : ViewModelBase
    {
        public ContactInGroup Item { get; set; }

        public string UnitName {get; set;} 

        public string PhoneNumber { get; set; }

        bool _selected;
        public bool Selected
        {
            get { return _selected; }
            set { _selected = value; OnPropertyChanged("Selected"); }
        }

        public int ListOrder { get { return Item.ListOrder; } set { Item.ListOrder = value; OnPropertyChanged("ListOrder"); } }

        public GroupItemViewModel()
        {
            Item = new ContactInGroup();
            Init();
        }
        public GroupItemViewModel(ContactInGroup gu)
        {
            Item = gu;
            Init();
        }

        private void Init()
        {
            if (Item.Contact.Number != null)
            {
                PhoneNumber = Item.Contact.Number;
                UnitName = Item.Contact.UnitName;
            }
        }

        private bool PhoneNumberFilter(object obj)
        {
            return false;
        }
    }
}
