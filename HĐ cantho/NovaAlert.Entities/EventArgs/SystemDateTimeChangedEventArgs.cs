using System;
using System.Runtime.Serialization;

namespace NovaAlert.Entities
{
    [DataContract]
    public class SystemDateTimeChangedEventArgs : BaseEventArgs
    {
        [DataMember]
        public DateTime DateTime { get; private set; }
        public SystemDateTimeChangedEventArgs(DateTime dt):base()
        {
            this.DateTime = dt;
        }
    }
}
