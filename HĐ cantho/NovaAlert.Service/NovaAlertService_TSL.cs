using NovaAlert.Common;
using NovaAlert.Common.Setting;
using NovaAlert.Dal;
using NovaAlert.Entities;
using NovaAlert.Service.TSL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace NovaAlert.Service
{
    public partial class NovaAlertService
    {
        IServerModem _currentServerModem = null;

        TslStatus INovaAlertService.GetLatestTslStatus(int phoneNumberId, eTslStatusType type)
        {
            return TslRepository.Instance.GetLatestStatus(phoneNumberId, type);
        }

        void INovaAlertService.UpdateTslStatus(int phoneNumberId, eTslStatusType type, eTslStatus status)
        {
            TslRepository.Instance.UpdateStatus(phoneNumberId, type, status);
            var client = GetClient();
            NotifyTslStatusChanged(phoneNumberId, type, status, client.Id);
        }

        void INovaAlertService.CancelTslTask()
        {
            if (_currentServerModem != null)
            {
                _currentServerModem.CancelWaitingTask();
            }
        }

        void INovaAlertService.ReplyFromRepareRequest(int id)
        {
            lock (_resources)
            {
                Monitor.PulseAll(_resources);
                // save log
                var client = GetClient();
            }
        }

        void INovaAlertService.DoTslTask(int phoneNumberId, eTslStatusType type)
        {
            var task = new TslTask() { Client = GetClient(), Type = type };

            var uv = _resources.UnitPhones.Where(u => u.PhoneNumberId == phoneNumberId).FirstOrDefault();
            if (uv == null) return;
            else task.Unit = uv.UnitPhone;
            
            var ts = new ThreadStart(() => DoTaskThread(task));
            var thread = new Thread(ts);
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();
        }

        void DoTaskThread(TslTask task)
        {
            var modem = _resources.Modem;
            if (modem.SelectedPanelId != task.Client.Id)
            {
                return;
            }

            var sm = CreateServerModem(modem);            
            
            try
            {
                _currentServerModem = sm;

                var result = new TslResult(); // false;

                if (task.Type == eTslStatusType.Prepare)
                {
                    result = sm.SendPrepareCommand(task.Unit);
                }
                else
                {
                    result = sm.SendAndReceiveResult(task.Unit);
                }

                var status = result.TslStatus;
                if (sm.IsCanceled)
                {
                    status = eTslStatus.Canceled;
                }
                
                TslRepository.Instance.UpdateStatus(task.Unit.PhoneNumberId, task.Type, status);
                NotifyTslStatusChanged(task.Unit.PhoneNumberId, task.Type, status, task.Client.Id);

                _currentServerModem = null;
            }
            catch(Exception ex)
            {
                task.Client.Callback.OnServerMessage(eServerMessageType.Error, ex.Message);
                LogService.Logger.Error(ex);                
            }
        }

        private IServerModem CreateServerModem(TSL_Modem modem)
        {
            if (ClientSetting.Instance.IsFakeSystem)
            {
                return new FakeServerModem();
            }
            return new ServerModem(modem);
        }

        void NotifyTslStatusChanged(int phoneNumberId, eTslStatusType type, eTslStatus status, int? hostClientId = null)
        {
            var clients = _resources.GetActiveClients();
            foreach(var client in  clients)
            {
                try
                {
                    client.Callback.OnTslStatusChanged(phoneNumberId, type, status, hostClientId);
                }
                catch (Exception ex)
                {
                }
            }
        }

        List<ResultData> INovaAlertService.GetSubResults(int phoneNumberId, eTaskType type)
        {
            return NovaAlertCommon.GetSubResults(phoneNumberId, type);
        }
    }
}
