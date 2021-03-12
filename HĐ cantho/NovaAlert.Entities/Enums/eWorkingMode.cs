using System.ComponentModel;

namespace NovaAlert.Entities
{
    public enum eWorkingMode
    {
        [Description("CHẾ ĐỘ CHUYỂN TRẠNG THÁI - SẲN SÀNG CHIẾN ĐẤU")]
        Alert = 1,

        [Description("CHẾ ĐỘ ĐIỆN THOẠI ĐA HƯỚNG")]
        MultiDest = 2,

        [Description("CHẾ ĐỘ BÁO HIỆU - BÁO ĐỘNG NỘI BỘ")]
        Alarm = 3,

        [Description("CHẾ ĐỘ BÁO ĐỘNG - CHUYỂN CẤP PHÒNG KHÔNG")]
        CCPK_Alert = 4,

        [Description("CHẾ ĐỘ CHUYỂN TRẠNG THÁI - SẲN SÀNG CHIẾN ĐẤU")]
        TSL_Alert = 5
    }
}
