using NovaAlert.Bll.Controller;
using NovaAlert.Common.Mvvm;

namespace NovaAlert.Bll
{
    public class TSL_AlertControlViewModel: ViewModelBase
    {
        public TSL_AlertController Controller { get; private set; }
        public RelayCommand PrepareCommand { get; private set; }
        public RelayCommand ReceiveResultCommand { get; private set; }
        public RelayCommand ShowResultCommand { get; private set; }

        public TSL_AlertControlViewModel(TSL_AlertController ctrl)
        {
            this.Controller = ctrl;
            this.PrepareCommand = new RelayCommand(p => this.Controller.OnDoTask(Entities.eTslStatusType.Prepare), p => this.Controller.CanPrepare());
            this.ReceiveResultCommand = new RelayCommand(p => this.Controller.OnDoTask(Entities.eTslStatusType.Result), p => this.Controller.CanReceiveResult());
            this.ShowResultCommand = new RelayCommand(p => this.Controller.OnShowResult(), p => this.Controller.CanShowResult());
        }
    }
}
