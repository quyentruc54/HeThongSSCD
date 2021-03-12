using NovaAlert.Bll.Controller;
using NovaAlert.Common.Mvvm;
using System.Linq;

namespace NovaAlert.Bll
{
    public class TSL_AlertOptionViewModel: ViewModelBase
    {
        public RelayCommand ClearPrepareCommand { get; set; }
        public RelayCommand ClearResultCommand { get; set; }

        public TSL_AlertController Controller { get; set; }

        public TSL_AlertOptionViewModel(TSL_AlertController ctrl)
        {
            this.Controller = ctrl;
            this.ClearPrepareCommand = new RelayCommand(p => this.Controller.ClearStatus(Entities.eTslStatusType.Prepare), p => CanClearPrepare());
            this.ClearResultCommand = new RelayCommand(p => this.Controller.ClearStatus(Entities.eTslStatusType.Result), p => CanClearResult());
        }

        private bool CanClearResult()
        {
            return !this.Controller.IsBusy && this.Controller.ClientApp.Units.GetSelectedItems()
                .OfType<TSL_ALertUnitPhoneViewModel>()
                .Any(u => u.ResultStatus != Entities.eTslStatus.None);
        }

        private bool CanClearPrepare()
        {
            return !this.Controller.IsBusy && this.Controller.Units.GetSelectedItems()
                .OfType<TSL_ALertUnitPhoneViewModel>()
                .Any(u => u.PrepareStatus != Entities.eTslStatus.None);
        }
    }
}
