namespace NovaAlert.Bll.Controller
{
    /// <summary>
    /// Trạng thái của một đầu cuối trong CTT
    /// </summary>
    public enum AlertMemberStatus
    {
        /// <summary>
        /// Bắt đầu
        /// </summary>
        Initialized = 0,

        /// <summary>
        /// Chờ nhập mã
        /// </summary>
        WaitForKeycode = 1,

        /// <summary>
        /// Chờ nhận lệnh
        /// </summary>
        WaitForCommand = 2,

        /// <summary>
        /// Đang nhận lệnh
        /// </summary>
        ReceivingCommand = 3,

        /// <summary>
        /// Chờ báo cáo
        /// </summary>
        WaitForReport = 4,

        /// <summary>
        /// Không thể kết nối
        /// </summary>
        CanNotConnect = 10
    }
}
