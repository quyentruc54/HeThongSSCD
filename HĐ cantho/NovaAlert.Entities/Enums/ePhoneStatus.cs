using System.ComponentModel;

namespace NovaAlert.Entities
{
    public enum ePhoneStatus: int
    {
        [Description("Đứt dây")]
        Disconnect = 0,
        [Description("Rỗi")]
        Free = 1,
        [Description("Bị chiếm")]
        Occupied = 2,
        [Description("Được chọn")]
        Selected = 3,
        [Description("Đổ chuông")]
        Ring = 4,
        [Description("Quay số")]
        Dial = 5,
        [Description("Đang nói")]
        Speaking = 6,
        [Description("Chỉ nghe")]
        Listening = 7,
        [Description("Chờ")]
        Holding = 8,

        // Alert
        [Description("Chờ nhập mã")]
        WaitForKeycode = 9,
        [Description("Chờ nhận lệnh")]
        WaitForCommand = 10,
        [Description("Chờ báo cáo")]
        WaitForReport = 11
    }

    public enum eLineStatus
    {
        Disconnect = 0x00,
        Occupied = 0x01,
        Good = 0x02,
        Ring = 0x03,
        ExtOffHook = 0x04
    }   
}
