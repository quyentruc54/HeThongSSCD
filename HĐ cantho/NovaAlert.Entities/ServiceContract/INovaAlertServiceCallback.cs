using System.ServiceModel;

namespace NovaAlert.Entities
{
    /// <summary>
    /// Dùng gởi tín hiệu / thông báo từ server tới các client đang kết nối 
    /// </summary>    
    public interface INovaAlertServiceCallback
    {
        /// <summary>
        /// Báo với client là có sự thay đổi việc chiếm dụng tài nguyên
        /// </summary>
        /// <param name="e"></param>
        [OperationContract(IsOneWay = true)]
        void OnResourceChanged(ResourceChangedEventArgs e);

        /// <summary>
        /// Báo trạng thái PO thay đổi
        /// </summary>
        /// <param name="e"></param>
        [OperationContract(IsOneWay = true)]
        void OnPOStatusChanged(POStatusChangedEventArgs e);

        /// <summary>
        /// Báo trạng thái kênh tiao tiếp thay đổi
        /// </summary>
        /// <param name="e"></param>
        [OperationContract(IsOneWay = true)]
        void OnChannelStatusChanged(ChannelStatusChangedEventArgs e);

        /// <summary>
        /// Báo hoàn thành y/c quay số
        /// </summary>
        /// <param name="e"></param>
        [OperationContract(IsOneWay = true)]
        void OnDialCompleted(ChannelEventArgs e);

        /// <summary>
        /// Báo có sự thay đổi ngày giờ hệ thống
        /// </summary>
        /// <param name="e"></param>
        [OperationContract(IsOneWay = true)]
        void OnSystemDateTimeChanged(SystemDateTimeChangedEventArgs e);

        /// <summary>
        /// Báo trạng thái kết nối của chuyển mạch
        /// </summary>
        /// <param name="isConnected"></param>
        [OperationContract(IsOneWay = true)]
        void OnSwitchStatusChanged(bool isConnected);

        /// <summary>
        /// Thông báo lỗi từ server
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="isError"></param>
        //[OperationContract(IsOneWay = true)]
        //void OnServerMessage(string msg, bool isError);
        [OperationContract(IsOneWay = true)]
        void OnServerMessage(eServerMessageType messageType, string msg);

        /// <summary>
        /// Y/c client lấy lại dữ liệu
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void Reload();

        /// <summary>
        /// Thông báo có sự thay đổi kết qua CTT
        /// </summary>
        /// <param name="e"></param>
        [OperationContract(IsOneWay = true)]
        void OnTaskChanged(TaskChangedEventArgs e);

        /// <summary>
        /// Y/c client chuẩn bị TSL
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void OnRequestPrepare();

        /// <summary>
        /// Trạng thái TSL thai đổi
        /// </summary>
        /// <param name="phoneNumberId"></param>
        /// <param name="type"></param>
        /// <param name="result"></param>
        /// <param name="hostClientId"></param>
        [OperationContract(IsOneWay = true)]
        void OnTslStatusChanged(int phoneNumberId, eTslStatusType type, eTslStatus result, int? hostClientId);
    }
}
