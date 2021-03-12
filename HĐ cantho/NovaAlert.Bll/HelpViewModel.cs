using NovaAlert.Common.Mvvm;
using NovaAlert.Entities;

namespace NovaAlert.Bll
{
    public class HelpViewModel: ViewModelBase
    {
        HelpItem _helpItem;
        public HelpItem HelpItem
        {
            get { return _helpItem; }
            set { _helpItem = value; OnPropertyChanged("HelpItem"); }
        }

        public HelpViewModel()
        {
            this.HelpItem = new HelpItem(false);
        }
    }

    public class InfoViewModel : ViewModelBase
    {
    }
}
