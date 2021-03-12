using NovaAlert.Common.Mvvm;

namespace NovaAlert.Entities.ViewModel
{
    public class POViewModel: ViewModelBase
    {
        public PO PO { get; private set; }

        public int Id { get { return this.PO.Id; } }

        ePOStatus _status;
        public ePOStatus Status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged("Status");
                }
            }
        }

        string _tone;
        public string Tone
        {
            get { return _tone; }
            set { _tone = value; OnPropertyChanged("Tone"); }
        }

        public string Name
        {
            get { return string.Format("PO {0}", this.Id); }
        }
        public POViewModel(PO po)
        {
            this.Status = ePOStatus.OnHook;
            this.PO = po;
        }

        public void ApplyChanges(POStatusChangedEventArgs e)
        {
            if (e.Status.HasValue)
            {
                this.Status = e.Status.Value;
            }

            if (e.Address == this.Id && !string.IsNullOrEmpty(e.Tone))
            {
                this.Tone = e.Tone;
            }
        }
    }
}
