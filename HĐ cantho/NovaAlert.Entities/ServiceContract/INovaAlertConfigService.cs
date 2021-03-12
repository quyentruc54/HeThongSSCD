using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace NovaAlert.Entities
{
    [ServiceContract]
    public interface INovaAlertConfigService
    {
        //[OperationContract]
        //void UpdateSystemDateTime(DateTime datetime);
        /// <summary>
        /// Trả về danh sách cài đặt ngày loại ngày trong tuần
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<DayTypeConfig> GetDayTypes();

        /// <summary>
        /// Lưu cài đặt
        /// </summary>
        /// <param name="dts"></param>
        [OperationContract]
        void SaveDayTypes(List< DayTypeConfig> dts);

        /// <summary>
        /// Trả về danh sách các số điện thoại lưu trong danh bạ
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<Contact> GetAllContacts();

        /// <summary>
        /// Lưu số điện thoại
        /// </summary>
        /// <param name="con"></param>
        [OperationContract]
        void SaveContact(Contact con);

        /// <summary>
        /// Xóa số điện thoại
        /// </summary>
        /// <param name="id"></param>
        [OperationContract]
        void DeleteContact(int id);

        /// <summary>
        /// Trả về danh sách các nhóm danh bạ
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<ContactGroup> GetAllContactGroups();

        /// <summary>
        /// Lưu nhóm danh bạ
        /// </summary>
        /// <param name="cg"></param>
        [OperationContract]
        void SaveContactGroup(ContactGroup cg);

        /// <summary>
        /// Trả về danh sách các báo hiệu / báo động
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<Alarm> GetAlarms();

        /// <summary>
        /// Lưu báo hiệu / báo động
        /// </summary>
        /// <param name="list"></param>
        [OperationContract]
        void SaveAlarms(List<Alarm> list);

        /// <summary>
        /// Danh sách các tên viết tắt của đơn vị
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<string> GetUnitNameAbbrs();

        /// <summary>
        /// Danh sách tên các đơn vị
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<string> GetUnitNames();

        /// <summary>
        /// Danh sách các kênh giao tiếp
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<Channel> GetAllChannels();

        /// <summary>
        /// Lưu khai báo kênh giao tiếp
        /// </summary>
        /// <param name="list"></param>
        [OperationContract]
        void SaveChannels(List<Channel> list);

        [OperationContract]
        NovaAlert.Entities.Enum GetEnum(string type, byte value);

        [OperationContract]
        List<NovaAlert.Entities.Enum> GetEnums(string type);

        /// <summary>
        /// Thực hiện tìm kiếm trong nhật ký giao tiếp
        /// </summary>
        /// <param name="cr"></param>
        /// <returns></returns>
        [OperationContract]
        List<SearchCallLogResult> SearchCallLog(SearchCallLogCriteria cr);

        /// <summary>
        /// Đếm số cuộc gọi
        /// </summary>
        /// <param name="cr"></param>
        /// <returns></returns>
        [OperationContract]
        uint CountCallLog(SearchCallLogCriteria cr);

        /// <summary>
        /// Chức năng báo hiệu được thực hiện trên phần mềm hay chuyển mạch
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        bool IsAlartOnSwitch();

        [OperationContract]
        void SetAlarmConfig(bool alarmOnSwitch);

        /// <summary>
        /// Xóa nhật ký giao tiếp
        /// </summary>
        /// <param name="cl"></param>
        [OperationContract]
        void DeleteCallLog(SearchCallLogResult cl);

        /// <summary>
        /// Thực hiện tìm kiếm trong nhật ký giao tiếp
        /// </summary>
        /// <param name="panelId"></param>
        /// <param name="date"></param>
        /// <param name="searchText"></param>
        /// <param name="startIndex"></param>
        /// <param name="numOfRecord"></param>
        /// <returns></returns>
        [OperationContract]
        List<LogItem> SearchLog(byte panelId, DateTime date, string searchText, int startIndex, int numOfRecord);

        [OperationContract]
        uint CountLog(byte panelId, DateTime date, string searchText);

        /// <summary>
        /// Trả về kết quả CTT hoặc BĐPK
        /// </summary>
        /// <param name="taskType"></param>
        /// <returns></returns>
        [OperationContract]
        List<ResultData> GetResults(eTaskType taskType);

        /// <summary>
        /// Trả về các cài đặt hiện thị kết quả
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<DisplayData> GetDisplayData();

        [OperationContract]
        void SaveDisplayData(List<DisplayData> list);

        [OperationContract]
        List<RadioTime> GetRadioTimes(byte dayType);

        [OperationContract]
        void SaveRadioTimes(List<RadioTime> list);

        [OperationContract]
        string GetParameterValue(eGlobalParameter name);

        /// <summary>
        /// Trả về các kết quả của các đơn vị cấp dưới
        /// </summary>
        /// <param name="phoneNumberId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [OperationContract]
        List<ResultData> GetSubResults(int phoneNumberId, eTaskType type);
    }
}
