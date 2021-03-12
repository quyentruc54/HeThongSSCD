using System.Linq;
using NovaAlert.Entities;
using NovaAlert.Entities.ViewModel;

namespace NovaAlert.Bll
{
    public class ChannelListViewModel: PhoneListViewModel<HostPhoneViewModel>
    { 
        public ChannelListViewModel(ClientAppViewModel app):base(app)
        {
            var address = this.App.Service.GetPOAddress(this.App.POId);
            this.PO = new POViewModel(new PO() { Id = this.App.POId, Address = address });
        }

        protected override void GetItemsFromServer()
        {            
            var channels = this.App.Service.GetAllChannels().Where(it => (this.App.IsMultiDestMode && it.MultiDestEnabled) ||
                (this.App.IsAlertMode && it.AlertEnabled) || (this.App.IsCCPKAlertMode && it.CCPKEnabled));
            
            foreach (var item in channels)
            {
                var vm = new HostPhoneViewModel(item);
                vm.LineStatus = eLineStatus.Good;                
                this.Items.Add(vm);                
            }
        }
    }    
}
