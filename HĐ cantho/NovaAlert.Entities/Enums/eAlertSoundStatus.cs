using System;
using System.Runtime.Serialization;

namespace NovaAlert.Entities
{
    [DataContract][Flags]
    public enum eAlertSoundStatus
    {
        [EnumMember]
        None = 0,

        [EnumMember]
        WaitForKeycode = 1,
        
        [EnumMember]
        WaitForCommand = 2,

        [EnumMember]
        WaitForReport = 3,
    }
}
