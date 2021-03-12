using System.ComponentModel;

namespace NovaAlert.Entities
{
    public enum eResourceType
    {
        [Description("Kênh")]
        Channel = 1,
        [Description("Đơn vị")]
        UnitPhone = 2,
        [Description("Máy trực")]
        PO = 3,
        [Description("Loa")]
        Speaker = 4,
        [Description("Micro")]
        Micro = 5,
        [Description("Âm hiệu")]
        SoundFile = 6,

        [Description("SoundChannel")]
        SoundChannel = 7,
        [Description("Amply")]
        Amply = 8,
        [Description("Trang âm")]
        TrangAm = 9,

        Modem = 10
    }
}
