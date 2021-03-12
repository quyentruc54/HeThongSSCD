using System.ComponentModel;

namespace NovaAlert.Entities
{
    public enum eClientType
    {
        [Description("BÀN ĐIỀU KHIỂN")]
        ControlPanel = 1,

        [Description("BẢNG ĐÈN")]
        LedPanel = 2
    }
}
