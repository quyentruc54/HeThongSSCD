using System;
using System.Linq;
using NovaAlert.Entities;
using NovaAlert.Entities.ViewModel;
using System.ComponentModel;

namespace NovaAlert.Bll.Controller
{
    /// <summary>
    /// Controller cho chế độ gọi nhóm
    /// </summary>
    public class MultiDestController: MainController
    {
        #region Ctor
        public MultiDestController(ClientAppViewModel app): base(app, true)
        {
        } 
        #endregion

        #region Override
        public override void OnResourceChanged(ResourceChangedEventArgs e)
        {
            base.OnResourceChanged(e);
            if (!e.Handled)
            {

                CheckOutgoingCall();
            }
        }

        public override void OnPOStatusChanged(POStatusChangedEventArgs e)
        {
            if (this.ClientApp.Channels.PO.Id == e.Address)
            {
                base.OnPOStatusChanged(e);
                if (!e.Handled)
                {
                    CheckOutgoingCall();
                }                
            }
        }

        public override void OnChannelStatusChanged(ChannelStatusChangedEventArgs e)
        {
            base.OnChannelStatusChanged(e);
            if (!e.Handled)
            {
                CheckOutgoingCall();
            }
        }

        public override void ChannelClicked(object sender, PhoneEventArgs<HostPhoneViewModel> e)
        {
            ProcessMessageInController(e);
            if (e.Handled)
            {
                return;
            }

            var item = e.Item;

            if(this.HasConference == true)
            {
                e.Handled = true;
                return;
            }

            switch (e.Item.Status)
            {
                case ePhoneStatus.Selected:
                    this.ClientApp.Service.Release(item.ResourceType, item.Id);
                    break;

                case ePhoneStatus.Free:
                    ReleaseAllFreeChannels();
                    
                    // Hot line
                    if(item.HostPhone.HotUnitId.HasValue)
                    {
                        ReleaseAllFreeUnits();
                        var hu = this.ClientApp.Units.Items.FirstOrDefault(u => u.Id == item.HostPhone.HotUnitId.Value);
                        if(hu == null)
                        {
                            this.ClientApp.Units.SetSelectContact(item.HostPhone.HotUnitId.Value);                            
                        }
                        else
                        {
                            this.ClientApp.Service.Request(eResourceType.UnitPhone, item.HostPhone.HotUnitId.Value);
                        }
                    }
                    this.ClientApp.Service.Request(item.ResourceType, item.Id);
                    break;

                case ePhoneStatus.Ring:
                    if (this.ClientApp.Channels.PO.Status == ePOStatus.OffHook)
                    {
                        var list = this.Controllers.OfType<ICallController>().Where(c => c.IsHolding == false).ToList();
                        if (list.Count == 0)
                        {
                            AddController(new InCallController(this.ClientApp, e.Item));
                        }
                        return;
                    }
                    break;
            }
            
            CheckOutgoingCall();
        }

        public override void UnitClicked(object sender, PhoneEventArgs<UnitPhoneViewModel> e)
        {
            base.UnitClicked(sender, e);
            if (e.Handled)
            {
                return;
            }

            // Hot line
            var hotChannel = this.ClientApp.GetChannels(c => c.HostPhone.HotUnitId.HasValue && c.HostPhone.HotUnitId.Value == e.Item.Id).FirstOrDefault();
            if(hotChannel != null && IsFreeChannel(hotChannel))
            {
                var list = this.Controllers.OfType<CallController>();
                foreach (var ctrl in list)
                {
                    ctrl.Finalise();
                }

                this.ClientApp.Service.Request(eResourceType.Channel, hotChannel.Id);
            }

            CheckOutgoingCall();
        }

        protected override void Controllers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.Controllers_CollectionChanged(sender, e);
            OnPropertyChanged("HasConference");
            OnPropertyChanged("IsRecording");
        }

        protected override void OnControllerDeactivate(IController oldController)
        {
            var c = oldController as ICallController;            
            if (c != null && c.CallStatus == eCallStatus.Connected)
            {
                var call = c as InCallController;
                if (call != null && call.FinalisedDate == null)
                {
                    call.DisconnectPOConnectionWhenFinalise = false;
                    c.Finalise();
                }
            }
        }

        protected override void Controller_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "IsHolding")
            {
                var c = sender as ICallController;
                if (c != null && !c.IsHolding)
                {
                    ReleaseAllFreeUnits();
                }
            }

            base.Controller_PropertyChanged(sender, e);
        }

        public override bool CanConnect()
        {
            if (base.CanConnect())
            {
                return true;
            }

            var outCalls = this.Controllers.OfType<CallController>();
            var inCalls = this.Controllers.OfType<InCallController>().Where(c => outCalls.Contains(c) == false);
            return inCalls.Any() || outCalls.Any(c => !string.IsNullOrEmpty(c.GetDialingNumber()));
        }

        protected override void Controller_OnFinalised(object sender, EventArgs e)
        {
            base.Controller_OnFinalised(sender, e);
            System.Windows.Application.Current.Dispatcher.BeginInvoke(
                    new Action(this.ClientApp.Units.ClearTempUnits),
                    System.Windows.Threading.DispatcherPriority.Normal);
        }

        readonly
        #endregion

        #region Helpers        
        private object _syncObj = new object();
        void CheckOutgoingCall()
        {
            lock (_syncObj)
            {
                var selectedChannel = this.ClientApp.GetChannels(ch => ch.SelectedPanelId == this.ClientApp.ClientId && IsFreeChannel(ch))
                    .OrderByDescending(ch => ch.LastClicked).FirstOrDefault();              

                if (selectedChannel != null)
                {
                    var selectedUnits = this.ClientApp.GetSelectedUnits(u => IsFreeUnit(u));
                    var lastUnit = selectedUnits.OrderByDescending(u => u.LastClicked).FirstOrDefault();
                    if (lastUnit == null && this.ClientApp.Channels.PO.Status == ePOStatus.OnHook)
                    {
                        return;
                    }

                    if(lastUnit != null)
                    {
                        foreach (var u in selectedUnits)
                        {
                            if (u.Id != lastUnit.Id)
                            {
                                this.ClientApp.Service.Release(eResourceType.UnitPhone, u.Id);
                            }
                        }

                        if (selectedChannel.CanMakeCall(lastUnit.AreaCode, this.ClientApp.RestrictedAreaCodes))
                        {
                            AddController(new CallController(this.ClientApp, selectedChannel, lastUnit));
                        }
                        else
                        {
                            this.ClientApp.ShowError(string.Format("Kênh được chọn không thể gọi đến đơn vị {0}", lastUnit.Name));
                            this.ClientApp.Service.Release(eResourceType.Channel, selectedChannel.Id);
                        }
                    }
                    else
                    {
                        AddController(new CallController(this.ClientApp, selectedChannel, null));
                    }
                }
            }
        }

        public void OnCallGroup()
        {
            this.ClientApp.AddLog("Gọi nhóm");
            lock (this.Controllers)
            {
                var ctrl = this.Controllers.OfType<ConferenceController>().FirstOrDefault();
                if (ctrl != null)
                {
                    if (ctrl.IsHolding)
                    {
                        ctrl.Resume();

                        // Tim cac cuoc goi don, dua vao hoi nghi
                        var calls = this.Controllers.OfType<InCallController>().ToList();
                        if (calls.Count > 0)
                        {
                            foreach (var c in calls)
                            {
                                var conStatus = eConferenceMemberStatus.Listening;
                                if (ctrl.GetSpeakerCount() < ConferenceControllerBase<ConferenceMember>.MaxSpeaker)
                                {
                                    conStatus = eConferenceMemberStatus.Speaking;
                                }
                                ctrl.Controllers.Add(new ConferenceMember(ctrl, c, conStatus, this.ClientApp.ClientId));
                            }
                        }
                    }
                    else
                    {
                        ctrl.Finalise();
                    }
                }
                else
                {
                    var units = this.ClientApp.GetSelectedUnits(u => IsFreeUnit(u));
                    if (units.Any())
                    {
                        ctrl = new ConferenceController(this.ClientApp, units);
                        this.Controllers.Add(ctrl);
                        this.ActiveController = ctrl;
                    }

                    var calls = this.Controllers.OfType<InCallController>().ToList();

                    if (calls.Any())
                    {
                        if (ctrl == null) ctrl = new ConferenceController(this.ClientApp, null);

                        foreach (var c in calls)
                        {                            
                            ctrl.Controllers.Add(new ConferenceMember(ctrl, c, eConferenceMemberStatus.Waiting, this.ClientApp.ClientId));
                        }
                    }

                    if(ctrl != null)
                    {
                        this.Controllers.Add(ctrl);
                        this.ActiveController = ctrl;
                        ctrl.InitMemberVolumn();
                    }                    
                }

                if (ctrl != null && ctrl.FinalisedDate == null)
                {
                    this.ActiveController = ctrl;
                }

                OnPropertyChanged("HasConference");
                OnPropertyChanged("IsRecording");
                ReleaseAllFreeUnits();
            }
        }

        public void OnPutOnHold()
        {
            var controller = this.ActiveController as ICallController;
            if(controller == null)
            {
                return;                
            }

            if (!controller.IsHolding)
            {
                controller.PutOnHold();
            }

            if (controller is ConferenceController)
            {
                OnPropertyChanged("HasConference");
            }
        }

        internal void OnRecord()
        {
            this.ClientApp.AddLog("Ghi âm");
            var controller = this.ActiveController as InCallController;
            if (controller != null && !controller.AutoRecording)
            {
                if(controller.IsRecording)
                {
                    controller.StopRecord();
                }
                else
                {
                    controller.StartRecord();
                }
            }
            else
            {
                if(controller == null)
                {
                    this.ClientApp.ShowError("Vào Cài đặt Kênh thông tin để chọn chế độ ghi âm tự động cho từng kênh");
                }
                else 
                {
                    if (controller.AutoRecording)
                    {
                        this.ClientApp.ShowError("Tương tác không có tác dụng. Kết thúc cuộc gọi và vào Cài đặt Kênh thông tin để bỏ chọn chế độ ghi tâm tự động cho kênh");
                    }
                }
            }
        }
        #endregion
    }
}
