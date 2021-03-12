using System.ComponentModel;

namespace NovaAlert.Entities
{
    public enum eTask
    {
        None = 0,
        A = 1,
        A2 = 2,
        A3 = 3, 
        A4 = 4
    }

    public enum eTaskLevel
    {
        None = 0,
        TC = 1, // Tan cuong
        CA = 2, // Cao
        TB = 3  // Toan bo
    }

    public enum eTaskResult
    {
        [Description("")]
        None = 0,
        [Description("Đã NL")]
        NL = 1,     // da nhan lenh
        [Description("Đã CTT")]
        CTT = 2,    // da chuyen trang thai

        [Description("")]
        Started = 98,    // da ket noi thanh cong

        [Description("")]
        Connected = 99,    // da ket noi thanh cong

        [Description("")]
        CanNotConnect = 100,    // da ket noi thanh cong
    }

    public enum eTaskType: byte
    {
        CTT = 1,
        CCPK = 2
    }
}
