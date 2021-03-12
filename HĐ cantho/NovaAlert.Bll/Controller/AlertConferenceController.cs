using NovaAlert.Entities;
using NovaAlert.Entities.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NovaAlert.Bll.Controller
{
    public class AlertConferenceController : ConferenceControllerBase<AlertMember>
    {
        #region Properties
        public virtual eTaskType TaskType { get { return eTaskType.CTT; } }
        public SwitchConnection MonitorConnection { get; private set; }

        public SwitchConnection DirectConnection { get; private set; }

        bool _issued;
        public bool Issued
        {
            get { return _issued; }
            set { _issued = value; OnPropertyChanged("Issued"); }
        }

        public AlertMember FirstMember { get; private set; }

        AlertMember _selectedMember;
        public AlertMember SelectedMember
        {
            get
            {
                return _selectedMember;
            }
            set
            {
                if (_selectedMember == value)
                {
                    return;
                }

                if (_selectedMember != null)
                {
                    _selectedMember.IsSelected = false;
                }

                _selectedMember = value;
                if (_selectedMember != null)
                {
                    _selectedMember.IsSelected = true;
                }
            }
        }
        #endregion

        #region Ctor
        public AlertConferenceController(ClientAppViewModel app, IEnumerable<UnitPhoneViewModel> units)
            : base(app, units)
        {
            _selectedMember = null;
            this.FirstMember = GetAllMembers().FirstOrDefault();
            CreateMonitorConnection(this.ClientApp.Channels.PO.Status == ePOStatus.OffHook);
            CheckAndPlaySound();
        }
        #endregion

        #region Override
        protected override void CreateMembers(IEnumerable<UnitPhoneViewModel> units)
        {
            if (units == null || !units.Any())
            {
                return;
            }

            var list = GetAvailableChannels(units);
            foreach (var item in list)
            {
                var mem = new AlertMember(this, item.Value, item.Key);
                AddController(mem);

                mem.Unit.Task.Result = eTaskResult.Started;
                this.ClientApp.UpdateTaskAsync(mem.Unit.Id, mem.Unit.Task.TaskObj, null, this.TaskType);

                mem.PropertyChanged += mem_PropertyChanged;
            }
        }

        public override void Finalise()
        {
            this.ClientApp.Service.UpdateAlertSoundStatus(this.ClientApp.ClientId, eAlertSoundStatus.None);
            Disconnect(this.MonitorConnection);
            Disconnect(this.DirectConnection);
            base.Finalise();
        }

        public override void OnPOStatusChanged(POStatusChangedEventArgs e)
        {
            this.ClientApp.Channels.PO.ApplyChanges(e);
            e.Handled = true;
            if (!e.Status.HasValue)
            {
                return;
            }

            CreateMonitorConnection(this.ClientApp.Channels.PO.Status == ePOStatus.OffHook);

            if (e.Status.Value == ePOStatus.OnHook)
            {
                if (this.Issued)
                {
                    Finalise();
                }
                else
                {
                    this.SelectedMember = null;
                }
            }
        }

        public override void UnitClicked(object sender, PhoneEventArgs<UnitPhoneViewModel> e)
        {
            if (this.ClientApp.Channels.PO.Status == ePOStatus.OnHook) return;
            var mem = GetAllMembers().FirstOrDefault(m => m.Unit.Id == e.Item.Id);
            if (this.Issued)
            {
                var connectedMembers = GetConnectedMembers();
                if (connectedMembers.All(m => m.AlertStatus == AlertMemberStatus.ReceivingCommand))
                {
                    foreach (var m in connectedMembers)
                    {
                        m.AlertStatus = AlertMemberStatus.WaitForReport;
                    }
                }
            }

            if (mem != null && this.SelectedMember != mem && mem.AlertStatus != AlertMemberStatus.CanNotConnect)
            {
                this.SelectedMember = mem;
            }
            else
            {
                this.SelectedMember = null;
            }

            this.ActiveController = this.SelectedMember;
            e.Handled = true;
        }
        #endregion

        void mem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AlertStatus")
            {
                CheckAndPlaySound();
            }
        }

        void CheckAndPlaySound()
        {
            var members = GetAllMembers();
            eAlertSoundStatus soundStatus = eAlertSoundStatus.None;

            if (members.Any(m => m.AlertStatus == AlertMemberStatus.WaitForKeycode))
            {
                soundStatus |= eAlertSoundStatus.WaitForKeycode;
            }

            if (members.Any(m => m.AlertStatus == AlertMemberStatus.WaitForCommand))
            {
                soundStatus |= eAlertSoundStatus.WaitForCommand;
            }

            if (members.Any(m => m.AlertStatus == AlertMemberStatus.WaitForReport))
            {
                soundStatus |= eAlertSoundStatus.WaitForReport;
            }

            this.ClientApp.Service.UpdateAlertSoundStatus(this.ClientApp.ClientId, soundStatus);
        }

        public bool CanIssue
        {
            get
            {
                return this.ClientApp.Channels.PO.Status == ePOStatus.OffHook &&
                    !this.Issued && GetAllMembers().Any(m => m.AlertStatus == AlertMemberStatus.WaitForCommand);
            }
        }

        public void OnIssue()
        {
            if (!CanIssue) return;

            // Clear current selected member
            this.SelectedMember = null;

            var allMembers = GetAllMembers();

            // Send connections to PO and update alert status           
            foreach (var mem in allMembers)
            {
                if (mem.AlertStatus == AlertMemberStatus.WaitForCommand)
                {
                    mem.AlertStatus = AlertMemberStatus.ReceivingCommand;
                    mem.Unit.Task.Result = eTaskResult.Connected;
                }
                else
                {
                    mem.AlertStatus = AlertMemberStatus.CanNotConnect;
                    mem.Unit.Task.Result = eTaskResult.CanNotConnect;
                }

                this.ClientApp.UpdateTaskAsync(mem.Unit.Id, mem.Unit.Task.TaskObj, mem.Unit.Task.LastDuration, this.TaskType);
            }

            this.Issued = true;
        }

        List<AlertMember> GetConnectedMembers()
        {
            return GetAllMembers().Where(m => m.AlertStatus != AlertMemberStatus.WaitForKeycode
                && m.AlertStatus != AlertMemberStatus.CanNotConnect).ToList();
        }

        public void OnReceive()
        {
            if (this.SelectedMember == null || this.SelectedMember.CanReceiveCommand() == false)
            {
                return;
            }

            if (this.SelectedMember.AlertStatus == AlertMemberStatus.WaitForReport)
            {
                var u = this.SelectedMember.Unit;
                u.Task.Result = eTaskResult.NL;
                this.ClientApp.UpdateTaskAsync(u.Id, u.Task.TaskObj, u.Task.LastDuration, this.TaskType);

                var mem = this.SelectedMember;
                this.SelectedMember = null;
                mem.Finalise();
            }

            var list = GetConnectedMembers();
            if (list.Count == 0)
            {
                this.Finalise();
            }
            else
            {
                foreach (var mem in list)
                {
                    mem.AlertStatus = AlertMemberStatus.WaitForReport;
                }
                CheckAndPlaySound();
            }
        }

        #region Monitor Connection
        private void CreateMonitorConnection(bool isPO_OffHook)
        {
            Disconnect(this.MonitorConnection);
            if (isPO_OffHook)
            {
                this.MonitorConnection = null;
            }
            else
            {
                this.MonitorConnection = new SwitchConnection(this.ClientApp.ClientId, new SwitchConnectionEnd(this.FirstMember.Channel.HostPhone), CreateMonitorSpeakerConnectionEnd());
            }

            if (this.MonitorConnection != null)
            {
                SendConnection(this.MonitorConnection);
            }

            Action act = new Action(() => this.FirstMember.RefeshStatus());
            act.BeginInvoke(null, null);
        }

        SwitchConnectionEnd CreateMonitorSpeakerConnectionEnd()
        {
            return new SwitchConnectionEnd(OtherDevice.Speakers[this.ClientApp.ClientId - 1], eVolumn.Volumn_6, false);
        }
        #endregion

        protected override CallLog CreateCallLog()
        {
            var callLog = base.CreateCallLog();
            callLog.CallType = eCallType.Alert;
            return callLog;
        }

        public override bool NeedRecord
        {
            get
            {
                return true;
            }
        }
    }
}
