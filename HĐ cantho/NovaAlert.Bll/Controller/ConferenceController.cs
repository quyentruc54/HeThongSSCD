using System.Collections.Generic;
using System.Linq;
using NovaAlert.Entities;
using NovaAlert.Entities.ViewModel;

namespace NovaAlert.Bll.Controller
{
    /// <summary>
    /// Controller cho gọi nhóm
    /// </summary>
    public class ConferenceController: ConferenceControllerBase<ConferenceMember>
    {
        public ConferenceConnection POConnection { get; protected set; }
        public ConferenceController(ClientAppViewModel app, IEnumerable<UnitPhoneViewModel> units):base(app, units)
        {
            // enable conference
            this.ClientApp.Service.SetConference(true);

            // Add PO to conference first
            POConnection = new ConferenceConnection(this.ClientApp.ClientId, this.ClientApp.Channels.PO.PO, eVolumn.Volumn_6, eVolumn.Volumn_2);
            SendConnection(POConnection, true);
        }

        public override void UnitClicked(object sender, PhoneEventArgs<UnitPhoneViewModel> e)
        {
            var members = GetAllMembers();
            var mem = members.FirstOrDefault(m => m.Unit != null && m.Unit.Id == e.Item.Id);
            if (mem != null)
            {
                OnMemberClicked(members, mem);
                e.Handled = true;                
            }
        }

        public override void ChannelClicked(object sender, PhoneEventArgs<HostPhoneViewModel> e)
        {
            var members = GetAllMembers();
            var mem = members.FirstOrDefault(m => m.Channel.Id == e.Item.Id);
            if (mem != null)
            {
                OnMemberClicked(members, mem);
                e.Handled = true;
            }
        }

        private void OnMemberClicked(IEnumerable<ConferenceMember> members, ConferenceMember mem)
        {
            if (mem.ConStatus == eConferenceMemberStatus.Speaking)
            {
                if (members.Any(m => m != mem && m.ConStatus == eConferenceMemberStatus.Speaking))
                {
                    mem.ConStatus = eConferenceMemberStatus.Listening;
                }
                else
                {
                    foreach (var m in members)
                    {
                        m.ConStatus = eConferenceMemberStatus.Waiting;
                    }
                }
            }
            else
            {
                var currentSpeakers = members
                    .Where(m => m.ConStatus == eConferenceMemberStatus.Speaking)
                    .OrderBy(mm => mm.StartSpeakingTime).ToList();

                if (currentSpeakers.Count >= MaxSpeaker)
                {
                    for (int i = 0; i < currentSpeakers.Count - MaxSpeaker + 1; i++)
                    {
                        currentSpeakers[i].ConStatus = eConferenceMemberStatus.Listening;
                    }

                    foreach (var m in members.Where(mm => mm != mem && !currentSpeakers.Contains(mm)))
                    {
                        m.ConStatus = eConferenceMemberStatus.Listening;
                    }
                }

                mem.ConStatus = eConferenceMemberStatus.Speaking;
            }

            this.ActiveController = mem;
            UpdateMemberVolumn();
        }

        void UpdateMemberVolumn()
        {
            foreach (var mem in GetAllMembers())
                mem.SendAdjustVolumn();
        }

        protected override void CreateMembers(IEnumerable<UnitPhoneViewModel> units)
        {
            if(units == null || !units.Any())
            {
                return;
            }

            var list = GetAvailableChannels(units);
            foreach (var item in list)
            {
                var mem = new ConferenceMember(this, item.Value, this.ClientApp.ClientId, item.Key);
                AddController(mem);
            }
            InitMemberVolumn();
        }

        public void InitMemberVolumn()
        {
            eConferenceMemberStatus status = eConferenceMemberStatus.Listening;
            var list = GetAllMembers().ToList();
            if (list.Count <= MaxSpeaker) status = eConferenceMemberStatus.Speaking;
            foreach (var mem in list)
            {
                mem.ConStatus = status;
                mem.SendAdjustVolumn();
            }
        }

        public int GetSpeakerCount()
        {
            return GetAllMembers().Count(m => m.ConStatus == eConferenceMemberStatus.Speaking);
        }

        void ChangePOConnection(bool connected)
        {
            POConnection.IsConnected = connected;
            SendConnection(POConnection);
        }

        public override bool IsHolding
        {
            get
            {                
                return !this.POConnection.IsConnected;
            }
        }

        public override void PutOnHold()
        {
            ChangePOConnection(false);
            base.PutOnHold();
        }

        public override void Resume()
        {
            ChangePOConnection(true);
            base.Resume();
        }

        public override void Finalise()
        {
            ChangePOConnection(false);
            base.Finalise();
        }

        public void Clear()
        {
            var lastMember = GetAllMembers().Where(c => c.ConStatus == eConferenceMemberStatus.Speaking)
                .OrderByDescending(c => c.Channel.LastClicked).FirstOrDefault();

            if (lastMember != null)
            {
                lastMember.Finalise();
                this.Controllers.Remove(lastMember);
            }

            if (this.Controllers.Count == 0)
            {
                Finalise();
            }
        }

        public override bool NeedRecord
        {
            get
            {
                return this.GetAllMembers().Select(m => m.Channel).Any(c => c.HostPhone.AutoRecording);
            }
        }
    }    
}
