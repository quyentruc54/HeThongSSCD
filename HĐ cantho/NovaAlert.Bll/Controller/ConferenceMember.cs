using System;
using NovaAlert.Common;
using NovaAlert.Entities;
using NovaAlert.Entities.ViewModel;

namespace NovaAlert.Bll.Controller
{
    public enum eConferenceMemberStatus
    {
        /// <summary>
        /// Đang chờ
        /// </summary>
        Waiting,
        /// <summary>
        /// Đang nói
        /// </summary>
        Speaking,
        /// <summary>
        /// Đang nghe
        /// </summary>
        Listening
    }

    /// <summary>
    /// Đầu cuối trong gọi nhóm
    /// </summary>
    public class ConferenceMember: ConferenceMemberBase
    {
        #region Properties
        public ConferenceController Parent { get; private set; }        
        protected override CallLog GetParentCallLog()
        {
            if (this.Parent != null)
            {
                return this.Parent.CallLog;
            }
            return null;
        }

        private eConferenceMemberStatus _conStatus;
        public eConferenceMemberStatus ConStatus
        {
            get { return _conStatus; }
            set
            {
                if (_conStatus != value)
                {
                    _conStatus = value;
                    if (_conStatus == eConferenceMemberStatus.Speaking)
                    {
                        this.StartSpeakingTime = ClientApp.Service.GetDateTime();
                    }
                    OnPropertyChanged("ConStatus");
                    UpdateConnectionVolumn(this.Connection);                                        
                }
            }
        }

        public override bool AutoRecording
        {
            get
            {
                return this.Channel.HostPhone.AutoRecording;
            }
        }
        public DateTime StartSpeakingTime { get; private set; }
        #endregion

        #region Ctor
        public ConferenceMember(ConferenceController parent, HostPhoneViewModel channel, byte conferenceId, UnitPhoneViewModel unit = null)
            : base(parent.ClientApp, channel, unit)
        {
            this.StartSpeakingTime = DateTime.Now;
            this.Parent = parent;
            _conStatus = eConferenceMemberStatus.Waiting;
            this.ConferenceId = conferenceId;
            this.Connection.ConferenceId = this.ConferenceId;

            if(this.Unit != null)
            {
                this.Channel.Tone = this.Unit.GetFullNumber();
            }
            
            Dial();
        }

        public ConferenceMember(ConferenceController parent, InCallController call, eConferenceMemberStatus conStatus, byte conferenceId): base(call.ClientApp, call.Channel, call.Unit)
        {
            this.Parent = parent;
            call.DisconnectPOConnectionWhenFinalise = false;
            call.KeepSelection = true;
            call.Finalise();
            
            this.ConferenceId = conferenceId;
            this.Connection.ConferenceId = this.ConferenceId;            

            SendConnection(this.Connection);
            this.ConStatus = conStatus;
        }
        #endregion

        #region Override
        protected override ConferenceConnection CreateConnection()
        {
            var conn = new ConferenceConnection(this.ConferenceId, this.Channel.HostPhone);
            UpdateConnectionVolumn(conn);            
            return conn;
        }

        public override void OnPOStatusChanged(POStatusChangedEventArgs e)
        {
            if (e.Status.HasValue && e.Status.Value == ePOStatus.OnHook && this.IsHolding == false)
            {
                Finalise();
            }
        }
        public override void OnResourceChanged(ResourceChangedEventArgs e)
        {
            if (e.ResourceType == eResourceType.Channel && e.Id == this.Channel.Id && e.PanelId == this.ClientApp.ClientId)
            {
                Dial();
            }
        }
        public override void PutOnHold()
        {
            this.CallStatus = eCallStatus.OnHold;
            if (this.IsRecording)
            {
                this.ClientApp.Service.PauseRecord((byte)this.Channel.Id, 0);
            }
        }

        public override void Resume()
        {
            this.CallStatus = eCallStatus.Connected;
            if (this.IsRecording)
            {
                this.ClientApp.Service.ResumeRecord((byte)this.Channel.Id);
            }
        }

        protected override void OnCallStatusChanged()
        {
            if (this.FinalisedDate.HasValue)
            {
                return;
            }

            if (this.IsHolding)
            {
                SetStatus(ePhoneStatus.Holding);
            }
            else
            {
                if (this.ConStatus == eConferenceMemberStatus.Speaking) SetStatus(ePhoneStatus.Speaking);
                else SetStatus(ePhoneStatus.Listening);
            }
            AsyncHelper.RunAsync(SaveLog);            
        }

        public override eVolumn Volumn
        {
            get
            {
                return this.Connection.InVolumn;
            }
            set
            {
                if (this.Connection.InVolumn != value)
                {
                    this.Connection.InVolumn = value;
                    OnPropertyChanged("Volumn");
                    SendAdjustVolumn();
                }
            }
        }

        public override void OnDialCompleted(ChannelEventArgs e)
        {
            if (e.Address == this.Channel.HostPhone.Address)
            {
                _dialCompletedEvent.Set();
                UpdateConnectionVolumn(this.Connection);
                SendConnection(this.Connection);
                e.Handled = true;
                if (this.CallStatus != eCallStatus.OnHold)
                {
                    this.CallStatus = eCallStatus.Connected;
                }
            }
        }
        
        #endregion

        #region Helpers
        public override void SendAdjustVolumn()
        {
            AsyncHelper.RunAsync<ConferenceConnection>(this.ClientApp.Service.AdjustConferenceVolumn, this.Connection);
        }

        private void UpdateConnectionVolumn(ConferenceConnection conn)
        {
            switch (_conStatus)
            {
                case eConferenceMemberStatus.Waiting:
                    conn.InVolumn = eVolumn.Volumn_4;
                    break;

                case eConferenceMemberStatus.Listening:
                    conn.InVolumn = eVolumn.Volumn_2;
                    break;

                case eConferenceMemberStatus.Speaking:
                    conn.InVolumn = eVolumn.Volumn_6;
                    break;
            }            
        }
        #endregion        
    }
}
