using NovaAlert.Entities;
using NovaAlert.Entities.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace NovaAlert.Bll.Controller
{
    /// <summary>
    /// Một đầu cuối trong chế độ CTT
    /// </summary>
    public class AlertMember : ConferenceMemberBase
    {
        #region Properties
        public AlertConferenceController Parent { get; private set; }

        protected override CallLog GetParentCallLog()
        {
            if (Parent != null)
            {
                return Parent.CallLog;
            }
            return null;
        }

        private AlertMemberStatus _alertStatus;
        public AlertMemberStatus AlertStatus
        {
            get { return _alertStatus; }
            set
            {
                if (_alertStatus != value)
                {
                    _alertStatus = value;
                    OnPropertyChanged("AlertStatus");
                    OnAlertStatusChanged();
                }
            }
        }

        bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                    OnAlertStatusChanged();
                }
            }
        }

        public List<KeyValuePair<AlertMemberStatus, SwitchConnection>> SoundConnections { get; private set; }

        SwitchConnection _issueCommandConnection;
        public SwitchConnection IssueCommandConnection
        {
            get
            {
                if (_issueCommandConnection == null)
                {
                    _issueCommandConnection = new SwitchConnection(ClientApp.ClientId,
                        new SwitchConnectionEnd(ClientApp.Channels.PO.PO),
                        new SwitchConnectionEnd(Channel.HostPhone, isConnected: false));
                }
                return _issueCommandConnection;
            }
        }

        public override bool AutoRecording
        {
            get
            {
                return true;
            }
        }

        SwitchConnection _monitorConnection;
        readonly ManualResetEvent _syncEvent = new ManualResetEvent(false);
        #endregion

        #region Ctor
        public AlertMember(AlertConferenceController parent, HostPhoneViewModel channel, UnitPhoneViewModel unit)
            : base(parent.ClientApp, channel, unit)
        {
            Parent = parent;
            _isSelected = false;
            _alertStatus = AlertMemberStatus.Initialized;

            SoundConnections = new List<KeyValuePair<AlertMemberStatus, SwitchConnection>>()
            {
                new KeyValuePair<AlertMemberStatus, SwitchConnection>(AlertMemberStatus.WaitForKeycode,
                    new SwitchConnection(ClientApp.ClientId, new SwitchConnectionEnd(OtherDevice.SoundChannels[1]), new SwitchConnectionEnd(Channel.HostPhone))),

                new KeyValuePair<AlertMemberStatus, SwitchConnection>(AlertMemberStatus.WaitForCommand,
                    new SwitchConnection(ClientApp.ClientId, new SwitchConnectionEnd(OtherDevice.SoundChannels[2]), new SwitchConnectionEnd(Channel.HostPhone))),

                new KeyValuePair<AlertMemberStatus, SwitchConnection>(AlertMemberStatus.WaitForReport,
                    new SwitchConnection(ClientApp.ClientId, new SwitchConnectionEnd(OtherDevice.SoundChannels[3]), new SwitchConnectionEnd(Channel.HostPhone)))
            };

            Dial();
        }
        #endregion

        #region Override
        protected override ConferenceConnection CreateConnection() => new ConferenceConnection(ConferenceId, Channel.HostPhone, isConnected: false);
        
        public override void OnDialCompleted(ChannelEventArgs e)
        {
            if (e.Address == Channel.HostPhone.Address)
            {
                _dialCompletedEvent.Set();
                AlertStatus = string.IsNullOrEmpty(Unit.Password) ? AlertMemberStatus.WaitForCommand : AlertMemberStatus.WaitForKeycode;                
                e.Handled = true;
            }
        }

        public override void OnChannelStatusChanged(ChannelStatusChangedEventArgs e)
        {
            if (e.Address != Channel.Id)
            {
                return;
            }
            if (AlertStatus == AlertMemberStatus.CanNotConnect)
            {
                // TODO: notify or writelog
                return;
            }

            bool isWaitingForKeycode = AlertStatus == AlertMemberStatus.WaitForKeycode && !string.IsNullOrEmpty(Unit.Password);
            if (isWaitingForKeycode && (e.Tone?.Contains(Unit.Password) ?? false))
            {
                AlertStatus = AlertMemberStatus.WaitForCommand;
                e.Handled = true;
            }

            base.OnChannelStatusChanged(e);
        }

        public override void Finalise()
        {
            base.Finalise();
            SetStatus(ePhoneStatus.Free);
            Disconnect(_issueCommandConnection);
            if (AlertStatus == AlertMemberStatus.CanNotConnect)
            {
                ClientApp.Service.Request(eResourceType.UnitPhone, Unit.Id);
            }
        }
        #endregion

        #region Helpers
        public void RefeshStatus()
        {
            OnAlertStatusChanged();
            OnPropertyChanged("AlertStatus");
        }

        void OnAlertStatusChanged()
        {
            SetMonitorConnection(IsSelected);
            if (IsSelected)
            {
                SetStatus(ePhoneStatus.Speaking);
                return;
            }

            var con = SoundConnections.Where(c => c.Key == AlertStatus).Select(c => c.Value).FirstOrDefault();
            if (con != null)
            {
                con.Source.IsConnected = true;
                con.Dest.IsConnected = false;
                System.Threading.Thread.Sleep(20);
                SendConnection(con);
            }

            switch (AlertStatus)
            {
                case AlertMemberStatus.CanNotConnect:
                    SetStatus(ePhoneStatus.Occupied);
                    Disconnect();
                    ClientApp.Service.Release(eResourceType.Channel, Channel.Id);
                    break;

                case AlertMemberStatus.WaitForKeycode:
                    SetStatus(ePhoneStatus.WaitForKeycode);
                    break;

                case AlertMemberStatus.WaitForCommand:
                    SetStatus(ePhoneStatus.WaitForCommand);

                    break;

                case AlertMemberStatus.ReceivingCommand:
                    System.Threading.Thread.Sleep(20);
                    SendConnection(IssueCommandConnection);
                    SetStatus(ePhoneStatus.Listening);
                    break;

                case AlertMemberStatus.WaitForReport:
                    SetStatus(ePhoneStatus.WaitForReport);
                    break;
            }

            SaveLog();
        }

        public bool CanReceiveCommand()
        {
            return AlertStatus != AlertMemberStatus.WaitForKeycode &&
                AlertStatus != AlertMemberStatus.CanNotConnect;
        }

        void SetMonitorConnection(bool connected)
        {
            if (connected)
            {
                if (_monitorConnection == null)
                {
                    _monitorConnection = new SwitchConnection(ClientApp.ClientId, new SwitchConnectionEnd(ClientApp.Channels.PO.PO),
                        new SwitchConnectionEnd(Channel.HostPhone));
                }

                _monitorConnection.IsConnected = true;
                SendConnection(_monitorConnection);
            }
            else
            {
                if (_monitorConnection != null)
                {
                    _monitorConnection.IsConnected = false;
                    SendConnection(_monitorConnection);
                }
            }

            OnPropertyChanged("Volumn");
        }
        #endregion

        public override eVolumn Volumn
        {
            get
            {
                return _monitorConnection?.Source.Volumn ?? eVolumn.Volumn_0;
            }
            set
            {
                if (_monitorConnection != null && _monitorConnection.Source.Volumn != value)
                {
                    _monitorConnection.Source.Volumn = value;
                    OnPropertyChanged("Volumn");
                    SendAdjustVolumn();
                }
            }
        }

        public override void SendAdjustVolumn()
        {
            if (_monitorConnection != null)
            {
                ClientApp.Service.AdjustConnectionEndVolumn(_monitorConnection.Source);
            }
        }
    }
}