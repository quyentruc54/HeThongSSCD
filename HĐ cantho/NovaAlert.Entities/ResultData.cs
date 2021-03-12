using System;
using System.Runtime.Serialization;
using System.Text;

namespace NovaAlert.Entities
{
    public class ResultData
    {
        public int DisplayId { get; set; }
        public int PhoneNumberId { get; set; }
        public string UnitName { get; set; }
        public eTask Task { get; set; }
        public eTaskLevel Level { get; set; }
        public eTaskResult Result { get; set; }
        public DateTime? TimeReceive { get; set; }
        public TimeSpan? IntervalReceive { get; set; }
        public DateTime? TimeChange { get; set; }
        public TimeSpan? IntervalChange { get; set; }

        public eTaskType TaskType { get; set; }

        public ResultData()
        {
        }

        [IgnoreDataMember]
        public bool A { get { return this.Task == eTask.A; } }

        [IgnoreDataMember]
        public bool A2 { get { return this.Task == eTask.A2; } }

        [IgnoreDataMember]
        public bool A3 { get { return this.Task == eTask.A3; } }

        [IgnoreDataMember]
        public bool A4 { get { return this.Task == eTask.A4; } }

        [IgnoreDataMember]
        public bool TC { get { return this.Level == eTaskLevel.TC; } }

        [IgnoreDataMember]        
        public bool CA { get { return this.Level == eTaskLevel.CA; } }

        [IgnoreDataMember]
        public bool TB { get { return this.Level == eTaskLevel.TB; } }

        [IgnoreDataMember]
        public bool IsCommandReceived { get { return this.TimeReceive.HasValue; } }

        [IgnoreDataMember]
        public bool IsStatusChanged { get { return this.TimeChange.HasValue; } }

        [IgnoreDataMember]
        public string S_IntervalReceive
        {
            get
            {
                if (this.IntervalReceive.HasValue)
                {
                    return ToVietNameseTimeSpan(this.IntervalReceive.Value);
                }
                return string.Empty;
            }
        }

        [IgnoreDataMember]
        public string S_IntervalChange
        {
            get
            {
                if (this.IntervalChange.HasValue)
                {
                    return ToVietNameseTimeSpan(this.IntervalChange.Value);
                }
                return string.Empty;
            }
        }

        [IgnoreDataMember]
        public string TaskText
        {
            get
            {
                return this.Task == eTask.None ? string.Empty : this.Task.ToString();
            }
        }

        #region CCPK
        [IgnoreDataMember]
        public bool IsCCPK_Level1 { get { return this.Level == eTaskLevel.TC; } }

        [IgnoreDataMember]
        public bool IsCCPK_Level2 { get { return this.Level == eTaskLevel.CA; } }
        #endregion

        string ToVietNameseTimeSpan(TimeSpan ts)
        {
            var sb = new StringBuilder();
            if (ts.Days > 0) sb.AppendFormat("{0} ngày ", ts.Days);
            if (ts.Hours > 0) sb.AppendFormat("{0} giờ ", ts.Hours);
            if (ts.Minutes > 0) sb.AppendFormat("{0} phút ", ts.Minutes);
            if (ts.Seconds > 0) sb.AppendFormat("{0} giây", ts.Seconds);
            return sb.ToString().TrimEnd();
        }
    }
}
