using NovaAlert.Entities;
using NovaAlert.Entities.ViewModel;

namespace NovaAlert.Bll.Controller
{
	public abstract class ConferenceMemberBase : CallControllerBase<ConferenceConnection>, IConferenceMember
	{
		#region Properties
		object _syncObj = new object();
		public byte ConferenceId { get; protected set; }
		public override bool IsRecording
		{
			get
			{
				return this.CallLogDetail != null && !string.IsNullOrEmpty(this.CallLogDetail.Record);
			}
		}
		#endregion

		#region Ctor
		public ConferenceMemberBase(ClientAppViewModel app, HostPhoneViewModel channel, UnitPhoneViewModel unit = null)
			:base(app, channel, unit)
		{
		}
		#endregion

		#region CallLog
		protected abstract CallLog GetParentCallLog();

		public CallLogDetail CallLogDetail { get; protected set; }
		protected override void SaveLog()
		{
			if (GetParentCallLog() == null) return;

			if (this.CallLogDetail == null)
			{
				this.CallLogDetail = new CallLogDetail()
				{
					CallLogId = GetParentCallLog().CallLogId,
					StartTime = this.StartTime,
					ChannelId = this.Channel.Id
				};

				if (this.Unit != null)
				{
					this.CallLogDetail.PhoneNumber = this.Unit.GetFullNumber();
					this.CallLogDetail.UnitId = this.Unit.Id;
					this.CallLogDetail.UnitName = this.Unit.Name;
				}
				else
				{
					this.CallLogDetail.PhoneNumber = this.Channel.Tone;
				}

				if(this.AutoRecording)
				{
					lock(_syncObj)
					{
						this.CallLogDetail.Record = this.ClientApp.Service.StartRecord((byte)this.Channel.Id, (byte)(this.ClientApp.Channels.PO.Id - 1));
						OnPropertyChanged("IsRecording");
					}                    
				}                
			}

			if (this.Unit != null)
			{
				this.CallLogDetail.UnitId = this.Unit.Id;
				this.CallLogDetail.UnitName = this.Unit.Name;
			}

			if (this.FinalisedDate.HasValue)
			{
				this.CallLogDetail.EndTime = this.FinalisedDate.Value;
				if (!string.IsNullOrEmpty(this.CallLogDetail.Record) && this.DisconnectPOConnectionWhenFinalise)
					this.ClientApp.Service.StopRecord((byte)this.Channel.Id);
			}

			this.ClientApp.Service.SaveCallLogDetail(this.CallLogDetail);
		}
		#endregion

		protected override void SetStatus(ePhoneStatus status)
		{
			base.SetStatus(status);

			byte channelId = (byte)this.Channel.Id;
			switch (status)
			{
				case ePhoneStatus.Speaking:
				case ePhoneStatus.Listening:
					this.ClientApp.Service.ResumeRecord(channelId);
					break;

				case ePhoneStatus.Holding:
					this.ClientApp.Service.PauseRecord(channelId, 0);
					break;

				case ePhoneStatus.WaitForKeycode:
					this.ClientApp.Service.PauseRecord(channelId, 1);
					break;

				case ePhoneStatus.WaitForCommand:
					this.ClientApp.Service.PauseRecord(channelId, 2);
					break;

				case ePhoneStatus.WaitForReport:
					this.ClientApp.Service.PauseRecord(channelId, 3);
					break;
			}
		}

		public abstract void SendAdjustVolumn();
	}
}
