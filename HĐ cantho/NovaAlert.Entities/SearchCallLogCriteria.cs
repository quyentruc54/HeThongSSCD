using System;
using System.Runtime.Serialization;

namespace NovaAlert.Entities
{
    [DataContract]
    public class SearchCallLogCriteria : BindableEntity
    {
        byte _callType;
        [DataMember]
        public byte CallType
        {
            get { return _callType; }
            set { _callType = value; OnPropertyChanged("CallType"); }
        }

        int? _channelId;
        [DataMember]
        public int? ChannelId
        {
            get { return _channelId; }
            set { _channelId = value; OnPropertyChanged("ChannelId"); }
        }

        string _unitName;
        [DataMember]
        public string UnitName
        { 
            get { return _unitName; }
            set { _unitName = value; OnPropertyChanged("UnitName"); }
        }

        DateTime? _startDate;
        [DataMember]
        public DateTime? StartDate
        {
            get { return _startDate; }
            set { _startDate = value; OnPropertyChanged("StartDate"); }
        }

        DateTime? _endDate;
        [DataMember]
        public DateTime? EndDate
        {
            get { return _endDate; }
            set { _endDate = value; OnPropertyChanged("EndDate"); }
        }

        [DataMember]
        public int StartIndex { get; set; }
        [DataMember]
        public int NumOfRecords { get; set; }        

        public SearchCallLogCriteria()
        {
            _callType = 1;
            _startDate = DateTime.Now;
        }

        public bool CanSearch()
        {
            return ChannelId.HasValue ||
                !string.IsNullOrEmpty(UnitName) ||
                StartDate.HasValue || EndDate.HasValue;
        }
    }
}
