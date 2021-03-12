using NovaAlert.Common.Setting;

namespace NovaAlert.Entities.ViewModel
{
    public class ModemViewModel: HostPhoneViewModel
    {
        public ModemViewModel(int id)
            : base(new HostPhone(id, "Modem"))
        {            
        }

        public override eResourceType ResourceType
        {
            get
            {
                return eResourceType.Modem;
            }
        }

        public override bool CanMakeCall(string areaCode, string excludeAreaCodes)
        {
            return false;
        }

        protected override void OnSelectedPanelChanged()
        {
            if (this.SelectedPanelId.HasValue)
            {
                if (this.SelectedPanelId.Value == ClientSetting.Instance.ClientId)
                {
                    if (this.Status == ePhoneStatus.Free)
                    {
                        this.Status = ePhoneStatus.Selected;
                    }
                }
                else
                {
                    this.Status = ePhoneStatus.Occupied;
                }
            }
            else
            {
                this.Status = ePhoneStatus.Free;
            }
        }
    }
}
