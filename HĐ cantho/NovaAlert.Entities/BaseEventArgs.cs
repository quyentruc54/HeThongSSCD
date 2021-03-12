using System.Runtime.Serialization;

namespace NovaAlert.Entities
{
    [DataContract]
    public abstract class BaseEventArgs: System.EventArgs
    {
        [IgnoreDataMember]
        public bool Handled { get; set; }
    }
}
