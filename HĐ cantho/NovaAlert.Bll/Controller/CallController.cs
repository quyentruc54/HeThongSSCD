using System.Linq;
using System.Text;
using NovaAlert.Entities;
using NovaAlert.Entities.ViewModel;

namespace NovaAlert.Bll.Controller
{
    /// <summary>
    /// Handle outgoing call
    /// </summary>
    public class CallController: InCallController
    {
        //string _number = null;

        public CallController(ClientAppViewModel app, HostPhoneViewModel channel, UnitPhoneViewModel unit = null)
            :base(app, channel,unit)
        {
            if (this.Unit != null) Dial();
            else this.CallStatus = eCallStatus.Connected;

            SendConnection(this.Connection);
            this.Channel.PropertyChanged += Channel_PropertyChanged;
            System.Diagnostics.Debug.WriteLine("Create " + this.ToString());
        }

        public override void OnResourceChanged(ResourceChangedEventArgs e)
        {
            if (this.Unit == null && e.ResourceType == eResourceType.UnitPhone && e.PanelId == this.ClientApp.ClientId)
            {
                var unit = this.ClientApp.GetUnits(u => u.Id == e.Id).FirstOrDefault();
                if (unit != null)
                {
                    unit.SelectedPanelId = e.PanelId;
                    this.Unit = unit;
                    Dial();
                    e.Handled = true;
                    return;
                }
            }

            base.OnResourceChanged(e);
        }

        void Channel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (this.Unit == null && e.PropertyName == "Tone")
            {
                // tim unit dua tren so dt
                var unit = this.ClientApp.Units.Search(this.Channel.Tone, this.Channel.AreaCode);                
                if (unit != null && unit.SelectedPanelId == null)
                {
                    this.ClientApp.Service.Request(eResourceType.UnitPhone, unit.Id);
                    if(unit != null)
                    {
                        this.Unit = unit;
                        if (this.CallLogDetail != null)
                        {
                            this.CallLogDetail.PhoneNumber = unit.GetFullNumber();
                        }
                    }
                    
                    SaveLog();
                }     
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("CallController Channel: {0}", this.Channel.Id);
            if (this.Unit != null)
            {
                sb.AppendFormat(" Unit: {0}", this.Unit.Id);
            }
            if (!string.IsNullOrWhiteSpace(this.GetDialingNumber()))
            {
                sb.AppendFormat(" Phone number: {0}", this.GetDialingNumber());
            }
            return sb.ToString();
        }

        protected override void SaveLog()
        {
            if (this.Unit != null || !string.IsNullOrEmpty(this.Channel.Tone))
            {
                if(this.Unit == null && this.CallLogDetail != null)
                {
                    this.CallLogDetail.PhoneNumber = this.Channel.Tone;
                }

                base.SaveLog();
            }
        }
        protected override void CreateCallLog()
        {
            base.CreateCallLog();
            this.CallLog.CallType = eCallType.Single;

            if (this.Unit != null)
            {
                this.CallLogDetail.PhoneNumber = this.Unit.GetFullNumber();
                this.CallLogDetail.UnitId = this.Unit.Id;
            }
            else this.CallLogDetail.PhoneNumber = this.Channel.Tone;            
        }

        public override string GetDialingNumber()
        {
            if (this.Unit != null)
            {
                if (this.Channel.AreaCode == this.Unit.AreaCode) return this.Unit.Number;
                else return this.Unit.GetFullNumber();
            }
            else return this.Channel.Tone;            
        }

        public override void UnitClicked(object sender, PhoneEventArgs<UnitPhoneViewModel> e)
        {
            base.UnitClicked(sender, e);

            if (this.Unit == null && e.Item != null)
            {
                var unit = e.Item;
                var hc = this.ClientApp.GetChannels(c => c.HostPhone.HotUnitId == unit.Id).FirstOrDefault();
                if(unit.Status == ePhoneStatus.Free && (hc == null || hc == this.Channel) 
                    && this.Channel.CanMakeCall(unit.AreaCode, this.ClientApp.RestrictedAreaCodes))
                {                    
                    this.Unit = unit;
                    this.ClientApp.Service.Request(unit.ResourceType, unit.Id);
                    Dial();
                    
                }
                else
                {
                    this.ClientApp.ShowError(string.Format("Kênh được chọn không thể gọi đến đơn vị {0}", unit.Name));
                }
                e.Handled = true;
            }
        }

        public override bool CanHold()
        {
            return base.CanHold() && !string.IsNullOrEmpty(GetDialingNumber());
        }
    }
}
