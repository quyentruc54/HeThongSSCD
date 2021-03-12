using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NovaAlert.Entities
{
    [DataContract]        
    public class SwitchConnection: ISwitchConnection
    {
        [DataMember]
        public byte ClientId { get; private set; }

        [DataMember]
        public SwitchConnectionEnd Source { get; private set; }

        [DataMember]
        public SwitchConnectionEnd Dest { get; private set; }

        public SwitchConnection(byte clientId, SwitchConnectionEnd scr, SwitchConnectionEnd dest)
        {
            this.ClientId = clientId;
            this.Source = scr;
            this.Dest = dest;
        }

        [IgnoreDataMember]
        public bool IsConnected
        {
            get { return Source.IsConnected || Dest.IsConnected; }
            set
            {
                if(value)
                {
                    Source.IsConnected = true;
                    Dest.IsConnected = true;
                }
                else
                {
                    Source.IsConnected = false;
                    Dest.IsConnected = false;
                }
            }
        }

        public IEnumerable<ISwitchAddress> Devices
        {
            get 
            {                
                yield return this.Source.Device;
                yield return this.Dest.Device;
            }
        }
    }
}
