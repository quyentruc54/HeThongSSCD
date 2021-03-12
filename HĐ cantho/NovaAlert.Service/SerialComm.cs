using NovaAlert.Common.Setting;
using NovaAlert.Communication.Base;
using NovaAlert.Dal;
using NovaAlert.Entities;
using NovaAlert.Service.Fake;
using NovaAlert.SwitchIC;
using NovaAlert.SwitchIC.PresentationLayer;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;

namespace NovaAlert.Service
{
    public class SerialComm
    {
        static ISwitchComm _switchComm;
        public static ISwitchComm SwitchComm
        {
            get
            {
                if (_switchComm == null)
                {
                    if(ClientSetting.Instance.IsFakeSystem)
                    {
                        _switchComm = new Fake.FakeSwitch();
                    }
                    else
                    {
                        var settings = GlobalSetting.Instance; // GlobalSetting.Instance;
                        SerialPort port = new SerialPort(settings.PortName, settings.BaudRate, settings.Parity, settings.DataBits, settings.StopBits);
                        var dataLink = new SwitchIC.DataLinkLayer.DataLink(port);
                        var presentation = new SwitchIC.PresentationLayer.Presentation(dataLink);
                        _switchComm = new SwitchComm(presentation);
                    }                    
                }
                return _switchComm;
            }
        }

        static ILPComm _LPComm;
        public static ILPComm LPComm
        {
            get 
            { 
                if(_LPComm == null)
                {
                    if(ClientSetting.Instance.IsFakeSystem)
                    {
                        _LPComm = new FakeLP();
                    }
                    else
                    {
                        var settings = GlobalSetting.Instance;
                        SerialPort port = new SerialPort(settings.LPPortName, settings.BaudRate, settings.Parity, settings.DataBits, settings.StopBits);
                        var dataLink = new SwitchIC.DataLinkLayer.DataLink(port);
                        var presentation = new SwitchIC.PresentationLayer.Presentation(dataLink);
                        _LPComm = new LPComm(presentation);
                    }
                }
                return _LPComm; 
            }
        }

        public static void SendAlarms(List<byte> typeIds = null)
        {
            try
            {
                var list = new List<Alarm>();
                if (typeIds == null)
                {
                    list = NovaAlertCommon.Instance.AlarmList;
                }
                else
                {
                    list = NovaAlertCommon.Instance.AlarmList.Where(a => typeIds.Contains(a.DayType)).ToList();
                }
                SerialComm.SwitchComm.SendAlarms(list, NovaAlertCommon.Instance.RadioTimeList);
            }
            catch (Exception ex)
            {
                NovaAlert.Common.LogService.Logger.Error(ex);
            }
        }

        public static void SendDayTypes()
        {
            try
            {
                var dayTypes = NovaAlertCommon.Instance.GetDayTypes();
                SerialComm.SwitchComm.SendDayTypes(dayTypes);
            }
            catch (Exception ex)
            {
                NovaAlert.Common.LogService.Logger.Error(ex);
            }
        }

        static object _staticSyncObj = new object();
        public static void SendResults(IPresentation pre, byte address, eTaskType taskType, int phoneNumberId = 0)
        {
            lock (_staticSyncObj)
            {
                var dbResults = NovaAlertCommon.GetResults(taskType, phoneNumberId);

                for (int i = 0; i < dbResults.Count; i++)
                {
                    var item = dbResults[i];
                    var msg = new LP_ResultMessage()
                    {
                        TypeDest = eDevice.LedPanel,
                        Address = address,
                        Id = (byte)item.DisplayId,
                        Alert = (byte)item.Task,
                        Level = (byte)item.Level
                    };

                    if (item.TimeReceive.HasValue && item.TimeChange.HasValue)
                    {
                        msg.Result = 3;
                    }
                    else
                    {
                        if (item.TimeChange.HasValue)
                        {
                            msg.Result = 2;
                        }
                        else
                        {
                            if (item.TimeReceive.HasValue)
                            {
                                msg.Result = 1;
                            }
                        }
                    }

                    pre.SendData(msg);
                    System.Threading.Thread.Sleep(51);
                }
            }
        }

        public static void SendAllResults(IPresentation pre, eTaskType taskType = eTaskType.CTT, int phoneNumberId = 0)
        {
            for (byte i = 0; i <= NovaAlert.Service.LPComm.MaxPanelId; i++)
            {
                SendResults(pre, (byte)(i + 1), eTaskType.CTT, phoneNumberId);
            }
        }

        public static void SendAllResultBySwitchComm(int phoneNumberId = 0)
        {
            var wc = SerialComm.SwitchComm as SwitchComm;
            if (wc != null)
            {
                Action<IPresentation, eTaskType, int> act = new Action<IPresentation, eTaskType, int>(SendAllResults);
                act.BeginInvoke(wc.Presentation, eTaskType.CTT, phoneNumberId, null, null);
            }
        }
    }
}
