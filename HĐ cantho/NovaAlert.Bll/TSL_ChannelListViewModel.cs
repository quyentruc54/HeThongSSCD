using NovaAlert.Entities.ViewModel;

namespace NovaAlert.Bll
{
    public class TSL_ChannelListViewModel : PhoneListViewModel<HostPhoneViewModel>
    {
        public TSL_ChannelListViewModel(ClientAppViewModel app):base(app)
        {
        }

        protected override void GetItemsFromServer()
        {            
            this.Items.Add(new ModemViewModel(0));
        }
    }
}
