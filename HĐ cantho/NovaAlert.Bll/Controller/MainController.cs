using NovaAlert.Entities;
using NovaAlert.Entities.ViewModel;
using System;
using System.Linq;

namespace NovaAlert.Bll.Controller
{
    /// <summary>
    /// The very first controller.
    /// </summary>
    public abstract class MainController: ComplexControllerBase
    {
        object _syncObj = new object();
        public bool CanReceiveCall { get; private set; }
        public MainController(ClientAppViewModel app, bool canReceiveCall): base(app)
        {
            this.CanReceiveCall = canReceiveCall;            
        }

        public override void OnResourceChanged(ResourceChangedEventArgs e)
        {
            ProcessMessageInController(e);
            if (e.Handled) return;

            switch (e.ResourceType)
            {
                case eResourceType.Channel:                
                    var c = this.ClientApp.GetChannels(u => u.Id == e.Id).FirstOrDefault();
                    if (c != null)
                    {
                        c.SelectedPanelId = e.PanelId;
                        e.Handled = true;                      
                    }
                    break;

                case eResourceType.UnitPhone:
                    if(this.ClientApp.Units != null)
                    {
                        var up = this.ClientApp.GetUnits(u => u.Id == e.Id).FirstOrDefault();
                        if (up != null)
                        {
                            up.SelectedPanelId = e.PanelId;
                            e.Handled = true;                        
                        }
                    }                    
                    break;

                case eResourceType.PO:
                    break;

                case eResourceType.Speaker:
                    break;

                case eResourceType.Micro:
                    break;

                case eResourceType.SoundFile:
                    break;

                case eResourceType.Modem:
                    if(this.ClientApp.TSLController != null)
                    {
                        this.ClientApp.TSLController.Modem.SelectedPanelId = e.PanelId;
                        e.Handled = true;
                    }
                    break;
            }
        }

        public override void OnPOStatusChanged(POStatusChangedEventArgs e)
        {
            if (this.ClientApp.Channels.PO.Id == e.Address)
            {
                ProcessMessageInController(e);
                if (e.Handled) return;

                this.ClientApp.Channels.PO.ApplyChanges(e);

                if (this.ClientApp.Channels.PO.Status == ePOStatus.OffHook && CanReceiveCall && !this.HasConnection)
                {
                    var ringChannels = this.ClientApp.GetChannels(ch => ch.LineStatus == eLineStatus.Ring && IsFreeChannel(ch)).ToList();
                    if (ringChannels.Count == 1)
                    {
                        var ringUnit = this.ClientApp.Units.Items.FirstOrDefault(u => u.Status == ePhoneStatus.Ring && IsFreeUnit(u));
                        if (ringUnit != null)
                        {
                            ClientApp.Service.Request(eResourceType.UnitPhone, ringUnit.Id);
                        }

                        AddController(new InCallController(this.ClientApp, ringChannels[0], ringUnit));
                        return;
                    }
                }
            }
        }

        public override void OnChannelStatusChanged(ChannelStatusChangedEventArgs e)
        {
            ProcessMessageInController(e);
            if (e.Handled) return;

            var channel = this.ClientApp.GetChannels(p => p.Id == e.Address).FirstOrDefault();
            if (channel != null)
            {
                // disable unused channel
                if ((this.ClientApp.WorkingMode == eWorkingMode.Alert && channel.HostPhone.AlertEnabled == false) ||
                    (this.ClientApp.WorkingMode == eWorkingMode.MultiDest && channel.HostPhone.MultiDestEnabled == false))
                {
                    channel.LineStatus = eLineStatus.Disconnect;
                    channel.Status = ePhoneStatus.Disconnect;
                }
                else
                {

                    var applyTone = channel.SelectedPanelId == ClientApp.ClientId;
                    if(applyTone == false)
                    {
                        applyTone = this.GetController(channel) != null;
                    }

                    channel.ApplyChanges(e, applyTone);

                    if (!string.IsNullOrEmpty(e.CallerId) && channel.LineStatus == eLineStatus.Ring)
                    {
                        var u = this.ClientApp.Units.Search(e.CallerId, channel.AreaCode);
                        if (u != null && u.SelectedPanelId == null)
                        {
                            u.Status = ePhoneStatus.Ring;
                        }
                    }
                }
                
            }
        }

        public override void UnitClicked(object sender, PhoneEventArgs<UnitPhoneViewModel> e)
        {
            ProcessMessageInController(e);
            if (e.Handled) return;
            
            if (this.HasConference == true)
            {
                e.Handled = true;
                return;
            }

            var item = e.Item;

            switch (e.Item.Status)
            {
                case ePhoneStatus.Free:
                    this.ClientApp.Service.Request(item.ResourceType, item.Id);
                    break;

                case ePhoneStatus.Occupied:
                    break;

                case ePhoneStatus.Selected:
                    ReleaseResource(item);
                    break;

                case ePhoneStatus.Dial:
                    break;
                case ePhoneStatus.Speaking:
                    break;
                case ePhoneStatus.Listening:
                    break;
                case ePhoneStatus.Holding:
                    break;

                case ePhoneStatus.Ring:
                    if (this.ClientApp.Channels.PO.Status == ePOStatus.OffHook && CanReceiveCall)
                    {
                        var list = this.Controllers.OfType<ICallController>().Where(c => c.IsHolding == false);
                        if(!list.Any())
                        {
                            var channel = this.ClientApp.GetChannels(c => c.Status == ePhoneStatus.Ring &&
                                c.CallerId != null && c.CallerId.Contains(e.Item.GetFullNumber())).FirstOrDefault();
                            if (channel != null)
                            {
                                AddController(new InCallController(this.ClientApp, channel, e.Item));
                            }
                        }                        
                        return;
                    }
                    break;
            }
        }

        protected virtual void ReleaseResource(PhoneViewModel item)
        {
            this.ClientApp.Service.Release(item.ResourceType, item.Id);
        }

        public virtual bool? HasConference
        {
            get
            {
                var ctrl = this.Controllers.OfType<ConferenceController>().FirstOrDefault();
               return !ctrl?.IsHolding ?? null;                
            }
        }

        public virtual bool IsRecording
        {
            get
            {
                var ctrl = this.ActiveController ?? this.Controllers.FirstOrDefault();
                return ctrl != null && ctrl is ICallController && ((ICallController)ctrl).IsRecording;
            }
        }

        public virtual bool CanConnect()
        {
            return this.HasConference.HasValue ||
                this.ClientApp.GetSelectedUnits(u => IsFreeUnit(u)).Any();
        }

        public virtual bool HasConnection
        {
            get
            {
                return this.Controllers.OfType<ICallController>().Any();
            }
        }

        public virtual bool IsHolding
        {
            get
            {
                return this.Controllers.OfType<ICallController>().All(c => c.IsHolding);
            }
        }

        protected override void Controller_OnFinalised(object sender, EventArgs e)
        {
            base.Controller_OnFinalised(sender, e);
            OnPropertyChanged("HasConference");
            OnPropertyChanged("IsRecording");
        }
    }
}
