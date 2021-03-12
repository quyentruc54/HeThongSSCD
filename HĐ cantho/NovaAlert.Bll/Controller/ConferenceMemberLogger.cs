using NovaAlert.Entities;
using System;

namespace NovaAlert.Bll.Controller
{
    public class ConferenceMemberLogger<T> where T:IConferenceMember
    {
        public Guid CallLogId { get; private set; }

        public T Call { get; private set; }

        public CallLogDetail CallLogDetail { get; private set; }

        public ConferenceMemberLogger(T call, Guid callLogId)
        {
            this.Call = call;
            this.CallLogId = callLogId;
        }

        public void SaveLog()
        {
            if (this.CallLogDetail == null)
            {
                this.CallLogDetail = new CallLogDetail()
                {
                    CallLogId = CallLogId,
                    StartTime = this.Call.StartTime,
                    ChannelId = this.Call.Channel.Id
                };

                if (this.Call.Unit != null)
                {
                    this.CallLogDetail.PhoneNumber = this.Call.Unit.GetFullNumber();
                    this.CallLogDetail.UnitId = this.Call.Unit.Id;
                }
                else
                {
                    this.CallLogDetail.PhoneNumber = this.Call.Channel.Tone;
                }
            }

            this.CallLogDetail.UnitId = this.Call.Unit?.Id;
            
            if (this.Call.FinalisedDate.HasValue)
            {
                this.CallLogDetail.EndTime = this.Call.FinalisedDate.Value;
            }

            this.Call.ClientApp.Service.SaveCallLogDetail(this.CallLogDetail);
        }
    }
}
