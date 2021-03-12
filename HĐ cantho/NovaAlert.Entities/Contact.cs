using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace NovaAlert.Entities
{
    public class Contact
    {
        public int PhoneNumberId { get; set; }
        public string AreaCode { get; set; }
        public string Number { get; set; }
        public string UnitName { get; set; }
        public string NameAbbr { get; set; }
        public string Password { get; set; }        
        public string TSLAreaCode { get; set; }        
        public string TSLNumber { get; set; }    

        public ePhoneNumberTypeCde TypeCde
        {
            get
            {
                if (!string.IsNullOrEmpty(this.AreaCode)) 
                {
                    int code;
                    if(int.TryParse(this.AreaCode, out code))
                    {
                        if (code == 69)
                        {
                            return ePhoneNumberTypeCde.Army;
                        }
                        else if (code < 90)
                        {
                            return ePhoneNumberTypeCde.Civil;
                        }
                        return ePhoneNumberTypeCde.Mobile;
                    }
                }

                return ePhoneNumberTypeCde.Army;
            }
        }

        public string FullName
        {
            get { return string.Format("{0} - {1}{2}", this.UnitName, GetAreaCode(), this.Number); }
        }

        string GetAreaCode()
        {
            if (string.IsNullOrEmpty(this.AreaCode))
            {
                return string.Empty;
            }
            return string.Format("({0})", this.AreaCode);
        }
    }

     public class ContactInGroup
    {
        public Contact Contact { get; set; }
        public int ListOrder { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class ContactGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ContactInGroup> Contacts { get; set; }
        public bool IsDeleted { get; set; }

        public ContactGroup()
        {
            this.IsDeleted = false;
        }
    }

    [DataContract]
    public class BaseEntity
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [IgnoreDataMember]
        public virtual eResourceType ResourceType { get { return eResourceType.Channel; } }

        public BaseEntity(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }

    [DataContract]
    public class Phone: BaseEntity
    {
        [DataMember]
        public string AreaCode { get; set; }
        [DataMember]
        public string Number { get; set; }
        [DataMember]
        public bool IsRestricted { get; set; }

        public Phone(int id, string name):base(id, name)
        {

        }
    }

    [DataContract]
    public class UnitPhone: Phone
    {
        [DataMember]
        public int ListOrder { get; set; }
        
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public int PhoneNumberId { get; set; }
        [DataMember]
        public Task Task { get; set; }

        [IgnoreDataMember]
        public override eResourceType ResourceType { get { return eResourceType.UnitPhone; } }

        [DataMember]
        public string TSLAreaCode { get; set; }
        [DataMember]
        public string TSLNumber { get; set; }        

        public UnitPhone(int id, string name):base(id, name)
        {
            this.Task = new Task();
        }
    }
    [DataContract]
    public class HostPhone: Phone, ISwitchAddress
    {
        [DataMember]
        public byte Address { get; set; }

        [IgnoreDataMember]
        public override eResourceType ResourceType { get { return eResourceType.Channel; } }
        
        [DataMember]
        public bool AutoRecording { get; set; }
        [DataMember]
        public bool AlertEnabled { get; set; }
        [DataMember]
        public bool MultiDestEnabled { get; set; }
        [DataMember]
        public int? HotUnitId { get; set; }

        [DataMember]
        public bool CCPKEnabled { get; set; }

        public HostPhone(int id, string name):base(id, name)
        {
        }
    }

    public class Alarm
    {
        public int AlarmId { get; set; }
        public byte DayType { get; set; }
        public byte AlarmType { get; set; }
        public string Name { get; set; }
        public System.DateTime Time { get; set; }
        public byte TimesOfPlaying { get; set; }
        public bool IsEnabled { get; set; }
    }

    public class RadioTime
    {
        public Byte DayType { get; set; }
        public Byte ListOrder { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsEnabled { get; set; }
    }

    public class DayTypeConfig
    {
        public byte DayOfWeek { get; set; }
        public byte DayType { get; set; }
    }

    public class Channel
    {
        public int ChannelId { get; set; }
        public string AreaCode { get; set; }
        public string Number { get; set; }
        public bool IsRestricted { get; set; }
        public bool AutoRecording { get; set; }
        public bool AlertEnabled { get; set; }
        public bool MultiDestEnabled { get; set; }
        public bool CCPKEnabled { get; set; }
        public int? HotUnitId { get; set; }
    }

    public class Enum
    {
        public string Type { get; set; }
        public byte Value { get; set; }
        public string Desc { get; set; }
        public Nullable<byte> ListOrder { get; set; }
        public string Desc_VN { get; set; }
    }

    #region CallLog
    public enum eCallType: byte
    {
        Single = 1,
        InComming = 2,
        Conference = 3,
        Alert = 4,
        CCPK = 5
    }
    public class CallLog
    {
        public CallLog()
        {
            this.CallLogId = Guid.NewGuid();
        }

        public Guid CallLogId { get; set; }
        public int POId { get; set; }
        public System.DateTime StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public eCallType CallType { get; set; }

        public byte iCallType
        {
            get { return (byte)this.CallType; }
        }
    }

    public class CallLogDetail
    {
        public CallLogDetail()
        {
            this.CallLogDetailId = Guid.NewGuid();
        }

        public Guid CallLogDetailId { get; set; }
        public Guid CallLogId { get; set; }
        public Nullable<int> ChannelId { get; set; }
        public Nullable<int> UnitId { get; set; }
        public string UnitName { get; set; }
        public string PhoneNumber { get; set; }
        public System.DateTime StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public string Record { get; set; }

        public string GetRecordFileName()
        {
            return string.Format(@"Kenh {0}\{1}", this.ChannelId - 1, this.Record);
        }
    } 

    public class SearchCallLogResult: CallLog, INotifyPropertyChanged
    {
        public List<CallLogDetail> Details { get; set; }
        public SearchCallLogResult()
        {
            _selected = false;
            this.Details = new List<CallLogDetail>();
        }

        bool _selected;
        public bool Selected
        {
            get { return _selected; }
            set { _selected = value; OnPropertyChanged("Selected"); }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
    #endregion

    public partial class Panel
    {
        public byte PanelId { get; set; }
        public byte POId { get; set; }
        public Nullable<byte> CurrentMode { get; set; }
        public Nullable<int> CurrentGroupId { get; set; }
    }

    public class GroupUnit
    {
        public int GroupId { get; set; }
        public int UnitId { get; set; }
        public int ListOrder { get; set; }
    }

    public class LogItem
    {
        public long Id { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public byte PanelId { get; set; }
        public Nullable<int> UserId { get; set; }
        public string Info { get; set; }
    }

    public class DisplayData
    {
        public int DisplayId { get; set; }
        public Nullable<int> PhoneNumber_1 { get; set; }
        public Nullable<int> PhoneNumber_2 { get; set; }
    }

    public class TslStatus
    {
        public int Id { get; set; }
        public eTslStatusType Type { get; set; }
        public eTslStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
