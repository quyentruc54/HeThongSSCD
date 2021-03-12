using NovaAlert.Common.Mvvm;
using NovaAlert.Bll.Controller;
using NovaAlert.Entities;

namespace NovaAlert.Bll
{
    public class SoundVolumnViewModel: ViewModelBase
    {
        IAdjustVolumn _obj;
        public IAdjustVolumn Obj
        {
            get { return _obj; }
            set { _obj = value; OnPropertyChanged("Obj"); OnPropertyChanged("Volumn"); }
        }

        public byte Volumn
        {
            get
            {
                if (this.Obj != null)
                {
                    return (byte)this.Obj.Volumn;
                }
                return (byte)eVolumn.Volumn_0;
            }
            set
            {
                if (this.Obj != null)
                {
                    this.Obj.Volumn = (eVolumn)value;
                }
                OnPropertyChanged("Volumn");
            }
        }

        int _minimum;
        public int Minimum
        {
            get { return _minimum; }
            set { _minimum = value; OnPropertyChanged("Minimum"); }
        }

        int _maximum;
        public int Maximum
        {
            get { return _maximum; }
            set { _maximum = value; OnPropertyChanged("Maximum"); }
        }

        bool _showPopup;
        public bool ShowPopup
        {
            get { return _showPopup; }
            set { _showPopup = value; OnPropertyChanged("ShowPopup"); }
        }

        public RelayCommand AdjustCommand { get; private set; }

        public SoundVolumnViewModel()
        {
            _obj = null;
            _minimum = 0;
            _maximum = 9;
            _showPopup = false;

            this.AdjustCommand = new RelayCommand(p => this.ShowPopup = true, p => this.Obj != null);
        }
    }
}
