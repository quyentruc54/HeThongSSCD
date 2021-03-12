using System.Runtime.Serialization;

namespace NovaAlert.Entities
{
    [DataContract]
    public class OtherDevice: BaseEntity, ISwitchAddress
    {
        [DataMember]
        public byte Address { get; private set; }

        public OtherDevice(int id, byte address, string name = null)
            : base(id, name)
        {
            this.Address = address;
        }

        private static OtherDevice[] _soundChannels;
        public static OtherDevice[] SoundChannels
        {
            get 
            {
                if (_soundChannels == null)
                {
                    _soundChannels = new OtherDevice[]
                    {
                        new OtherDevice(1, 0x19, "Left Sound Card 1"),
                        new OtherDevice(2, 0x1A, "Right Sound Card 1"),
                        new OtherDevice(3, 0x1B, "Left Sound Card 2"),
                        new OtherDevice(4, 0x1C, "Right Sound Card 2")
                    };
                }
                return _soundChannels; 
            }
        }

        private static OtherDevice[] _speakers;
        public static OtherDevice[] Speakers
        {
            get 
            {
                if (_speakers == null)
                {
                    _speakers = new OtherDevice[]
                    {
                        new OtherDevice(1, 0x19, "Loa phụ 1"),
                        new OtherDevice(2, 0x1A, "Loa phụ 2"),
                        new OtherDevice(3, 0x1B, "Loa phụ 3"),
                        new OtherDevice(4, 0x1C, "Loa phụ 4")
                    };
                }
                return _speakers; 
            }            
        }

        private static OtherDevice _micro;
        public static OtherDevice Micro
        {
            get 
            {
                if (_micro == null)
                {
                    _micro = new OtherDevice(1, 0x1D, "Micro");
                }
                return _micro; 
            }
        }

        private static OtherDevice _amply;
        public static OtherDevice Amply
        {
            get 
            {
                if (_amply == null)
                {
                    _amply = new OtherDevice(1, 0x1D, "Amply");
                }
                return _amply; 
            }            
        }

        private static OtherDevice _trangAm;
        public static OtherDevice TrangAm
        {
            get 
            {
                if (_trangAm == null)
                {
                    _trangAm = new OtherDevice(1, 0x1E, "Trang âm hội nghị");
                }
                return _trangAm; 
            }
        }

        private static OtherDevice _waitingMusic;
        public static OtherDevice WaitingMusic
        {
            get 
            {
                if (_waitingMusic == null)
                {
                    _waitingMusic = new OtherDevice(1, 0x1F, "Nhạc chờ");
                }
                return _waitingMusic; 
            }            
        }
    }    
}
