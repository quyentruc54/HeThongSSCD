using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace NovaAlert.Entities
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "INovaAlertService" in both code and config file together.
    [ServiceContract(CallbackContract=typeof(INovaAlertServiceCallback))]    
    public interface INovaAlertService
    {
        /// <summary>
        /// Đăng ký client với server, được gọi khi có 1 client bắt đầu chạy
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="type"></param>
        [OperationContract]
        void Subscribe(int clientId, eClientType type);

        /// <summary>
        /// Hủy đăng ký client với server, gọi khi client kết thúc (chương trình client thoát)
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="type"></param>
        [OperationContract(IsOneWay = true)]
        void UnSubscribe(int clientId, eClientType type);

        /// <summary>
        /// Client yêu cầu chiếm tài nguyên 
        /// </summary>
        /// <param name="resourceType"></param>
        /// <param name="resourceId"></param>
        [OperationContract(IsOneWay=true)]
        void Request(eResourceType resourceType, int resourceId);

        /// <summary>
        /// Client giải phóng tài nguyên
        /// </summary>
        /// <param name="resourceType"></param>
        /// <param name="resourceId"></param>
        [OperationContract(IsOneWay = true)]
        void Release(eResourceType resourceType, int resourceId);

        /// <summary>
        /// Client y/c thực hiện kết nối
        /// </summary>
        /// <param name="conn"></param>
        [OperationContract(IsOneWay=true)]
        void SendConnection(SwitchConnection conn);

        /// <summary>
        /// Điều chỉnh âm lượng của kết nối
        /// </summary>
        /// <param name="switchCN"></param>
        [OperationContract(IsOneWay = true)]
        void AdjustConnectionEndVolumn(SwitchConnectionEnd switchCN);  

        /// <summary>
        /// Client y/c danh sách các đơn vị
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="taskType"></param>
        /// <param name="includeTask"></param>
        /// <returns></returns>
        [OperationContract]
        List<UnitPhone> GetUnitPhones(int? groupId, eTaskType taskType, bool includeTask);

        /// <summary>
        /// Client y/c danh sách các đơn vị có TSL
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<UnitPhone> GetTSLUnitPhones();

        /// <summary>
        /// Y/c trạng thái của 1 đơn vị.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="taskType"></param>
        /// <returns></returns>
        [OperationContract]
        UnitPhone GetUnitPhone(int id, eTaskType taskType);

        /// <summary>
        /// Y/c danh sách các kênh giao tiếp
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<HostPhone> GetAllChannels();

        /// <summary>
        /// Y/c danh sách các nhóm danh bạ
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<BaseEntity> GetAllGroups();

        /// <summary>
        /// Y/c thông tin của bàn điều khiển
        /// </summary>
        /// <param name="panelId"></param>
        /// <returns></returns>
        [OperationContract]
        Panel GetPanelInfo(int panelId);

        /// <summary>
        /// Y/c địa chỉ trên chuyển mạch của PO
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        byte GetPOAddress(byte id);

        /// <summary>
        /// Gởi y/c xóa tone
        /// </summary>
        /// <param name="address"></param>
        /// <param name="clear"></param>
        [OperationContract(IsOneWay = true)]
        void SendDeleteTone(byte address, bool clear);

        /// <summary>
        /// Cởi y/c quay số điện thoại
        /// </summary>
        /// <param name="address"></param>
        /// <param name="number"></param>
        [OperationContract(IsOneWay = true)]
        void SendDial(byte address, string number);

        /// <summary>
        /// Y/c tắt / mở tính năng gọi nhóm.
        /// </summary>
        /// <param name="enable"></param>
        [OperationContract(IsOneWay = true)]
        void SetConference(bool enable);

        /// <summary>
        /// Gởi y/c tạo kết nối gọi nhóm.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="isFirstOne"></param>
        [OperationContract(IsOneWay = true)]
        void SendConferenceConnection(ConferenceConnection conn, bool isFirstOne);

        /// <summary>
        /// Điều chỉnh âm lượng trong gọi nhóm
        /// </summary>
        /// <param name="conn"></param>
        [OperationContract(IsOneWay = true)]
        void AdjustConferenceVolumn(ConferenceConnection conn);

        /// <summary>
        /// Kiểm tra xem server có hoạt động hay không.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void Ping();

        /// <summary>
        /// Gởi y/c tắt / mở chuông ngoài
        /// </summary>
        /// <param name="ring"></param>
        [OperationContract(IsOneWay=true)]
        void SendExtRingPower(bool ring);

        /// <summary>
        /// Y/c cập nhật trạng thái kênh giao tiếp
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void UpdateAllStatus();

        // Alert
        /// <summary>
        /// Phát âm thanh báo hiệu / báo động
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="soundId"></param>
        [OperationContract(IsOneWay = true)]
        void PlayAlertSound(byte channelId, byte soundId);

        /// <summary>
        /// Phát âm thanh
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="pos"></param>
        /// <param name="filename"></param>
        [OperationContract(IsOneWay = true)]
        void PlaySound(byte channelId, int pos, string filename);

        /// <summary>
        /// Ngừng phát âm thanh
        /// </summary>
        /// <param name="channelId"></param>
        [OperationContract(IsOneWay = true)]
        void StopSound(byte channelId);

        /// <summary>
        /// Cập nhật kết quả
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="task"></param>
        /// <param name="duration"></param>
        /// <param name="taskType"></param>
        [OperationContract(IsOneWay = true)]
        void UpdateTask(int unitId, Task task, long? duration, eTaskType taskType);

        /// <summary>
        /// Lấy kết quả mới nhất
        /// </summary>
        /// <param name="unitId"></param>
        [OperationContract(IsOneWay = true)]
        void RefeshTask(int unitId);

        /// <summary>
        /// Cập nhật ngày giờ trên Server
        /// </summary>
        /// <param name="datetime"></param>
        [OperationContract(IsOneWay=true)]
        void UpdateSystemDateTime(DateTime datetime);

        /// <summary>
        /// Y/c server phát file âm thanh CTT tương ứng.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="status"></param>
        [OperationContract(IsOneWay = true)]
        void UpdateAlertSoundStatus(int clientId, eAlertSoundStatus status);

        /// <summary>
        /// Lưu nhật ký giao tiếp
        /// </summary>
        /// <param name="en"></param>
        [OperationContract(IsOneWay = true)]
        void SaveCallLog(CallLog en);

        /// <summary>
        /// Lưu chi tiết của nhật ký giao tiếp
        /// </summary>
        /// <param name="en"></param>
        [OperationContract(IsOneWay = true)]
        void SaveCallLogDetail(CallLogDetail en);

        /// <summary>
        /// Bắt đầu ghi âm
        /// </summary>
        /// <param name="id"></param>
        /// <param name="poId"></param>
        /// <returns></returns>
        [OperationContract()]
        string StartRecord(byte id, byte poId);

        /// <summary>
        /// Dừng ghi âm
        /// </summary>
        /// <param name="id"></param>
        [OperationContract(IsOneWay = true)]
        void StopRecord(byte id);

        /// <summary>
        /// Tạm dừng ghi âm
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        [OperationContract(IsOneWay = true)]
        void PauseRecord(byte id, byte type);

        /// <summary>
        /// Tiếp tục ghi âm sau khi tạm dừng
        /// </summary>
        /// <param name="id"></param>
        [OperationContract(IsOneWay = true)]
        void ResumeRecord(byte id);

        /// <summary>
        /// Lấy đường dẫn chức file ghi âm trên server
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        string GetRecordFolder();

        /// <summary>
        /// Tắt / mở amply
        /// </summary>
        /// <param name="on"></param>
        [OperationContract(IsOneWay = true)]
        void SendAmplyPower(bool on);

        /// <summary>
        /// Lấy danh sách kết quả CTT / CCPK
        /// </summary>
        /// <param name="taskType"></param>
        /// <returns></returns>
        [OperationContract]
        List<ResultData> GetResults(eTaskType taskType);

        /// <summary>
        /// Ghi nhật ký sữ dụng
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="userId"></param>
        /// <param name="info"></param>
        [OperationContract]
        void AddLog(int clientId, int userId, string info);

        /// <summary>
        /// Lấy đường dẫn file âm thanh trên server
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        string GetSoundPath();

        [OperationContract]
        string GetParameterValue(eGlobalParameter name);

        // TSL
        [OperationContract]
        TslStatus GetLatestTslStatus(int phoneNumberId, eTslStatusType type);

        [OperationContract(IsOneWay = true)]
        void UpdateTslStatus(int phoneNumberId, eTslStatusType type, eTslStatus status);

        [OperationContract(IsOneWay = true)]
        void CancelTslTask();

        [OperationContract]
        void ReplyFromRepareRequest(int id);

        [OperationContract]
        void DoTslTask(int phoneNumberId, eTslStatusType type);

        [OperationContract]
        List<ResultData> GetSubResults(int phoneNumberId, eTaskType type);

        /// <summary>
        /// BDK hoi may chu xem co ghi am ko
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        bool CanRecord();

        /// <summary>
        /// Lay thoi gian tren server
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        DateTime GetDateTime();
    }
}
