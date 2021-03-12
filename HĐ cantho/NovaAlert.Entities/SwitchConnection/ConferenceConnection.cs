using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NovaAlert.Entities
{
    [DataContract]
    [KnownType(typeof(PO))]
    [KnownType(typeof(HostPhone))]
    [KnownType(typeof(OtherDevice))]
    public class ConferenceConnection: BindableEntity, ISwitchConnection
    {
        [IgnoreDataMember]
        public byte Address { get { return this.Device.Address; } }

        eVolumn _inVolumn;
        [DataMember]
        public eVolumn InVolumn
        {
            get { return _inVolumn; }
            set { _inVolumn = value; OnPropertyChanged("InVolumn"); }
        }

        eVolumn _outVolumn;
        [DataMember]
        public eVolumn OutVolumn
        {
            get { return _outVolumn; }
            set { _outVolumn = value; OnPropertyChanged("OutVolumn"); }
        }

        bool _isConnected;
        [DataMember]
        public bool IsConnected
        {
            get { return _isConnected; }
            set { _isConnected = value; OnPropertyChanged("IsConnected"); }
        }

        [DataMember]
        public ISwitchAddress Device { get; private set; }

        [DataMember]
        public byte ConferenceId { get; set; }
        public ConferenceConnection(byte id, ISwitchAddress device, eVolumn inVolumn = eVolumn.Volumn_6, eVolumn outVolumn = eVolumn.Volumn_1, bool isConnected = true)
        {            
            this.ConferenceId = id;
            this.Device = device;
            _inVolumn = inVolumn;
            _outVolumn = outVolumn;
            _isConnected = isConnected;
        }

        public IEnumerable<ISwitchAddress> Devices
        {
            get { yield return this.Device; }
        }
    }
}
