using System.Runtime.Serialization;

namespace NovaAlert.Entities
{
    [DataContract]
    public class ChannelEventArgs: BaseEventArgs
    {
        [DataMember]
        public int Address { get; private set; }
        public ChannelEventArgs(int address)
        {
            this.Address = address;
        }
    }
}
