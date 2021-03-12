using NovaAlert.Common;
using NovaAlert.Common.Setting;
using NovaAlert.Dal;
using NovaAlert.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;

namespace NovaAlert.Service
{
    public class NovaAlertConfigService: INovaAlertConfigService
    {
        List<DayTypeConfig> INovaAlertConfigService.GetDayTypes()
        {
            return NovaAlertCommon.Instance.GetDayTypes();
        }

        void INovaAlertConfigService.SaveDayTypes(List<DayTypeConfig> dts)
        {
            NovaAlertCommon.Instance.SaveDayTypes(dts);
            SerialComm.SendDayTypes();
        }

        List<Contact> INovaAlertConfigService.GetAllContacts()
        {
            return NovaAlertCommon.Instance.GetAllContacts();
        }

        void INovaAlertConfigService.SaveContact(Contact con)
        {
            NovaAlertCommon.Instance.SaveContact(con);
        }

        void INovaAlertConfigService.DeleteContact(int id)
        {
            NovaAlertCommon.Instance.DeleteContact(id);
        }

        List<ContactGroup> INovaAlertConfigService.GetAllContactGroups()
        {
            return NovaAlertCommon.Instance.GetAllContactGroups();
        }

        void INovaAlertConfigService.SaveContactGroup(ContactGroup cg)
        {
            NovaAlertCommon.Instance.SaveGroup(cg);
        }

        List<Alarm> INovaAlertConfigService.GetAlarms()
        {
            return NovaAlertCommon.Instance.AlarmList;
        }


        List<string> INovaAlertConfigService.GetUnitNameAbbrs()
        {
            return NovaAlertCommon.Instance.GetUnitNameAbbrs();
        }

        List<string> INovaAlertConfigService.GetUnitNames()
        {
            return NovaAlertCommon.Instance.GetUnitNames();
        }


        List<Channel> INovaAlertConfigService.GetAllChannels()
        {
            return NovaAlertCommon.Instance.GetAllChannels();
        }

        void INovaAlertConfigService.SaveChannels(List<Channel> list)
        {
            NovaAlertCommon.Instance.SaveChannels(list);
            RaiseSystemChangedEvent();
        }        
        
        Entities.Enum INovaAlertConfigService.GetEnum(string type, byte value)
        {
            return NovaAlertCommon.Instance.GetEnum(type, value);
        }


        List<Entities.Enum> INovaAlertConfigService.GetEnums(string type)
        {
            return NovaAlertCommon.Instance.GetEnums(type);
        }


        void INovaAlertConfigService.SaveAlarms(List<Alarm> list)
        {            
            NovaAlertCommon.Instance.SaveAlarms(list);
            SerialComm.SendAlarms(list.Select(a => a.DayType).Distinct().ToList());
        }


        List<SearchCallLogResult> INovaAlertConfigService.SearchCallLog(SearchCallLogCriteria cr)
        {
            return CallLogDal.Instance.Search(cr);
        }

        uint INovaAlertConfigService.CountCallLog(SearchCallLogCriteria cr)
        {
            return CallLogDal.Instance.Count(cr);
        }

        bool INovaAlertConfigService.IsAlartOnSwitch()
        {
            return GlobalSetting.Instance.AlarmOnSwitch;
        }

        void INovaAlertConfigService.SetAlarmConfig(bool alarmOnSwitch)
        {
            var settings = GlobalSetting.Instance;
            settings.AlarmOnSwitch = alarmOnSwitch;
            settings.Save();
            SerialComm.SwitchComm.SendAlarmConfig(settings.AlarmOnSwitch);
            if(alarmOnSwitch)
            {
                SerialComm.SendAlarms();
                SerialComm.SendDayTypes();
            }
        }

        void INovaAlertConfigService.DeleteCallLog(SearchCallLogResult cl)
        {
            try
            {
                CallLogDal.Instance.DeleteCallLog(cl.CallLogId);

                var recordFolder = GlobalSetting.Instance.RecordFolder;
                // Delete file
                foreach (var det in cl.Details)
                {
                    var path = Path.Combine(recordFolder, det.GetRecordFileName());
                    if (File.Exists(path)) File.Delete(path);
                }
            }
            catch(Exception ex)
            {
                LogService.Logger.Error("DeleteCallLog", ex);
            }
        }


        List<LogItem> INovaAlertConfigService.SearchLog(byte panelId, DateTime date, string searchText, int startIndex, int numOfRecords)
        {
            return DbLogger.Instance.SearchLog(date, date, panelId, searchText, startIndex, numOfRecords);
        }

        uint INovaAlertConfigService.CountLog(byte panelId, DateTime date, string searchText)
        {
            return DbLogger.Instance.CountLog(date, date, panelId, searchText);
        }

        List<ResultData> INovaAlertConfigService.GetResults(eTaskType taskType)
        {
            return NovaAlertCommon.GetResults(taskType);
        }


        List<DisplayData> INovaAlertConfigService.GetDisplayData()
        {
            return NovaAlertCommon.GetDisplayData();
        }

        void INovaAlertConfigService.SaveDisplayData(List<DisplayData> list)
        {
            NovaAlertCommon.SaveDisplayData(list);
        }

        private void RaiseSystemChangedEvent()
        {
            var host = OperationContext.Current.Host as ConfigServiceHost;
            if (host != null) host.RaiseSystemChangedEvent();
        }

        List<RadioTime> INovaAlertConfigService.GetRadioTimes(byte dayType)
        {
            return NovaAlertCommon.Instance.RadioTimeList.Where(r => r.DayType == dayType).ToList();
        }

        void INovaAlertConfigService.SaveRadioTimes(List<RadioTime> list)
        {
            NovaAlertCommon.Instance.SaveRadioTimes(list);
            SerialComm.SendAlarms(list.Select(a => a.DayType).Distinct().ToList());
        }

        string INovaAlertConfigService.GetParameterValue(eGlobalParameter name)
        {
            return GlobalSetting.Instance.GetParamByName(name.ToString());
        }

        List<ResultData> INovaAlertConfigService.GetSubResults(int phoneNumberId, eTaskType type)
        {
            return NovaAlertCommon.GetSubResults(phoneNumberId, type);
        }
    }
}
