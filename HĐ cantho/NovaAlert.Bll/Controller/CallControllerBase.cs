using System;
using System.Collections.Generic;
using System.Threading;
using NovaAlert.Common;
using NovaAlert.Entities;
using NovaAlert.Entities.ViewModel;

namespace NovaAlert.Bll.Controller
{
    /// <summary>
    /// Một đầu cuối trong gọi nhóm hoặc CTT
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CallControllerBase<T> : ControllerBase, ICallController, IAdjustVolumn where T : ISwitchConnection
    {
        #region Members & Properties
        public HostPhoneViewModel Channel { get; protected set; }
        UnitPhoneViewModel _unit;
        public UnitPhoneViewModel Unit
        {
            get { return _unit; }
            protected set
            {
                if(_unit != null)
                {
                    this.ClientApp.Service.Release(eResourceType.UnitPhone, _unit.Id);
                }

                _unit = value;
                if (_unit != null)
                {
                    _unit.Status = ePhoneStatus.Speaking;
                }
            }
        }
        
        public SwitchConnection HoldConnection { get; private set; }
        
        protected ManualResetEvent _dialCompletedEvent;        

        T _connection;
        public T Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = CreateConnection();
                }
                return _connection;
            }
        }

        public bool KeepSelection { get; set; }

        public bool DisconnectPOConnectionWhenFinalise { get; set; }

        private eCallStatus _callStatus;
        public eCallStatus CallStatus
        {
            get { return _callStatus; }
            protected set 
            { 
                _callStatus = value; 
                OnPropertyChanged("CallStatus");
                OnPropertyChanged("IsHolding"); 
            }
        }

        public virtual eVolumn Volumn { get; set; }

        public DateTime StartTime { get; private set; }

        public virtual bool IsRecording { get { return false; } }
        public virtual bool AutoRecording { get { return false; } }
        public virtual bool IsHolding
        {
            get { return this.CallStatus == eCallStatus.OnHold; }
        }

        public virtual string GetDialingNumber()
        {
            if (this.Unit != null)
            {
                if (this.Channel.AreaCode == this.Unit.AreaCode)
                {
                    return this.Unit.Number;
                }

                return this.Unit.GetFullNumber();
            }
            return string.Empty;
        }
        #endregion

        #region Ctor
        public CallControllerBase(ClientAppViewModel app, HostPhoneViewModel channel, UnitPhoneViewModel unit = null)
            : base(app)
        {
            this.StartTime = app.Service.GetDateTime();
            this.KeepSelection = false;
            this.DisconnectPOConnectionWhenFinalise = true;
            _connection = default(T);
            this.Channel = channel;
            this.Unit = unit;
            _dialCompletedEvent = new ManualResetEvent(true);            
            this.CallStatus = eCallStatus.Created;

            this.PropertyChanged += OnPropertyChanged;            
        }
        #endregion

        #region Helpers
        public void Dial()
        {
            AsyncHelper.RunAsync(DialThread);
        }

        void DialThread()
        {
            if (this.Channel.SelectedPanelId == null)
            {
                this.ClientApp.Service.Request(eResourceType.Channel, this.Channel.Id);
            }

            _dialCompletedEvent.Reset();
            this.ClientApp.Service.SendDial(this.Channel.HostPhone.Address, this.GetDialingNumber());
            this.CallStatus = eCallStatus.Dialed;
        }

        public virtual void PutOnHold()
        {
            if (this.IsRecording)
            {
                this.ClientApp.Service.PauseRecord((byte)this.Channel.Id, 0);
            }

            OnPropertyChanged("IsRecording");
            this.CallStatus = eCallStatus.OnHold;
            AsyncHelper.RunAsync(HoldThread);
        }

        void HoldThread()
        {
            // Wait for dial complete
            _dialCompletedEvent.WaitOne();

            // Disconnect current PO connection
            this.Connection.IsConnected = false;
            SendConnection(this.Connection);

            // Hold Connection
            if (this.HoldConnection == null)
            {
                var scr = new SwitchConnectionEnd(OtherDevice.WaitingMusic) { IsConnected = true };
                var dest = new SwitchConnectionEnd(this.Channel.HostPhone) { IsConnected = false };
                this.HoldConnection = new SwitchConnection(this.ClientApp.ClientId, scr, dest);
            }

            this.HoldConnection.Source.IsConnected = true;
            this.HoldConnection.Dest.IsConnected = false;
            SendConnection(this.HoldConnection);
        }

        public virtual void Resume()
        {
            if(this.HoldConnection != null)
            {
                this.HoldConnection.IsConnected = false;
                SendConnection(this.HoldConnection);
            }            
            
            this.Connection.IsConnected = true;
            SendConnection(this.Connection);            
            this.CallStatus = eCallStatus.Connected;
            if (this.IsRecording)
            {
                this.ClientApp.Service.ResumeRecord((byte)this.Channel.Id);
            }
            OnPropertyChanged("IsRecording");
        }

        public override IEnumerable<ISwitchConnection> GetConnections()
        {
            yield return this.Connection;
            if (this.HoldConnection != null)
            {
                yield return this.HoldConnection;
            }
        }

        public override IEnumerable<UnitPhoneViewModel> GetUnits()
        {
            if (this.Unit != null)
            {                
                yield return Unit;
            }            
        }
        
        public override void Finalise()
        {
            try
            {
                Disconnect();
                base.Finalise();
                SaveLog();                

                if(!KeepSelection)
                {
                    this.ClientApp.Service.Release(eResourceType.Channel, Channel.Id);
                    this.Channel.SelectedPanelId = null;
                    this.Channel.LineStatus = eLineStatus.Good;
                    this.Channel.Status = ePhoneStatus.Free;
                    this.Channel.Tone = null;
                    this.Channel.CallerId = null;

                    if (this.Unit != null)
                    {
                        this.Unit.LineStatus = eLineStatus.Good;
                        this.ClientApp.Service.Release(eResourceType.UnitPhone, this.Unit.Id);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        protected void Disconnect()
        {
            if (this.DisconnectPOConnectionWhenFinalise && this.Connection.IsConnected)
            {
                this.Connection.IsConnected = false;
                SendConnection(this.Connection);
            }

            if (this.HoldConnection != null && this.HoldConnection.IsConnected)
            {
                this.HoldConnection.IsConnected = false;
                SendConnection(this.HoldConnection);
            }
        }

        public override bool CanFinalise()
        {
            return IsHolding == false;
        }

        protected abstract T CreateConnection();
        
        #endregion

        #region IController
        public override void OnResourceChanged(ResourceChangedEventArgs e)
        {
            if ((e.ResourceType == eResourceType.Channel && e.Id == this.Channel.Id) ||
                (this.Unit != null && e.ResourceType == eResourceType.UnitPhone && this.Unit.Id == e.Id))
            {
                if (e.PanelId.HasValue && e.PanelId != this.ClientApp.ClientId)
                {
                    Finalise();
                }
                else if(this.Channel.LineStatus != eLineStatus.Occupied)
                {
                    this.Channel.LineStatus = eLineStatus.Occupied;
                }

                e.Handled = true;
            }
        }

        public override void OnPOStatusChanged(POStatusChangedEventArgs e)
        {
            if (e.Status.HasValue && e.Status.Value == ePOStatus.OnHook && this.IsHolding == false
                && this.ClientApp.Channels.PO.Status != e.Status.Value)
            {
                this.ClientApp.Channels.PO.ApplyChanges(e);
                Finalise();                
                e.Handled = true;
            }
        }

        public override void ChannelClicked(object sender, PhoneEventArgs<HostPhoneViewModel> e)
        {
            if (e.Item == this.Channel)
            {
                if (this.IsHolding) Resume();
                else Finalise();
                e.Handled = true;
            }
        }

        public override void UnitClicked(object sender, PhoneEventArgs<UnitPhoneViewModel> e)
        {
            if (this.Unit != null && e.Item == this.Unit)
            {
                if (this.IsHolding) Resume();
                else Finalise();
                e.Handled = true;
            }
            else if(this.IsHolding == false)
            {
                e.Handled = true;
            }
        }

        public override void OnDialCompleted(ChannelEventArgs e)
        {
            if (e.Address == this.Channel.HostPhone.Address)
            {
                _dialCompletedEvent.Set();
                SendConnection(this.Connection);                
                e.Handled = true;
                if(this.CallStatus != eCallStatus.OnHold) this.CallStatus = eCallStatus.Connected;
            }
        }
        #endregion

        protected virtual void SetStatus(ePhoneStatus status)
        {
            this.Channel.Status = status;
            if (this.Unit != null)
            {
                this.Unit.Status = status;
            }
        }

        protected virtual void OnCallStatusChanged()
        {
            if(this.FinalisedDate.HasValue) return;

            switch(this.CallStatus)
            {
                case eCallStatus.Dialed:
                    SetStatus(ePhoneStatus.Dial);
                    break;

                case eCallStatus.Connected:
                    SetStatus(ePhoneStatus.Speaking);
                    break;

                case eCallStatus.OnHold:
                    SetStatus(ePhoneStatus.Holding);
                    break;
            }

            SaveLog();
        }

        void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CallStatus" || e.PropertyName == "ConStatus")
            {
                OnCallStatusChanged();
            }
        }

        protected virtual void SaveLog()
        {            
        }

        public virtual bool CanHold()
        {
            return !this.IsHolding && this.FinalisedDate == null;
        }
    }    
}
