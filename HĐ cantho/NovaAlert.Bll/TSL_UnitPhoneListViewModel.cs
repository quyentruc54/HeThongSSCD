namespace NovaAlert.Bll
{
    public class TSL_UnitPhoneListViewModel : UnitPhoneListViewModel
    {
        public TSL_UnitPhoneListViewModel(ClientAppViewModel app):base(app)
        {
        }

        protected override void GetItemsFromServer()
        {
            var list = this.App.Service.GetTSLUnitPhones();
            foreach (var item in list)
            {
                var vm = new TSL_ALertUnitPhoneViewModel(item);
                this.Items.Add(vm);
                UpdateUnitStatus(vm);
            }
        }

        private void UpdateUnitStatus(TSL_ALertUnitPhoneViewModel vm)
        {
            var tslStatus = this.App.Service.GetLatestTslStatus(vm.PhoneNumberId, Entities.eTslStatusType.Prepare);
            if (tslStatus != null && tslStatus.DeletedDate == null)
            {
                vm.PrepareStatus = tslStatus.Status;
            }
            else
            {
                vm.PrepareStatus = Entities.eTslStatus.None;
            }

            tslStatus = this.App.Service.GetLatestTslStatus(vm.PhoneNumberId, Entities.eTslStatusType.Result);
            if (tslStatus != null && tslStatus.DeletedDate == null)
            {
                vm.ResultStatus = tslStatus.Status;
            }
            else
            {
                vm.ResultStatus = Entities.eTslStatus.None;
            }
        }
    }
}
