using NovaAlert.Entities;
using NovaAlert.Entities.ViewModel;

namespace NovaAlert.Bll.Controller
{
    /// <summary>
    /// Controller which handle incomming call.
    /// </summary>
    public class InCallController : CallControllerBase<SwitchConnection>
    {
        public InCallController(ClientAppViewModel app, HostPhoneViewModel channel, UnitPhoneViewModel unit = null)
            : base(app, channel, unit)
        {
            this.ClientApp.Service.Request(eResourceType.Channel, this.Channel.Id);            
        }

        public override void OnResourceChanged(ResourceChangedEventArgs e)
        {
            if (e.ResourceType == eResourceType.Channel && e.PanelId == this.ClientApp.ClientId && e.Id == this.Channel.Id)
            {
                SendConnection(this.Connection);
                this.CallStatus = eCallStatus.Connected;
            }
            else
            {
                base.OnResourceChanged(e);
            }
        }

        public override void OnChannelStatusChanged(ChannelStatusChangedEventArgs e)
        {
            if (e.Address == this.Channel.HostPhone.Address)
            {
                this.Channel.ApplyChanges(e);
                SaveLog();
                e.Handled = true;
            }
        }

        protected override SwitchConnection CreateConnection()
        {
            var scr = new SwitchConnectionEnd(this.ClientApp.Channels.PO.PO) { IsConnected = true };
            var dest = new SwitchConnectionEnd(this.Channel.HostPhone) { IsConnected = true };
            return new SwitchConnection(this.ClientApp.ClientId, scr, dest);
        }

        public override eVolumn Volumn
        {
            get { return this.Connection.Source.Volumn; }
            set 
            {
                if (this.Connection.Source.Volumn != value)
                {
                    this.Connection.Source.Volumn = value;
                    OnPropertyChanged("Volumn");
                    this.ClientApp.Service.AdjustConnectionEndVolumn(this.Connection.Source);
                }
            }
        }

        public override bool IsRecording
        {
            get
            {
                var hasRecord = this.CallLogDetail != null && !string.IsNullOrEmpty(this.CallLogDetail.Record);
                if (this.AutoRecording)
                {
                    return hasRecord;
                }

                return hasRecord && _isManuallyRecording;
            }
        }

        public override bool AutoRecording
        {
            get
            {
                return this.Channel.HostPhone.AutoRecording;
            }
        }

        bool _isManuallyRecording = false;

        #region CallLog
        public CallLog CallLog { get; set; }
        public CallLogDetail CallLogDetail { get; set; }

        protected override void SaveLog()
        {

            if (this.CallLog == null)
            {
                CreateCallLog();
            }
            else
            {
                if (this.Unit != null)
                {
                    this.CallLogDetail.UnitId = this.Unit.Id;
                }

                if (this.FinalisedDate.HasValue)
                {
                    this.CallLog.EndTime = this.FinalisedDate.Value;
                    this.CallLogDetail.EndTime = this.FinalisedDate.Value;
                    if (!string.IsNullOrEmpty(this.CallLogDetail.Record))
                    {
                        this.ClientApp.Service.StopRecord((byte)this.Channel.Id);
                    }
                }
            }

            this.ClientApp.Service.SaveCallLog(this.CallLog);
            this.ClientApp.Service.SaveCallLogDetail(this.CallLogDetail);            
        }
        
        protected virtual void CreateCallLog()
        {
            this.CallLog = new CallLog()
            {                
                CallType = eCallType.InComming,
                POId = this.ClientApp.Channels.PO.Id,
                StartTime = this.StartTime
            };

            this.CallLogDetail = new CallLogDetail()
            {
                CallLogId = this.CallLog.CallLogId,
                StartTime = this.StartTime,
                ChannelId = this.Channel.Id,
                PhoneNumber = this.Channel.CallerId ?? string.Empty
            };

            if (this.Unit != null)
            {
                this.CallLogDetail.UnitId = this.Unit.Id;
                this.CallLogDetail.UnitName = this.Unit.Name;
            }

            if(this.AutoRecording)
            {
                this.CallLogDetail.Record = this.ClientApp.Service.StartRecord((byte)this.Channel.Id, (byte)(this.ClientApp.Channels.PO.Id - 1));
                OnPropertyChanged("IsRecording");
            }
        }        
        #endregion

        public void StartRecord()
        {
            if (this.IsRecording) return;

            if (string.IsNullOrEmpty(this.CallLogDetail.Record))
            {
                this.CallLogDetail.Record = this.ClientApp.Service.StartRecord((byte)this.Channel.Id, (byte)(this.ClientApp.Channels.PO.Id - 1));
                SaveLog();
            }
            else
            {
                this.ClientApp.Service.ResumeRecord((byte)this.Channel.Id);
            }

            _isManuallyRecording = true;
            OnPropertyChanged("IsRecording");
        }

        public void StopRecord()
        {
            if(this.IsRecording && _isManuallyRecording)
            {
                this.ClientApp.Service.PauseRecord((byte)this.Channel.Id, 0);
                _isManuallyRecording = false;
                OnPropertyChanged("IsRecording");
            }
        }

        public override string GetDialingNumber()
        {
            return this.Channel.CallerId;
        }
    }
}
