using NovaAlert.Common.Mvvm;

namespace NovaAlert.Bll
{
    public class MultiDestOptionViewModel: ViewModelBase
    {
        int _volumn;
        public int Volumn
        {
            get { return _volumn; }
            set { _volumn = value; OnPropertyChanged("Volumn"); }
        }

        public RelayCommand ContactCommand { get; set; }
        public RelayCommand IncreaseVolumnCommand { get; set; }
        public RelayCommand DescreaseVolumnCommand { get; set; }        
    }
}
