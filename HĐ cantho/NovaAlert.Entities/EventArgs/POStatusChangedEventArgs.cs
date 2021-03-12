using System.Runtime.Serialization;

namespace NovaAlert.Entities
{
    [DataContract] 
    public class POStatusChangedEventArgs: BaseEventArgs // EventArgs
    {
        [DataMember]
        public int Address { get; private set; }
        
        [DataMember]
        public ePOStatus? Status { get; set; }

        [DataMember]
        public string Tone { get; set; }

        public POStatusChangedEventArgs(int address)
        {
            this.Handled = false;
            this.Address = address;
            this.Status = null;
            this.Tone = null;
        }
    }

}
