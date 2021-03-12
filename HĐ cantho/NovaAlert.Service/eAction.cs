using System.ComponentModel;

namespace NovaAlert.Service
{
    public enum eAction
    {
        [Description("Đăng ký")]
        Subscribe = 1,

        [Description("Hủy đăng ký")]
        Unsubscribe = 2,

        [Description("Yêu cầu")]
        Request = 3,

        [Description("Giải phóng")]
        Release = 4
    }
}
