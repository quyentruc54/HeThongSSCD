using System.Runtime.Serialization;

namespace NovaAlert.Entities
{
    [DataContract]
    public class ChannelStatusChangedEventArgs : BaseEventArgs
    {
        [DataMember]
        public int Address { get; private set; }

        [DataMember]
        public eLineStatus? Status { get; set; }

        [DataMember]
        public bool IsLooped { get; set; }

        [DataMember]
        public string Tone { get; set; }

        [DataMember]
        public string CallerId { get; set; }

        public ChannelStatusChangedEventArgs(int address)
        {
            this.Handled = false;
            this.Address = address;
            this.Status = null;
            this.IsLooped = false;
            this.Tone = null;
            this.CallerId = null;
        }
    }
}
