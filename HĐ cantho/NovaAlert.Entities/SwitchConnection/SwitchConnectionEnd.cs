using System.Runtime.Serialization;

namespace NovaAlert.Entities
{
    [DataContract]
    [KnownType(typeof(PO))]
    [KnownType(typeof(HostPhone))]
    [KnownType(typeof(OtherDevice))]
    public class SwitchConnectionEnd : BindableEntity, ISwitchConnectionEnd
    {
        [IgnoreDataMember]
        public byte Address { get { return this.Device.Address; } }

        eVolumn _volumn;
        [DataMember]
        public eVolumn Volumn
        {
            get { return _volumn; }
            set { _volumn = value; OnPropertyChanged("Volumn"); }
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

        public SwitchConnectionEnd(ISwitchAddress device, eVolumn vol = eVolumn.Volumn_6, bool isConnected = true)
        {
            this.Device = device;            
            this.Volumn = vol;
            this.IsConnected = isConnected; 
        }

        public void Update(eVolumn volumn, bool isConnect)
        {
            this.Volumn = volumn;
            this.IsConnected = isConnect;
        }
    }
}
